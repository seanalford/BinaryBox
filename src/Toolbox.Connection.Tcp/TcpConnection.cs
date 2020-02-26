using ReactiveUI.Fody.Helpers;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Connection.Tcp
{
    public interface ITcpConnection : IConnection
    {
        string Host { get; set; }
        int Port { get; set; }
        Task<ConnectionState> ConnectAsync(string host, int port);
    }

    public class TcpConnection : Connection, ITcpConnection
    {
        private TcpClient Client = null;
        [Reactive] public string Host { get; set; }
        [Reactive] public int Port { get; set; }

        #region Constructors
        public TcpConnection() : base(new ConnectionSettings())
        {

        }

        public TcpConnection(IConnectionSettings settings) : base(settings)
        {

        }
        #endregion

        public async Task<ConnectionState> ConnectAsync(string host, int port)
        {
            Host = host;
            Port = port;
            return await ConnectAsync();
        }

        public async override Task<bool> DataAvailableAsync()
        {
            return await Task.Run(() => Client.GetStream().DataAvailable);
        }

        public override void Dispose()
        {
            Client?.Close();
        }

        protected async override Task<bool> ConnectTask()
        {
            if (Client == null)
            {
                Client = new TcpClient
                {
                    ReceiveTimeout = Settings.ReceiveTimeoutOuter,
                    SendTimeout = Settings.SendTimeout
                };
            }

            IPAddress ipAddress = Dns.GetHostAddresses(Host)[0];
            await Client.ConnectAsync(ipAddress, Port);

            return Client.Connected;
        }

        protected async override Task<bool> DisconnectTask()
        {
            return await Task.Run(() =>
            {
                bool result = true;
                try
                {
                    Client?.Close();
                }
                catch (Exception ex)
                {
                    result = false;
                    //TODO:Log.Exception(ex);
                }
                return result;
            });
        }

        protected async override Task<int> ReadTask(byte[] data, CancellationToken cancellationToken)
        {
            return await Client.GetStream().ReadAsync(data, 0, data.Length, cancellationToken);
        }

        protected async override Task<bool> WriteTask(byte[] data, CancellationToken cancellationToken)
        {
            bool result = true;
            try
            {
                await Client.GetStream().WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                result = false;
                //TODO:Log.Exception(ex);
            }
            return result;
        }
    }
}
