using System;

namespace Toolbox.Protocol
{
    public interface IMethodResult<TMethodStatus>
       where TMethodStatus : struct
    {
        TMethodStatus Status { get; }
        string Text { get; }

    }
    public abstract class MethodResult<TStatus> : IMethodResult<TStatus>
        where TStatus : struct
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

    //public abstract class MethodResult<TStatus, TData> : IMethodResult<TStatus>
    //    where TStatus : struct
    //    where TData : IDictionary<string, object>
    //{
    //    public TStatus Status { get; protected set; }
    //    public string Text { get; protected set; }

    //    public MethodResult(TStatus status, string text = "")
    //    {
    //        if (!typeof(TStatus).IsEnum)
    //        {
    //            throw new ArgumentException("TStatus must be an enumerated type");
    //        }
    //        Status = status;
    //        Text = text;
    //    }
    //}
}
