using BinaryBox.Protocol.Settings;
using Microsoft.Extensions.Logging;

namespace BinaryBox.Core.ProtocolV2
{
    public abstract class Protocol : IProtocol
    {
        public abstract void Dispose();

        public ILogger Log { get; protected set; }
        public IProtocolSettings Settings { get; protected set; }

        public Protocol(ILogger logger, IProtocolSettings settings)
        {
            Log = logger;
            Settings = settings;
        }
    }
}
