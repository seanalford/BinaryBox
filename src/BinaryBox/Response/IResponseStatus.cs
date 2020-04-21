namespace BinaryBox.Core.Response
{
    public interface IResponseStatus<TStatusCode>
        where TStatusCode : struct
    {
        TStatusCode Code { get; }
        bool Success { get; }
        string Description { get; }
    }
}
