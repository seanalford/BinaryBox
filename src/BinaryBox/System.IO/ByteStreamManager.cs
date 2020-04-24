using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Core.System.IO
{
    public class ByteStreamManager : IByteStreamManager
    {
        private IByteStream _btyeStream;
        private Mutex _mutex = new Mutex();

        public ILogger Log { get; protected set; }

        public IByteStreamSettings Settings { get; set; }

        public ByteStreamState State => _btyeStream.State;

        public event PropertyChangedEventHandler PropertyChanged;

        public ByteStreamManager(IByteStream byteStream, IByteStreamSettings settings, ILogger logger = default)
        {
            _btyeStream = byteStream;
            Settings = settings;
            Log = logger;
        }

        public async Task<ByteStreamResponse<ByteStreamState>> CloseAsync(CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<ByteStreamState> result = null;
            if (State == ByteStreamState.Closed)
            {
                result = new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.AlreadyClosed, State);
            }
            else if (_mutex.WaitOne(Settings.OpenCloseTimeout))
            {
                try
                {
                    try
                    {
                        result = await _btyeStream.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        Log?.LogError(ex, ex.Message);
                        throw;
                    }
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            }
            else
            {
                result = new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OpenCloseTimeout, State);
            }
            return result;
        }

        public void Dispose()
        {
            if (_mutex.WaitOne())
            {
                try
                {
                    try
                    {
                        // Dispose disposables here.
                    }
                    catch (Exception ex)
                    {
                        Log?.LogError(ex, ex.Message);
                        throw;
                    }
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            }
        }

        public async Task<ByteStreamResponse<ByteStreamState>> OpenAsync(CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<ByteStreamState> result = null;
            if (State == ByteStreamState.Open)
            {
                result = new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.AlreadyOpen, State);
            }
            else if (_mutex.WaitOne(Settings.OpenCloseTimeout))
            {
                try
                {
                    try
                    {
                        result = await _btyeStream.OpenAsync();
                    }
                    catch (Exception ex)
                    {
                        Log?.LogError(ex, ex.Message);
                        throw;
                    }
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            }
            else
            {
                result = new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.OpenCloseTimeout, State);
            }
            return result;
        }

        public async Task<ByteStreamResponse<byte[]>> ReadAsync(int bytesToRead, CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<byte[]> result = null;
            if (State != ByteStreamState.Open)
            {
                result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.NotOpen);
            }
            else if (_mutex.WaitOne(Settings.PrimaryReadTimeout))
            {
                try
                {
                    try
                    {
                        result = await _btyeStream.ReadAsync(bytesToRead, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Log?.LogError(ex, ex.Message);
                        throw;
                    }
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            }
            else
            {
                result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.PrimaryReadTimeout);
            }
            return result;
        }

        public async Task<ByteStreamResponse<byte[]>> ReadAsync(byte endOfText, int checksumLength = 0, CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<byte[]> result = null;
            if (State != ByteStreamState.Open)
            {
                result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.NotOpen);
            }
            else if (_mutex.WaitOne(Settings.PrimaryReadTimeout))
            {
                try
                {
                    try
                    {
                        result = await _btyeStream.ReadAsync(endOfText, checksumLength, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Log?.LogError(ex, ex.Message);
                        throw;
                    }
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            }
            else
            {
                result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.PrimaryReadTimeout);
            }
            return result;
        }

        public async Task<ByteStreamResponse<bool>> WriteAsync(byte[] data, CancellationToken cancellationToken)
        {
            ByteStreamResponse<bool> result = null;
            if (State != ByteStreamState.Open)
            {
                result = new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.NotOpen, false);
            }
            else if (_mutex.WaitOne(Settings.WriteTimeout))
            {
                try
                {
                    try
                    {
                        result = await _btyeStream.WriteAsync(data, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Log?.LogError(ex, ex.Message);
                        throw;
                    }
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            }
            else
            {
                result = new ByteStreamResponse<bool>(ByteStreamResponseStatusCode.WriteTimeout, false);
            }
            return result;
        }
    }

}
