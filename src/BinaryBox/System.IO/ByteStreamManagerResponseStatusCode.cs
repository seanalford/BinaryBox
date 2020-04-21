namespace BinaryBox.Core.System.IO
{
    public enum ByteStreamManagerResponseStatusCode
    {
        OK,
        OpenCloseTimeout,
        AlreadyClosed,
        AlreadyOpen,
        NotOpen,
        WriteTimeout,
        PrimaryReadTimeout,
        SecondaryReadTimeout,
    }
}
