using System;

namespace BinaryBox.Core.System.IO
{
    public interface IByteStreamSettings : IDisposable
    {
        int OpenCloseTimeout { get; set; }
        int BufferSize { get; set; }
        int PrimaryReadTimeout { get; set; }
        int SecondaryReadTimeout { get; set; }
        int WriteTimeout { get; set; }
    }
}
