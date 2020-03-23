using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Checksum;
using Xunit;

namespace Toolbox.Connection.Test
{
    public partial class TestConnection
    {
        public static class LoggerFactory
        {
            public static ILogger Build()
            {

                ILogger logger = null;
                return logger;
            }
        }

        [Fact(DisplayName = "Connection Create with Settings", Timeout = 1000)]
        public void TestConnectionCreationWithSettings()
        {
            // Arrange
            IConnectionSettings connectionSettings;
            IConnection connection;

            // Act
            connectionSettings = new ConnectionSettings();
            connection = new ConnectionFake(LoggerFactory.Build(), connectionSettings);

            // Assert
            connection.Should().BeAssignableTo<IConnection>();
        }

        [Fact(DisplayName = "Connection ConnectAsync", Timeout = 1000)]
        public async Task TestConnectionConnectAsync()
        {
            // Arrange
            var connection = new ConnectionFake(LoggerFactory.Build(), new ConnectionSettings()) { ExpectedResult = true };

            // Act
            ConnectionState result = await connection.ConnectAsync();

            // Assert
            result.Should().Be(ConnectionState.Connected);
        }

        [Fact(DisplayName = "Connection ConnectAsync with Delay", Timeout = 1000)]
        public async Task TestConnectionConnectAsyncWithDelay()
        {
            // Arrange
            var connection = new ConnectionFake(LoggerFactory.Build(), new ConnectionSettings()) { ExpectedResult = true, TaskDelay = 100 };

            // Act
            connection.ConnectAsync();

            // Assert
            connection.State.Should().Be(ConnectionState.Connecting);
        }

        [Fact(DisplayName = "Connection DisconnectAsync", Timeout = 1000)]
        public async Task TestConnectionDisconnectAsync()
        {
            // Arrange
            var connection = new ConnectionFake(LoggerFactory.Build(), new ConnectionSettings()) { ExpectedResult = true };

            // Act
            await connection.ConnectAsync();
            await connection.DisconnectAsync();

            // Assert
            connection.State.Should().Be(ConnectionState.Disconnected);
        }

        [Fact(DisplayName = "Connection DisconnectAsync with Delay", Timeout = 1000)]
        public async Task TestConnectionDisconnectAsyncWithDelay()
        {
            // Arrange
            var connection = new ConnectionFake(LoggerFactory.Build(), new ConnectionSettings()) { ExpectedResult = true, TaskDelay = 100 };

            // Act
            await connection.ConnectAsync();
            connection.DisconnectAsync();

            // Assert
            connection.State.Should().Be(ConnectionState.Disconnecting);
        }

        [Fact(DisplayName = "Connection DataAvailableAsync No Data", Timeout = 1000)]
        public async Task TestConnectionNoDataAvailable()
        {
            // Arrange
            var connection = new ConnectionFake(LoggerFactory.Build(), new ConnectionSettings());

            // Act
            var result = await connection.DataAvailableAsync();

            // Assert
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "Connection DataAvailableAsync Data Available", Timeout = 1000)]
        public async Task TestConnectionDataAvailable()
        {
            // Arrange
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.SecondaryReadTimeout = 15000;
            connectionSettings.PrimaryReadTimeout = 1000;
            ConnectionFake connection = new ConnectionFake(LoggerFactory.Build(), connectionSettings);

            // Act
            await connection.WriteToRxBuffer(new byte[] { 1 });
            bool dataAvailable = await connection.DataAvailableAsync();

            // Assert
            dataAvailable.Should().BeTrue();
        }

        [Fact(DisplayName = "Connection WriteAsync", Timeout = 1000)]
        public async Task TestConnectionWriteAsync()
        {
            // Arrange
            var connection = new ConnectionFake(LoggerFactory.Build(), new ConnectionSettings());
            byte[] txMessage = { 1, 2, 3, 4, 5 };

            // Act
            bool result = await connection.WriteAsync(txMessage, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Theory(Timeout = 1000)]
        [InlineData(new byte[] { 1, 2, 3, 4, 5 }, 1, new byte[] { 1 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5 }, 2, new byte[] { 1, 2 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5 }, 3, new byte[] { 1, 2, 3 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5 }, 4, new byte[] { 1, 2, 3, 4 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5 }, 5, new byte[] { 1, 2, 3, 4, 5 })]
        public async Task TestConnectionReadAsyncBytes(byte[] rxMessage, int bytesToRead, byte[] expectedResult)
        {
            // Arrange
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.SecondaryReadTimeout = 15000;
            connectionSettings.PrimaryReadTimeout = 1000;
            ConnectionFake connection = new ConnectionFake(LoggerFactory.Build(), connectionSettings);
            await connection.WriteToRxBuffer(rxMessage);

            // Act
            var result = await connection.ReadAsync(bytesToRead, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Theory(Timeout = 100)]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 1, new byte[] { 2 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 2, new byte[] { 3, 4 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 3, new byte[] { 4, 5, 6 })]
        public async Task TestConnectionReadAsyncMultiBytes(byte[] rxMessage, int bytesToRead, byte[] expectedResult)
        {
            // Arrange
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.SecondaryReadTimeout = 15000;
            connectionSettings.PrimaryReadTimeout = 1000;
            ConnectionFake connection = new ConnectionFake(LoggerFactory.Build(), connectionSettings);
            await connection.WriteToRxBuffer(rxMessage);

            // Act
            var result = await connection.ReadAsync(bytesToRead, CancellationToken.None);
            result = await connection.ReadAsync(bytesToRead, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Theory(Timeout = 100)]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1, 10, new byte[] { 2 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4, 10, new byte[] { 5, 6, 7, 8 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1, 250, new byte[] { 2 })]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4, 250, new byte[] { 5, 6, 7, 8 })]
        public async Task TestConnectionReadAsyncMultiBytesWithDelay(byte[] rxMessage, int bytesToRead, int delay,
            byte[] expectedResult)
        {
            // Arrange
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.SecondaryReadTimeout = 15000;
            connectionSettings.PrimaryReadTimeout = 1000;
            ConnectionFake connection = new ConnectionFake(LoggerFactory.Build(), connectionSettings);
            byte[] result = new byte[0];
            await connection.WriteToRxBuffer(rxMessage, delay);

            // Act
            result = await connection.ReadAsync(bytesToRead, CancellationToken.None);
            result = await connection.ReadAsync(bytesToRead, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);

        }

        [Theory(Timeout = 100)]
        [InlineData(1000)]
        public async Task TestConnectionReadAsyncBytesPrimaryTimeouts(int timeout)
        {
            // Arrange
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.SecondaryReadTimeout = timeout;
            ConnectionFake connection = new ConnectionFake(LoggerFactory.Build(), connectionSettings);

            // Act
            var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(1, CancellationToken.None));

            // Assert
            result.Should().BeOfType<TimeoutException>();
            result.Message.Should().Be(Connection.PRIMARY_READ_TIMEOUT_EXCEPTION);
        }

        [Theory(Timeout = 100)]
        [InlineData(new byte[] { 1 }, 2, 1000, 1001)]
        [InlineData(new byte[] { 1, 2, 3, 4 }, 4, 1000, 1001)]
        [InlineData(new byte[] { 1, 2, 3, 4 }, 2, 2000, 2001)]
        [InlineData(new byte[] { 1, 2, 3, 4 }, 4, 2000, 2001)]
        public async Task TestConnectionReadAsyncBytesSecondaryTimeout(byte[] rxMessage, int bytesToRead, int timeout, int delay)
        {
            // Arrange
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.PrimaryReadTimeout = timeout;
            ConnectionFake connection = new ConnectionFake(LoggerFactory.Build(), connectionSettings);
            await connection.WriteToRxBuffer(rxMessage, delay);

            // Act
            var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(bytesToRead, CancellationToken.None));

            // Assert
            result.Should().BeOfType<TimeoutException>();
            result.Message.Should().Be(Connection.SECONDARY_READ_TIMEOUT_EXCEPTION);
        }

        [Theory(Timeout = 100)]
        [InlineData(new byte[] { 1, 2, 3, 4 })]
        public async Task TestConnectionReadAsyncBytesPrimaryCancel(byte[] rxMessage)
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource =
                new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.SecondaryReadTimeout = 15000;
            connectionSettings.PrimaryReadTimeout = 1000;
            ConnectionFake connection = new ConnectionFake(LoggerFactory.Build(), connectionSettings);
            await connection.WriteToRxBuffer(rxMessage, 5000);

            // Act
            var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(1, cancellationTokenSource.Token));

            // Assert
            result.Should().BeOfType<OperationCanceledException>();
            result.Message.Should().Be(Connection.PRIMARY_READ_CANCELLATION_EXCEPTION);

        }

        [Theory(Timeout = 100)]
        [InlineData(new byte[] { 1, 2, 3, 4 })]
        public async Task TestConnectionReadAsyncBytesSecondaryCancel(byte[] rxMessage)
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource =
                new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.SecondaryReadTimeout = 15000;
            connectionSettings.PrimaryReadTimeout = 1000;
            ConnectionFake connection = new ConnectionFake(LoggerFactory.Build(), connectionSettings);
            await connection.WriteToRxBuffer(rxMessage);

            // Act
            var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(5, cancellationTokenSource.Token));

            // Assert
            result.Should().BeOfType<OperationCanceledException>();
            result.Message.Should().Be(Connection.SECONDARY_READ_CANCELLATION_EXCEPTION);
        }

        [Theory(Timeout = 5000)]
        [InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 })]
        [InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 })]
        [InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 })]
        [InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 })]
        [InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 })]
        [InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 })]
        [InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 })]
        [InlineData(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1, 2 })]
        [InlineData(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2, 3 })]
        [InlineData(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3, 4 })]
        [InlineData(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4, 5 })]
        [InlineData(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5, 6 })]
        [InlineData(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6, 7 })]
        public async Task TestConnectionReadAsyncEndOfText(ChecksumTypes checksum, bool includeChecksum, byte[] rxMessage, byte endOfText, byte[] expectedResult)
        {
            // Arrange        
            int checksumLength = (includeChecksum == true) ? checksum.Length() : 0;
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.SecondaryReadTimeout = 15000;
            connectionSettings.PrimaryReadTimeout = 1000;
            using ConnectionFake connection = new ConnectionFake(LoggerFactory.Build(), connectionSettings);
            await connection.WriteToRxBuffer(rxMessage, 250);

            // Act
            var result = await connection.ReadAsync(endOfText, CancellationToken.None, checksumLength);

            // Assert    
            result.Should().BeEquivalentTo(expectedResult);
        }

        public class HostFake : IDisposable
        {
            private bool Disposing = false;
            private bool WriteToRxBufferRunning = false;
            private int InputRxBufferDelay = 0;
            private byte[] InputRxBuffer = new byte[0];
            private byte[] RxBuffer = new byte[0];
            private byte[] TxBuffer = new byte[0];

            public HostFake()
            {
                WriteToRxBuffer();
            }

            public async Task<bool> ConnectAsync(int delay, bool result)
            {
                await Task.Delay(delay);
                return result;
            }

            public async Task<bool> DataAvaliable()
            {
                return await Task.Run(() =>
                {
                    lock (RxBuffer)
                    {
                        return RxBuffer.Length > 0;

                    }
                });
            }

            public async Task<bool> DisconnectAsync(int delay, bool result)
            {
                await Task.Delay(delay);
                return result;
            }

            public void Dispose()
            {
                Disposing = true;
                Task.Run(() =>
                {
                    while (WriteToRxBufferRunning)
                    {
                        if (!WriteToRxBufferRunning) break;
                    }

                }).Wait();
            }

            public async Task<int> ReadAsync(byte[] data, CancellationToken cancellationToken)
            {
                return await Task.Run(() =>
                {
                    int bytesRead = 0;
                    lock (RxBuffer)
                    {
                        if (RxBuffer?.Length > 0)
                        {
                            bytesRead = RxBuffer.Length;
                            RxBuffer.CopyTo(data, 0);
                            Array.Resize(ref RxBuffer, 0);
                        }
                    }
                    if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();
                    return bytesRead;
                });
            }

            public async Task WriteToInputRxBuffer(byte[] data, int delayPerByte = 0)
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        lock (InputRxBuffer)
                        {
                            if (InputRxBuffer.Length == 0)
                            {
                                InputRxBufferDelay = delayPerByte;
                                Array.Resize(ref InputRxBuffer, data.Length);
                                data.CopyTo(InputRxBuffer, 0);
                                break;
                            }
                        }
                    }
                });
            }

            private void WriteToRxBuffer()
            {
                WriteToRxBufferRunning = true;
                try
                {
                    Task.Run(async () =>
                    {
                        while (!Disposing)
                        {
                            if (InputRxBuffer.Length > 0)
                            {
                                for (int i = 0; i < InputRxBuffer.Length; i++)
                                {
                                    await Task.Delay(InputRxBufferDelay);
                                    lock (InputRxBuffer)
                                    {
                                        lock (RxBuffer)
                                        {
                                            Array.Resize(ref RxBuffer, RxBuffer.Length + 1);
                                            Array.Copy(InputRxBuffer, i, RxBuffer, RxBuffer.Length - 1, 1);
                                        }
                                    }
                                }
                                Array.Resize(ref InputRxBuffer, 0);
                            }
                        }
                    });
                }
                finally
                {
                    WriteToRxBufferRunning = false;
                }
            }

            public async Task<bool> WriteAsync(byte[] data, CancellationToken cancellationToken)
            {
                return await Task.Run(() =>
                {
                    Array.Resize(ref TxBuffer, data.Length);
                    data.CopyTo(TxBuffer, 0);
                    if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();
                    return true;
                });
            }


        }
    }
}
