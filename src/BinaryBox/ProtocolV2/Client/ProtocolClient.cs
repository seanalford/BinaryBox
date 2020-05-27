using BinaryBox.Protocol.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Core.ProtocolV2
{
    public interface IProtocolMessageData<TData> : IDisposable
    {
        TData Data { get; }
        void Clear();
    }

    public interface IProtocolClientResult<TStatus, TData>
        where TStatus : struct
    {
        TData Data { get; }

    }

    public interface IProtocolClientStatus<TStatus>
        where TStatus : struct
    {
        TStatus Status { get; }
        string Description { get; }
        bool Success { get; }
    }

    public enum ProtocolMessageStatus
    {
        AlreadyClosed,
        AlreadyOpen,
        Cancelled,
        Failed,
        NotOpen,
        OK,
        OpenCloseTimeout,
        PrimaryReadTimeout,
        ReceiveRetryLimitExceeded,
        SecondaryReadTimeout,
        SendRetryLimitExceeded,
        WriteTimeout,
    }

    public class ProtocolClientResult<TData> : IProtocolClientResult<ProtocolMessageStatus, TData>
    {
        public ProtocolMessageStatus Status { get; protected set; }
        public TData Data { get; protected set; }
        public string Description { get; protected set; }
        public bool Success { get; protected set; }
        public ProtocolClientResult(ProtocolMessageStatus status, TData data)
        {
            Data = data;
            Status = status;
            //TODO: Move string literals to english resource file.            
            switch (Status)
            {
                case ProtocolMessageStatus.AlreadyClosed:
                    Success = false;
                    Description = "Operation Failed: Already Closed";
                    break;
                case ProtocolMessageStatus.AlreadyOpen:
                    Success = false;
                    Description = "Operation Failed: Already Open";
                    break;
                case ProtocolMessageStatus.Cancelled:
                    Success = false;
                    Description = "Operation Failed: Cancelled";
                    break;
                case ProtocolMessageStatus.Failed:
                    Success = false;
                    Description = "Operation Failed";
                    break;
                case ProtocolMessageStatus.NotOpen:
                    Success = false;
                    Description = "Operation Failed: Not Open";
                    break;
                case ProtocolMessageStatus.OK:
                    Success = true;
                    Description = "Operation Succesfully";
                    break;
                case ProtocolMessageStatus.OpenCloseTimeout:
                    Success = false;
                    Description = "Operation Failed: Timed Out";
                    break;
                case ProtocolMessageStatus.PrimaryReadTimeout:
                    Success = false;
                    Description = "Operation Failed: Primary Read Timeout";
                    break;
                case ProtocolMessageStatus.ReceiveRetryLimitExceeded:
                    Success = false;
                    Description = "Operation Failed: Receive Retry Limit Exceeded";
                    break;
                case ProtocolMessageStatus.SecondaryReadTimeout:
                    Success = false;
                    Description = "Operation Failed: Secondary Read Timeout";
                    break;
                case ProtocolMessageStatus.SendRetryLimitExceeded:
                    Success = false;
                    Description = "Operation Failed: Send Retry Limit Exceeded";
                    break;
                case ProtocolMessageStatus.WriteTimeout:
                    Success = false;
                    Description = "Operation Failed: Write Timeout";
                    break;
            }
        }
    }

    public interface IProtocolMessage<TData> : IProtocol
    {
        IProtocolMessageData<TData> Data { get; }
    }

    public abstract class ProtocolMessage<TData> : Protocol, IProtocolMessage<TData>
    {
        public IProtocolMessageData<TData> Data { get; }

        public ProtocolMessage(ILogger logger, IProtocolSettings settings) : base(logger, settings)
        {

        }
    }

    public interface IProtocolClient<TStatus, TData> : IProtocol
        where TStatus : struct
    {
        Task<IProtocolClientResult<TStatus, TData>> SendAsync(IProtocolMessage<TData> message, CancellationToken cancellationToken);
    }
    public class ProtocolClient<TStatus, TData> : Protocol, IProtocolClient<TStatus, TData>
        where TStatus : struct
    {
        public ProtocolClient(ILogger logger, IProtocolSettings settings) : base(logger, settings)
        {
        }

        public override void Dispose()
        {
            // TODO: Cleanup
        }

        public async Task<IProtocolClientResult<TStatus, TData>> SendAsync(IProtocolMessage<TData> message, CancellationToken cancellationToken)
        {
            try
            {
                // Clear message data.
                message.Data.Clear();

                var result = await Tx(message, cancellationToken);

                //if (result.Status.Success)
                //{
                //    result = await Rx(message, cancellationToken);
                //}
                return result;

            }
            catch (Exception ex)
            {
                Log?.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IProtocolClientResult<TStatus, TData>> Tx(IProtocolMessage<TData> message, CancellationToken cancellationToken)
        {
            try
            {
                //IProtocolClientResult<TStatus, TData> result = default;

                int retires = 0;
                while (true)
                {

                    // Send encoded message.
                    //var txResponse = await Connection.WriteAsync(message.Encode(), cancellationToken);
                    //if (txResponse.Success)
                    //{
                    //    // Validate Trasmission?
                    //    if (message.ValidateTx)
                    //    {
                    //        var response = await TxResult(message, cancellationToken);
                    //        if (response.Success)
                    //        {
                    //            // Valid Repsonce?
                    //            if (message.ValidTx(response.Data))
                    //            {
                    //                result = new ProtocolResponse<TProtocolMessageData>(ProtocolResponseStatusCode.OK);
                    //                break;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        result = new ProtocolResponse<TProtocolMessageData>(ProtocolResponseStatusCode.OK);
                    //        break;
                    //    }
                    //}
                    if (retires++ == Settings.SendRetries)
                    {
                        var result = new ProtocolClientResult<TData>(ProtocolMessageStatus.SendRetryLimitExceeded, message.Data.Data);
                        Log?.LogInformation(result.Description);
                        break;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, ex.Message);
                throw;
            }
        }

        public Task<IProtocolClientResult<TStatus, TData>> Rx(IProtocolMessage<TData> message, CancellationToken cancellationToken)
        {
            return null;
        }

    }
}
