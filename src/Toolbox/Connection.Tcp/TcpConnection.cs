using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Connection.Tcp
{
    public class TcpConnection : Connection
    {
        private TcpClient Client = null;
        [Reactive] public string Host { get; private set; }
        [Reactive] public int Port { get; private set; }

        public async override Task<bool> ConnectAsync()
        {
            State = ConnectionState.Connecting;
            try
            {
                if (Client == null)
                {
                    Client = new TcpClient();
                    Client.SendTimeout = this.SendTimeout;
                    Client.ReceiveTimeout = this.ReceiveTimeoutOuter;
                }
                await Client.ConnectAsync(Host, Port);
                Connected = Client.Connected;
            }
            finally
            {
                if (Connected) State = ConnectionState.Conneted; else State = ConnectionState.Disconnected;
            }
            return Connected;
        }

        public async Task<bool> ConnectAsnyc(string host, int port)
        {
            Host = host;
            Port = port;
            return await this.ConnectAsync();
        }
        public async override Task<bool> DisconnectAsync()
        {
            State = ConnectionState.Disconnecting;
            try
            {
                if (Client != null) await Task.Run(() => Client.Close());
                Connected = Client.Connected;
            }
            finally
            {
                if (Connected) State = ConnectionState.Conneted; else State = ConnectionState.Disconnected;
                if (!Connected) { Client.Dispose(); Client = null; }
            }
            return !Connected;
        }

        public async override Task<bool> SendAsync(byte[] data, CancellationToken cancellationToken)
        {
            bool result = false;
            try
            {
                await Client.GetStream().WriteAsync(data, 0, data.Length, cancellationToken);
                result = true;
            }
            catch (Exception)
            {
                // TODO: Log Exception
                throw;
            }
            return result;
        }

        public async override Task<int> ReceiveAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            int result = 0;

            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                while(true)
                {
                    if(DataAvaliable)
                    {
                        while(DataAvaliable)
                        {
                            result += await Client.GetStream().ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        }
                        break;                        
                    }
                    if (stopwatch.ElapsedMilliseconds > ReceiveTimeoutOuter) throw new TimeoutException();
                }                
            }
            catch (Exception)
            {
                // TODO: Log Exception
                throw;
            }
            return result;
        }

        public Task SendAsync(object none)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            if (Client != null)
            {
                if (Client.Connected) Client.Close();
            }
            Client.Dispose();
        }

        public async override Task<byte[]> ReceiveAsync(int bytesToRead, CancellationToken cancellationToken)
        {
            // TODO: Fix this...  Use System.Net.Pipelines.Pipes
            byte[] buffer = new byte[1024];
            var bytesRead = await Client.GetStream().ReadAsync(buffer, 0, bytesToRead);
            return buffer;
        }
    }
}
