﻿using System.Diagnostics;
using System.IO;
using System.Net;

namespace GitHubBuilder.Console
{
    public class Patch
    {

        public static string DownloadAndApply(PullRequest pullRequest, string sourcePath, string gitPath)
        {
            var diffPath = Download(pullRequest);
            var applyLog = Apply(gitPath, sourcePath, diffPath);
            File.Delete(diffPath);
            return applyLog;
        }

        private static string Download(PullRequest pullRequest)
        {
            var diffUrl = Path.GetTempFileName();
            using (var wc = new WebClient())
            {
                wc.DownloadFile(pullRequest.DiffUrl, diffUrl);
                return diffUrl;
            }
        }

        private static string Apply(string gitPath, string sourcePath, string diffPath)
        {
            var args = string.Format("apply --whitespace=nowarn \"{0}\"", diffPath);
            var si = new ProcessStartInfo(gitPath, args)
                         {
                             WorkingDirectory = sourcePath,
                             UseShellExecute = false,
                             RedirectStandardOutput = true,
                             RedirectStandardError = true
                         };
            var p = Process.Start(si);
            p.WaitForExit();
            var errorOutput = p.StandardError.ReadToEnd();
            Debug.WriteLine(errorOutput);
            var output = p.StandardOutput.ReadToEnd();
            output = output + "\r\n" + errorOutput;
            return output;
        }

    }
}
