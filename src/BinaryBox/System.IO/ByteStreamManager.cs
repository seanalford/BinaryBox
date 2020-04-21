using Microsoft.Extensions.Logging;
using System;
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

        public ByteStreamManager(IByteStream byteStream, IByteStreamSettings settings, ILogger logger = default)
        {
            _btyeStream = byteStream;
            Settings = settings;
            Log = logger;
        }

        public async Task<ByteStreamManagerResponse<ByteStreamState>> CloseAsync()
        {
            ByteStreamManagerResponse<ByteStreamState> result = null;
            if (State == ByteStreamState.Closed)
            {
                result = new ByteStreamManagerResponse<ByteStreamState>(ByteStreamManagerResponseStatusCode.AlreadyClosed, State);
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
                result = new ByteStreamManagerResponse<ByteStreamState>(ByteStreamManagerResponseStatusCode.OpenCloseTimeout, State);
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

        public async Task<ByteStreamManagerResponse<ByteStreamState>> OpenAsync()
        {
            ByteStreamManagerResponse<ByteStreamState> result = null;
            if (State == ByteStreamState.Open)
            {
                result = new ByteStreamManagerResponse<ByteStreamState>(ByteStreamManagerResponseStatusCode.AlreadyOpen, State);
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
                result = new ByteStreamManagerResponse<ByteStreamState>(ByteStreamManagerResponseStatusCode.OpenCloseTimeout, State);
            }
            return result;
        }

        public async Task<ByteStreamManagerResponse<byte[]>> ReadAsync(int bytesToRead, CancellationToken cancellationToken = default)
        {
            ByteStreamManagerResponse<byte[]> result = null;
            if (State != ByteStreamState.Open)
            {
                result = new ByteStreamManagerResponse<byte[]>(ByteStreamManagerResponseStatusCode.NotOpen, null);
            }
            else if (_mutex.WaitOne(Settings.ReadPrimaryTimeout))
            {
                try
                {
                    try
                    {
                        // TODO result = await ReadPrimaryAsync(bytesToRead,cancellationToken);
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
                result = new ByteStreamManagerResponse<byte[]>(ByteStreamManagerResponseStatusCode.PrimaryReadTimeout, null);
            }
            return result;
        }

        public async Task<ByteStreamManagerResponse<byte[]>> ReadAsync(byte endOfText, CancellationToken cancellationToken = default, int checksumLength = 0)
        {
            ByteStreamManagerResponse<byte[]> result = null;
            if (State != ByteStreamState.Open)
            {
                result = new ByteStreamManagerResponse<byte[]>(ByteStreamManagerResponseStatusCode.NotOpen, null);
            }
            else if (_mutex.WaitOne(Settings.ReadPrimaryTimeout))
            {
                try
                {
                    try
                    {
                        // TODO result = await ReadPrimaryAsync(endOfText, checksumLength, cancellationToken);
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
                result = new ByteStreamManagerResponse<byte[]>(ByteStreamManagerResponseStatusCode.PrimaryReadTimeout, null);
            }
            return result;
        }

        public async Task<ByteStreamManagerResponse<bool>> WriteAsync(byte[] data, CancellationToken cancellationToken)
        {
            ByteStreamManagerResponse<bool> result = null;
            if (State != ByteStreamState.Open)
            {
                result = new ByteStreamManagerResponse<bool>(ByteStreamManagerResponseStatusCode.NotOpen, false);
            }
            else if (_mutex.WaitOne(Settings.WriteTimeout))
            {
                try
                {
                    try
                    {
                        result = await _btyeStream.WriteAsync(data, 0, data.Length);
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
                result = new ByteStreamManagerResponse<bool>(ByteStreamManagerResponseStatusCode.WriteTimeout, false);
            }
            return result;
        }
    }

}
