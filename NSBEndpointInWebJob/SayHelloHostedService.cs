using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace NSBEndpointInWebJob
{
    public class SayHelloHostedService : IHostedService
    {
        public SayHelloHostedService(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            worker = SimulateWork(cancellationTokenSource.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource.Cancel();
            return worker;
        }

        async Task SimulateWork(CancellationToken cancellationToken)
        {
            try
            {
                var session = provider.GetService<IMessageSession>();

                while (!cancellationToken.IsCancellationRequested)
                {
                    await session.SendLocal(new SayHello()).ConfigureAwait(false);
                    await Task.Delay(2000, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        readonly IServiceProvider provider;
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task worker;
    }
}
