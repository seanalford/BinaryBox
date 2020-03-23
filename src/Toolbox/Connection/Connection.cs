using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Connection
{
    public abstract class Connection : IConnection, INotifyPropertyChanged
    {
        public const string PRIMARY_READ_CANCELLATION_EXCEPTION = "Primary Read Cancellation Exception";
        public const string SECONDARY_READ_CANCELLATION_EXCEPTION = "Secondary Read Cancellation Exception";
        public const string PRIMARY_READ_TIMEOUT_EXCEPTION = "Primary Read Timeout Exception";
        public const string SECONDARY_READ_TIMEOUT_EXCEPTION = "Secondary Read Timeout Exception";

        private Pipe Pipe = null;
        public event PropertyChangedEventHandler PropertyChanged;
        private ReadResult ReadResult;
        public ILogger Log { get; private set; }
        public IConnectionSettings Settings { get; set; }
        public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

        #region Constructors
        public Connection(ILogger logger, IConnectionSettings settings)
        {
            Log = logger;
            Settings = settings;
        }
        #endregion

        /// <summary>
        /// Connects the IConnection to a remote end point.
        /// </summary>
        /// <returns>Returns the current connection state</returns>
        public async Task<ConnectionState> ConnectAsync()
        {
            State = ConnectionState.Connecting;
            try
            {
                bool state = await ConnectTask().ConfigureAwait(false);
                State = state == true ? ConnectionState.Connected : ConnectionState.Disconnected;
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, ex.Message);
                throw;
            }
            return State;
        }

        protected abstract Task<bool> ConnectTask();

        /// <summary>
        /// Indicates whether data is available on the IConnection to be read.
        /// </summary>
        /// <returns>true if data is available on the IConnection to be read; otherwise, false.</returns>
        public abstract Task<bool> DataAvailableAsync();

        /// <summary>
        /// Disconnects the IConnection from a remote end point.
        /// </summary>
        /// <returns>Returns the current connection state</returns>
        public async Task<ConnectionState> DisconnectAsync()
        {
            State = ConnectionState.Disconnecting;
            try
            {
                bool state = await DisconnectTask().ConfigureAwait(false);
                State = state == true ? ConnectionState.Disconnected : ConnectionState.Connected;
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, ex.Message);
                throw;
            }
            return State;
        }
        protected abstract Task<bool> DisconnectTask();

        public abstract void Dispose();

        /// <summary>
        /// Reads data from the IConnection as an asynchronous operation.
        /// </summary>
        /// <param name="bytesToRead">The number of bytes to read.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>        
        /// <returns>A task that represents the asynchronous read operation. The value of its Result property contains a byte[] of data that was read.</returns>
        public async Task<byte[]> ReadAsync(int bytesToRead, CancellationToken cancellationToken)
        {
            byte[] result = new byte[0];

            PipeInit(cancellationToken);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) { throw new OperationCanceledException(PRIMARY_READ_CANCELLATION_EXCEPTION, cancellationToken); }
                if (stopwatch.ElapsedMilliseconds > Settings.SecondaryReadTimeout) { Pipe.Reader.Complete(); throw new TimeoutException(PRIMARY_READ_TIMEOUT_EXCEPTION); }

                if (Pipe.Reader.TryRead(out ReadResult))
                {
                    result = await ReadBytesAsyncInner(bytesToRead, cancellationToken).ConfigureAwait(false);
                }
                if (result?.Length == bytesToRead) break;
            }

            return result;
        }

        /// <summary>
        /// Reads data from the IConnection as an asynchronous operation.
        /// </summary>
        /// <param name="endOfText">Defines the last byte to read.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <param name="includeChecksum">Causes additional checksum bytes to be returned.</param>
        /// <returns>A task that represents the asynchronous read operation. The value of its Result property contains a byte[] of data that was read.</returns>
        public async Task<byte[]> ReadAsync(byte endOfText, CancellationToken cancellationToken, int checksumLength = 0)
        {
            byte[] result = new byte[0];

            PipeInit(cancellationToken);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) { throw new OperationCanceledException(PRIMARY_READ_CANCELLATION_EXCEPTION, cancellationToken); }
                if (stopwatch.ElapsedMilliseconds > Settings.SecondaryReadTimeout) { Pipe.Reader.Complete(); throw new TimeoutException(PRIMARY_READ_TIMEOUT_EXCEPTION); }

                if (Pipe.Reader.TryRead(out ReadResult))
                {
                    result = await ReadEndOfTextAsyncInner(endOfText, cancellationToken, checksumLength).ConfigureAwait(false);
                }
                if (Array.FindIndex(result, (x) => x == endOfText) >= 0) break;
            }
            return result;
        }

        private async Task<byte[]> ReadBytesAsyncInner(int bytesToRead, CancellationToken cancellationToken)
        {
            byte[] result = new byte[0];

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) { throw new OperationCanceledException(SECONDARY_READ_CANCELLATION_EXCEPTION, cancellationToken); }
                if (stopwatch.ElapsedMilliseconds > Settings.PrimaryReadTimeout) { throw new TimeoutException(SECONDARY_READ_TIMEOUT_EXCEPTION); }

                if (ReadResult.Buffer.Length < bytesToRead)
                {
                    int bytesToConsume = (int)ReadResult.Buffer.Length;
                    Array.Resize(ref result, result.Length + bytesToConsume);
                    ReadResult.Buffer.Slice(0, bytesToConsume).ToArray().CopyTo(result, result.Length - bytesToConsume);
                    Pipe.Reader.AdvanceTo(ReadResult.Buffer.GetPosition(bytesToConsume));
                    await Task.Run(() => Pipe.Reader.TryRead(out ReadResult));
                    if (ReadResult.Buffer.Length > 0)
                    {
                        stopwatch.Restart();
                    }
                }
                else
                {
                    int bytesToConsume = bytesToRead - result.Length;
                    Array.Resize(ref result, result.Length + bytesToConsume);
                    ReadResult.Buffer.Slice(0, bytesToConsume).ToArray().CopyTo(result, result.Length - bytesToConsume);
                    Pipe.Reader.AdvanceTo(ReadResult.Buffer.GetPosition(bytesToConsume));
                }
                if (result?.Length == bytesToRead) break;
            }
            return result;
        }

        private async Task<byte[]> ReadEndOfTextAsyncInner(byte endOfText, CancellationToken cancellationToken, int checksumLength = 0)
        {
            byte[] result = new byte[0];

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) { throw new OperationCanceledException(SECONDARY_READ_CANCELLATION_EXCEPTION, cancellationToken); }
                //if (stopwatch.ElapsedMilliseconds > Settings.ReceiveTimeoutInner) { throw new ReadTimeoutInnerException(); }

                int endOfTextIndex = Array.FindIndex(ReadResult.Buffer.ToArray(), (x) => x == endOfText);
                if (endOfTextIndex < 0)
                {
                    int bytesToConsume = (int)ReadResult.Buffer.Length;
                    Array.Resize(ref result, result.Length + bytesToConsume);
                    ReadResult.Buffer.Slice(0, bytesToConsume).ToArray().CopyTo(result, result.Length - bytesToConsume);
                    Pipe.Reader.AdvanceTo(ReadResult.Buffer.GetPosition(bytesToConsume));
                    await Task.Run(() => Pipe.Reader.TryRead(out ReadResult));
                    if (ReadResult.Buffer.Length > 0)
                    {
                        stopwatch.Restart();
                    }
                }
                else
                {
                    int bytesToConsume = endOfTextIndex + 1;
                    Array.Resize(ref result, result.Length + bytesToConsume);
                    ReadResult.Buffer.Slice(0, bytesToConsume).ToArray().CopyTo(result, result.Length - bytesToConsume);
                    Pipe.Reader.AdvanceTo(ReadResult.Buffer.GetPosition(bytesToConsume));
                    await Task.Run(() => Pipe.Reader.TryRead(out ReadResult));
                }
                if (Array.FindIndex(result, (x) => x == endOfText) >= 0) break;
            }

            if (checksumLength > 0)
            {
                while (true)
                {
                    if (checksumLength > ReadResult.Buffer.Length)
                    {
                        await Task.Run(() => Pipe.Reader.TryRead(out ReadResult));
                        Pipe.Reader.AdvanceTo(ReadResult.Buffer.Start);
                    }
                    else
                    {
                        Array.Resize(ref result, result.Length + checksumLength);
                        ReadResult.Buffer.Slice(0, checksumLength).ToArray().CopyTo(result, result.Length - checksumLength);
                        //Pipe.Reader.AdvanceTo(ReadResult.Buffer.GetPosition(checksumLength));
                        break;
                    }
                }
            }
            return result;
        }

        protected abstract Task<int> ReadTask(byte[] data, CancellationToken cancellationToken);

        /// <summary>
        /// Writes data to the IConnection from the specified byte array as an asynchronous operation.
        /// </summary>
        /// <param name="data">A byte array that contains the data to be written</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task<bool> WriteAsync(byte[] data, CancellationToken cancellationToken)
        {
            bool result;
            try
            {
                result = await WriteTask(data, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }
        protected abstract Task<bool> WriteTask(byte[] data, CancellationToken cancellationToken);

        private void PipeInit(CancellationToken cancellationToken)
        {
            if (Pipe == null)
            {
                Pipe = new Pipe();
                Task.Run(() => FillPipe(cancellationToken));
            }
        }

        private async Task FillPipe(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (await DataAvailableAsync().ConfigureAwait(false))
                {
                    // Allocate bytes from the PipeWriter
                    Memory<byte> memory = Pipe.Writer.GetMemory(Settings.ReadBufferSize);
                    byte[] buffer = new byte[Settings.ReadBufferSize];
                    try
                    {
                        int bytesRead = await ReadTask(buffer, cancellationToken).ConfigureAwait(false);
                        buffer.CopyTo(memory);
                        Pipe.Writer.Advance(bytesRead);
                    }
                    catch (Exception ex)
                    {
                        Log?.LogError(ex, ex.Message);
                        break;
                    }
                }

                // Make the data available to the PipeReader
                FlushResult result = await Pipe.Writer.FlushAsync().ConfigureAwait(false);

                // Is the reader still reading?
                if (result.IsCompleted)
                {
                    break;
                }
            }
            // Tell the PipeReader that there's no more data coming
            Pipe.Writer.Complete();
        }

    }
}