using GitHubBuilder.AuthToken;
using GitHubBuilder.Model;
using Topshelf;

namespace GitHubBuilder
{
    public class Service : ServiceControl
    {

        public bool Start(HostControl hostControl)
        {
            var token = new TokenSource().GetToken();

            var requests = PullRequest.List();

            return !string.IsNullOrWhiteSpace(token);
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }

    }
}
