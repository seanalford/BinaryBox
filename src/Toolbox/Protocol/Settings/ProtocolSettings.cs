using BinaryBox.Checksum;

namespace BinaryBox.Protocol
{
    public class ProtocolSettings : IProtocolSettings
    {
        public const ChecksumTypes DEFAULT_CHECKSUM_TYPE = ChecksumTypes.None;
        public const int DEFAULT_CONNECT_RETIRES = 3;
        public const int DEFAULT_RECEIVE_RETRIES = 3;
        public const int DEFAULT_SEND_RETIRES = 3;

        public ChecksumTypes Checksum { get; set; } = DEFAULT_CHECKSUM_TYPE;
        public int ConnectRetries { get; set; } = DEFAULT_CONNECT_RETIRES;
        public int ReceiveRetries { get; set; } = DEFAULT_RECEIVE_RETRIES;
        public int SendRetries { get; set; } = DEFAULT_SEND_RETIRES;
    }
}
