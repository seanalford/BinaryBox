using BinaryBox.Checksum;

namespace BinaryBox.Protocol
{
    public interface IProtocolSettings
    {
        ChecksumTypes Checksum { get; set; }
        int ConnectRetries { get; set; }
        int ReceiveRetries { get; set; }
        int SendRetries { get; set; }
    }
}
