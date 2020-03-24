using System;

namespace BinaryBox.Protocol
{
    public abstract class ProtocolException : Exception
    {
        public ProtocolException()
        {
        }

        public ProtocolException(string message) : base(message)
        {
        }

        public ProtocolException(string message, System.Exception inner) : base(message, inner)
        {
        }
    }
}
