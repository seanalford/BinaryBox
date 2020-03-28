using FluentAssertions;
using System.Net;
using System.Net.Sockets;
using Xunit;

namespace BinaryBox.Connection.Tcp.Tests
{
    public class TestTcpConnection
    {
        TcpListener Server = null;

        private void StartServer(string ipAddress, int port)
        {
            // Set the TcpListener on port 13000.            
            IPAddress localAddr = IPAddress.Parse(ipAddress);

            // TcpListener server = new TcpListener(port);
            Server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            Server.Start();
        }

        [Theory]
        [InlineData("127.0.0.1", 7777, ConnectionState.Connected)]
        public async void TestDefaultConnectAsyncWithDefaultSettings(string host, int port, ConnectionState expectedResult)
        {
            // Arange            
            StartServer(host, port);
            using TcpConnection connection = new TcpConnection(null, new ConnectionSettings()) { Host = host, Port = port };

            // Act
            ConnectionState state = await connection.ConnectAsync();
            Server.Stop();

            // Assert
            state.Should().Be(expectedResult);
            connection.State.Should().Be(expectedResult);

        }

        [Theory]
        [InlineData("127.0.0.1", 7777, ConnectionState.Connected)]
        public async void TestConnectAsyncWithDefaultSettings(string host, int port, ConnectionState expectedResult)
        {
            // Arange            
            StartServer(host, port);
            using TcpConnection connection = new TcpConnection(null, new ConnectionSettings());

            // Act
            ConnectionState state = await connection.ConnectAsync(host, port);
            Server.Stop();

            // Assert
            state.Should().Be(expectedResult);
            connection.State.Should().Be(expectedResult);

        }

        [Theory]
        [InlineData("127.0.0.1", 7777, ConnectionState.Connected)]
        public async void TestConnectAsyncWithCustomSettings(string host, int port, ConnectionState expectedResult)
        {
            // Arange
            StartServer(host, port);
            using TcpConnection connection = new TcpConnection(null, new ConnectionSettings() { PrimaryReadTimeout = 500 });

            // Act
            ConnectionState state = await connection.ConnectAsync(host, port);
            Server.Stop();

            // Assert
            state.Should().Be(expectedResult);
            connection.State.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("127.0.0.1", 7777, ConnectionState.Disconnected)]
        public async void TestDisconnectAsyncWithDefaultSettings(string host, int port, ConnectionState expectedResult)
        {
            // Arange            
            StartServer(host, port);
            using TcpConnection connection = new TcpConnection(null, new ConnectionSettings());

            // Act
            ConnectionState state = await connection.ConnectAsync(host, port);
            state = await connection.DisconnectAsync();
            Server.Stop();

            // Assert
            state.Should().Be(expectedResult);
            connection.State.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("127.0.0.1", 7777, ConnectionState.Disconnected)]
        public async void TestDisconnectAsyncWithCustomSettings(string host, int port, ConnectionState expectedResult)
        {
            // Arange
            StartServer(host, port);
            using TcpConnection connection = new TcpConnection(null, new ConnectionSettings() { PrimaryReadTimeout = 500 });

            // Act
            ConnectionState state = await connection.ConnectAsync(host, port);
            state = await connection.DisconnectAsync();
            Server.Stop();

            // Assert
            state.Should().Be(expectedResult);
            connection.State.Should().Be(expectedResult);
        }
    }
}