
namespace BinaryBox.Core.Response
{
    public abstract class Response<TStatus, TStatusCode, TData> : IResponse<TStatus, TStatusCode, TData>
        where TStatus : IResponseStatus<TStatusCode>
        where TStatusCode : struct
    {
        public TStatus Status { get; protected set; }

        public TData Data { get; protected set; }

        public Response(TStatusCode code, TData data = default)
        {
            Data = data;
        }
    }
}
