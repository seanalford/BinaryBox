using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace BinaryBox.Connection.Test
{
    public sealed class ConnectionTests
    {
        public class TheConnectAsyncMethod
        {
            [Fact]
            public async Task ShouldCallConnectionTask()
            {
                // Arrange
                var client = Substitute.For<IClientStub>();
                client.Result<bool>().Returns(Task.FromResult(true));
                ConnectionMock connection = new ConnectionMockFixture().WithClient(client);

                // Act
                await connection.ConnectAsync();

                // Assert
                await client.Received().Result<bool>();
            }

            [Fact]
            public async Task ShouldReturnConnected()
            {
                // Arrange
                var client = Substitute.For<IClientStub>();
                client.Result<bool>().Returns(Task.FromResult(true));
                ConnectionMock connection = new ConnectionMockFixture().WithClient(client);

                // Act
                var result = await connection.ConnectAsync();

                // Assert
                result.Should().Be(ConnectionState.Connected);
            }

            [Fact]
            public async Task ShouldReturnDisconnected()
            {
                // Arrange
                var client = Substitute.For<IClientStub>();
                client.Result<bool>().Returns(Task.FromResult(false));
                ConnectionMock connection = new ConnectionMockFixture().WithClient(client);

                // Act
                var result = await connection.ConnectAsync();

                // Assert
                result.Should().Be(ConnectionState.Disconnected);
            }
        }

        public class TheDisconnectAsyncMethod
        {
            [Fact]
            public async Task ShouldCallDisconnectionTask()
            {
                // Arrange
                var client = Substitute.For<IClientStub>();
                client.Result<bool>().Returns(Task.FromResult(true));
                ConnectionMock connection = new ConnectionMockFixture().WithClient(client);

                // Act
                await connection.DisconnectAsync();

                // Assert
                await client.Received().Result<bool>();
            }
        
            [Fact]
            public async Task ShouldReturnDisconnected()
            {
                // Arrange
                var client = Substitute.For<IClientStub>();
                client.Result<bool>().Returns(Task.FromResult(true));
                ConnectionMock connection = new ConnectionMockFixture().WithClient(client);

                // Act
                var result = await connection.DisconnectAsync();

                // Assert
                result.Should().Be(ConnectionState.Disconnected);
            }

            [Fact]
            public async Task ShouldReturnConnected()
            {
                // Arrange
                var client = Substitute.For<IClientStub>();
                client.Result<bool>().Returns(Task.FromResult(false));
                ConnectionMock connection = new ConnectionMockFixture().WithClient(client);

                // Act
                var result = await connection.DisconnectAsync();

                // Assert
                result.Should().Be(ConnectionState.Connected);
            }
        }

        public class TheReadAsyncMethod
        {
            // TODO: This shows there is some flaw in the read as the call fires and never finishes.
            // [Fact]
            public async Task ShouldCallReadTask()
            {
                // Arrange
                var client = Substitute.For<IClientStub>();
                client.Result<bool>().Returns(Task.FromResult(true));
                ConnectionMock connection = new ConnectionMockFixture().WithClient(client);

                // Act
                await connection.ReadAsync(5, CancellationToken.None);

                // Assert
                await client.Received().Result<int>();
            }
        }
    }
}