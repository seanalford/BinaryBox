using BinaryBox.Checksum;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BinaryBox.Connection.Test
{
    public partial class TestConnection
    {

        public static IConnectionSettings DefaultConnectionSettings()
        {
            IConnectionSettings connectionSettings = new ConnectionSettings
            {
                PrimaryReadTimeout = 15000,
                SecondaryReadTimeout = 1000,
            };
            return connectionSettings;
        }

        #region Basic Connection Test - Connect, Disconnect, DataAvailable, Write
        [Fact]
        public void TestConnectionCreation()
        {
            // Arrange
            IConnectionSettings connectionSettings = DefaultConnectionSettings();

            // Act
            using (ConnectionFake connection = new ConnectionFake(null, connectionSettings))
            {
                // Assert
                connection.Should().BeAssignableTo<IConnection>();
            }
        }

        [Fact]
        public async Task TestConnectionConnectAsync()
        {
            // Arrange            
            using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()) { ExpectedResult = true })
            {
                // Act
                ConnectionState result = await connection.ConnectAsync();

                // Assert
                result.Should().Be(ConnectionState.Connected);
            }
        }

        [Fact]
        public async Task TestConnectionConnectAsyncWithDelay()
        {
            // Arrange            
            using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()) { ExpectedResult = true, TaskDelay = 100 })
            {
                // Act
                connection.ConnectAsync();

                // Assert
                connection.State.Should().Be(ConnectionState.Connecting);
            }
        }

        [Fact]
        public async Task TestConnectionDisconnectAsync()
        {
            // Arrange            
            using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()) { ExpectedResult = true })
            {
                // Act
                await connection.ConnectAsync();
                await connection.DisconnectAsync();

                // Assert
                connection.State.Should().Be(ConnectionState.Disconnected);
            }
        }

        [Fact]
        public async Task TestConnectionDisconnectAsyncWithDelay()
        {
            // Arrange            
            using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()) { ExpectedResult = true, TaskDelay = 100 })
            {
                // Act
                await connection.ConnectAsync();
                connection.DisconnectAsync();

                // Assert
                connection.State.Should().Be(ConnectionState.Disconnecting);
            }
        }

        [Fact]
        public async Task TestConnectionNoDataAvailable()
        {
            // Arrange            
            using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
            {
                // Act
                var result = await connection.DataAvailableAsync();

                // Assert
                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task TestConnectionDataAvailable()
        {
            // Arrange            
            using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
            {
                // Act
                await connection.WriteToRxBuffer(new byte[] { 1 });
                bool dataAvailable = await connection.DataAvailableAsync();

                // Assert
                dataAvailable.Should().BeTrue();
            }
        }

        [Fact]
        public async Task TestConnectionWriteAsync()
        {
            // Arrange                        
            using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
            {
                byte[] txMessage = { 1, 2, 3, 4, 5 };

                // Act
                bool result = await connection.WriteAsync(txMessage, CancellationToken.None);

                // Assert
                result.Should().BeTrue();
            }
        }
        #endregion

        #region Connection Cancel Tests
        // [Theory()]
        // [InlineData(new byte[] { 1, 2, 3, 4 })]
        // public async Task TestConnectionCancelPrimaryReadAsyncNumberOfBytes(byte[] rxMessage)
        // {
        //     // Arrange
        //     using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
        //     {
        //         await connection.WriteToRxBuffer(rxMessage, 500);
        //         CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
        //
        //         // Act
        //         var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(1, cancellationTokenSource.Token));
        //
        //         // Assert
        //         result.Should().BeOfType<OperationCanceledException>();
        //         result.Message.Should().Be(Connection.PRIMARY_READ_CANCELLATION_EXCEPTION);
        //     }
        // }
        //
        // [Theory()]
        // [InlineData(new byte[] { 1, 2, 3, 4 })]
        // public async Task TestConnectionCancelPrimaryReadAsyncEndOfText(byte[] rxMessage)
        // {
        //     // Arrange
        //     using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
        //     {
        //         await connection.WriteToRxBuffer(rxMessage, 500);
        //         CancellationTokenSource cancellationTokenSource =
        //             new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
        //
        //         // Act
        //         var result = await Record.ExceptionAsync(async () => await connection.ReadAsync((byte)5, cancellationTokenSource.Token));
        //
        //         // Assert
        //         result.Should().BeOfType<OperationCanceledException>();
        //         result.Message.Should().Be(Connection.PRIMARY_READ_CANCELLATION_EXCEPTION);
        //     }
        // }
        //
        // [Theory()]
        // [InlineData(new byte[] { 1, 2, 3, 4 })]
        // public async Task TestConnectionCancelSecondaryReadAsyncNumberOfBytes(byte[] rxMessage)
        // {
        //     // Arrange
        //     using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
        //     {
        //         await connection.WriteToRxBuffer(rxMessage);
        //         CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
        //
        //         // Act
        //         var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(5, cancellationTokenSource.Token));
        //
        //         // Assert
        //         result.Should().BeOfType<OperationCanceledException>();
        //         result.Message.Should().Be(Connection.SECONDARY_READ_CANCELLATION_EXCEPTION);
        //     }
        // }
        //
        // [Theory()]
        // [InlineData(new byte[] { 1, 2, 3, 4 })]
        // public async Task TestConnectionCancelSecondaryReadAsyncEndOfText(byte[] rxMessage)
        // {
        //     // Arrange
        //     using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
        //     {
        //         await connection.WriteToRxBuffer(rxMessage);
        //         CancellationTokenSource cancellationTokenSource =
        //             new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
        //
        //         // Act
        //         var result = await Record.ExceptionAsync(async () => await connection.ReadAsync((byte)5, cancellationTokenSource.Token));
        //
        //         // Assert
        //         result.Should().BeOfType<OperationCanceledException>();
        //         result.Message.Should().Be(Connection.SECONDARY_READ_CANCELLATION_EXCEPTION);
        //     }
        // }
        #endregion

        #region Connection Timeout Tests
        // [Theory()]
        // [InlineData(10)]
        // [InlineData(100)]
        // [InlineData(1000)]
        // public async Task TestConnectionTimeoutPrimaryReadAsyncNumberOfBytes(int primaryTimeout)
        // {
        //     // Arrange
        //     using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
        //     {
        //         connection.Settings.PrimaryReadTimeout = primaryTimeout;
        //         // Act
        //         var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(1, CancellationToken.None));
        //
        //         // Assert
        //         result.Should().BeOfType<TimeoutException>();
        //         result.Message.Should().Be(Connection.PRIMARY_READ_TIMEOUT_EXCEPTION);
        //     }
        // }
        //
        // [Theory()]
        // [InlineData(10)]
        // [InlineData(100)]
        // [InlineData(1000)]
        // public async Task TestConnectionTimeoutPrimaryReadAsyncEndOfText(int primaryTimeout)
        // {
        //     // Arrange            
        //     using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
        //     {
        //         connection.Settings.PrimaryReadTimeout = primaryTimeout;
        //
        //         // Act
        //         var result = await Record.ExceptionAsync(async () => await connection.ReadAsync((byte)1, CancellationToken.None));
        //
        //         // Assert
        //         result.Should().BeOfType<TimeoutException>();
        //         result.Message.Should().Be(Connection.PRIMARY_READ_TIMEOUT_EXCEPTION);
        //     }
        // }
        //
        // [Theory()]
        // [InlineData(new byte[] { 1, 2, 3, 4 }, 2, 100)]
        // [InlineData(new byte[] { 1, 2, 3, 4 }, 4, 100)]
        // [InlineData(new byte[] { 1, 2, 3, 4 }, 2, 200)]
        // [InlineData(new byte[] { 1, 2, 3, 4 }, 4, 200)]
        // public async Task TestConnectionTimeoutSecondaryReadAsyncNumberOfBytes(byte[] rxMessage, int bytesToRead, int timeout)
        // {
        //     // Arrange
        //     using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
        //     {
        //         connection.Settings.PrimaryReadTimeout = timeout * 2;
        //         connection.Settings.SecondaryReadTimeout = timeout;
        //         await connection.WriteToRxBuffer(rxMessage, timeout + 50);
        //
        //         // Act
        //         var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(bytesToRead, CancellationToken.None));
        //
        //         // Assert
        //         result.Should().BeOfType<TimeoutException>();
        //         result.Message.Should().Be(Connection.SECONDARY_READ_TIMEOUT_EXCEPTION);
        //     }
        // }
        //
        // [Theory()]
        // [InlineData(new byte[] { 1, 2, 3, 4 }, 2, 100)]
        // [InlineData(new byte[] { 1, 2, 3, 4 }, 4, 100)]
        // [InlineData(new byte[] { 1, 2, 3, 4 }, 2, 200)]
        // [InlineData(new byte[] { 1, 2, 3, 4 }, 4, 200)]
        // public async Task TestConnectionTimeoutSecondaryReadAsyncEndOfText(byte[] rxMessage, byte endOfText, int timeout)
        // {
        //     // Arrange            
        //     using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
        //     {
        //         connection.Settings.PrimaryReadTimeout = timeout * 2;
        //         connection.Settings.SecondaryReadTimeout = timeout;
        //         await connection.WriteToRxBuffer(rxMessage, timeout + 50);
        //
        //         // Act
        //         var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(endOfText, CancellationToken.None));
        //
        //         // Assert
        //         result.Should().BeOfType<TimeoutException>();
        //         result.Message.Should().Be(Connection.SECONDARY_READ_TIMEOUT_EXCEPTION);
        //     }
        // }
        #endregion

        #region Connection Read
        [Theory()]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 1, new byte[] { 2 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 2, new byte[] { 3, 4 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 3, new byte[] { 4, 5, 6 })]
        public async Task TestConnectionReadAsyncNumberOfBytesX1WithNoDelay(byte[] rxMessage, int bytesToRead, byte[] expectedResult)
        {
            // Arrange            
            using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
            {
                await connection.WriteToRxBuffer(rxMessage);

                // Act
                var result = await connection.ReadAsync(bytesToRead, CancellationToken.None);
                result = await connection.ReadAsync(bytesToRead, CancellationToken.None);

                // Assert
                result.Should().BeEquivalentTo(expectedResult);
            }
        }

        [Theory()]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1, 10, new byte[] { 2 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4, 10, new byte[] { 5, 6, 7, 8 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1, 250, new byte[] { 2 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4, 250, new byte[] { 5, 6, 7, 8 })]
        public async Task TestConnectionReadAsyncNumberOfBytesX2WithDelay(byte[] rxMessage, int bytesToRead, int delay, byte[] expectedResult)
        {
            // Arrange            
            using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
            {
                // byte[] result = new byte[0];
                // await connection.WriteToRxBuffer(rxMessage, delay);
                //
                // // Act
                // result = await connection.ReadAsync(bytesToRead, CancellationToken.None);
                // result = await connection.ReadAsync(bytesToRead, CancellationToken.None);
                // connection.Dispose();
                //
                // // Assert
                // result.Should().BeEquivalentTo(expectedResult);
            }
        }

        [Theory()]
        [InlineData(Checksum.ChecksumTypes.None, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 })]
        [InlineData(Checksum.ChecksumTypes.None, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 })]
        [InlineData(Checksum.ChecksumTypes.None, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 })]
        [InlineData(Checksum.ChecksumTypes.None, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 })]
        [InlineData(Checksum.ChecksumTypes.None, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 })]
        [InlineData(Checksum.ChecksumTypes.None, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 })]
        [InlineData(Checksum.ChecksumTypes.None, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 })]
        [InlineData(Checksum.ChecksumTypes.LRC, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1, 2 })]
        [InlineData(Checksum.ChecksumTypes.LRC, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2, 3 })]
        [InlineData(Checksum.ChecksumTypes.LRC, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3, 4 })]
        [InlineData(Checksum.ChecksumTypes.LRC, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4, 5 })]
        [InlineData(Checksum.ChecksumTypes.LRC, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5, 6 })]
        [InlineData(Checksum.ChecksumTypes.LRC, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6, 7 })]
        public async Task TestConnectionReadAsyncEndOfTextX1WithNoDelay(ChecksumTypes checksum, byte[] rxMessage, byte endOfText, byte[] expectedResult)
        {
            // Arrange
            using (ConnectionFake connection = new ConnectionFake(null, DefaultConnectionSettings()))
            {
                // await connection.WriteToRxBuffer(rxMessage);
                //
                // // Act
                // var result = await connection.ReadAsync(endOfText, CancellationToken.None, checksum.Length());
                //
                // // Assert
                // result.Should().BeEquivalentTo(expectedResult);
            }
        }
        #endregion

    }
}
