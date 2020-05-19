using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace NSBEndpointInWebJob
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureHost(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseNServiceBus(ctx =>
            {
                Console.Title = "NSBEndpointWebJob";
                var endpointConfiguration = new EndpointConfiguration("NSBEndpointWebJob");

                var transportConnectionString = ctx.Configuration.GetConnectionString("TransportConnectionString");

                var transport = endpointConfiguration.UseTransport<AzureStorageQueueTransport>();
                transport.ConnectionString(transportConnectionString);
                transport.SanitizeQueueNamesWith(queueName => queueName.Replace('.', '-'));

                endpointConfiguration.UsePersistence<InMemoryPersistence>();
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
                
                endpointConfiguration.SendFailedMessagesTo("NSBEndpointWebJob.Error");
                endpointConfiguration.AuditProcessedMessagesTo("NSBEndpointWebJob.Audit");

                endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);
                
                endpointConfiguration.EnableInstallers();

                return endpointConfiguration;
            });

            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddHostedService<SayHelloHostedService>();
            });

            return hostBuilder;
        }

        private static async Task OnCriticalError(ICriticalErrorContext context)
        {
            var fatalMessage = $"The following critical error was encountered:{Environment.NewLine}{context.Error}{Environment.NewLine}Process is shutting down. StackTrace: {Environment.NewLine}{context.Exception.StackTrace}";
            EventLog.WriteEntry(".NET Runtime", fatalMessage, EventLogEntryType.Error);
            try
            {
                await context.Stop().ConfigureAwait(false);
            }
            finally
            {
                Environment.FailFast(fatalMessage, context.Exception);
            }
        }
    }
}