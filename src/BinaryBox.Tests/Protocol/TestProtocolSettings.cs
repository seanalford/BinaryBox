using BinaryBox.Checksum;
using FluentAssertions;
using Xunit;

namespace BinaryBox.Protocol.Test
{
    public class TestProtocolSettings
    {
        [Fact]
        public void TestDefaultProtocolSettings()
        {
            // Arrange
            IProtocolSettings protocolSettings = new ProtocolSettings();

            // Act

            // Assert
            protocolSettings.Checksum.Should().Be(ProtocolSettings.DEFAULT_CHECKSUM_TYPE);
            protocolSettings.ConnectRetries.Should().Be(ProtocolSettings.DEFAULT_CONNECT_RETIRES);
            protocolSettings.ReceiveRetries.Should().Be(ProtocolSettings.DEFAULT_RECEIVE_RETRIES);
            protocolSettings.SendRetries.Should().Be(ProtocolSettings.DEFAULT_SEND_RETIRES);

        }

        [Theory]
        [InlineData(ChecksumTypes.None, 1, 1, 1)]
        [InlineData(ChecksumTypes.LRC, 2, 2, 2)]
        [InlineData(ChecksumTypes.CRC16, 3, 3, 3)]
        public void TestConstrucorAssignedProtocolSettings(ChecksumTypes checksum, int connectRetries, int receiveRetries, int sendRetries)
        {
            // Arrange / Act
            IProtocolSettings protocolSettings = new ProtocolSettings()
            {
                Checksum = checksum,
                ConnectRetries = connectRetries,
                ReceiveRetries = receiveRetries,
                SendRetries = sendRetries,
            };

            // Assert
            protocolSettings.Checksum.Should().Be(checksum);
            protocolSettings.ConnectRetries.Should().Be(connectRetries);
            protocolSettings.ReceiveRetries.Should().Be(receiveRetries);
            protocolSettings.SendRetries.Should().Be(sendRetries);

        }

        [Theory]
        [InlineData(ChecksumTypes.None, 1, 1, 1)]
        [InlineData(ChecksumTypes.LRC, 2, 2, 2)]
        [InlineData(ChecksumTypes.CRC16, 3, 3, 3)]
        public void TestAssignedProtocolSettings(ChecksumTypes checksum, int connectRetries, int receiveRetries, int sendRetries)
        {
            // Arrange
            IProtocolSettings protocolSettings = new ProtocolSettings();

            // Act            
            protocolSettings.Checksum = checksum;
            protocolSettings.ConnectRetries = connectRetries;
            protocolSettings.ReceiveRetries = receiveRetries;
            protocolSettings.SendRetries = sendRetries;

            // Assert
            protocolSettings.Checksum.Should().Be(checksum);
            protocolSettings.ConnectRetries.Should().Be(connectRetries);
            protocolSettings.ReceiveRetries.Should().Be(receiveRetries);
            protocolSettings.SendRetries.Should().Be(sendRetries);

        }
    }
}
