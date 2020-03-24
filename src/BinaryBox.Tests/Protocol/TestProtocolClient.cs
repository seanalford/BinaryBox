using BinaryBox.Checksum;
using BinaryBox.Connection;
using FluentAssertions;
using NSubstitute;
using System.Threading;
using Xunit;

namespace BinaryBox.Protocol.Test
{
    public class TestProtocolClient
    {
        // Test Tx Retries
        [Theory]
        [InlineData(ChecksumTypes.None, 1, ProtocolClientStatusCodes.SendRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 2, ProtocolClientStatusCodes.SendRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 3, ProtocolClientStatusCodes.SendRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 4, ProtocolClientStatusCodes.SendRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 5, ProtocolClientStatusCodes.SendRetryLimitExceeded)]
        public async void TestFakeClientSendTxRetiresFailedSend(ChecksumTypes checksum, int sendRetries, ProtocolClientStatusCodes expectedResult)
        {
            // Arrange                 
            CancellationToken cancellationToken = new CancellationToken();
            IConnection connection = Substitute.For<IConnection>();
            IFakeProtocolSettings settings = new FakeProtocolSettings() { Checksum = checksum, SendRetries = sendRetries };
            var client = new FakeClient(LoggerFactory.Build(), connection, settings);
            var message = FakeProtocol.Get(LoggerFactory.Build(), settings).Item(1);
            connection.WriteAsync(Arg.Any<byte[]>(), CancellationToken.None).Returns(false);

            // Act
            var result = await client.SendAsync(message, cancellationToken);

            // Assert
            result.Status.Should().Be(expectedResult);
        }

        // Test Tx Retries
        [Theory]
        [InlineData(ChecksumTypes.None, 1, ProtocolClientStatusCodes.SendRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 2, ProtocolClientStatusCodes.SendRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 3, ProtocolClientStatusCodes.SendRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 4, ProtocolClientStatusCodes.SendRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 5, ProtocolClientStatusCodes.SendRetryLimitExceeded)]
        public async void TestFakeClientSendTxRetiresNak(ChecksumTypes checksum, int sendRetries, ProtocolClientStatusCodes expectedResult)
        {
            // Arrange                 
            CancellationToken cancellationToken = new CancellationToken();
            IConnection connection = Substitute.For<IConnection>();
            IFakeProtocolSettings settings = new FakeProtocolSettings() { Checksum = checksum, SendRetries = sendRetries };
            var client = new FakeClient(LoggerFactory.Build(), connection, settings);
            var message = FakeProtocol.Get(LoggerFactory.Build(), settings).Item(1);
            connection.WriteAsync(Arg.Any<byte[]>(), CancellationToken.None).Returns(true);
            connection.ReadAsync(1, cancellationToken).Returns(message.Nak);

            // Act
            var result = await client.SendAsync(message, cancellationToken);

            // Assert
            result.Status.Should().Be(expectedResult);
        }

        // Test Rx Retries
        [Theory]
        [InlineData(ChecksumTypes.None, 1, ProtocolClientStatusCodes.ReceiveRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 2, ProtocolClientStatusCodes.ReceiveRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 3, ProtocolClientStatusCodes.ReceiveRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 4, ProtocolClientStatusCodes.ReceiveRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 5, ProtocolClientStatusCodes.ReceiveRetryLimitExceeded)]
        public async void TestFakeClientSendRxRetiresNoData(ChecksumTypes checksum, int receiveRetries, ProtocolClientStatusCodes expectedResult)
        {
            // Arrange                 
            CancellationToken cancellationToken = new CancellationToken();
            IConnection connection = Substitute.For<IConnection>();
            IFakeProtocolSettings settings = new FakeProtocolSettings() { Checksum = checksum, ReceiveRetries = receiveRetries };
            var client = new FakeClient(LoggerFactory.Build(), connection, settings);
            var message = FakeProtocol.Get(LoggerFactory.Build(), settings).Item(1);
            connection.WriteAsync(Arg.Any<byte[]>(), CancellationToken.None).Returns(true);
            connection.ReadAsync(1, cancellationToken).Returns(message.Ack);
            connection.ReadAsync((byte)MessageTokens.ETX, cancellationToken, settings.Checksum.Length()).Returns(new byte[0]);

            // Act
            //var result = await Record.ExceptionAsync(async () => await client.SendAsync(message, cancellationToken));
            var result = await client.SendAsync(message, cancellationToken);

            // Assert
            result.Status.Should().Be(expectedResult); //.BeOfType<ReceiveRetryLimitExceededException>();
        }

        // Test Rx Retries
        [Theory]
        [InlineData(ChecksumTypes.None, 1, ProtocolClientStatusCodes.ReceiveRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 2, ProtocolClientStatusCodes.ReceiveRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 3, ProtocolClientStatusCodes.ReceiveRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 4, ProtocolClientStatusCodes.ReceiveRetryLimitExceeded)]
        [InlineData(ChecksumTypes.None, 5, ProtocolClientStatusCodes.ReceiveRetryLimitExceeded)]
        public async void TestFakeClientSendRxRetiresWithData(ChecksumTypes checksum, int receiveRetries, ProtocolClientStatusCodes expectedResult)
        {
            // Arrange                 
            CancellationToken cancellationToken = new CancellationToken();
            IConnection connection = Substitute.For<IConnection>();
            IFakeProtocolSettings settings = new FakeProtocolSettings() { Checksum = checksum, ReceiveRetries = receiveRetries };
            var client = new FakeClient(LoggerFactory.Build(), connection, settings);
            var message = FakeProtocol.Get(LoggerFactory.Build(), settings).Item(1);
            connection.WriteAsync(Arg.Any<byte[]>(), CancellationToken.None).Returns(true);
            connection.ReadAsync(1, cancellationToken).Returns(message.Ack);
            connection.ReadAsync((byte)MessageTokens.ETX, cancellationToken, settings.Checksum.Length()).Returns(new byte[5] { 3, 2, 3, 4, 5 });

            // Act
            var result = await client.SendAsync(message, cancellationToken);

            // Assert
            result.Status.Should().Be(expectedResult);
        }

        [Theory]                                                                        // [STX][MSGT]  [Item 0-FFFF ]  [IEEE Float                  ][ETX]              
        [InlineData(ProtocolClientStatusCodes.OK, 1, 0, ChecksumTypes.None, new byte[] { 2, 48, 48, 48, 48, 48, 49, 48, 48, 48, 48, 48, 48, 48, 48, 3 })]
        [InlineData(ProtocolClientStatusCodes.OK, 2, 1, ChecksumTypes.None, new byte[] { 2, 48, 48, 48, 48, 48, 50, 51, 70, 56, 48, 48, 48, 48, 48, 3 })]
        public async void TestFakeClientSendValid(ProtocolClientStatusCodes expectedStatus, int expectedItem, float expectedValue, ChecksumTypes checksum, byte[] rxMessage)
        {
            // Arrange                        
            IConnection connection = Substitute.For<IConnection>();
            IFakeProtocolSettings settings = new FakeProtocolSettings() { Checksum = checksum };
            var client = new FakeClient(LoggerFactory.Build(), connection, settings);
            var message = FakeProtocol.Get(LoggerFactory.Build(), settings).Item(expectedItem);
            CancellationToken cancellationToken = new CancellationToken();

            //connection.WriteAsync(Arg.Is<byte[]>(x => x.SequenceEqual(message.Encode())), cancellationToken).Returns(true);
            connection.WriteAsync(Arg.Any<byte[]>(), cancellationToken).Returns(true);
            connection.ReadAsync(1, cancellationToken).Returns(message.Ack);
            connection.ReadAsync((byte)MessageTokens.ETX, cancellationToken, settings.Checksum.Length()).Returns(rxMessage);

            // Act
            var result = await client.SendAsync(message, CancellationToken.None);

            // Arrange
            result.Status.Should().Be(expectedStatus);
            message.Data.Item.Should().Be(expectedItem);
            message.Data.Value.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(ProtocolClientStatusCodes.OK, ChecksumTypes.None, new byte[] { 2, 48, 48, 48, 48, 48, 49, 48, 48, 48, 48, 48, 48, 48, 48, 3 })]
        public async void TestFakeClientSendValidNoTxValidate(ProtocolClientStatusCodes expectedStatus, ChecksumTypes checksum, byte[] rxMessage)
        {
            // Arrange                        
            IConnection connection = Substitute.For<IConnection>();
            IFakeProtocolSettings settings = new FakeProtocolSettings() { Checksum = checksum };
            var client = new FakeClient(LoggerFactory.Build(), connection, settings);
            var message = FakeProtocol.Set(LoggerFactory.Build(), settings);
            CancellationToken cancellationToken = new CancellationToken();
            connection.WriteAsync(Arg.Any<byte[]>(), cancellationToken).Returns(true);
            connection.ReadAsync((byte)MessageTokens.ETX, cancellationToken, settings.Checksum.Length()).Returns(rxMessage);

            // Act
            var result = await client.SendAsync(message, CancellationToken.None);

            // Arrange
            result.Status.Should().Be(expectedStatus);
        }
    }
}
