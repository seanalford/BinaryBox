using FluentAssertions;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BinaryBox.Core.System.IO.Test
{
    public class TestByteStreamManager
    {
        // TODO:
        // Test Open
        // * Open Success
        // - Open Cancel
        // * Open Already Open        
        // ? Open Timeout
        // * Open Unhandled Exception        
        // ? Open Mutex Timeout ???        
        // Test Close
        // - Close Success
        // - Close Cancel
        // - Close Already Closed
        // - Close Timeout
        // - Close Unhandled Exception
        // - Close Mutex Timeout ???        
        // Test Read NumberOfBytes
        // - Success (1,2,3,4,5,n)
        // - Cancel
        // - NotOpen        
        // - PrimaryTimeout        
        // - SecondaryTimeout        
        // - Unhandled Exception        
        // - Mutex Timeout ???  
        // Test Read EndOfText
        // - Success (1,2,3,4,5,n)
        // - Cancel
        // - NotOpen        
        // - PrimaryTimeout        
        // - SecondaryTimeout        
        // - Unhandled Exception        
        // - Mutex Timeout ???  
        // Test Write
        // - Success (1,2,3,4,5,n)
        // - Cancel
        // - NotOpen        
        // - WriteTimeout                
        // - Unhandled Exception        
        // - Mutex Timeout ???

        [Fact]
        public async Task TestOpenSuccess()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.OpenAsync().Returns(new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OK, ByteStreamState.Open));
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            // Act
            var result = await byteStreamManager.OpenAsync();

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.OK);
            result.Success.Should().BeTrue();
            result.Data.Should().Be(ByteStreamState.Open);

        }

        [Fact]
        public async Task TestOpenAlreadyOpen()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.State.Returns(ByteStreamState.Open);
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            // Act
            var result = await byteStreamManager.OpenAsync();

            // Assert
            result.Status.Should().Be(ByteStreamResponseStatusCode.AlreadyOpen);
            result.Success.Should().BeFalse();
            result.Data.Should().Be(ByteStreamState.Open);

        }

        //[Fact]
        //public async Task TestOpenTimeout()
        //{
        //    // Arrange                             
        //    IByteStream byteStream = Substitute.For<IByteStream>();
        //    byteStream.When(x => x.OpenAsync()).Do(x => { TODO DELAY });
        //    IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

        //    // Act
        //    var result = await byteStreamManager.OpenAsync();

        //    // Assert
        //    result.Status.Code.Should().Be(ByteStreamManagerResponseStatusCode.OpenCloseTimeout);
        //    result.Status.Success.Should().BeFalse();
        //    result.Data.Should().Be(ByteStreamState.Closed);

        //}

        [Fact]
        public async Task TestOpenUnhandledException()
        {
            // Arrange                             
            IByteStream byteStream = Substitute.For<IByteStream>();
            byteStream.When(x => x.OpenAsync()).Do(x => { throw new Exception(); });
            IByteStreamManager byteStreamManager = new ByteStreamManager(byteStream, new ByteStreamSettings());

            // Act
            Func<Task> func = async () => { await byteStreamManager.OpenAsync(); };

            // Assert
            await func.Should().ThrowAsync<Exception>();

        }
    }
}
