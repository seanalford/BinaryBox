using Toolbox.Checksum;

namespace Toolbox.Protocol
{
    public interface IProtocolSettings
    {
        ChecksumTypes Checksum { get; set; }
        int ConnectRetries { get; set; }
        int ReceiveRetries { get; set; }
        int SendRetries { get; set; }
    }
}
