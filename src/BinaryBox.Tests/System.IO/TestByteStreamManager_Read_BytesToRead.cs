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

        [Fact]
        public async Task TestSuccess()
        {
            // Arrange
            byte[][] data = new byte[][] { new byte[] { 1 }, new byte[] { 1, 2 }, new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4, 5 } };
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.OpenAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, ByteStreamState.Open));
            byteStream.State.Returns(ByteStreamState.Open);
            byteStream.DataAvailableAsync().Returns(new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.OK, true));
            byteStream.ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns
                (
                    x => { x[0] = data[1]; return new ByteStreamResponse<int>(ByteStreamResponseStatusCode.OK, x.ArgAt<int>(2)); },
                    x => { x[0] = data[2]; return new ByteStreamResponse<int>(ByteStreamResponseStatusCode.OK, x.ArgAt<int>(2)); },
                    x => { x[0] = data[3]; return new ByteStreamResponse<int>(ByteStreamResponseStatusCode.OK, x.ArgAt<int>(2)); },
                    x => { x[0] = data[4]; return new ByteStreamResponse<int>(ByteStreamResponseStatusCode.OK, x.ArgAt<int>(2)); },
                    x => { x[0] = data[5]; return new ByteStreamResponse<int>(ByteStreamResponseStatusCode.OK, x.ArgAt<int>(2)); }
                );
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            for (int i = 0; i < 5; i++)
            {
                // Act
                var result = await byteStreamManager.ReadAsync(i);

                // Assert
                result.Status.Should().Be(ByteStreamResponseStatusCode.OK);
                result.Success.Should().BeTrue();
                if (i == 1) result.Data.Should().BeEquivalentTo(data[i]);
            }
        }

        [Fact]
        public async Task TestCancel()
        {
            // Arrange                

            // Act

            // Assert
        }


        [Fact]
        public async Task TestNotOpen()
        {
            // Arrange                             

            // Act

            // Assert            

        }

        [Fact]
        public async Task TestPrimaryTimeout()
        {
            // Arrange                             

            // Act            

            // Assert
        }

        [Fact]
        public async Task TestSecondaryTimeout()
        {
            // Arrange                             

            // Act            

            // Assert            
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
