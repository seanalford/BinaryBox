namespace BinaryBox.Protocol
{
    public class SendRetryLimitExceededException : ProtocolException
    {
        private const string CONST_EXCEPTION = "Send Retry Limit Exceeded Exception";

        public SendRetryLimitExceededException() : base(CONST_EXCEPTION)
        {
        }

        public SendRetryLimitExceededException(string message)
            : base($"{CONST_EXCEPTION}: {message}")
        {
        }

        public SendRetryLimitExceededException(string message, System.Exception inner)
            : base($"{CONST_EXCEPTION}: {message}", inner)
        {
        }
    }
}
