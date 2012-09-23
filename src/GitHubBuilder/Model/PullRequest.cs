using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using GitHubBuilder.AuthToken;
using RestSharp;

namespace GitHubBuilder.Model
{
    public class PullRequest
    {

        private const string RepositoryUserKey = "Repository.User";
        private const string RepositoryNameKey = "Repository.Name";

        private static string RepositoryUser { get { return ConfigurationManager.AppSettings[RepositoryUserKey]; } }
        private static string RepositoryName { get { return ConfigurationManager.AppSettings[RepositoryNameKey]; } }

        public static PullRequest[] List()
        {
            if (string.IsNullOrWhiteSpace(RepositoryUser))
                throw new ApplicationException(string.Format("Application Settings should contain {0}", RepositoryUserKey));
            if (string.IsNullOrWhiteSpace(RepositoryName))
                throw new ApplicationException(string.Format("Application Settings should contain {0}", RepositoryNameKey));

            var request = new RestRequest("/repos/{user}/{repo}/pulls")
                .AddUrlSegment("user", RepositoryUser)
                .AddUrlSegment("repo", RepositoryName)
                .AddParameter("state", "open");

            var client = new RestClient()
                             {
                                 BaseUrl = RestApi.BaseUrl.ToString(),
                                 Authenticator = new OAuth2UriQueryParameterAuthenticator(new TokenSource().GetToken())
                             };

            var response = client.Execute<List<PullRequest>>(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new ApplicationException(string.Format("{0}: {1}", response.StatusCode, response.StatusDescription));
            return response.Data.ToArray();
        }

        public IEnumerable<Comment> Comments()
        {
            var request = new RestRequest(Links.Comments.Href, Method.GET);
            var client = new RestClient()
                             {
                                 Authenticator = new OAuth2UriQueryParameterAuthenticator(new TokenSource().GetToken())
                             };
            var response = client.Execute<List<Comment>>(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new ApplicationException(string.Format("{0}: {1}", response.StatusCode, response.StatusDescription));

            return response.Data;
        }

        public string Url { get; set; }
        public string DiffUrl { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        
        public Commit Base { get; set; }
        public Commit Head { get; set; }

        public LinksContainer Links { get; set; }
        
        public class LinksContainer
        {
            public Link Comments { get; set; }
            public Link ReviewComments { get; set; }
        }

        public class Link
        {
            public string Href { get; set; }
        }
        
    }
}
