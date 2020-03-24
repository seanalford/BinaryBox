namespace BinaryBox.Protocol
{
    public class ReceiveRetryLimitExceededException : ProtocolException
    {
        private const string CONST_EXCEPTION = "Retry Limit Exceeded Exception";

        public ReceiveRetryLimitExceededException() : base(CONST_EXCEPTION)
        {
        }

        public ReceiveRetryLimitExceededException(string message)
            : base($"{CONST_EXCEPTION}: {message}")
        {
        }

        public ReceiveRetryLimitExceededException(string message, System.Exception inner)
            : base($"{CONST_EXCEPTION}: {message}", inner)
        {
        }
    }
}
