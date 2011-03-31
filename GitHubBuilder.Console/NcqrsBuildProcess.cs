using System.Diagnostics;
using System.IO;

namespace GitHubBuilder.Console
{
    public class NcqrsBuildProcess
        : BuildProcess
    {

        public NcqrsBuildProcess(string sourcePath) : base(sourcePath)
        {
        }

        protected override ProcessStartInfo BuildProcessStartInfo()
        {
            var nantPath = Path.Combine(SourcePath, "tools", "nant", "nant.exe");
            const string args = "/f:\"MAIN.build\"";
            return new ProcessStartInfo(nantPath, args)
                       {
                           WorkingDirectory = SourcePath,
                           RedirectStandardOutput = true,
                           UseShellExecute = false
                       };
        }

    }
}
