using System;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Toolbox.Checksum.Test
{
    public class TestChecksumLrcExtentions
    {
        [Theory]
        [InlineData(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, (byte)0)]
        [InlineData(new byte[] { 0, 0, 0, 0, 0, 3, 0, 1, 0, 0, 0, 1, 0, 1, 0, 8, 4, 0, 4, 0, 1, 0, 6, 3 }, (byte)14)]
        [InlineData(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0 }, (byte)1)]
        [InlineData(new byte[] { 48, 48, 48, 49, 65, 66, 48, 48, 50, 49, 48, 48, 70, 70, 50, 48, 67, 111, 114, 114, 32, 86, 111, 108, 117, 109, 101, 32, 32, 32, 32, 32, 32, 32, 32, 48, 48, 48, 114, 114, 32, 86, 111, 108, 117, 109, 101, 32, 32, 32, 32, 32, 32, 32, 32, 48, 48, 48, 48, 48, 48, 48, 48, 32, 32, 32, 32, 32 }, (byte)63)]
        public void TestLrcChecksumBytesToByte(byte[] data, byte pExpectedResult)
        {
            // Arrange, Act, Assert
            data.Lrc().Should().Be(pExpectedResult);
        }
    }
}
