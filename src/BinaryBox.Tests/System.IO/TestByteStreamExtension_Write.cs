﻿using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinaryBox.Core.System.IO.Test
{
    public class TestByteStreamExtension_Write
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
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.WriteAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.OK, true));

            // Act
            var result = await byteStream.WriteAsync(data, default);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.OK);
            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();
        }

        [Fact]
        public async Task TestCancel()
        {
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.WriteAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.Cancelled, false));
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

            // Act
            // NOTE: The cancellationTokenSource.Token here does not actually cause the cancellation as you might expect.
            //       It has been added to the test for completeness.  The byteStreamMananager simply passes the token along            
            //       to the byteStream.  In this case the byteStream substitution is faking the cancallation for us.
            var result = await byteStream.WriteAsync(new byte[] { 0, 1, 2 }, cancellationTokenSource.Token);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.Cancelled);
            result.Success.Should().BeFalse();
            result.Data.Should().BeFalse();
        }

        [Fact]
        public async Task TestNotOpen()
        {
            // Arrange                        
            IByteStream byteStream = Substitute.For<IByteStream>();

            // Act
            var result = await byteStream.WriteAsync(new byte[] { 0, 1, 2 }, default);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.NotOpen);
            result.Success.Should().BeFalse();
            result.Data.Should().BeFalse();

        }

        [Fact]
        public async Task TestUnhandledException()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.When(x => x.WriteAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())).Do(x => { throw new Exception(); });

            // Act
            Func<Task> func = async () => { await byteStream.WriteAsync(new byte[] { 0, 1, 2 }, default); };

            // Assert
            await func.Should().ThrowAsync<Exception>();

        }
    }
}
