using BinaryBox.Core.Response;

namespace BinaryBox.Protocol
{
    public class ProtocolResponseStatus : ResponseStatus<ProtocolResponseStatusCode>, IProtocolResponseStatus
    {
        public ProtocolResponseStatus(ProtocolResponseStatusCode code) : base(code)
        {
        }

        protected override void Initialize()
        {
            //TODO: Move string literals to english resource file.            
            switch (Code)
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
