using Microsoft.Extensions.Logging;

namespace Toolbox.Protocol
{
    public abstract class Protocol : IProtocol
    {
        public abstract void Dispose();

        public ILogger Log { get; protected set; }

        public Protocol(ILogger logger)
        {
            Log = logger;
        }
    }
}
