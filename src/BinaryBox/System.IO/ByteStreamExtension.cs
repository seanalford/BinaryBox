using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Core.System.IO
{
    public static class ByteStreamExtension
    {
        private async static Task<ByteStreamResponse<byte[]>> ReadPrimaryAsync(this IByteStream byteStream, CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<byte[]> result = default;
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested) { result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.Cancelled); break; }
                    if (stopwatch.ElapsedMilliseconds > byteStream.Settings.PrimaryReadTimeout) { result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.PrimaryReadTimeout); break; }

                    var response = await byteStream.DataAvailableAsync();

                    if (response?.Success == true && response?.Data == true) { result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.OK); break; }
                }
            }
            catch (Exception ex)
            {
                byteStream.Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }

        private async static Task<ByteStreamResponse<byte[]>> ReadSecondaryAsync(this IByteStream byteStream, int bytesToRead, CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<byte[]> result = default;
            byte[] resultData = new byte[bytesToRead];
            int byteOffset = 0;
            int bytesRemaining = bytesToRead;
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                while (bytesRemaining > 0)
                {
                    if (cancellationToken.IsCancellationRequested) { result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.Cancelled); break; }
                    if (stopwatch.ElapsedMilliseconds > byteStream.Settings.SecondaryReadTimeout) { result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.SecondaryReadTimeout); break; }

                    var response = await byteStream.ReadAsync(resultData, byteOffset, bytesRemaining, cancellationToken);

                    if (response?.Success == true && response?.Data > 0)
                    {
                        byteOffset += response.Data;
                        bytesRemaining -= response.Data;
                        stopwatch.Restart();
                    }
                    else
                    {
                        if (response?.Status == ByteStreamResponseStatusCode.Cancelled)
                        {
                            result = new ByteStreamResponse<byte[]>(response.Status);
                            break;
                        }
                    }
                }

                if (result == default)
                {
                    result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.OK, resultData);
                }
            }
            catch (Exception ex)
            {
                byteStream.Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }
        private async static Task<ByteStreamResponse<byte[]>> ReadSecondaryAsync(this IByteStream byteStream, byte endOfText, int checksumLength = 0, CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<byte[]> result = default;
            List<byte> resultData = new List<byte>();
            byte[] buffer = new byte[1];
            try
            {
                // Read to end of text
                while (true)
                {
                    var response = await byteStream.ReadSecondaryAsync(1, cancellationToken);

                    if (response?.Success == true && response?.Data.Length > 0)
                    {
                        resultData.Add(response.Data[0]);
                        if (response.Data[0] == endOfText) { break; }
                    }
                    else
                    {
                        result = response;
                        break;
                    }
                }

                if (result == default && checksumLength > 0)
                {
                    var response = await byteStream.ReadSecondaryAsync(checksumLength, cancellationToken);
                    if (response?.Success == true && response?.Data.Length > 0)
                    {
                        resultData.AddRange(response.Data);
                    }
                    else
                    {
                        result = response;
                    }
                }

                if (result == default)
                {
                    result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.OK, resultData.ToArray());
                }
            }
            catch (Exception ex)
            {
                byteStream.Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }

        public async static Task<ByteStreamResponse<byte[]>> ReadAsync(this IByteStream byteStream, int bytesToRead, CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<byte[]> result = default;
            try
            {
                if (byteStream.State == ByteStreamState.Open)
                {
                    var response = await byteStream.ReadPrimaryAsync(cancellationToken);
                    if (response?.Success == true)
                    {
                        result = await byteStream.ReadSecondaryAsync(bytesToRead, cancellationToken);
                    }
                    else
                    {
                        result = new ByteStreamResponse<byte[]>(response.Status);
                    }
                }
                else
                {
                    result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.NotOpen);
                }
            }
            catch (Exception ex)
            {
                byteStream.Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }

        public async static Task<ByteStreamResponse<byte[]>> ReadAsync(this IByteStream byteStream, byte endOfText, int checksumLength = 0, CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<byte[]> result = default;
            try
            {
                if (byteStream.State == ByteStreamState.Open)
                {
                    var response = await byteStream.ReadPrimaryAsync(cancellationToken);
                    if (response?.Success == true)
                    {
                        result = await byteStream.ReadSecondaryAsync(endOfText, checksumLength, cancellationToken);
                    }
                    else
                    {
                        result = new ByteStreamResponse<byte[]>(response.Status);
                    }
                }
                else
                {
                    result = new ByteStreamResponse<byte[]>(ByteStreamResponseStatusCode.NotOpen);
                }
            }

            catch (Exception ex)
            {
                byteStream.Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }

        public async static Task<ByteStreamResponse<bool>> WriteAsync(this IByteStream byteStream, byte[] data, CancellationToken cancellationToken = default)
        {
            ByteStreamResponse<bool> result = default;
            try
            {
                result = await byteStream?.WriteAsync(data, 0, data.Length, cancellationToken);
            }
            catch (Exception ex)
            {
                byteStream.Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }
    }
}
