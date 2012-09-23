using Topshelf;

namespace GitHubBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
                                {
                                    x.Service<Service>(s =>
                                                           {
                                                               s.ConstructUsing(sf => new Service());
                                                               s.WhenStarted((s1, hc) => s1.Start(hc));
                                                               s.WhenStopped((s1, hc) => s1.Stop(hc));
                                                           });
                                    x.RunAsNetworkService();
                                    x.SetDescription("Builds & tests GitHub pull requests");
                                    x.SetDisplayName("GitHub Builder");
                                    x.SetServiceName("GitHubBuilder");
                                    x.DependsOnEventLog();
                                });
        }
    }
}
