using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinaryBox.Core.System.IO.Test
{
    public class TestByteStreamManager_Read_EndOfText
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

            // Act

            // Assert
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
