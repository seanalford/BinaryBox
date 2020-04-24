namespace BinaryBox.Core.Response
{
    public abstract class Response<TStatus, TData> : IResponse<TStatus, TData>
        where TStatus : struct
    {
        public TData Data { get; protected set; }
        public string Description { get; protected set; }
        public TStatus Status { get; protected set; }
        public bool Success { get; protected set; }

        public Response(TStatus status, TData data = default)
        {
            Data = data;
            Status = status;
            Initialize();
        }

        protected abstract void Initialize();
    }
}
