using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using Toolbox.Checksum;
using Xunit;

namespace Toolbox.Protocol.Test
{
    public static class LoggerFactory
    {
        public static ILogger Build()
        {

            ILogger logger = null;
            return logger;
        }
    }

    public class TestProtocolMessage
    {


        [Fact]
        public void TestDefaultProtocolMessage()
        {
            // Arrange / Act            
            IProtocolMessage<IFakeProtocolSettings, FakeProtcolMessageStatus> protocolMessage = new FakeProtocolMessageGet(LoggerFactory.Build(), new FakeProtocolSettings());

            //// Assert
            protocolMessage.Abort.Should().BeEquivalentTo(BitConverter.GetBytes(MessageTokens.ESC));
            protocolMessage.Ack.Should().BeEquivalentTo(BitConverter.GetBytes(MessageTokens.ACK));
            protocolMessage.Complete.Should().Be(true);
            protocolMessage.Nak.Should().BeEquivalentTo(BitConverter.GetBytes(MessageTokens.NAK));
            protocolMessage.RxBytesToRead.Should().Be(0);
            protocolMessage.RxEndOfMessageToken.Should().Be((byte)MessageTokens.ETX);
            protocolMessage.TxBytesToRead.Should().Be(1);
            protocolMessage.TxEndOfMessageToken.Should().Be(0);
            protocolMessage.ValidateTx.Should().BeTrue();
        }

        [Theory]
        [InlineData(FakeProtcolMessageStatus.SUCCESS, 1, 0, ChecksumTypes.None, new byte[] { 2, 48, 48, 48, 48, 48, 49, 48, 48, 48, 48, 48, 48, 48, 48, 3 })]
        [InlineData(FakeProtcolMessageStatus.SUCCESS, 2, 1, ChecksumTypes.None, new byte[] { 2, 48, 48, 48, 48, 48, 50, 51, 70, 56, 48, 48, 48, 48, 48, 3 })]
        public void TestProtocolMessageDecode(FakeProtcolMessageStatus expectedStatus, int expectedItem, float expectedValue, ChecksumTypes checksum, byte[] rxMessage)
        {
            // Arrange
            var protocolMessage = new FakeProtocolMessageGet(LoggerFactory.Build(), new FakeProtocolSettings() { Checksum = checksum });
            bool result = protocolMessage.Decode(rxMessage);

            // Act
            protocolMessage.DecodeData();

            // Arrange 
            result.Should().BeTrue();
            protocolMessage.Status.Should().Be(expectedStatus);
            protocolMessage.Data.Item.Should().Be(expectedItem);
            protocolMessage.Data.Value.Should().Be(expectedValue);

        }

        [Theory]
        [InlineData(new byte[] { 2, 48, 48, 48, 48, 48, 49, 48, 48, 48, 48, 48, 48, 48, 48, 3 }, ChecksumTypes.None)]
        [InlineData(new byte[] { 2, 48, 48, 48, 48, 48, 49, 48, 48, 48, 48, 48, 48, 48, 48, 3, 1 }, ChecksumTypes.LRC)]
        [InlineData(new byte[] { 2, 48, 48, 48, 48, 48, 49, 48, 48, 48, 48, 48, 48, 48, 48, 3, 242, 132 }, ChecksumTypes.CRC16)]
        public void TestProtocolMessageEncode(byte[] expected, ChecksumTypes checksum)
        {
            // Arrange            
            var protocolMessage = new FakeProtocolMessageGet(LoggerFactory.Build(), new FakeProtocolSettings() { Checksum = checksum });

            // Act
            protocolMessage.Item(1);
            byte[] result = protocolMessage.Encode();

            // Arrange
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(new byte[] { 2, 48, 48, 48, 48, 48, 49, 48, 48, 48, 48, 48, 48, 48, 48, 3 }, ChecksumTypes.None)]
        [InlineData(new byte[] { 2, 48, 48, 48, 48, 48, 49, 48, 48, 48, 48, 48, 48, 48, 48, 3, 1 }, ChecksumTypes.LRC)]
        [InlineData(new byte[] { 2, 48, 48, 48, 48, 48, 49, 48, 48, 48, 48, 48, 48, 48, 48, 3, 242, 132 }, ChecksumTypes.CRC16)]
        public void TestFakeProtocol(byte[] expected, ChecksumTypes checksum)
        {
            // Arrange
            var fakeProtocolMessage = FakeProtocol.Get(LoggerFactory.Build(), new FakeProtocolSettings() { Checksum = checksum });

            // Act
            fakeProtocolMessage.Item(1);
            byte[] result = fakeProtocolMessage.Encode();

            // Arrange
            result.Should().BeEquivalentTo(expected);
        }
    }

}
