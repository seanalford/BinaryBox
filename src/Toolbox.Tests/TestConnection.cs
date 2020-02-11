using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Checksum;

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

            // Act            
            connection = new ConnectionFake();
            connection.ConnectSuccess = true;
            await connection.ConnectAsync();

            // Asert            
            Assert.IsTrue(connection.State == ConnectionState.Conneted);
        }

        [TestMethod]
        public void TestConnectionConnectAsyncWithDelay()
        {
            // Arrange            
            ConnectionFake connection;

            // Act            
            connection = new ConnectionFake();
            connection.TaskDelayMilliseconds = 10;
            connection.ConnectSuccess = true;
            connection.ConnectAsync();

            // Asert            
            Assert.IsTrue(connection.State == ConnectionState.Connecting);
        }

        [TestMethod]
        public async Task TestConnectionDisconnectAsync()
        {
            // Arrange            
            ConnectionFake connection;

            // Act            
            connection = new ConnectionFake();
            connection.ConnectSuccess = true;
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

            // Act            
            connection = new ConnectionFake();
            connection.TaskDelayMilliseconds = 10;
            connection.ConnectSuccess = true;
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

            // Act            
            connection = new ConnectionFake();

            // Asert            
            Assert.IsFalse(await connection.DataAvaliableAsync());
        }

        [TestMethod]
        public async Task TestConnectionDataAvaliable()
        {
            // Arrange            
            ConnectionFake connection;

            // Act            
            connection = new ConnectionFake();
            connection.RxBuffer = new byte[ConnectionSettings.DEFAULT_RECEIVE_BUFFER_SIZE];

            // Asert            
            Assert.IsTrue(await connection.DataAvaliableAsync());
        }

        [TestMethod]
        public async Task TestConnectionWriteAsync()
        {
            // Arrange            
            ConnectionFake connection;
            connection = new ConnectionFake();
            byte[] TxMessage = { 1, 2, 3, 4, 5 };

            // Act                        
            await connection.WriteAsync(TxMessage, CancellationToken.None);

            // Asert            
            CollectionAssert.AreEqual(connection.TxBuffer, TxMessage);

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
            ConnectionFake connection;
            connection = new ConnectionFake();
            connection.RxBuffer = rxMessage;

            // Act            
            var result = await connection.ReadAsync(bytesToRead, CancellationToken.None);

            // Asert     
            CollectionAssert.AreEqual(expectedResult, result.ToArray());

        }

        [DataTestMethod]
        [DataRow(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 1, new byte[] { 2 }, DisplayName = "Read 1 byte 1 2 times")]
        [DataRow(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 2, new byte[] { 3, 4 }, DisplayName = "Read 2 byte 2 times")]
        [DataRow(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 3, new byte[] { 4, 5, 6 }, DisplayName = "Read 3 byte 2 times")]
        public async Task TestConnectionReadAsyncMultiBytes(byte[] rxMessage, int bytesToRead, byte[] expectedResult)
        {
            // Arrange                            
            ConnectionFake connection;
            connection = new ConnectionFake();
            connection.RxBuffer = rxMessage;

            // Act            
            var result = await connection.ReadAsync(bytesToRead, CancellationToken.None);
            result = await connection.ReadAsync(bytesToRead, CancellationToken.None);

            // Asert     
            CollectionAssert.AreEqual(expectedResult, result.ToArray());

        }

        [DataTestMethod]
        [DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.None, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.None, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.None,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.LRC, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.LRC, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.LRC,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)6, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.CRC16, false, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)7, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = false)")]
        [DataRow(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)1, new byte[] { 1, 2, 3 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)2, new byte[] { 1, 2, 3, 4 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)3, new byte[] { 1, 2, 3, 4, 5 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)4, new byte[] { 1, 2, 3, 4, 5, 6 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        [DataRow(Checksum.ChecksumTypes.CRC16, true, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, (byte)5, new byte[] { 1, 2, 3, 4, 5, 6, 7 }, DisplayName = "ReadAsync(Checksum.CRC16,IncludeChecksum = true)")]
        public async Task TestConnectionReadAsyncEndOfMessageNoChecksum(ChecksumTypes checksum, bool includeChecksum, byte[] rxMessage, byte endofMessage, byte[] expectedResult)
        {
            // Arrange                            
            ConnectionFake connection = new ConnectionFake();
            connection.Settings.Checksum = checksum;
            connection.RxBuffer = rxMessage;

            // Act            
            var result = await connection.ReadAsync(endofMessage, CancellationToken.None, includeChecksum);

            // Asert     
            CollectionAssert.AreEqual(expectedResult, result.ToArray());
        }

        public class ConnectionFake : Connection
        {
            public int TaskDelayMilliseconds = 0;
            public bool ConnectSuccess = false;
            public byte[] RxBuffer = null;
            public byte[] TxBuffer = null;

            public ConnectionFake() : base() { }

            public ConnectionFake(IConnectionSettings settings) : base(settings) { }

            public async override Task<bool> DataAvaliableAsync()
            {
                await Task.Delay(TaskDelayMilliseconds);
                return await Task.Run(() => RxBuffer != null);
            }

            public override void Dispose()
            {
                RxBuffer = null;
            }

            protected async override Task<ConnectionState> ConnectTask()
            {
                await Task.Delay(TaskDelayMilliseconds);
                ConnectionState result = ConnectionState.Disconnected;
                if (ConnectSuccess)
                {
                    result = await Task.Run(() => ConnectionState.Conneted);
                }
                return result;
            }

            protected async override Task<ConnectionState> DisconnectTask()
            {
                await Task.Delay(TaskDelayMilliseconds);
                return await Task.Run(() => ConnectionState.Disconnected);
            }

            protected async override Task<int> ReadTask(Memory<byte> data, CancellationToken cancellationToken)
            {
                await Task.Delay(TaskDelayMilliseconds);
                return await Task<int>.Run(() =>
                {
                    RxBuffer.CopyTo(data);
                    return data.Length;
                });
            }

            protected async override Task<bool> WriteTask(Memory<byte> data, CancellationToken cancellationToken)
            {
                await Task.Delay(TaskDelayMilliseconds);
                await Task.Run(() =>
                {
                    TxBuffer = new byte[data.Length];
                    data.CopyTo(TxBuffer);
                });
                return true;
            }
        }

    }
}
