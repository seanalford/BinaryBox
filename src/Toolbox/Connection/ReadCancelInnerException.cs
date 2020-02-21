namespace Toolbox.Connection
{
    public class ReadCancelInnerException : System.Exception
    {
        private const string CONST_EXCEPTION = "Connection cancel inner read.";

        public ReadCancelInnerException()
        {
        }

        public ReadCancelInnerException(string message)
            : base(CONST_EXCEPTION + message)
        {
        }

        public ReadCancelInnerException(string message, System.Exception inner)
            : base(CONST_EXCEPTION + message, inner)
        {
        }
    }
}
