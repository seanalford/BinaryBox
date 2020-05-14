using BinaryBox.Core.System.IO;
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
        [InlineData("127.0.0.1", 7777, ByteStreamState.Open)]
        public async void TestDefaultConnectAsyncWithDefaultSettings(string host, int port, ByteStreamState expectedResult)
        {
            // Arange            
            StartServer(host, port);
            using TcpConnection connection = new TcpConnection(null, new FakeProtocolSettings()) { Host = host, Port = port };

            // Act
            var response = await connection.OpenAsync();
            Server.Stop();

            // Assert
            response.Data.Should().Be(expectedResult);
            connection.State.Should().Be(expectedResult);

        }

        [Theory]
        [InlineData("127.0.0.1", 7777, ByteStreamState.Open)]
        public async void TestConnectAsyncWithDefaultSettings(string host, int port, ByteStreamState expectedResult)
        {
            // Arange            
            StartServer(host, port);
            using TcpConnection connection = new TcpConnection(null, new FakeProtocolSettings());

            // Act
            var state = await connection.OpenAsync(host, port);
            Server.Stop();

            // Assert
            state.Data.Should().Be(expectedResult);
            connection.State.Should().Be(expectedResult);

        }

        [Theory]
        [InlineData("127.0.0.1", 7777, ByteStreamState.Open)]
        public async void TestConnectAsyncWithCustomSettings(string host, int port, ByteStreamState expectedResult)
        {
            // Arange
            StartServer(host, port);
            using TcpConnection connection = new TcpConnection(null, new FakeProtocolSettings() { PrimaryReadTimeout = 500 });

            // Act
            var state = await connection.OpenAsync(host, port);
            Server.Stop();

            // Assert
            state.Data.Should().Be(expectedResult);
            connection.State.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("127.0.0.1", 7777, ByteStreamState.Closed)]
        public async void TestDisconnectAsyncWithDefaultSettings(string host, int port, ByteStreamState expectedResult)
        {
            // Arange            
            StartServer(host, port);
            using TcpConnection connection = new TcpConnection(null, new FakeProtocolSettings());

            // Act
            var state = await connection.OpenAsync(host, port);
            state = await connection.CloseAsync();
            Server.Stop();

            // Assert
            state.Data.Should().Be(expectedResult);
            connection.State.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("127.0.0.1", 7777, ByteStreamState.Closed)]
        public async void TestDisconnectAsyncWithCustomSettings(string host, int port, ByteStreamState expectedResult)
        {
            // Arange
            StartServer(host, port);
            using TcpConnection connection = new TcpConnection(null, new FakeProtocolSettings() { PrimaryReadTimeout = 500 });

            // Act
            var state = await connection.OpenAsync(host, port);
            state = await connection.CloseAsync();
            Server.Stop();

            // Assert
            state.Should().Be(expectedResult);
            connection.State.Should().Be(expectedResult);
        }
    }
}