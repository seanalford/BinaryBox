using BinaryBox.Core.Response;

namespace BinaryBox.Core.System.IO
{
    public class ByteStreamManagerResponse<TData> : Response<ByteStreamManagerResponseStatus, ByteStreamManagerResponseStatusCode, TData>, IResponse<ByteStreamManagerResponseStatus, ByteStreamManagerResponseStatusCode, TData>
    {
        public ByteStreamManagerResponse(ByteStreamManagerResponseStatusCode code, TData data = default) : base(code, data)
        {
            Status = new ByteStreamManagerResponseStatus(code);
        }
    }
}
