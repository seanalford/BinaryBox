using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinaryBox.Core.System.IO.Test
{
    public class TestByteStreamManager_Close
    {
        // Test Close
        // * Success
        // * Cancel
        // * Already Close        
        // * Timeout
        // * Unhandled Exception

        [Fact]
        public async Task TestSuccess()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.OpenAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, ByteStreamState.Open));
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.CloseAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, ByteStreamState.Closed));
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());
            var result = await byteStreamManager.OpenAsync();

            // Act
            result = await byteStreamManager.CloseAsync();

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.OK);
            result.Success.Should().BeTrue();
            result.Data.Should().Be(ByteStreamState.Closed);
        }

        [Fact]
        public async Task TestCancel()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.OpenAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, ByteStreamState.Open));
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.CloseAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.Cancelled, ByteStreamState.Open));
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
            var result = await byteStreamManager.OpenAsync(cancellationTokenSource.Token);

            // Act
            // NOTE: The cancellationTokenSource.Token here does not actually cause the cancellation as you might expect.
            //       It has been added to the test for completeness.  The byteStreamMananager simply passes the token along            
            //       to the byteStream.  In this case the byteStream substitution is faking the cancallation for us.
            result = await byteStreamManager.CloseAsync(cancellationTokenSource.Token);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.Cancelled);
            result.Success.Should().BeFalse();
            result.Data.Should().Be(ByteStreamState.Open);
        }


        [Fact]
        public async Task TestAlreadyClosed()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.State.Returns(ByteStreamState.Closed);
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            // Act
            var result = await byteStreamManager.CloseAsync();

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.AlreadyClosed);
            result.Success.Should().BeFalse();
            result.Data.Should().Be(ByteStreamState.Closed);
        }

        [Fact]
        public async Task TestTimeout()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.CloseAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OpenCloseTimeout, ByteStreamState.Open));
            byteStream.State.Returns(ByteStreamState.Open);
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            // Act
            var result = await byteStreamManager.CloseAsync();

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.OpenCloseTimeout);
            result.Success.Should().BeFalse();
            result.Data.Should().Be(ByteStreamState.Open);
        }

        [Fact]
        public async Task TestUnhandledException()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.When(x => x.CloseAsync()).Do(x => { throw new Exception(); });
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            // Act
            Func<Task> func = async () => { await byteStreamManager.CloseAsync(); };

            // Assert
            await func.Should().ThrowAsync<Exception>();
        }
    }
}
