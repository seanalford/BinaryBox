using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Toolbox.Connection.Test
{
    public class TestConnection
    {
        [Fact]
        public void TestConnectionCreationWithoutSettings()
        {
            // Arrange, Act
            IConnection connection = new ConnectionFake();

            // Assert
            connection.Should().BeAssignableTo<IConnection>();
        }

        [Fact]
        public void TestConnectionCreationWithSettings()
        {
            // Arrange
            IConnectionSettings connectionSettings;
            IConnection connection;

            // Act
            connectionSettings = new ConnectionSettings();
            connection = new ConnectionFake(connectionSettings);

            // Assert
            connection.Should().BeAssignableTo<IConnection>();
        }

        [Fact]
        public async Task TestConnectionConnectAsync()
        {
            // Arrange
            var connection = new ConnectionFake { ExpectedResult = true };

            // Act
            ConnectionState result = await connection.ConnectAsync();

            // Assert
            result.Should().Be(ConnectionState.Connected);
        }

        [Fact]
        public async Task TestConnectionConnectAsyncWithDelay()
        {
            // Arrange
            var connection = new ConnectionFake { ExpectedResult = true, TaskDelay = 10 };

            // Act
            await connection.ConnectAsync();

            // Assert
            connection.State.Should().Be(ConnectionState.Connected);
        }

        [Fact]
        public async Task TestConnectionDisconnectAsync()
        {
            // Arrange
            var connection = new ConnectionFake { ExpectedResult = true };

            // Act
            await connection.ConnectAsync();
            await connection.DisconnectAsync();

            // Assert
            connection.State.Should().Be(ConnectionState.Disconnected);
        }

        [Fact]
        public async Task TestConnectionDisconnectAsyncWithDelay()
        {
            // Arrange
            var connection = new ConnectionFake { ExpectedResult = true, TaskDelay = 10 };

            // Act
            await connection.ConnectAsync();
            await connection.DisconnectAsync();

            // Assert
            connection.State.Should().Be(ConnectionState.Disconnecting);
        }

        [Fact]
        public async Task TestConnectionNoDataAvailable()
        {
            // Arrange
            var connection = new ConnectionFake();

            // Act
            var result = await connection.DataAvailableAsync();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task TestConnectionDataAvailable()
        {
            // Arrange
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutOuter = 15000;
            connectionSettings.ReceiveTimeoutInner = 1000;
            ConnectionFake connection = new ConnectionFake(connectionSettings);

            // Act
            await connection.WriteToRxBuffer(new byte[] {1});
            bool dataAvailable = await connection.DataAvailableAsync();

            // Assert
            dataAvailable.Should().BeTrue();
        }

        [Fact]
        public async Task TestConnectionWriteAsync()
        {
            // Arrange
            var connection = new ConnectionFake();
            byte[] txMessage = {1, 2, 3, 4, 5};

            // Act
            bool result = await connection.WriteAsync(txMessage, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(new byte[] {1, 2, 3, 4, 5}, 1, new byte[] {1})]
        [InlineData(new byte[] {1, 2, 3, 4, 5}, 2, new byte[] {1, 2})]
        [InlineData(new byte[] {1, 2, 3, 4, 5}, 3, new byte[] {1, 2, 3})]
        [InlineData(new byte[] {1, 2, 3, 4, 5}, 4, new byte[] {1, 2, 3, 4})]
        [InlineData(new byte[] {1, 2, 3, 4, 5}, 5, new byte[] {1, 2, 3, 4, 5})]
        public async Task TestConnectionReadAsyncBytes(byte[] rxMessage, int bytesToRead, byte[] expectedResult)
        {
            // Arrange
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutOuter = 15000;
            connectionSettings.ReceiveTimeoutInner = 1000;
            ConnectionFake connection = new ConnectionFake(connectionSettings);
            await connection.WriteToRxBuffer(rxMessage);

            // Act
            var result = await connection.ReadAsync(bytesToRead, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(new byte[] {1, 2, 3, 4, 5, 6, 7}, 1, new byte[] {2})]
        [InlineData(new byte[] {1, 2, 3, 4, 5, 6, 7}, 2, new byte[] {3, 4})]
        [InlineData(new byte[] {1, 2, 3, 4, 5, 6, 7}, 3, new byte[] {4, 5, 6})]
        public async Task TestConnectionReadAsyncMultiBytes(byte[] rxMessage, int bytesToRead, byte[] expectedResult)
        {
            // Arrange
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutOuter = 15000;
            connectionSettings.ReceiveTimeoutInner = 1000;
            ConnectionFake connection = new ConnectionFake(connectionSettings);
            await connection.WriteToRxBuffer(rxMessage);

            // Act
            var result = await connection.ReadAsync(bytesToRead, CancellationToken.None);
            result = await connection.ReadAsync(bytesToRead, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(new byte[] {1, 2, 3, 4, 5, 6, 7, 8}, 1, 10, new byte[] {2})]
        [InlineData(new byte[] {1, 2, 3, 4, 5, 6, 7, 8}, 4, 10, new byte[] {5, 6, 7, 8})]
        [InlineData(new byte[] {1, 2, 3, 4, 5, 6, 7, 8}, 1, 250, new byte[] {2})]
        [InlineData(new byte[] {1, 2, 3, 4, 5, 6, 7, 8}, 4, 250, new byte[] {5, 6, 7, 8})]
        public async Task TestConnectionReadAsyncMultiBytesWithDelay(byte[] rxMessage, int bytesToRead, int delay,
            byte[] expectedResult)
        {
            // Arrange
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutOuter = 15000;
            connectionSettings.ReceiveTimeoutInner = 1000;
            ConnectionFake connection = new ConnectionFake(connectionSettings);
            byte[] result = new byte[0];
            await connection.WriteToRxBuffer(rxMessage, delay);

            // Act
            result = await connection.ReadAsync(bytesToRead, CancellationToken.None);
            result = await connection.ReadAsync(bytesToRead, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);

        }

        [Theory(DisplayName = "Test ReadAsync(bytesToRead) outer timeout.")]
        [InlineData(1000)]
        public async Task TestConnectionReadAsyncBytesOuterTimeouts(int timeout)
        {
            // Arrange
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutOuter = timeout;
            ConnectionFake connection = new ConnectionFake(connectionSettings);

            // Act
            var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(1, CancellationToken.None));

            // Assert
            result.Should().BeOfType<ReadTimeoutOuterException>();
        }

        [Theory]
        [InlineData(new byte[] { 1 }, 2, 1000, 1001)]
        [InlineData(new byte[] { 1, 2, 3, 4 }, 4, 1000, 1001)]
        [InlineData(new byte[] { 1, 2, 3, 4 }, 2, 2000, 2001)]
        [InlineData(new byte[] { 1, 2, 3, 4 }, 4, 2000, 2001)]
        public async Task TestConnectionReadAsyncBytesInnerTimeout(byte[] rxMessage, int bytesToRead, int timeout, int delay)
        {
            // Arrange
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutInner = timeout;
            ConnectionFake connection = new ConnectionFake(connectionSettings);
            await connection.WriteToRxBuffer(rxMessage, delay);

            // Act
            var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(bytesToRead, CancellationToken.None));

            // Assert
            result.Should().BeOfType<ReadTimeoutInnerException>();
        }

        [Theory]
        [InlineData(new byte[] {1, 2, 3, 4})]
        public async Task TestConnectionReadAsyncBytesOuterCancel(byte[] rxMessage)
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource =
                new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutOuter = 15000;
            connectionSettings.ReceiveTimeoutInner = 1000;
            ConnectionFake connection = new ConnectionFake(connectionSettings);
            await connection.WriteToRxBuffer(rxMessage, 5000);

            // Act
            var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(1, cancellationTokenSource.Token));

            // Assert
            result.Should().BeOfType<ReadCancelOuterException>();
        }

        [Theory]
        [InlineData(new byte[] {1, 2, 3, 4})]
        public async Task TestConnectionReadAsyncBytesInnerCancel(byte[] rxMessage)
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource =
                new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutOuter = 15000;
            connectionSettings.ReceiveTimeoutInner = 1000;
            ConnectionFake connection = new ConnectionFake(connectionSettings);
            await connection.WriteToRxBuffer(rxMessage);

            // Act
            var result = await Record.ExceptionAsync(async () => await connection.ReadAsync(5 ,cancellationTokenSource.Token));

            // Assert
            result.Should().BeOfType<ReadCancelInnerException>();
        }

        //[Theory]
        //[InlineData(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[InlineData(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        //[InlineData(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        //public async Task TestConnectionReadAsyncEndOfMessage(ChecksumTypes checksum, bool includeChecksum, byte[] rxMessage, byte endofMessage, byte[] expectedResult)
        //{
        //    // Arrange        
        //    IConnectionSettings connectionSettings = new ConnectionSettings();
        //    connectionSettings.Checksum = checksum;
        //    connectionSettings.ReceiveTimeoutOuter = 15000;
        //    connectionSettings.ReceiveTimeoutInner = 1000;
        //    ConnectionFake connection = new ConnectionFake(connectionSettings);
        //    connection.WriteToRxBuffer(rxMessage, 100);

        //    // Act
        //    var result = await connection.ReadAsync(endofMessage, CancellationToken.None, includeChecksum);

        //    // Assert     
        //    CollectionAssert.AreEqual(expectedResult, result);
        //}

        public class ConnectionFake : Connection
        {
            private HostFake Host;
            public int TaskDelay = 0;
            public bool ExpectedResult = false;

            public ConnectionFake() : base()
            {
                Host = new HostFake();
            }

            public ConnectionFake(IConnectionSettings settings) : base(settings)
            {
                Host = new HostFake();
            }

            public override async Task<bool> DataAvailableAsync()
            {
                return await Host.DataAvaliable();
            }

            public override void Dispose()
            {
                Host.Dispose();
            }

            protected async override Task<bool> ConnectTask()
            {
                return await Host.ConnectAsync(TaskDelay, ExpectedResult);
            }

            protected async override Task<bool> DisconnectTask()
            {
                return await Host.DisconnectAsync(TaskDelay, ExpectedResult);
            }

            protected async override Task<int> ReadTask(Memory<byte> data, CancellationToken cancellationToken)
            {
                int bytesRead = 0;
                byte[] bytes = new byte[data.Length];

                bytesRead = await Host.ReadAsync(bytes, cancellationToken);

                bytes.CopyTo(data);

                return bytesRead;
            }

            protected async override Task<bool> WriteTask(byte[] data, CancellationToken cancellationToken)
            {
                return await Host.WriteAsync(data, cancellationToken);
            }

            public async Task WriteToRxBuffer(byte[] data, int delayPerByte = 0)
            {
                await Host.WriteToInputRxBuffer(data, delayPerByte);
            }

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
