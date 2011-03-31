using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace GitHubBuilder.Console
{

    public class Gist
    {
        private readonly string _repo;

        public static Gist CreateGist(
            string userName,
            string password,
            string fileName,
            string content,
            bool isPrivate
            )
        {
            const string url = "https://gist.github.com/api/v1/json/new";
            var postValues = new NameValueCollection();
            var extension = Path.GetExtension(fileName);
            if (!string.IsNullOrWhiteSpace(extension))
                postValues["file_ext[gistfile1]"] = extension.Substring(1);
            postValues["file_contents[gistfile1]"] = content;
            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                postValues["login"] = userName;
                postValues["token"] = password;
            }
            if (isPrivate)
                postValues["private"] = "on";
            postValues["file_name[gistfile1]"] = fileName;

            var wc = new WebClient();
            var data = wc.UploadValues(url, postValues);
            var jsonData = Encoding.UTF8.GetString(data);
            var o = JObject.Parse(jsonData);
            var gist = (JObject) o["gists"][0];
            return new Gist((string) gist["repo"]);
        }


        private Gist(string repo)
        {
            _repo = repo;
        }


        public string GistUrl { get { return string.Format("https://gist.github.com/{0}", _repo); } }

    }

}
