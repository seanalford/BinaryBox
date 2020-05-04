using BinaryBox.Core.System.IO;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinaryBox.Tests.System.IO
{
    public class TestByteStreamExtension_ReadAsync_BytesToRead
    {
        [Theory]
        [InlineData(1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 1 })]
        [InlineData(2, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 1, 2 })]
        [InlineData(3, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 1, 2, 3 })]
        [InlineData(4, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 1, 2, 3, 4 })]
        [InlineData(5, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new byte[] { 1, 2, 3, 4, 5 })]
        public async Task TestSuccess(int bytesToRead, byte[] data, byte[] expected)
        {
            // Arrange                        
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.OpenAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, ByteStreamState.Open));
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.DataAvailableAsync().Returns(new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.OK, true));
            byteStream.ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new ByteStreamResponse<int>(ByteStreamResponseStatusCode.OK, bytesToRead));
            byteStream.WhenForAnyArgs(x => x.ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()))
                .Do(x =>
                {
                    for (int i = 0; i < bytesToRead; i++)
                    {
                        x.Arg<byte[]>()[i] = data[i];
                    }
                });

            // Act
            var result = await byteStream.ReadAsync(bytesToRead);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.OK);
            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(expected);

        }
    }
}
