namespace Toolbox.Protocol
{
    public class ReceiveRetryLimitExceededException : System.Exception
    {
        private const string CONST_EXCEPTION = "Retry Limit Exceeded Exception: ";

        public ReceiveRetryLimitExceededException()
        {
        }

        public ReceiveRetryLimitExceededException(string message)
            : base(CONST_EXCEPTION + message)
        {
        }

        public ReceiveRetryLimitExceededException(string message, System.Exception inner)
            : base(CONST_EXCEPTION + message, inner)
        {
        }
    }
}
