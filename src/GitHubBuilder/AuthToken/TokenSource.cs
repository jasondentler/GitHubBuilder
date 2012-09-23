using System;
using System.Configuration;

namespace GitHubBuilder.AuthToken
{
    public class TokenSource
    {

        private const string UserNameKey = "GitHub.UserName";
        private const string PasswordKey = "GitHub.Password";

        private static string UserName { get { return ConfigurationManager.AppSettings[UserNameKey]; } }
        private static string Password { get { return ConfigurationManager.AppSettings[PasswordKey]; } }

        private static readonly object LockObject = new object();
        private static string _token;

        public string GetToken()
        {

            if (!string.IsNullOrWhiteSpace(_token))
                return _token;

            lock (LockObject)
            {
                if (!string.IsNullOrWhiteSpace(_token))
                    return _token;

                _token = GitConfigTokenSource.GetToken();

                if (!string.IsNullOrWhiteSpace(_token))
                    return _token;

                if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
                {
                    _token = GenerateToken();
                    return _token;
                }
            }

            const string msg = "Application Settings should contain {0} and {1}, or git config should contain {2}";
            throw new ApplicationException(string.Format(msg, UserNameKey, PasswordKey, GitConfigTokenSource.TokenKey));
        }


        private static string GenerateToken()
        {
            var token = GenerateToken(UserName, Password);
            GitConfigTokenSource.SetToken(token);
            RemoveSensitiveData();
            return token;
        }

        private static string GenerateToken(string userName, string password)
        {
            var response = new Authentication().Authenticate(userName, password, Authentication.Scopes.Repository);
            return response.Token;
        }
        
        private static void RemoveSensitiveData()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(UserNameKey);
            config.AppSettings.Settings.Remove(PasswordKey);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
