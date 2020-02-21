using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Connection.Test
{
    [TestClass]
    public class TestConnection
    {

        [TestMethod]
        public void TestConnectionCreationWithoutSettings()
        {
            // Arrange
            IConnection connection;
            // Act
            connection = new ConnectionFake();

            // Asert
            Assert.IsTrue(connection is IConnection);
        }

        [TestMethod]
        public void TestConnectionCreationWithSettings()
        {
            // Arrange
            IConnectionSettings connectionSettings;
            IConnection connection;

            // Act
            connectionSettings = new ConnectionSettings();
            connection = new ConnectionFake(connectionSettings);

            // Asert
            Assert.IsTrue(connection is IConnection);
        }

        [TestMethod]
        public async Task TestConnectionConnectAsync()
        {
            // Arrange            
            ConnectionFake connection;
            connection = new ConnectionFake();
            connection.ExpectedResult = true;

            // Act            
            ConnectionState result = await connection.ConnectAsync();

            // Asert            
            Assert.IsTrue((result == ConnectionState.Conneted) && (connection.State == ConnectionState.Conneted));
        }

        [TestMethod]
        public void TestConnectionConnectAsyncWithDelay()
        {
            // Arrange            
            ConnectionFake connection;
            connection = new ConnectionFake();
            connection.ExpectedResult = true;
            connection.TaskDelay = 10;

            // Act            
            connection.ConnectAsync();

            // Asert            
            Assert.IsTrue(connection.State == ConnectionState.Connecting);
        }

        [TestMethod]
        public async Task TestConnectionDisconnectAsync()
        {
            // Arrange            
            ConnectionFake connection;
            connection = new ConnectionFake();
            connection.ExpectedResult = true;

            // Act            
            await connection.ConnectAsync();
            await connection.DisconnectAsync();

            // Asert            
            Assert.IsTrue(connection.State == ConnectionState.Disconnected);
        }

        [TestMethod]
        public async Task TestConnectionDisconnectAsyncWithDelay()
        {
            // Arrange            
            ConnectionFake connection;
            connection = new ConnectionFake();
            connection.ExpectedResult = true;
            connection.TaskDelay = 10;

            // Act            
            await connection.ConnectAsync();
            connection.DisconnectAsync();

            // Asert            
            Assert.IsTrue(connection.State == ConnectionState.Disconnecting);
        }

        [TestMethod]
        public async Task TestConnectionNoDataAvaliable()
        {
            // Arrange            
            ConnectionFake connection;
            connection = new ConnectionFake();

            // Act            
            bool dataAvaliable = await connection.DataAvaliableAsync();

            // Asert            
            Assert.IsFalse(dataAvaliable);
        }

        [TestMethod]
        public async Task TestConnectionDataAvaliable()
        {
            // Arrange            
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutOuter = 15000;
            connectionSettings.ReceiveTimeoutInner = 1000;
            ConnectionFake connection = new ConnectionFake(connectionSettings);
            connection.WriteToRxBuffer(new byte[] { 1 });

            // Act            
            bool dataAvaliable = await connection.DataAvaliableAsync();

            // Asert            
            Assert.IsTrue(dataAvaliable);
        }

        [TestMethod]
        public async Task TestConnectionWriteAsync()
        {
            // Arrange            
            ConnectionFake connection;
            connection = new ConnectionFake();
            byte[] TxMessage = { 1, 2, 3, 4, 5 };

            // Act                        
            bool result = await connection.WriteAsync(TxMessage, CancellationToken.None);

            // Asert            
            Assert.IsTrue(result);

        }

        [DataTestMethod]
        [DataRow(new byte[] { 1, 2, 3, 4, 5 }, 1, new byte[] { 1 }, DisplayName = "Read 1 byte")]
        [DataRow(new byte[] { 1, 2, 3, 4, 5 }, 2, new byte[] { 1, 2 }, DisplayName = "Read 2 bytes")]
        [DataRow(new byte[] { 1, 2, 3, 4, 5 }, 3, new byte[] { 1, 2, 3 }, DisplayName = "Read 3 bytes")]
        [DataRow(new byte[] { 1, 2, 3, 4, 5 }, 4, new byte[] { 1, 2, 3, 4 }, DisplayName = "Read 4 bytes")]
        [DataRow(new byte[] { 1, 2, 3, 4, 5 }, 5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "Read 5 bytes")]
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

            // Asert     
            CollectionAssert.AreEqual(expectedResult, result);

        }

        [DataTestMethod]
        [DataRow(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 1, new byte[] { 2 }, DisplayName = "Read 1 byte 2 times")]
        [DataRow(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 2, new byte[] { 3, 4 }, DisplayName = "Read 2 byte 2 times")]
        [DataRow(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 3, new byte[] { 4, 5, 6 }, DisplayName = "Read 3 byte 2 times")]
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

            // Asert     
            CollectionAssert.AreEqual(expectedResult, result);

        }

        [DataTestMethod]
        [DataRow(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1, 10, new byte[] { 2 }, DisplayName = "Read 1 byte 2 times with 10 millisecond RX delay")]
        [DataRow(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4, 10, new byte[] { 5, 6, 7, 8 }, DisplayName = "Read 4 byte 2 times with 10 millisecond RX delay")]
        [DataRow(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1, 250, new byte[] { 2 }, DisplayName = "Read 1 byte 2 times with 500 millisecond RX delay")]
        [DataRow(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4, 250, new byte[] { 5, 6, 7, 8 }, DisplayName = "Read 4 byte 2 times with 500 millisecond RX delay")]
        public async Task TestConnectionReadAsyncMultiBytesWithDelay(byte[] rxMessage, int bytesToRead, int delay, byte[] expectedResult)
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

            // Asert
            CollectionAssert.AreEqual(expectedResult, result);

        }

        [DataTestMethod]
        [Timeout(5000)]
        [DataRow(1000, DisplayName = "Test ReadAsync(bytesToRead) outer timeout.")]
        public async Task TestConnectionReadAsyncBytesOuterTimeouts(int timeout)
        {
            // Arrange                            
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutOuter = timeout;
            ConnectionFake connection = new ConnectionFake(connectionSettings);

            // Act / Asert 
            await Assert.ThrowsExceptionAsync<ReadTimeoutOuterException>(async () =>
            {
                var result = await connection.ReadAsync(1, CancellationToken.None);
            });

        }

        [DataTestMethod]
        [DataRow(new byte[] { 1 }, 2, 1000, 1001, DisplayName = "Read 2 byte with 1000 millisecond RX delay")]
        [DataRow(new byte[] { 1, 2, 3, 4 }, 4, 1000, 1001, DisplayName = "Read 4 byte with 1000 millisecond RX delay")]
        [DataRow(new byte[] { 1, 2, 3, 4 }, 2, 2000, 2001, DisplayName = "Read 2 byte with 2000 millisecond RX delay")]
        [DataRow(new byte[] { 1, 2, 3, 4 }, 4, 2000, 2001, DisplayName = "Read 4 byte with 2000 millisecond RX delay")]
        public async Task TestConnectionReadAsyncBytesInnerTimeout(byte[] rxMessage, int bytesToRead, int timeout, int delay)
        {
            // Arrange          
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutInner = timeout;
            ConnectionFake connection = new ConnectionFake(connectionSettings);
            await connection.WriteToRxBuffer(rxMessage, delay);

            // Act / Asert 
            await Assert.ThrowsExceptionAsync<ReadTimeoutInnerException>(async () =>
            {
                var result = await connection.ReadAsync(bytesToRead, CancellationToken.None);
            });

        }

        [DataTestMethod]
        [DataRow(new byte[] { 1, 2, 3, 4 }, DisplayName = "Read 2 byte with 1000 millisecond RX delay")]
        public async Task TestConnectionReadAsyncBytesOuterCancel(byte[] rxMessage)
        {
            // Arrange          
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutOuter = 15000;
            connectionSettings.ReceiveTimeoutInner = 1000;
            ConnectionFake connection = new ConnectionFake(connectionSettings);
            await connection.WriteToRxBuffer(rxMessage, 5000);

            // Act / Asert 
            await Assert.ThrowsExceptionAsync<ReadCacncelOuterException>(async () =>
            {
                await connection.ReadAsync(1, cancellationTokenSource.Token);
            });

        }

        [DataTestMethod]
        [DataRow(new byte[] { 1, 2, 3, 4 }, DisplayName = "Read 2 byte with 1000 millisecond RX delay")]
        public async Task TestConnectionReadAsyncBytesInnerCancel(byte[] rxMessage)
        {
            // Arrange          
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
            IConnectionSettings connectionSettings = new ConnectionSettings();
            connectionSettings.ReceiveTimeoutOuter = 15000;
            connectionSettings.ReceiveTimeoutInner = 1000;
            ConnectionFake connection = new ConnectionFake(connectionSettings);
            await connection.WriteToRxBuffer(rxMessage);

            // Act / Asert 
            await Assert.ThrowsExceptionAsync<ReadCacncelInnerException>(async () =>
            {
                await connection.ReadAsync(5, cancellationTokenSource.Token);
            });

        }

        //[DataTestMethod]
        //[DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        //[DataRow(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        //[DataRow(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
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

        //    // Asert     
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

            public async override Task<bool> DataAvaliableAsync()
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
