namespace HangfireBasicAuthenticationFilter
{
    public class BasicAuthenticationTokens
    {
        private readonly string[] _tokens;

        public string Username => _tokens[0];
        public string Password => _tokens[1];

        public BasicAuthenticationTokens(string[] tokens)
        {
            _tokens = tokens;
        }

        public bool Are_Invalid()
        {
            return Not_Contains_Two_Tokens() || Invalid_Token_Value(Username) || Invalid_Token_Value(Password);
        }

        public bool Credentials_Match(string user, string pass)
        {
            return Username.Equals(user) && Password.Equals(pass);
        }

        private bool Invalid_Token_Value(string token)
        {
            return string.IsNullOrWhiteSpace(token);
        }

        private bool Not_Contains_Two_Tokens()
        {
            return _tokens.Length != 2;
        }
    }
}
