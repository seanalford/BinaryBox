using BinaryBox.Checksum;
using BinaryBox.Core.System.IO;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Protocol
{
    public abstract class ProtocolClient<TProtocolSettings, TProtocolMessage, TProtocolMessageData> : Protocol, IProtocolClient<TProtocolSettings, TProtocolMessage, TProtocolMessageData>
        where TProtocolSettings : IProtocolSettings
        where TProtocolMessage : IProtocolMessage<TProtocolSettings, TProtocolMessageData>
        where TProtocolMessageData : IProtocolMessageData
    {
        public IByteStreamManager Connection { get; protected set; }
        public TProtocolSettings Settings { get; protected set; }

        public ProtocolClient(ILogger logger, IByteStreamManager connection, TProtocolSettings settings) : base(logger)
        {
            Connection = connection;
            Settings = settings;
        }

        /// <summary>
        /// Sends the TMessage to the host as an asynchronous operation.
        /// </summary>
        /// <param name="message">The TMessage to send to the host.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Returns the resulting messageg status</returns>
        public async Task<IProtocolResponse<TProtocolMessageData>> SendAsync(TProtocolMessage message, CancellationToken cancellationToken)
        {
            message.ClearData();

            var result = await Tx(message, cancellationToken);

            if (result.Success)
            {
                result = await Rx(message, cancellationToken);
            }
            return result;
        }

        /// <summary>
        /// Transmites the IHexAsciiMessage to host as an asynchronous operation.
        /// </summary>
        /// <param name="message">IHexAsciiMessage to send</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Returns True is if response is HexAsciiMessageStatus.ACK, otherwise False.</returns>
        private async Task<IProtocolResponse<TProtocolMessageData>> Tx(TProtocolMessage message, CancellationToken cancellationToken)
        {
            IProtocolResponse<TProtocolMessageData> result;
            int retires = 0;

            while (true)
            {
                try
                {
                    // Send encoded message.
                    var txResponse = await Connection.WriteAsync(message.Encode(), cancellationToken);
                    if (txResponse.Success)
                    {
                        // Validate Trasmission?
                        if (message.ValidateTx)
                        {
                            var response = await TxResult(message, cancellationToken);
                            if (response.Success)
                            {
                                // Valid Repsonce?
                                if (message.ValidTx(response.Data))
                                {
                                    result = new ProtocolResponse<TProtocolMessageData>(ProtocolResponseStatusCode.OK);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = new ProtocolResponse<TProtocolMessageData>(ProtocolResponseStatusCode.OK);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, ex.Message);
                    throw;
                }

                if (retires++ == Settings.SendRetries)
                {
                    result = new ProtocolResponse<TProtocolMessageData>(ProtocolResponseStatusCode.SendRetryLimitExceeded);
                    Log?.LogInformation(result.Description);
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Receives the IHexAsciiMessageResult from the host as an asynchronous operation.
        /// </summary>
        /// <param name="message">The IHexAsciiMessage to be received.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Returns the resulting HexAsciiMessageResult</returns>
        private async Task<IProtocolResponse<TProtocolMessageData>> Rx(TProtocolMessage message, CancellationToken cancellationToken)
        {
            IProtocolResponse<TProtocolMessageData> result;
            int retires = 0;

            while (true)
            {
                try
                {
                    var response = await RxRead(message, cancellationToken);
                    if (response.Success)
                    {
                        if (message.Decode(response.Data))
                        {
                            // Send ACK to host to signal message received.
                            await SendAck(message, cancellationToken);

                            message.DecodeData();

                            if (message.Complete)
                            {
                                result = new ProtocolResponse<TProtocolMessageData>(ProtocolResponseStatusCode.OK, message.Data);
                                break;
                            }
                        }
                        else
                        {
                            if (retires++ == Settings.ReceiveRetries)
                            {
                                result = new ProtocolResponse<TProtocolMessageData>(ProtocolResponseStatusCode.ReceiveRetryLimitExceeded);
                                Log?.LogInformation(result.Description);
                                // Send Abort signal to the host?
                                await SendAbort(message, cancellationToken);
                                break;
                            }
                            else
                            {
                                await SendNak(message, cancellationToken);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, ex.Message);
                    throw;
                }
            }
            return result;
        }

        private async Task<ByteStreamResponse<byte[]>> RxRead(TProtocolMessage message, CancellationToken cancellationToken)
        {
            ByteStreamResponse<byte[]> result = default;
            if (message.RxBytesToRead > 0)
            {
                result = await Connection.ReadAsync(message.RxBytesToRead, cancellationToken);
            }
            else
            {
                result = await Connection.ReadAsync(message.RxEndOfMessageToken, Settings.Checksum.Length(), cancellationToken);
            }
            return result;
        }

        private async Task<ByteStreamResponse<bool>> SendAck(TProtocolMessage message, CancellationToken cancellationToken)
        {
            ByteStreamResponse<bool> result = default;
            // Send ACK to host to signal message received.
            if (message.Ack?.Length > 0)
            {
                result = await Connection.WriteAsync(message.Ack, cancellationToken);
            }
            return result;
        }

        private async Task<ByteStreamResponse<bool>> SendNak(TProtocolMessage message, CancellationToken cancellationToken)
        {
            ByteStreamResponse<bool> result = default;
            // Send ACK to host to signal message received.
            if (message.Nak?.Length > 0)
            {
                result = await Connection.WriteAsync(message.Nak, cancellationToken);
            }
            return result;
        }

        private async Task<ByteStreamResponse<bool>> SendAbort(TProtocolMessage message, CancellationToken cancellationToken)
        {
            ByteStreamResponse<bool> result = default;
            // Send ACK to host to signal message received.
            if (message.Abort?.Length > 0)
            {
                result = await Connection.WriteAsync(message.Abort, cancellationToken);
            }
            return result;
        }

        private async Task<ByteStreamResponse<byte[]>> TxResult(TProtocolMessage message, CancellationToken cancellationToken)
        {
            ByteStreamResponse<byte[]> result = default;

            try
            {
                if (message.TxBytesToRead > 0)
                {
                    result = await Connection.ReadAsync(message.TxBytesToRead, cancellationToken);
                }
                else
                {
                    result = await Connection.ReadAsync((byte)message.TxEndOfMessageToken, Settings.Checksum.Length(), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, ex.Message);
                throw;
            }
            return result;
        }
    }
}
