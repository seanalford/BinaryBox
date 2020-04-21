using System;

namespace BinaryBox.Core.System.IO
{
    public interface IByteStreamSettings : IDisposable
    {
        int OpenCloseTimeout { get; set; }
        int BufferSize { get; set; }
        int ReadPrimaryTimeout { get; set; }
        int ReadSecondaryTimeout { get; set; }
        int WriteTimeout { get; set; }
    }

}
