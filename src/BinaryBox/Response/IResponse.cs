
namespace BinaryBox.Core.Response
{
    public interface IResponse<TStatus, TStatusCode, TData>
        where TStatus : IResponseStatus<TStatusCode>
        where TStatusCode : struct
    {
        TStatus Status { get; }
        TData Data { get; }
    }

}
