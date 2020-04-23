namespace BinaryBox.Core.System.IO
{
    public enum ByteStreamResponseStatusCode
    {
        OK,
        Cancelled,
        OpenCloseTimeout,
        AlreadyClosed,
        AlreadyOpen,
        NotOpen,
        WriteTimeout,
        PrimaryReadTimeout,
        SecondaryReadTimeout,
    }
}
