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

            var pull = new PullRequest(userName, repoName, 25);
            var data = JsonConvert.SerializeObject(pull);
            System.Console.WriteLine(data);
            Debug.WriteLine(data);
            
            System.Console.WriteLine("Downloading base source");
            var baseSource = new Source(userName, repoName);
            var sourceDir = baseSource.Download(path, pull.BaseSha);

            System.Console.WriteLine("Source downloaded and unzipped to {0}.", sourceDir);
            
            System.Console.WriteLine("Building base source to prove build is not broken.");
            var build = new NcqrsBuildProcess(sourceDir);
            build.Build();

            if (build.Success)
            {
                System.Console.WriteLine("It's working. Let's see what you broke.");
            } else
            {
                System.Console.WriteLine("The build was already broken. Here's the log:");
                System.Console.WriteLine(build.Output);
                System.Console.Error.WriteLine(build.ErrorOutput);
                System.Console.WriteLine("The build was already broken. Press any key to exit.");
            }
            
            System.Console.ReadKey();
        }



    }
}
