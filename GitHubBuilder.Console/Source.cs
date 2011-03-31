using System.IO;
using System.Linq;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;

namespace GitHubBuilder.Console
{

    public class Source
    {

        private readonly string _userName;
        private readonly string _repoName;


        public Source(string userName, string repoName)
        {
            _userName = userName;
            _repoName = repoName;
        }

        private void DownloadZip(string path, string branchOrSha)
        {
            if (string.IsNullOrWhiteSpace(branchOrSha))
                branchOrSha = "master";

            var url = string.Format("https://github.com/{0}/{1}/zipball/{2}",
                                    _userName,
                                    _repoName,
                                    branchOrSha);

            var wc = new WebClient();
            wc.DownloadFile(url, path);
        }

        private string DownloadAndExtract(string path, string branchOrSha)
        {
            var zipPath = Path.GetTempFileName();
            DownloadZip(zipPath, branchOrSha);
            var fz = new FastZip();
            fz.ExtractZip(zipPath, path, null);
            File.Delete(zipPath);
            var sourceDirectory = Directory.GetDirectories(path).Single();
            return sourceDirectory;
        }

        private static void RepavePath(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            Directory.CreateDirectory(path);
        }

        public string Download(string path)
        {
            return Download(path, "master");
        }

        public string Download(string path, string branchOrSha)
        {
            RepavePath(path);
            return DownloadAndExtract(path, branchOrSha);
        }

    }

}
