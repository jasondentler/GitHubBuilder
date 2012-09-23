using System;
using System.Configuration;
using System.Diagnostics;

namespace GitHubBuilder.AuthToken
{
    public class GitConfigTokenSource
    {

        private const string GitPathKey = "Git.Path";
        private const string GitPathDefault = @"C:\Program Files (x86)\Git\cmd\git.cmd";
        public const string TokenKey = "GitHubBuilder.Token";
        
        private static string GitPath { get { return ConfigurationManager.AppSettings[GitPathKey] ?? GitPathDefault; } }
        
        public static string GetToken()
        {
            var si = new ProcessStartInfo(GitPath, String.Format("config --global --get {0}", TokenKey))
                         {
                             RedirectStandardOutput = true,
                             UseShellExecute = false,
                             CreateNoWindow = true
                         };
            var p = Process.Start(si);
            p.WaitForExit();
            return p.StandardOutput.ReadToEnd().Trim();
        }

        public static void SetToken(string token)
        {
            var si = new ProcessStartInfo(GitPath, String.Format("config --global --add githubbuilder.token {0}", token))
                         {
                             UseShellExecute = false,
                             CreateNoWindow = true
                         };
            var p = Process.Start(si);
            p.WaitForExit();

            var savedToken = token;
            var fetchedToken = GetToken();

            if (fetchedToken != savedToken)
                throw new ApplicationException("Attempt to save githubbuilder.token to git config failed.");
        }
    }
}
