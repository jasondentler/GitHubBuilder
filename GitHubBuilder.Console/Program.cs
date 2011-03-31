using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace GitHubBuilder.Console
{
    class Program
    {

        static void Main(string[] args)
        {
            var userName = "ncqrs";
            var repoName = "ncqrs";
            var path = @"c:\myNcqrsBuilder";

            var pull = new PullRequest(userName, repoName, 57);
            var data = JsonConvert.SerializeObject(pull);
            System.Console.WriteLine(data);
            Debug.WriteLine(data);
            
            System.Console.WriteLine("Downloading base source");
            var baseSource = new Source(userName, repoName);
            baseSource.Download(path, pull.BaseSha);

            var subDir = Path.Combine(
                path,
                string.Format("{0}-{1}-{2}",
                              userName, repoName, pull.BaseSha.Substring(0, 7)));

            System.Console.WriteLine("Source downloaded and unzipped to {0}.", subDir);
            System.Console.WriteLine("Building base source to prove build is not broken.");
            
            var nantPath = Path.Combine(subDir, "tools", "nant", "nant.exe");
            var nantParams = @"/f:""MAIN.build""";

            var si = new ProcessStartInfo(nantPath, nantParams);
            si.WorkingDirectory = subDir;
            var p = Process.Start(si);
            p.WaitForExit();
            var exitCode = p.ExitCode;

            System.Console.WriteLine("Exit code = {0}", exitCode);
            if (exitCode == 0)
                System.Console.WriteLine("It's working. Let's see what you broke.");



            System.Console.ReadKey();
        }



    }
}
