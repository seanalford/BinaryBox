using System;

namespace BinaryBox.Connection
{
    public abstract class ConnectionException : Exception
    {
        public ConnectionException()
        {
        }

        public ConnectionException(string message) : base(message)
        {
        }

        public ConnectionException(string message, System.Exception inner) : base(message, inner)
        {
        }
    }
}
