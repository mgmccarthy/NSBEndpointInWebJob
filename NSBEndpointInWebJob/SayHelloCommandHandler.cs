using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace NSBEndpointInWebJob
{
    public class SayHelloCommandHandler : IHandleMessages<SayHello>
    {
        private static readonly ILog Logger = LogManager.GetLogger<SayHelloCommandHandler>();

        public Task Handle(SayHello message, IMessageHandlerContext context)
        {
            Logger.Info("hello ...");
            return Task.CompletedTask;
        }
    }
}
