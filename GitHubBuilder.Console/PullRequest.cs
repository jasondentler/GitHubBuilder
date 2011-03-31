using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace GitHubBuilder.Console
{

    public class PullRequest
    {
        private readonly string _repoUserName;
        private readonly string _repoName;

        public PullRequest(string repoUserName, string repoName, int pullRequestNumber)
        {
            PullRequestNumber = pullRequestNumber;
            _repoUserName = repoUserName;
            _repoName = repoName;
            Download();
        }

        private void Download()
        {
            var urlTemplate = @"https://github.com/api/v2/json/pulls/{0}/{1}/{2}";
            var o = JsonHelpers.GetJson(urlTemplate, _repoUserName, _repoName, PullRequestNumber);
            var pull = (JObject) o["pull"];
            Parse(pull);
        }

        private void Parse(JObject pull)
        {
            Body = (string)pull["body"];
            Title = (string)pull["title"];
            var @base = (JObject)pull["base"];
            BaseSha = (string)@base["sha"];
            DiffUrl = (string)pull["diff_url"];
            IsOpen = (string)pull["state"] == "open";
        }

        public int PullRequestNumber { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public string BaseSha { get; private set; }
        public string DiffUrl { get; private set; }
        public bool IsOpen { get; private set; }
     
   
        public void Comment(string userName, string token, string comment)
        {
            var urlTemplate = "https://github.com/api/v2/json/issues/comment/{0}/{1}/{2}";
            var url = string.Format(urlTemplate, _repoUserName, _repoName, PullRequestNumber);
            var postValues = new NameValueCollection();
            postValues["login"] = userName;
            postValues["token"] = token;
            postValues["comment"] = comment;
            using (var wc = new WebClient())
            {
                wc.UploadValues(url, postValues);
            }

        }

    }

}
