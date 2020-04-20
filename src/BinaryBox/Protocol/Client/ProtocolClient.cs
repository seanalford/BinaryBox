using BinaryBox.Checksum;
using BinaryBox.Connection;
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
        public IConnection Connection { get; protected set; }
        public TProtocolSettings Settings { get; protected set; }

        public ProtocolClient(ILogger logger, IConnection connection, TProtocolSettings settings) : base(logger)
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
        public async Task<IProtocolClientResult<TProtocolMessageData>> SendAsync(TProtocolMessage message, CancellationToken cancellationToken)
        {
            message.ClearData();

            var result = await Tx(message, cancellationToken);

            if (result.Status == ProtocolClientStatus.OK)
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
        /// <returns>Returns True is if responce is HexAsciiMessageStatus.ACK, otherwise False.</returns>
        private async Task<IProtocolClientResult<TProtocolMessageData>> Tx(TProtocolMessage message, CancellationToken cancellationToken)
        {
            IProtocolClientResult<TProtocolMessageData> result;
            int retires = 0;

            while (true)
            {
                try
                {
                    // Send encoded message.
                    if (await Connection.WriteAsync(message.Encode(), cancellationToken))
                    {
                        // Validate Trasmission?
                        if (message.ValidateTx)
                        {
                            // Valid Repsonce?
                            if (message.ValidTx(await TxResult(message, cancellationToken)))
                            {
                                result = new ProtocolClientResult<TProtocolMessageData>(ProtocolClientStatus.OK);
                                break;
                            }
                        }
                        else
                        {
                            result = new ProtocolClientResult<TProtocolMessageData>(ProtocolClientStatus.OK);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, ex.Message);
                }

                if (retires++ == Settings.SendRetries)
                {
                    result = new ProtocolClientResult<TProtocolMessageData>(ProtocolClientStatus.SendRetryLimitExceeded);
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
        private async Task<IProtocolClientResult<TProtocolMessageData>> Rx(TProtocolMessage message, CancellationToken cancellationToken)
        {
            IProtocolClientResult<TProtocolMessageData> result;
            int retires = 0;

            while (true)
            {
                try
                {
                    byte[] data = await RxRead(message, cancellationToken);

                    if (message.Decode(data))
                    {
                        // Send ACK to host to signal message received.
                        await SendAck(message, cancellationToken);

                        message.DecodeData();

                        if (message.Complete)
                        {
                            result = new ProtocolClientResult<TProtocolMessageData>(ProtocolClientStatus.OK, message.Data);
                            break;
                        }
                    }
                    else
                    {
                        if (retires++ == Settings.ReceiveRetries)
                        {
                            result = new ProtocolClientResult<TProtocolMessageData>(ProtocolClientStatus.ReceiveRetryLimitExceeded);
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
                catch (Exception ex)
                {
                    Log?.LogError(ex, ex.Message);
                }
            }
            return result;
        }

        private async Task<byte[]> RxRead(TProtocolMessage message, CancellationToken cancellationToken)
        {
            byte[] result;
            if (message.RxBytesToRead > 0)
            {
                result = await Connection.ReadAsync(message.RxBytesToRead, cancellationToken);
            }
            else
            {
                result = await Connection.ReadAsync(message.RxEndOfMessageToken, cancellationToken, Settings.Checksum.Length());
            }
            return result;
        }

        private async Task<bool> SendAck(TProtocolMessage message, CancellationToken cancellationToken)
        {
            bool result = false;
            // Send ACK to host to signal message received.
            if (message.Ack?.Length > 0)
            {
                result = await Connection.WriteAsync(message.Ack, cancellationToken);
            }
            return result;
        }

        private async Task<bool> SendNak(TProtocolMessage message, CancellationToken cancellationToken)
        {
            bool result = false;
            // Send ACK to host to signal message received.
            if (message.Nak?.Length > 0)
            {
                result = await Connection.WriteAsync(message.Nak, cancellationToken);
            }
            return result;
        }

        private async Task<bool> SendAbort(TProtocolMessage message, CancellationToken cancellationToken)
        {
            bool result = false;
            // Send ACK to host to signal message received.
            if (message.Abort?.Length > 0)
            {
                result = await Connection.WriteAsync(message.Abort, cancellationToken);
            }
            return result;
        }

        private async Task<byte[]> TxResult(TProtocolMessage message, CancellationToken cancellationToken)
        {
            byte[] result = null;

            try
            {
                if (message.TxBytesToRead > 0)
                {
                    result = await Connection.ReadAsync(message.TxBytesToRead, cancellationToken);
                }
                else
                {
                    result = await Connection.ReadAsync((byte)message.TxEndOfMessageToken, cancellationToken, Settings.Checksum.Length());
                }
            }
            catch (Exception ex)
            {
                //TODO:Log?.Exception(ex);                
            }
            return result;
        }
    }
}
