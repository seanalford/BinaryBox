using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Core.System.IO
{
    public interface IByteStream : IDisposable
    {
        ILogger Log { get; }
        IByteStreamSettings Settings { get; set; }
        ByteStreamState State { get; }
        Task<ByteStreamManagerResponse<ByteStreamState>> OpenAsync();
        Task<ByteStreamManagerResponse<ByteStreamState>> CloseAsync();
        Task<ByteStreamManagerResponse<int>> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default);
        Task<ByteStreamManagerResponse<bool>> WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default);
    }

}
