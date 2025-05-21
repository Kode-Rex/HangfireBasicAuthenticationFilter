using System;
using System.Net.Http.Headers;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;

namespace HangfireBasicAuthenticationFilter
{
    public class HangfireCustomBasicAuthenticationFilter : IDashboardAuthorizationFilter
    {
        private readonly ILogger _logger;
        public string User { get; set; }
        public string Pass { get; set; }

        private const string _AuthenticationScheme = "Basic";

        public HangfireCustomBasicAuthenticationFilter() : this(
            new NullLogger<HangfireCustomBasicAuthenticationFilter>())
        {
        }

        public HangfireCustomBasicAuthenticationFilter(ILogger logger)
        {
            _logger = logger;
        }

        // https://gist.github.com/ndc/a1cc8e2515e5e0d941a884fc6a6267f5
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var header = httpContext.Request.Headers["Authorization"];

            if (Missing_Authorization_Header(header))
            {
                _logger.LogInformation("Request is missing Authorization Header");
                SetChallengeResponse(httpContext);
                return false;
            }

            var authValues = AuthenticationHeaderValue.Parse(header);

            if (Not_Basic_Authentication(authValues))
            {
                _logger.LogInformation("Request is NOT BASIC authentication");
                SetChallengeResponse(httpContext);
                return false;
            }

            var tokens = Extract_Authentication_Tokens(authValues);

            if (tokens.Are_Invalid())
            {
                _logger.LogInformation("Authentication tokens are invalid (empty, null, whitespace)");
                SetChallengeResponse(httpContext);
                return false;
            }

            if (tokens.Credentials_Match(User, Pass))
            {
                _logger.LogInformation("Awesome, authentication tokens match configuration!");
                return true;
            }

            _logger.LogInformation($"Boo! Authentication tokens [{tokens.Username}] [{tokens.Password}] do not match configuration");

            SetChallengeResponse(httpContext);
            return false;
        }

        private static bool Missing_Authorization_Header(StringValues header)
        {
            return string.IsNullOrWhiteSpace(header);
        }

        private static BasicAuthenticationTokens Extract_Authentication_Tokens(AuthenticationHeaderValue authValues)
        {
            var parameter = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));
            var parts = parameter.Split(':');
            return new BasicAuthenticationTokens(parts);
        }

        private static bool Not_Basic_Authentication(AuthenticationHeaderValue authValues)
        {
            return !_AuthenticationScheme.Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase);
        }

        private void SetChallengeResponse(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
        }
    }
}
