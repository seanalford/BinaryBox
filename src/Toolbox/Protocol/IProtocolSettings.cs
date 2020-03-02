using Toolbox.Checksum;

namespace Toolbox.Protocol
{
    public interface IProtocolSettings
    {
        ChecksumTypes Checksum { get; }
        int ConnectRetries { get; }
        int ReceiveRetries { get; }
        int SendRetries { get; }
    }
}
