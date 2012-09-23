using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using GitHubBuilder.Model;
using RestSharp;

namespace GitHubBuilder.Operations
{
    public class Authentication
    {

        [Flags]
        public enum Scopes
        {
            PublicReadOnly = 1,
            UserProfile = 2,
            Public = 4,
            Repository = 8,
            RepositoryNoCode = 16,
            DeleteRepository = 32,
            Gist = 64
        }


        public AuthenticateResponse Authenticate(string userName, string password, Scopes scopes)
        {

            var request = new RestRequest("/authorizations", Method.POST)
                              {
                                  RequestFormat = DataFormat.Json
                              }
                .AddBody(new
                             {
                                 scopes = Convert(scopes),
                                 note = "GitHub Builder",
                                 note_url = ConfigurationManager.AppSettings["GitHubBuilderURL"]
                             });

            var client = new RestClient
                             {
                                 BaseUrl = RestApi.BaseUrl.ToString(),
                                 Authenticator = new HttpBasicAuthenticator(userName, password)
                             };

            var response = client.Execute<AuthenticateResponse>(request);

            if (response.StatusCode != HttpStatusCode.Created)
                throw new ApplicationException(string.Format("{0}: {1}", response.StatusCode, response.StatusDescription));

            return response.Data;
        }

        private static string[] Convert(Scopes scopes)
        {
            var items = new HashSet<string>();

            if ((scopes & Scopes.Gist) == Scopes.Gist)
                items.Add("gist");

            if ((scopes & Scopes.DeleteRepository) == Scopes.DeleteRepository)
                items.Add("delete_repo");

            if ((scopes & Scopes.RepositoryNoCode) == Scopes.RepositoryNoCode)
                items.Add("repo:status");

            if ((scopes & Scopes.Repository) == Scopes.Repository)
                items.Add("repo");

            if ((scopes & Scopes.Public) == Scopes.Public)
                items.Add("public_repo");

            if ((scopes & Scopes.UserProfile) == Scopes.UserProfile)
                items.Add("user");

            return items.ToArray();
        }


    }
}
