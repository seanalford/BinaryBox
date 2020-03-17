using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Connection
{
    public interface IConnection : IDisposable
    {
        ILogger Log { get; }
        IConnectionSettings Settings { get; set; }

        ConnectionState State { get; }

        /// <summary>
        /// Connects the IConnection to a remote end point.
        /// </summary>
        /// <returns>Returns the current connection state</returns>
        Task<ConnectionState> ConnectAsync();

        /// <summary>
        /// Indicates whether data is available on the IConnection to be read.
        /// </summary>
        /// <returns>true if data is available on the IConnection to be read; otherwise, false.</returns>
        Task<bool> DataAvailableAsync();

        /// <summary>
        /// Disconnects the IConnection from a remote end point.
        /// </summary>
        /// <returns>Returns the current connection state</returns>
        Task<ConnectionState> DisconnectAsync();

        /// <summary>
        /// Reads data from the IConnection as an asynchronous operation.
        /// </summary>
        /// <param name="bytesToRead">The number of bytes to read.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>        
        /// <returns>A task that represents the asynchronous read operation. The value of its Result property contains a byte[] of data that was read.</returns>
        Task<byte[]> ReadAsync(int bytesToRead, CancellationToken cancellationToken);

        /// <summary>
        /// Reads data from the IConnection as an asynchronous operation.
        /// </summary>
        /// <param name="endOfText">Defines the last byte to read.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <param name="includeChecksum">Causes additional checksum bytes to be returned.</param>
        /// <returns>A task that represents the asynchronous read operation. The value of its Result property contains a byte[] of data that was read.</returns>
        Task<byte[]> ReadAsync(byte endOfText, CancellationToken cancellationToken, int checksumLength = 0);

        /// <summary>
        /// Writes data to the IConnection from the specified byte array as an asynchronous operation.
        /// </summary>
        /// <param name="data">A byte array that contains the data to be written</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task<bool> WriteAsync(byte[] data, CancellationToken cancellationToken);
    }
}