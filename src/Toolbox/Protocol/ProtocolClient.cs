using System;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Checksum;
using Toolbox.Connection;

namespace Toolbox.Protocol
{
    public abstract class ProtocolClient<TMessage, TMessageStatus> : IProtocolClient<TMessage, TMessageStatus>
        where TMessage : IProtocolMessage<TMessageStatus>
        where TMessageStatus : struct
    {
        public IConnection Connection { get; protected set; }
        public IProtocolSettings Settings { get; protected set; }

        public ProtocolClient(IConnection connection, IProtocolSettings settings)
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
        public async Task<TMessageStatus> SendAsync(TMessage message, CancellationToken cancellationToken)
        {
            TMessageStatus result = default;
            if (await Tx(message, cancellationToken))
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
        private async Task<bool> Tx(TMessage message, CancellationToken cancellationToken)
        {
            bool result = false;
            int retires = 0;

            while (true)
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
                            result = true;
                            break;
                        }
                    }
                    else { result = true; break; }
                }
                if (retires++ == Settings.SendRetries) { throw new SendRetryLimitExceededException(); }
            }
            return result;
        }

        /// <summary>
        /// Receives the IHexAsciiMessageResult from the host as an asynchronous operation.
        /// </summary>
        /// <param name="message">The IHexAsciiMessage to be received.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Returns the resulting HexAsciiMessageResult</returns>
        private async Task<TMessageStatus> Rx(TMessage message, CancellationToken cancellationToken)
        {
            TMessageStatus result = default;
            int retires = 0;

            while (true)
            {
                byte[] data;

                data = await RxRead(message, cancellationToken);
                if (message.Decode(data))
                {
                    // Send ACK to host to signal message received.
                    await SendAck(message, cancellationToken);

                    if (message.Complete) { result = message.Status; break; }

                }
                else
                {
                    if (retires++ == Settings.ReceiveRetries)
                    {
                        // Send Abort signal to the host?
                        await SendAbort(message, cancellationToken);
                        throw new ReceiveRetryLimitExceededException();
                    }
                    else { await SendNak(message, cancellationToken); }
                }
            }
            return result;
        }

        private async Task<byte[]> RxRead(TMessage message, CancellationToken cancellationToken)
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

        private async Task<bool> SendAck(TMessage message, CancellationToken cancellationToken)
        {
            bool result = false;
            // Send ACK to host to signal message received.
            if (message.Ack?.Length > 0)
            {
                result = await Connection.WriteAsync(message.Ack, cancellationToken);
            }
            return result;
        }

        private async Task<bool> SendNak(TMessage message, CancellationToken cancellationToken)
        {
            bool result = false;
            // Send ACK to host to signal message received.
            if (message.Nak?.Length > 0)
            {
                result = await Connection.WriteAsync(message.Nak, cancellationToken);
            }
            return result;
        }

        private async Task<bool> SendAbort(TMessage message, CancellationToken cancellationToken)
        {
            bool result = false;
            // Send ACK to host to signal message received.
            if (message.Abort?.Length > 0)
            {
                result = await Connection.WriteAsync(message.Abort, cancellationToken);
            }
            return result;
        }

        private async Task<byte[]> TxResult(TMessage message, CancellationToken cancellationToken)
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
                //TODO:Log.Exception(ex);                
            }
            return result;
        }

        public abstract void Dispose();

    }
}
