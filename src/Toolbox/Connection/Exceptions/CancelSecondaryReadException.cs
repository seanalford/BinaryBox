namespace Toolbox.Connection
{
    public class CancelSecondaryReadException : ConnectionException
    {
        private const string CONST_EXCEPTION = "Cancel Secondary Read Exception";

        public CancelSecondaryReadException() : base(CONST_EXCEPTION)
        {
        }

        public CancelSecondaryReadException(string message)
            : base($"{CONST_EXCEPTION}: {message}")
        {
        }

        public CancelSecondaryReadException(string message, System.Exception inner)
            : base($"{CONST_EXCEPTION}: {message}", inner)
        {
        }
    }
}