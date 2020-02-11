namespace Toolbox.Protocol
{
    public class RetryLimitExceededException : System.Exception
    {
        private const string CONST_EXCEPTION = "Retry Limit Exceeded Exception: ";

        public RetryLimitExceededException()
        {
        }

        public RetryLimitExceededException(string message)
            : base(CONST_EXCEPTION + message)
        {
        }

        public RetryLimitExceededException(string message, System.Exception inner)
            : base(CONST_EXCEPTION + message, inner)
        {
        }
    }
}
