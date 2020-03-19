namespace Toolbox.Protocol
{
    public enum ProtocolClientStatusCodes { OK, SendRetryLimitExceeded, ReceiveRetryLimitExceeded, }

    public class ProtocolClientStatus : IProtocolClientStatus
    {
        public ProtocolClientStatusCodes Status { get; }
        public string Description { get; }

        public ProtocolClientStatus(ProtocolClientStatusCodes code)
        {
            Status = code;

        }
    }
}
