namespace BinaryBox.Core.System.IO
{
    public enum ByteStreamManagerResponseStatusCode
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
