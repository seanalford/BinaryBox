namespace BinaryBox.Protocol
{
    // TODO: Convert to ENUM
    public class MessageTokens
    {
        public const char STX = '\x2';
        public const char ETX = '\x3';
        public const char ACK = '\x6';
        public const char NAK = '\x21';
        public const char ESC = '\x27';
    }
}
