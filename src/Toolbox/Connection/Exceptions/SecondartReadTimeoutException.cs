namespace Toolbox.Connection
{
    public class SecondartReadTimeoutException : ConnectionException
    {
        private const string CONST_EXCEPTION = "Secondary Read Timeout Exception";

        public SecondartReadTimeoutException()
        {
        }

        public SecondartReadTimeoutException(string message)
            : base($"{CONST_EXCEPTION}: {message}")
        {
        }

        public SecondartReadTimeoutException(string message, System.Exception inner)
            : base($"{CONST_EXCEPTION}: {message}", inner)
        {
        }
    }
}
