using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using Toolbox.Checksum;
using Toolbox.Connection;
using Xunit;

namespace Toolbox.Protocol.Test
{
    public class TestProtocolClient
    {
        // Test Tx Retries
        [Theory]
        [InlineData(ChecksumTypes.None, 1)]
        [InlineData(ChecksumTypes.None, 2)]
        [InlineData(ChecksumTypes.None, 3)]
        [InlineData(ChecksumTypes.None, 4)]
        [InlineData(ChecksumTypes.None, 5)]
        public async void TestFakeClientSendTxRetiresFailedSend(ChecksumTypes checksum, int sendRetries)
        {
            // Arrange                 
            CancellationToken cancellationToken = new CancellationToken();
            IConnection connection = Substitute.For<IConnection>();
            IFakeProtocolSettings settings = new FakeProtocolSettings() { Checksum = checksum, SendRetries = sendRetries };
            var client = new FakeClient(connection, settings);
            var message = FakeProtocol.Get(settings).Item(1);
            connection.WriteAsync(Arg.Any<byte[]>(), CancellationToken.None).Returns(false);

            // Act
            var result = await Record.ExceptionAsync(async () => await client.SendAsync(message, cancellationToken));

            // Assert
            result.Should().BeOfType<SendRetryLimitExceededException>();
        }

        // Test Tx Retries
        [Theory]
        [InlineData(ChecksumTypes.None, 1)]
        [InlineData(ChecksumTypes.None, 2)]
        [InlineData(ChecksumTypes.None, 3)]
        [InlineData(ChecksumTypes.None, 4)]
        [InlineData(ChecksumTypes.None, 5)]
        public async void TestFakeClientSendTxRetiresNak(ChecksumTypes checksum, int sendRetries)
        {
            // Arrange                 
            CancellationToken cancellationToken = new CancellationToken();
            IConnection connection = Substitute.For<IConnection>();
            IFakeProtocolSettings settings = new FakeProtocolSettings() { Checksum = checksum, SendRetries = sendRetries };
            var client = new FakeClient(connection, settings);
            var message = FakeProtocol.Get(settings).Item(1);
            connection.WriteAsync(Arg.Any<byte[]>(), CancellationToken.None).Returns(true);
            connection.ReadAsync(1, cancellationToken).Returns(message.Nak);

            // Act
            var result = await Record.ExceptionAsync(async () => await client.SendAsync(message, cancellationToken));

            // Assert
            result.Should().BeOfType<SendRetryLimitExceededException>();
        }

        // Test Rx Retries
        [Theory]
        [InlineData(ChecksumTypes.None, 1)]
        [InlineData(ChecksumTypes.None, 2)]
        [InlineData(ChecksumTypes.None, 3)]
        [InlineData(ChecksumTypes.None, 4)]
        [InlineData(ChecksumTypes.None, 5)]
        public async void TestFakeClientSendRxRetires(ChecksumTypes checksum, int receiveRetries)
        {
            // Arrange                 
            CancellationToken cancellationToken = new CancellationToken();
            IConnection connection = Substitute.For<IConnection>();
            IFakeProtocolSettings settings = new FakeProtocolSettings() { Checksum = checksum, ReceiveRetries = receiveRetries };
            var client = new FakeClient(connection, settings);
            var message = FakeProtocol.Get(settings).Item(1);
            connection.WriteAsync(Arg.Any<byte[]>(), CancellationToken.None).Returns(true);
            connection.ReadAsync(1, cancellationToken).Returns(message.Ack);
            connection.ReadAsync((byte)MessageTokens.ETX, cancellationToken, settings.Checksum.Length()).Returns(BitConverter.GetBytes(MessageTokens.NAK));

            // Act
            var result = await Record.ExceptionAsync(async () => await client.SendAsync(message, cancellationToken));

            // Assert
            result.Should().BeOfType<ReceiveRetryLimitExceededException>();
        }

        [Theory]                                                                        // [STX][MSGT]  [Item 0-FFFF ]  [IEEE Float                  ][ETX]              
        [InlineData(FakeProtcolMessageStatus.SUCCESS, 1, 0, ChecksumTypes.None, new byte[] { 2, 48, 48, 48, 48, 48, 49, 48, 48, 48, 48, 48, 48, 48, 48, 3 })]
        [InlineData(FakeProtcolMessageStatus.SUCCESS, 2, 1, ChecksumTypes.None, new byte[] { 2, 48, 48, 48, 48, 48, 50, 51, 70, 56, 48, 48, 48, 48, 48, 3 })]
        public async void TestFakeClientSendValid(FakeProtcolMessageStatus expectedStatus, int expectedItem, float expectedValue, ChecksumTypes checksum, byte[] rxMessage)
        {
            // Arrange                        
            IConnection connection = Substitute.For<IConnection>();
            IFakeProtocolSettings settings = new FakeProtocolSettings() { Checksum = checksum };
            var client = new FakeClient(connection, settings);
            var message = FakeProtocol.Get(settings).Item(expectedItem);
            CancellationToken cancellationToken = new CancellationToken();

            //connection.WriteAsync(Arg.Is<byte[]>(x => x.SequenceEqual(message.Encode())), cancellationToken).Returns(true);
            connection.WriteAsync(Arg.Any<byte[]>(), cancellationToken).Returns(true);
            connection.ReadAsync(1, cancellationToken).Returns(message.Ack);
            connection.ReadAsync((byte)MessageTokens.ETX, cancellationToken, settings.Checksum.Length()).Returns(rxMessage);

            // Act
            FakeProtcolMessageStatus result = await client.SendAsync(message, CancellationToken.None);

            // Arrange
            result.Should().Be(expectedStatus);
            message.Data.Item.Should().Be(expectedItem);
            message.Data.Value.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(FakeProtcolMessageStatus.SUCCESS, ChecksumTypes.None, new byte[] { 2, 48, 48, 48, 48, 48, 49, 48, 48, 48, 48, 48, 48, 48, 48, 3 })]
        public async void TestFakeClientSendValidNoTxValidate(FakeProtcolMessageStatus expectedStatus, ChecksumTypes checksum, byte[] rxMessage)
        {
            // Arrange                        
            IConnection connection = Substitute.For<IConnection>();
            IFakeProtocolSettings settings = new FakeProtocolSettings() { Checksum = checksum };
            var client = new FakeClient(connection, settings);
            var message = FakeProtocol.Set(settings);
            CancellationToken cancellationToken = new CancellationToken();
            connection.WriteAsync(Arg.Any<byte[]>(), cancellationToken).Returns(true);
            connection.ReadAsync((byte)MessageTokens.ETX, cancellationToken, settings.Checksum.Length()).Returns(rxMessage);

            // Act
            FakeProtcolMessageStatus result = await client.SendAsync(message, CancellationToken.None);

            // Arrange
            result.Should().Be(expectedStatus);
        }
    }
}
