using FluentAssertions;
using Xunit;

namespace BinaryBox.Checksum.Test
{
    public class TestChecksumCrcExtentions
    {
        [Theory]
        [InlineData(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 91, 11 })]
        [InlineData(new byte[] { 0, 0, 0, 0, 0, 3, 0, 1, 0, 0, 0, 1, 0, 1, 0, 8, 4, 0, 4, 0, 1, 0, 6, 3 }, new byte[] { 235, 115 })]
        [InlineData(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0 }, new byte[] { 43, 193 })]
        [InlineData(new byte[] { 48, 48, 48, 49, 65, 66, 48, 48, 50, 49, 48, 48, 70, 70, 50, 48, 67, 111, 114, 114, 32, 86, 111, 108, 117, 109, 101, 32, 32, 32, 32, 32, 32, 32, 32, 48, 48, 48, 114, 114, 32, 86, 111, 108, 117, 109, 101, 32, 32, 32, 32, 32, 32, 32, 32, 48, 48, 48, 48, 48, 48, 48, 48, 32, 32, 32, 32, 32 }, new byte[] { 63, 149 })]
        public void TestCrc16Checksum(byte[] data, byte[] pExpectedResult)
        {
            // Arrange
            byte[] result = new byte[2];

            // Act
            result = data.Crc16();

            // Assert
            result.Should().BeEquivalentTo(pExpectedResult);
        }
    }
}
