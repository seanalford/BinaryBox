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
    public interface IConnection : IDisposable
    {
        IConnectionSettings Settings { get; set; }
        ConnectionState State { get; }
        Task<ConnectionState> ConnectAsync();
        Task<bool> DataAvaliableAsync();
        Task<ConnectionState> DisconnectAsync();
        Task<byte[]> ReadAsync(int bytesToRead, CancellationToken cancellationToken);
        Task<byte[]> ReadAsync(byte endOfMessageToken, CancellationToken cancellationToken, bool IncludeChecksum = true);
        Task<bool> WriteAsync(byte[] data, CancellationToken cancellationToken);
    }

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

        public async Task<ConnectionState> ConnectAsync()
        {
            State = ConnectionState.Connecting;
            try
            {
                State = await ConnectTask();
            }
            catch (Exception ex)
            {
                //TODO:Log.Exception(ex);
                throw ex;
            }
            return State;
        }
        protected abstract Task<ConnectionState> ConnectTask();
        public abstract Task<bool> DataAvaliableAsync();
        public async Task<ConnectionState> DisconnectAsync()
        {
            State = ConnectionState.Disconnecting;
            try
            {
                State = await DisconnectTask();
            }
            catch (Exception ex)
            {
                //TODO:Log.Exception(ex);
                throw ex;
            }
            return State;
        }
        protected abstract Task<ConnectionState> DisconnectTask();
        public abstract void Dispose();

        public async Task<byte[]> ReadAsync(int bytesToRead, CancellationToken cancellationToken)
        {
            byte[] result = null;

            PipeInit(cancellationToken);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (stopwatch.ElapsedMilliseconds > Settings.ReceiveTimeoutOuter) { throw new ReadTimeoutOuterException(); Pipe.Reader.Complete(); }

                if (Pipe.Reader.TryRead(out ReadResult))
                {
                    result = await ReadAsyncInner(bytesToRead, cancellationToken);
                }
                if (result?.Length == bytesToRead) break;
            }

            return result;
        }

        private async Task<byte[]> ReadAsyncInner(int bytesToRead, CancellationToken cancellationToken)
        {
            byte[] result = new byte[0];

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (stopwatch.ElapsedMilliseconds > Settings.ReceiveTimeoutInner) { throw new ReadTimeoutInnerException(); }

                if (ReadResult.Buffer.Length < bytesToRead)
                {
                    int bytesToConsume = (int)ReadResult.Buffer.Length;
                    Array.Resize(ref result, result.Length + bytesToConsume);
                    ReadResult.Buffer.Slice(0, bytesToConsume).ToArray().CopyTo(result, result.Length - bytesToConsume);
                    Pipe.Reader.AdvanceTo(ReadResult.Buffer.GetPosition(bytesToConsume));
                    Pipe.Reader.TryRead(out ReadResult);
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

        public async Task<byte[]> ReadAsync(byte endOfMessage, CancellationToken cancellationToken, bool includeChecksum = true)
        {
            PipeInit(cancellationToken);

            byte[] result = null;

            Stopwatch outerStopwatch = new Stopwatch();
            outerStopwatch.Start();
            while (true)
            {
                if (outerStopwatch.ElapsedMilliseconds > Settings.ReceiveTimeoutOuter) { throw new ReadTimeoutOuterException(); }

                ReadResult readResult = await Pipe.Reader.ReadAsync(cancellationToken);

                ReadOnlySequence<byte> buffer = readResult.Buffer;
                SequencePosition? position = null;

                if (!buffer.IsEmpty)
                {
                    Stopwatch innerStopwatch = new Stopwatch();
                    innerStopwatch.Start();
                    do
                    {
                        if (innerStopwatch.ElapsedMilliseconds > Settings.ReceiveTimeoutInner) { throw new ReadTimeoutInnerException(); }
                        innerStopwatch.Restart();

                        // Look for a end of message token in the buffer
                        position = buffer.PositionOf(endOfMessage);
                        int checksumLength = includeChecksum == true ? Settings.Checksum.Length() : 0;

                        if (position != null)
                        {
                            result = buffer.Slice(0, position.Value.GetInteger() + 1 + checksumLength).ToArray();
                            buffer = buffer.Slice(position.Value.GetInteger() + 1 + checksumLength);
                        }
                    }
                    while (position == null);

                    // Tell the PipeReader how much of the buffer we have consumed
                    Pipe.Reader.AdvanceTo(buffer.Start, buffer.End);

                    // Stop reading if there's no more data coming
                    if (position != null || readResult.IsCompleted || readResult.IsCanceled)
                    {
                        break;
                    }
                }
            }

            // Mark the PipeReader as complete
            Pipe.Reader.Complete();

            return result;
        }

        protected abstract Task<int> ReadTask(Memory<byte> data, CancellationToken cancellationToken);
        public async Task<bool> WriteAsync(byte[] data, CancellationToken cancellationToken)
        {
            bool result = false;
            try
            {
                await WriteTask(data, cancellationToken);
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
                if (await DataAvaliableAsync())
                {
                    // Allocate bytes from the PipeWriter
                    Memory<byte> memory = Pipe.Writer.GetMemory(Settings.ReceiveBufferSize);
                    try
                    {
                        int bytesRead = await ReadTask(memory, cancellationToken);
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
                FlushResult result = await Pipe.Writer.FlushAsync();

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