using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Hosting;

namespace NSBEndpointInWebJob
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host
                .CreateDefaultBuilder()
                .ConfigureHost()
                .Build();

            var cancellationToken = new WebJobsShutdownWatcher().Token;
            using (host)
            {
                await host.RunAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
