using BinaryBox.Core.System.IO;
using System.Threading.Tasks;

namespace BinaryBox.Connection.Tcp
{
    public interface ITcpConnection : IByteStream
    {
        string Host { get; set; }
        int Port { get; set; }
        Task<ByteStreamResponse<ByteStreamState>> OpenAsync(string host, int port);
    }
}
