using GitHubBuilder.AuthToken;
using Topshelf;

namespace GitHubBuilder
{
    public class Service : ServiceControl
    {

        public bool Start(HostControl hostControl)
        {
            var token = new TokenSource().GetToken();
            return !string.IsNullOrWhiteSpace(token);
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }

    }
}
