using System;
using System.Net.Http.Headers;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HangfireBasicAuthenticationFilter
{
    public class HangfireCustomBasicAuthenticationFilter : IDashboardAuthorizationFilter
    {
        public string User { get; set; }
        public string Pass { get; set; }

        private const string _AuthenticationScheme = "Basic";

        // https://gist.github.com/ndc/a1cc8e2515e5e0d941a884fc6a6267f5
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var header = httpContext.Request.Headers["Authorization"];

            if (Missing_Authorization_Header(header))
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            var authValues = AuthenticationHeaderValue.Parse(header);

            if (Not_Basic_Authentication(authValues))
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            var tokens = Extract_Authentication_Tokens(authValues);

            if (tokens.Are_Invalid())
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            if (tokens.Credentials_Match(User, Pass))
            {
                return true;
            }

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
            httpContext.Response.WriteAsync("Authentication is required.");
        }
    }
}
