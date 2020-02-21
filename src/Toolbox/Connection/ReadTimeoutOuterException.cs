namespace Toolbox.Connection
{
    public class ReadTimeoutOuterException : System.Exception
    {
        private const string CONST_EXCEPTION = "Connection timeout outer read.";

        public ReadTimeoutOuterException()
        {
        }

        public ReadTimeoutOuterException(string message)
            : base(CONST_EXCEPTION + message)
        {
        }

        public ReadTimeoutOuterException(string message, System.Exception inner)
            : base(CONST_EXCEPTION + message, inner)
        {
        }
    }
}