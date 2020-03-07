namespace Toolbox.Protocol
{
    public class SendRetryLimitExceededException : System.Exception
    {
        private const string CONST_EXCEPTION = "Send Retry Limit Exceeded Exception: ";

        public SendRetryLimitExceededException()
        {
        }

        public SendRetryLimitExceededException(string message)
            : base(CONST_EXCEPTION + message)
        {
        }

        public SendRetryLimitExceededException(string message, System.Exception inner)
            : base(CONST_EXCEPTION + message, inner)
        {
        }
    }
}
