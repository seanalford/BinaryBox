using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinaryBox.Core.System.IO.Test
{
    public class TestByteStreamManager_Read_BytesToRead
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
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            // Act
            var result = await byteStreamManager.ReadAsync(bytesToRead);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.OK);
            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(expected);

        }

        [Fact]
        public async Task TestCancel()
        {
            // Arrange                        
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.OpenAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, ByteStreamState.Open));
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.DataAvailableAsync().Returns(new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.OK, true));
            byteStream.ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new ByteStreamResponse<int>(ByteStreamResponseStatusCode.Cancelled, 0));
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

            // Act
            // NOTE: The cancellationTokenSource.Token here does not actually cause the cancellation as you might expect.
            //       It has been added to the test for completeness.  The byteStreamMananager simply passes the token along            
            //       to the byteStream.  In this case the byteStream substitution is faking the cancallation for us.
            var result = await byteStreamManager.ReadAsync(10, cancellationTokenSource.Token);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.Cancelled);
            result.Success.Should().BeFalse();
            result.Data.Should().BeEquivalentTo(default);
        }


        [Fact]
        public async Task TestNotOpen()
        {
            // Arrange                        
            IByteStream byteStream = Substitute.For<IByteStream>();
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            // Act
            var result = await byteStreamManager.ReadAsync(10);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.NotOpen);
            result.Success.Should().BeFalse();
            result.Data.Should().BeEquivalentTo(default);
        }

        [Fact]
        public async Task TestPrimaryTimeout()
        {
            // Arrange                        
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.OpenAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, ByteStreamState.Open));
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.DataAvailableAsync().Returns(new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.OK, false));
            byteStream.ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new ByteStreamResponse<int>(ByteStreamResponseStatusCode.OK, 0));
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            // Act          
            var result = await byteStreamManager.ReadAsync(10);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.PrimaryReadTimeout);
            result.Success.Should().BeFalse();
            result.Data.Should().BeEquivalentTo(default);

        }

        [Fact]
        public async Task TestSecondaryTimeout()
        {
            // Arrange                        
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.OpenAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, ByteStreamState.Open));
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.DataAvailableAsync().Returns(new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.OK, true));
            byteStream.ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new ByteStreamResponse<int>(ByteStreamResponseStatusCode.OK, 0));
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            // Act          
            var result = await byteStreamManager.ReadAsync(10);

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.SecondaryReadTimeout);
            result.Success.Should().BeFalse();
            result.Data.Should().BeEquivalentTo(default);
        }

        [Fact]
        public async Task TestUnhandledException()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.OpenAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, ByteStreamState.Open));
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.DataAvailableAsync().Returns(new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.OK, true));
            byteStream.When(x => x.ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())).Do(x => { throw new Exception(); });
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            // Act
            Func<Task> func = async () => { await byteStreamManager.ReadAsync(10); };

            // Assert
            await func.Should().ThrowAsync<Exception>();

        }
    }
}
