using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinaryBox.Core.System.IO.Test
{
    public class TestByteStreamExtension_Read_EndOfText
    {
        // Test Read bytesToRead
        // - Success (1,2,3,4,5,n)
        // - Cancel
        // - NotOpen        
        // - PrimaryTimeout        
        // - SecondaryTimeout        
        // - Unhandled Exception                


        [Theory]
        [InlineData(1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 1 })]
        [InlineData(2, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 1, 2 })]
        [InlineData(3, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 1, 2, 3 })]
        [InlineData(4, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 1, 2, 3, 4 })]
        [InlineData(5, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 1, 2, 3, 4, 5 })]
        public async Task TestSuccess(byte endOfText, byte[] data, byte[] expected)
        {
            // Arrange    
            int index = 0;
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.DataAvailableAsync().Returns(new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.OK, true));
            byteStream.ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new ByteStreamResponse<int>(ByteStreamResponseStatusCode.OK, expected.Length));
            byteStream.WhenForAnyArgs(x => x.ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()))
                .Do(x =>
                {
                    x.Arg<byte[]>()[0] = data[index++];
                });

            // Act
            var result = await byteStream.ReadAsync(endOfText);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.OK);
            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(expected);
        }
    }
}
