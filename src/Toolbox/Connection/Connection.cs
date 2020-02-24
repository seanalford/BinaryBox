﻿using ReactiveUI.Fody.Helpers;
using System;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

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
        public abstract Task<bool> DataAvailableAsync();
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

        public async Task<byte[]> ReadAsync(int bytesToRead, CancellationToken cancellationToken)
        {
            byte[] result = new byte[0];

            PipeInit(cancellationToken);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) { throw new ReadCancelOuterException(); }
                if (stopwatch.ElapsedMilliseconds > Settings.ReceiveTimeoutOuter) { throw new ReadTimeoutOuterException(); Pipe.Reader.Complete(); }

                if (Pipe.Reader.TryRead(out ReadResult))
                {
                    result = await ReadBytesAsyncInner(bytesToRead, cancellationToken).ConfigureAwait(false);
                }
                if (result?.Length == bytesToRead) break;
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

        protected abstract Task<int> ReadTask(byte[] data, CancellationToken cancellationToken);
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
                    byte[] buffer = new byte[Settings.ReceiveBufferSize];
                    try
                    {
                        int bytesRead = await ReadTask(buffer, cancellationToken).ConfigureAwait(false);
                        buffer.CopyTo(memory);
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