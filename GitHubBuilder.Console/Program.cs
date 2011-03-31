using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;

namespace GitHubBuilder.Console
{
    class Program
    {
        private const string GitPath = @"C:\Program Files (x86)\Git\cmd\git.cmd";
        private static string _userName;
        private static string _password;

        static void Main(string[] args)
        {
            GetGitHubApiCredentials();

            var repoUserName = "ncqrs";
            var repoName = "ncqrs";
            var path = @"c:\myNcqrsBuilder";
            var comment = new StringBuilder();

            var pull = new PullRequest(repoUserName, repoName, 59);
            var data = JsonConvert.SerializeObject(pull);
            System.Console.WriteLine(data);
            Debug.WriteLine(data);
            
            System.Console.WriteLine("Downloading base source");
            var baseSource = new Source(repoUserName, repoName);
            var sourceDir = baseSource.Download(path, pull.BaseSha);

            System.Console.WriteLine("Source downloaded and unzipped to {0}.", sourceDir);
            
            System.Console.WriteLine("Building base source to prove build is not broken.");
            var prePatchBuild = new NcqrsBuildProcess(sourceDir);
            prePatchBuild.Build();
            
            if (!prePatchBuild.Success)
            {
                var prePatchGist = Gist.CreateGist(
                    _userName, _password,
                    string.Format("buildLog.before-pullRequest{0}.txt", pull.PullRequestNumber),
                    prePatchBuild.Output, false);
                comment.AppendLine("The build was broken before your pull request was applied. It's not your fault.");
                comment.AppendFormat("[Here's the build log.]({0})", prePatchGist.GistUrl);
                comment.AppendLine();
                comment.AppendLine();
            }

            System.Console.WriteLine("Apply patch to source.");
            var applyLog = Patch.DownloadAndApply(pull, sourceDir, GitPath);

            System.Console.WriteLine("Building patched source.");
            var postPatchBuild = new NcqrsBuildProcess(sourceDir);
            postPatchBuild.Build();

            if (postPatchBuild.Success)
            {
                System.Console.WriteLine("Build success!");
                comment.AppendLine(
                    prePatchBuild.Success
                        ? "Thanks! Your pull request didn't break the build (assuming no other commits have been merged)."
                        : "Congratulations! You fixed the broken build. Thank you!");
            } else
            {
                System.Console.WriteLine("The build is broken.");
                var postPatchGist = Gist.CreateGist(
                    _userName, _password,
                    string.Format("buildLog.after-pullRequest{0}.txt", pull.PullRequestNumber),
                    postPatchBuild.Output, false);

                comment.AppendLine(
                    prePatchBuild.Success
                        ? "Uh oh. This pull request breaks the build."
                        : "After applying your pull request, the build is still broken.");
                comment.AppendFormat("[Here's the build log.]({0})", postPatchGist.GistUrl);
            }

            comment.AppendLine();
            comment.AppendLine();
            comment.AppendLine("This is an automated message from [GitHub Builder](http://github.com/jasondentler/GitHubBuilder).");

            pull.Comment(_userName, _password, comment.ToString());

            System.Console.WriteLine(comment.ToString());
            System.Console.WriteLine("Press any key");
            System.Console.ReadKey();
        }

        public static void GetGitHubApiCredentials()
        {
            _userName = GetProcessOutput(GitPath, "config github.user");
            _password = GetProcessOutput(GitPath, "config github.token");
        }

        private static string GetProcessOutput(string fileName, string args)
        {
            var si = new ProcessStartInfo(fileName, args)
                         {
                             UseShellExecute = false,
                             RedirectStandardOutput = true
                         };
            var p = Process.Start(si);
            p.WaitForExit();
            return p.StandardOutput.ReadToEnd().Trim();
        }

    }
}
