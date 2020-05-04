using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinaryBox.Core.System.IO.Test
{
    public class TestByteStreamManager_Write
    {
        //Success​
        //Failure
        //Cancel​
        //Not Open        ​
        //Timeout​
        //Unhandled Exception

        [Theory]
        [InlineData( new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } )]
        public async Task TestSuccess(byte[] data)
        {
            // Arrange    
            int index = 0;
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.WriteAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.OK, true));

            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            // Act
            var result = await byteStreamManager.WriteAsync(data, default);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.OK);
            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();

        }
    }
}
