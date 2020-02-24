using ReactiveUI.Fody.Helpers;
using System;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Checksum;

namespace Toolbox.Connection
{
    public abstract class Connection : IConnection
    {
        private Pipe Pipe = null;
        private ReadResult ReadResult;

        [Reactive] public IConnectionSettings Settings { get; set; }
        [Reactive] public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

        #region Constructors
        public Connection() : this(new ConnectionSettings())
        {

        }

        public Connection(IConnectionSettings settings)
        {
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
                //TODO:Log.Exception(ex);
                throw ex;
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
                //TODO:Log.Exception(ex);
                throw ex;
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
                if (cancellationToken.IsCancellationRequested) { throw new ReadCancelOuterException(); }
#if (DEBUG)
                if (!System.Diagnostics.Debugger.IsAttached)
#endif
                    if (stopwatch.ElapsedMilliseconds > Settings.ReceiveTimeoutOuter) { Pipe.Reader.Complete(); throw new ReadTimeoutOuterException(); }

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
        public async Task<byte[]> ReadAsync(byte endOfText, CancellationToken cancellationToken, bool includeChecksum = true)
        {
            byte[] result = new byte[0];

            PipeInit(cancellationToken);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) { throw new ReadCancelOuterException(); }
#if (DEBUG)
                if (!System.Diagnostics.Debugger.IsAttached)
#endif
                    if (stopwatch.ElapsedMilliseconds > Settings.ReceiveTimeoutOuter) { Pipe.Reader.Complete(); throw new ReadTimeoutOuterException(); }

                if (Pipe.Reader.TryRead(out ReadResult))
                {
                    result = await ReadEndOfTextAsyncInner(endOfText, cancellationToken).ConfigureAwait(false);
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
                if (cancellationToken.IsCancellationRequested) { throw new ReadCancelInnerException(); }
#if(DEBUG)
                if (!System.Diagnostics.Debugger.IsAttached)
#endif
                    if (stopwatch.ElapsedMilliseconds > Settings.ReceiveTimeoutInner) { throw new ReadTimeoutInnerException(); }

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

        private async Task<byte[]> ReadEndOfTextAsyncInner(byte endOfText, CancellationToken cancellationToken, bool includeChecksum = true)
        {
            byte[] result = new byte[0];
            int checksumLength = includeChecksum == true ? Settings.Checksum.Length() : 0;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) { throw new ReadCancelInnerException(); }
#if (DEBUG)
                if (!System.Diagnostics.Debugger.IsAttached)
#endif
                    if (stopwatch.ElapsedMilliseconds > Settings.ReceiveTimeoutInner) { throw new ReadTimeoutInnerException(); }

                if (Array.FindIndex(ReadResult.Buffer.ToArray(), (x) => x == endOfText) < 0)
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
                    int bytesToConsume = (Array.FindIndex(ReadResult.Buffer.ToArray(), (x) => x == endOfText) + 1) + checksumLength;
                    Array.Resize(ref result, result.Length + bytesToConsume);
                    ReadResult.Buffer.Slice(0, bytesToConsume).ToArray().CopyTo(result, result.Length - bytesToConsume);
                    Pipe.Reader.AdvanceTo(ReadResult.Buffer.GetPosition(bytesToConsume));
                }
                if (Array.FindIndex(result, (x) => x == endOfText) >= 0) break;
            }
            return result;
        }

        protected abstract Task<int> ReadTask(Memory<byte> data, CancellationToken cancellationToken);

        /// <summary>
        /// Writes data to the IConnection from the specified byte array as an asynchronous operation.
        /// </summary>
        /// <param name="data">A byte array that contains the data to be written</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task<bool> WriteAsync(byte[] data, CancellationToken cancellationToken)
        {
            bool result = false;
            try
            {
                await WriteTask(data, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //TODO:Log.Exception(ex);
                throw ex;
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
                    Memory<byte> memory = Pipe.Writer.GetMemory(Settings.ReceiveBufferSize);
                    try
                    {
                        int bytesRead = await ReadTask(memory, cancellationToken).ConfigureAwait(false);
                        //if (bytesRead == 0)
                        //{
                        //    break;
                        //}
                        // Tell the PipeWriter how much was read from the Socket
                        Pipe.Writer.Advance(bytesRead);
                    }
                    catch (Exception ex)
                    {
                        //TODO:Log.Exception(ex);
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