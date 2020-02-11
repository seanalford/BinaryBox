namespace Toolbox.Connection.Tcp
{
    //public class TcpConnection : Connection
    //{
    //    private TcpClient Client = null;
    //    [Reactive] public string Host { get; private set; }
    //    [Reactive] public int Port { get; private set; }

    //    public async override Task<ConnectionState> ConnectAsync()
    //    {
    //        State = ConnectionState.Connecting;

    //        if (Client == null)
    //        {
    //            Client = new TcpClient();
    //            Client.ReceiveTimeout = ReceiveTimeoutOuter;
    //            Client.SendTimeout = SendTimeout;
    //        }

    //        IPAddress ipAddress = Dns.GetHostAddresses(Host)[0];
    //        await Client.ConnectAsync(ipAddress, Port);

    //        return State = Client.Connected ? ConnectionState.Conneted : ConnectionState.Disconnected;
    //    }

    //    public async Task<ConnectionState> ConnectAsync(string host, int port)
    //    {
    //        Host = host;
    //        Port = port;
    //        return await ConnectAsync();
    //    }

    //    public async override Task<ConnectionState> DisconnectAsync()
    //    {
    //        State = ConnectionState.Disconnecting;
    //        await Task.Run(() => Client?.Close());
    //        Client?.Dispose();
    //        return State = Client.Connected ? ConnectionState.Conneted : ConnectionState.Disconnected;
    //    }

    //    public override void Dispose()
    //    {
    //        Client?.Close();
    //        Client?.Dispose();
    //    }

    //    public async override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    //    {
    //        return await Client.GetStream().ReadAsync(buffer, offset, count, cancellationToken);
    //    }

    //    public async override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    //    {
    //        await Client.GetStream().WriteAsync(buffer, offset, count, cancellationToken);
    //    }
    //}
}
