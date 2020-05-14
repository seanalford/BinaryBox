using BinaryBox.Protocol.Settings;
using BinaryBox.Protocol.Test;
using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinaryBox.Core.System.IO.Test
{   
    public static class TestExtension
    {
        public static Task<T> Delayed<T>(this T value, int milliseconds) => Task.Delay(milliseconds).ContinueWith(x => value);
    }

    public class TestByteStreamManager_Open
    {
        // Test Open
        // * Success
        // * Cancel
        // * Already Open        
        // * Timeout
        // * Unhandled Exception

        [Fact]
        public async Task TestSuccess()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.OpenAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, ByteStreamState.Open));
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new FakeProtocolSettings());

            // Act
            var result = await byteStreamManager.OpenAsync();

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.OK);
            result.Success.Should().BeTrue();
            result.Data.Should().Be(ByteStreamState.Open);
        }

        [Fact]
        public async Task TestCancel()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.OpenAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.Cancelled, ByteStreamState.Closed));
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new FakeProtocolSettings());
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

            // Act
            // NOTE: The cancellationTokenSource.Token here does not actually cause the cancellation as you might expect.
            //       It has been added to the test for completeness.  The byteStreamMananager simply passes the token along            
            //       to the byteStream.  In this case the byteStream substitution is faking the cancallation for us.
            var result = await byteStreamManager.OpenAsync(cancellationTokenSource.Token);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.Cancelled);
            result.Success.Should().BeFalse();
            result.Data.Should().Be(ByteStreamState.Closed);
        }


        [Fact]
        public async Task TestAlreadyOpen()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.State.Returns(ByteStreamState.Open);
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new FakeProtocolSettings());

            // Act
            var result = await byteStreamManager.OpenAsync();

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.AlreadyOpen);
            result.Success.Should().BeFalse();
            result.Data.Should().Be(ByteStreamState.Open);

        }

        [Fact]
        public async Task TestTimeout()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.OpenAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OpenCloseTimeout, ByteStreamState.Closed));
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new FakeProtocolSettings());

            // Act
            var result = await byteStreamManager.OpenAsync();

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.OpenCloseTimeout);
            result.Success.Should().BeFalse();
            result.Data.Should().Be(ByteStreamState.Closed);
        }

        [Fact]
        public async Task TestUnhandledException()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.When(x => x.OpenAsync()).Do(x => { throw new Exception(); });
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new FakeProtocolSettings());

            // Act
            Func<Task> func = async () => { await byteStreamManager.OpenAsync(); };

            // Assert
            await func.Should().ThrowAsync<Exception>();

        }
    }
}
