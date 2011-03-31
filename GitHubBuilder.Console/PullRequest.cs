using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace GitHubBuilder.Console
{

    public class PullRequest
    {
        private readonly string _userName;
        private readonly string _repoName;

        public PullRequest(string userName, string repoName, int pullRequestNumber)
        {
            PullRequestNumber = pullRequestNumber;
            _userName = userName;
            _repoName = repoName;
            Download();
        }

        private void Download()
        {
            var urlTemplate = @"https://github.com/api/v2/json/pulls/{0}/{1}/{2}";
            var o = JsonHelpers.GetJson(urlTemplate, _userName, _repoName, PullRequestNumber);
            var pull = (JObject) o["pull"];
            Parse(pull);
        }

        private void Parse(JObject pull)
        {
            Body = (string)pull["body"];
            Title = (string)pull["title"];
            var @base = (JObject)pull["base"];
            BaseSha = (string)@base["sha"];
            PatchUrl = (string)pull["patch_url"];
            IsOpen = (string)pull["state"] == "open";
        }

        public int PullRequestNumber { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public string BaseSha { get; private set; }
        public string PatchUrl { get; private set; }
        public bool IsOpen { get; private set; }
        
    }

}
