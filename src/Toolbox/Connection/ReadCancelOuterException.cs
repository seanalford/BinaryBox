namespace Toolbox.Connection
{
    public class ReadCancelOuterException : System.Exception
    {
        private const string CONST_EXCEPTION = "Connection cancel outer read.";

        public ReadCancelOuterException()
        {
        }

        public ReadCancelOuterException(string message)
            : base(CONST_EXCEPTION + message)
        {
        }

        public ReadCancelOuterException(string message, System.Exception inner)
            : base(CONST_EXCEPTION + message, inner)
        {
        }
    }
}