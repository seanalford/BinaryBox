using BinaryBox.Protocol.Settings;
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

        public ILogger Log { get; protected set; }

        public IProtocolSettings Settings { get; set; }

        public ByteStreamState State => _btyeStream.State;

        public event PropertyChangedEventHandler PropertyChanged;

        public ByteStreamManager(IByteStream byteStream, IProtocolSettings settings, ILogger logger = default)
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
            else
            {
                try
                {
                    result = await _btyeStream.CloseAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, ex.Message);
                    throw;
                }

            }
            return result;
        }

        public void Dispose()
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

        public async Task<ByteStreamResponse<ByteStreamState>> OpenAsync(CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<ByteStreamState> result = null;
            if (State == ByteStreamState.Open)
            {
                result = new ByteStreamResponse<ByteStreamState>(ByteStreamResponseStatusCode.AlreadyOpen, State);
            }
            else
            {
                try
                {
                    result = await _btyeStream.OpenAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, ex.Message);
                    throw;
                }
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
            else
            {
                try
                {
                    result = await _btyeStream.ReadAsync(bytesToRead, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, ex.Message);
                    throw;
                }
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
            else
            {
                try
                {
                    result = await _btyeStream.ReadAsync(endOfText, checksumLength, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, ex.Message);
                    throw;
                }
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
            else
            {
                try
                {
                    result = await _btyeStream.WriteAsync(data, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, ex.Message);
                    throw;
                }
            }
            return result;
        }
    }

}
