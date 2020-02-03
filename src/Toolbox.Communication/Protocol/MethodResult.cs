using System;

namespace Toolbox.Communication.Protocol
{
    public abstract class MethodResult<TStatus> : IMethodResult<TStatus> where TStatus : struct
    {
        public TStatus Status { get; protected set; }
        public string Text { get; protected set; }

        public MethodResult(TStatus status, string text = "")
        {
            if (!typeof(TStatus).IsEnum)
            {
                throw new ArgumentException("TStatus must be an enumerated type");
            }
            Status = status;
            Text = text;
        }
    }
}
