using BinaryBox.Checksum;
using System;
using System.ComponentModel;

namespace BinaryBox.ProtocolV2.Settings
{
    public interface IProtocolSettings : IDisposable, INotifyPropertyChanged
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
