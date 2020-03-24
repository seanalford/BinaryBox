using Microsoft.Extensions.Logging;

namespace BinaryBox.Protocol
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
