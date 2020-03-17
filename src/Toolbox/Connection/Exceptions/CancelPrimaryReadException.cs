namespace Toolbox.Connection
{
    public class CancelPrimaryReadException : ConnectionException
    {
        private const string CONST_EXCEPTION = "Cancel Primary Read Exception";

        public CancelPrimaryReadException()
        {
        }

        public CancelPrimaryReadException(string message)
            : base($"{CONST_EXCEPTION}: {message}")
        {
        }

        public CancelPrimaryReadException(string message, System.Exception inner)
            : base($"{CONST_EXCEPTION}: {message}", inner)
        {
        }
    }
}