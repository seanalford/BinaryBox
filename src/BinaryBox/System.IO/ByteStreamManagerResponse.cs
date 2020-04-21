using BinaryBox.Core.Response;

namespace BinaryBox.Core.System.IO
{
    public class ByteStreamManagerResponse<TData> : IResponse<ByteStreamManagerResponseStatus, ByteStreamManagerResponseStatusCode, TData>
    {
        public ByteStreamManagerResponseStatus Status { get; }

        public TData Data { get; }

        public ByteStreamManagerResponse(ByteStreamManagerResponseStatusCode code, TData data)
        {
            Status = new ByteStreamManagerResponseStatus(code);
            Data = data;
        }
    }

}
