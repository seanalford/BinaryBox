namespace Toolbox.Connection
{
    public class ReadCacncelInnerException : System.Exception
    {
        private const string CONST_EXCEPTION = "Connection cancel inner read.";

        public ReadCacncelInnerException()
        {
        }

        public ReadCacncelInnerException(string message)
            : base(CONST_EXCEPTION + message)
        {
        }

        public ReadCacncelInnerException(string message, System.Exception inner)
            : base(CONST_EXCEPTION + message, inner)
        {
        }
    }

    public class ReadCacncelOuterException : System.Exception
    {
        private const string CONST_EXCEPTION = "Connection cancel outer read.";

        public ReadCacncelOuterException()
        {
        }

        public ReadCacncelOuterException(string message)
            : base(CONST_EXCEPTION + message)
        {
        }

        public ReadCacncelOuterException(string message, System.Exception inner)
            : base(CONST_EXCEPTION + message, inner)
        {
        }
    }
}
