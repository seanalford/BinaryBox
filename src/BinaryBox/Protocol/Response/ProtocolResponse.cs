using BinaryBox.Core.Response;

namespace BinaryBox.Protocol
{
    public class ProtocolResponse<TData> : Response<ProtocolResponseStatusCode, TData>, IProtocolResponse<TData>
        where TData : IProtocolMessageData
    {
        public ProtocolResponse(ProtocolResponseStatusCode status, TData data = default) : base(status, data)
        {
        }

        protected override void Initialize()
        {
            //TODO: Move string literals to english resource file.            
            switch (Status)
            {
                case ProtocolResponseStatusCode.OK:
                    Success = true;
                    Description = "Operation Succesfully";
                    break;
                case ProtocolResponseStatusCode.SendRetryLimitExceeded:
                    Success = false;
                    Description = "Operation Failed: Send Retry Limit Exceeded";
                    break;
                case ProtocolResponseStatusCode.ReceiveRetryLimitExceeded:
                    Success = false;
                    Description = "Operation Failed: Receive Retry Limit Exceeded";
                    break;
            }
        }
    }
}
