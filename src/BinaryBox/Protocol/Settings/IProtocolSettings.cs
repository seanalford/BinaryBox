using BinaryBox.Checksum;
using System;

namespace BinaryBox.Protocol.Settings
{
    public interface IProtocolSettings : IDisposable
    {
        int BufferSize { get; set; }
        ChecksumTypes Checksum { get; set; }
        int ConnectRetries { get; set; }
        int OpenCloseTimeout { get; set; }
        int PrimaryReadTimeout { get; set; }
        int ReceiveRetries { get; set; }
        int SendRetries { get; set; }
        int SecondaryReadTimeout { get; set; }
        int WriteTimeout { get; set; }
    }
}
