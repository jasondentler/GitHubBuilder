using System;
using System.Configuration;

namespace GitHubBuilder
{
    public class RestApi
    {
        public static readonly Uri BaseUrl = new Uri(ConfigurationManager.AppSettings["GitHubAPI"], UriKind.Absolute);

    }
}
