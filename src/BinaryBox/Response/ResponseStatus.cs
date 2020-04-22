namespace BinaryBox.Core.Response
{
    public abstract class ResponseStatus<TStatusCode> : IResponseStatus<TStatusCode>
        where TStatusCode : struct
    {
        public TStatusCode Code { get; protected set; }

        public bool Success { get; protected set; }

        public string Description { get; protected set; }

        public ResponseStatus(TStatusCode code)
        {
            Code = code;
            Initialize();
        }

        protected abstract void Initialize();
    }
}
