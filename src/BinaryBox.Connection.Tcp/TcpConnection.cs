using BinaryBox.Core.System.IO;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Connection.Tcp
{
    public class TcpConnection : ByteStream, ITcpConnection
    {
        public string Host { get; set; }
        public int Port { get; set; }

        private TcpClient Client = null;

        public TcpConnection(ILogger logger, IByteStreamSettings settings) : base(logger, settings)
        {
        }

        public async override Task<ByteStreamResponse<ByteStreamState>> CloseAsync(CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<ByteStreamState> result = default;
            try
            {
                if (Client?.Connected == false) { result = new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.AlreadyClosed, ByteStreamState.Closed); }
                if (result == default)
                {
                    var reponse = await Task.Run(() =>
                    {
                        Client.Close();
                        return Client.Connected == false;
                    });

                    if (reponse)
                    {
                        State = ByteStreamState.Closed;
                        Client.Dispose();
                        Client = null;
                    }

                    result = new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, State);
                }
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }

        public async override Task<ByteStreamResponse<bool>> DataAvailableAsync()
        {
            ByteStreamResponse<bool> result = default;
            try
            {
                var reponse = await Task.Run(() => { return Client?.GetStream().DataAvailable == true; });
                result = new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.OK, reponse);
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }

        public override void Dispose()
        {
            DisposeClient();
        }

        private void DisposeClient()
        {
            if (Client != null)
            {
                if (Client.Connected) Client.Close();
                Client.Dispose();
            }
        }

        public override async Task<ByteStreamResponse<ByteStreamState>> OpenAsync(CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<ByteStreamState> result = default;
            try
            {
                if (Client?.Connected == true) { result = new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.AlreadyOpen, ByteStreamState.Open); }
                if (result == default)
                {
                    DisposeClient();
                    Client = new TcpClient
                    {
                        ReceiveTimeout = Settings.SecondaryReadTimeout,
                        SendTimeout = Settings.WriteTimeout
                    };

                    IPAddress ipAddress = Dns.GetHostAddresses(Host)[0];
                    await Client.ConnectAsync(ipAddress, Port);

                    if (Client.Connected)
                    {
                        State = ByteStreamState.Open;
                        result = new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, State);
                    }
                    else
                    {
                        result = new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.Failed, State);
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }

        public async override Task<ByteStreamResponse<int>> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<int> result = default;
            try
            {
                if (Client?.Connected == false) { result = new ByteStreamResponse<int>(ByteStreamResponseStatusCode.NotOpen, 0); }
                if (result == default)
                {
                    var responce = await Client.GetStream().ReadAsync(buffer, offset, count, cancellationToken);

                    // NOTE: WriteAsync only returns number of bytes written so we're assuming OK, and that
                    //       an exception will be thrown otherwise.
                    result = new ByteStreamResponse<int>(ByteStreamResponseStatusCode.OK, responce);
                }
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }

        public async override Task<ByteStreamResponse<bool>> WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<bool> result = default;
            try
            {
                if (Client?.Connected == false) { result = new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.NotOpen, false); }
                if (result == default)
                {
                    await Client.GetStream().WriteAsync(buffer, offset, count, cancellationToken);

                    // NOTE: WriteAsync does not return a status so we're assuming OK, and that
                    //       an exception will be thrown otherwise.
                    result = new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.OK, true);
                }
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }

        public async Task<ByteStreamResponse<ByteStreamState>> OpenAsync(string host, int port)
        {
            ByteStreamResponse<ByteStreamState> result = default;
            try
            {
                if (Client?.Connected == true) { result = new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.AlreadyOpen, ByteStreamState.Open); }
                if (result == default)
                {
                    Host = host;
                    Port = port;
                    result = await OpenAsync();
                }
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }
    }
}
