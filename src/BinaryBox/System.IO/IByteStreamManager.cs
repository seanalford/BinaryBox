using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Core.System.IO
{
    public interface IByteStreamManager : IDisposable
    {
        ILogger Log { get; }
        IByteStreamSettings Settings { get; set; }
        ByteStreamState State { get; }
        Task<ByteStreamManagerResponse<ByteStreamState>> OpenAsync();
        Task<ByteStreamManagerResponse<ByteStreamState>> CloseAsync();
        Task<ByteStreamManagerResponse<byte[]>> ReadAsync(int bytesToRead, CancellationToken cancellationToken = default);
        Task<ByteStreamManagerResponse<byte[]>> ReadAsync(byte endOfText, CancellationToken cancellationToken = default, int checksumLength = 0);
        Task<ByteStreamManagerResponse<bool>> WriteAsync(byte[] data, CancellationToken cancellationToken);
    }

}
