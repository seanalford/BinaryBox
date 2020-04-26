namespace BinaryBox.Core.System.IO
{
    public enum ByteStreamResponseStatusCode
    {
        AlreadyClosed,
        AlreadyOpen,
        Cancelled,
        Failed,
        NotOpen,
        OK,
        OpenCloseTimeout,
        PrimaryReadTimeout,
        SecondaryReadTimeout,
        WriteTimeout,
    }
}
