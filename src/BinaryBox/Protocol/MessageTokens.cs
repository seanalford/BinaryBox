namespace BinaryBox.Protocol
{
    // TODO: Convert to ENUM
    public class MessageTokens
    {
        public const byte STX = 2; //'\x2';
        public const byte ETX = 3; //'\x3';
        public const byte ACK = 6; // '\x6';
        public const byte NAK = 21; //'\x21';
        public const byte ESC = 27; //'\x27';
    }
}
