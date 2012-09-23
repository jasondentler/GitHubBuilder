using System;
using System.Net;
using GitHubBuilder.AuthToken;
using RestSharp;

namespace GitHubBuilder.Model
{
    public class User
    {

        public int Id { get; set; }

        public static User Current()
        {

            var request = new RestRequest("/user", Method.GET);
            var client = new RestClient(RestApi.BaseUrl.ToString())
                             {
                                 Authenticator = new OAuth2UriQueryParameterAuthenticator(new TokenSource().GetToken()),
                             };

            var response = client.Execute<User>(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new ApplicationException(string.Format("{0}: {1}", response.StatusCode, response.StatusDescription));

            return response.Data;
        }

    }
}
