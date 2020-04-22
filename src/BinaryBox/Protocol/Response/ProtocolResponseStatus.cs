using BinaryBox.Core.Response;

namespace BinaryBox.Protocol
{
    public class ProtocolResponseStatus : IResponseStatus<ProtocolResponseStatusCode>
    {
        public ProtocolResponseStatusCode Code { get; }

        public string Description { get; }

        public bool Success { get; }

        public ProtocolResponseStatus(ProtocolResponseStatusCode code)
        {
            //TODO: Move string literals to english resource file.
            Code = code;
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
