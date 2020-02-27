using System.Threading.Tasks;

namespace Toolbox.Connection.Tcp
{
    public interface ITcpConnection : IConnection
    {
        string Host { get; set; }
        int Port { get; set; }
        Task<ConnectionState> ConnectAsync(string host, int port);
    }
}
