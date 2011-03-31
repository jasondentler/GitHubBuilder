using System;
using System.Diagnostics;
using System.Text;

namespace GitHubBuilder.Console
{
    public abstract class BuildProcess
    {

        public string SourcePath { get; private set; }
        public string Output { get; private set; }
        public string ErrorOutput { get; private set; }
        public bool Success { get; private set; }

        protected BuildProcess(string sourcePath)
        {
            SourcePath = sourcePath;
        }

        protected abstract ProcessStartInfo BuildProcessStartInfo();

        public void Build()
        {
            var si = BuildProcessStartInfo();
            var p = Process.Start(si);
            var outputStream = p.StandardOutput;
            var errorStream = p.StandardError;
            var output = new StringBuilder();
            var errorOutput = new StringBuilder();
            while (!p.HasExited)
            {
                p.WaitForExit((int) TimeSpan.FromSeconds(1).TotalMilliseconds);
                var newOutput = outputStream.ReadToEnd();
                var newError = errorStream.ReadToEnd();
                output.Append(newOutput);
                errorOutput.Append(newError);
                Debug.Write(newOutput);
                Debug.Write(newError);
            }
            Output = output.ToString();
            ErrorOutput = errorOutput.ToString();
            Success = p.ExitCode == 0;
        }

    }
}
