namespace BinaryBox.Core.Response
{
    public interface IResponse<TStatus, TData>
        where TStatus : struct
    {
        TData Data { get; }
        string Description { get; }
        TStatus Status { get; }
        bool Success { get; }
    }
}
