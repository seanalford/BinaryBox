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
        Task<Memory<byte>> ReadAsync(int bytesToRead, CancellationToken cancellationToken);
        Task<Memory<byte>> ReadAsync(byte endOfMessageToken, CancellationToken cancellationToken, bool IncludeChecksum = true);
        Task<bool> WriteAsync(Memory<byte> data, CancellationToken cancellationToken);
    }

    public abstract class Connection : IConnection
    {
        private Pipe Pipe = null;

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

        public async Task<Memory<byte>> ReadAsync(int bytesToRead, CancellationToken cancellationToken)
        {
            await PipeInit(cancellationToken);

            Memory<byte> result = null;

            Stopwatch outerStopwatch = new Stopwatch();
            outerStopwatch.Start();
            while (result.Length == 0)
            {
                ReadResult readResult = await Pipe.Reader.ReadAsync(cancellationToken);

                ReadOnlySequence<byte> buffer = readResult.Buffer;

                //Stopwatch innerStopwatch = new Stopwatch();
                do
                {
                    //innerStopwatch.Start();
                    if (buffer.Length >= bytesToRead)
                    {
                        result = new Memory<byte>(buffer.Slice(0, bytesToRead).ToArray());
                        buffer = buffer.Slice(bytesToRead);
                    }
                    //if (innerStopwatch.ElapsedMilliseconds > Settings.ReceiveTimeoutInner) { throw new TimeoutException("Rx inner timeout"); }
                }
                while (result.Length == 0);

                // Tell the PipeReader how much of the buffer we have consumed
                Pipe.Reader.AdvanceTo(buffer.Start, buffer.End);

                // Stop reading if there's no more data coming
                if (readResult.IsCompleted)
                {
                    break;
                }

                //if (outerStopwatch.ElapsedMilliseconds > Settings.ReceiveTimeoutOuter) { throw new TimeoutException("Rx outer timeout"); }

            }
            return result;
        }

        public async Task<Memory<byte>> ReadAsync(byte endOfMessage, CancellationToken cancellationToken, bool includeChecksum = true)
        {
            await PipeInit(cancellationToken);

            Memory<byte> result = null;

            while (true)
            {
                ReadResult readResult = await Pipe.Reader.ReadAsync(cancellationToken);

                ReadOnlySequence<byte> buffer = readResult.Buffer;
                SequencePosition? position = null;

                do
                {
                    // Look for a end of message token in the buffer
                    position = buffer.PositionOf(endOfMessage);
                    int checksumLength = includeChecksum == true ? Settings.Checksum.Length() : 0;

                    if (position != null)
                    {
                        result = new Memory<byte>(buffer.Slice(0, position.Value.GetInteger() + 1 + checksumLength).ToArray());

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

            // Mark the PipeReader as complete
            Pipe.Reader.Complete();

            return result;
        }

        protected abstract Task<int> ReadTask(Memory<byte> data, CancellationToken cancellationToken);
        public async Task<bool> WriteAsync(Memory<byte> data, CancellationToken cancellationToken)
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
        protected abstract Task<bool> WriteTask(Memory<byte> data, CancellationToken cancellationToken);

        private async Task PipeInit(CancellationToken cancellationToken)
        {
            if (Pipe != null)
            {
                FlushResult flushResult = await Pipe.Writer.FlushAsync(cancellationToken);
                if (flushResult.IsCompleted || flushResult.IsCanceled)
                {
                    Pipe = null;
                }
            }

            if (Pipe == null)
            {
                Pipe = new Pipe();
                PipeFill(Pipe.Writer, cancellationToken);
            }
        }
        private async Task PipeFill(PipeWriter writer, CancellationToken cancellationToken)
        {
            while (true)
            {
                // Allocate bytes from the PipeWriter
                Memory<byte> memory = writer.GetMemory(Settings.ReceiveBufferSize);
                try
                {
                    if (await DataAvaliableAsync())
                    {
                        int bytesRead = await ReadTask(memory, cancellationToken);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        // Tell the PipeWriter how much was read from the Socket
                        writer.Advance(bytesRead);
                    }
                }
                catch (Exception ex)
                {
                    //TODO:Log.Exception(ex);
                    break;
                }

                // Make the data available to the PipeReader
                FlushResult result = await writer.FlushAsync();

                // Is the reader still reading?
                if (result.IsCompleted || result.IsCanceled)
                {
                    break;
                }
            }
            // Tell the PipeReader that there's no more data coming
            writer.Complete();
        }
    }
}