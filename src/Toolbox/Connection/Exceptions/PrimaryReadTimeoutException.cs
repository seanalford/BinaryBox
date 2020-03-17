namespace Toolbox.Connection
{
    public class PrimaryReadTimeoutException : ConnectionException
    {
        private const string CONST_EXCEPTION = "Primary Read Timeout Exception";

        public PrimaryReadTimeoutException()
        {
        }

        public PrimaryReadTimeoutException(string message)
            : base($"{CONST_EXCEPTION}: {message}")
        {
        }

        public PrimaryReadTimeoutException(string message, System.Exception inner)
            : base($"{CONST_EXCEPTION}: {message}", inner)
        {
        }
    }
}