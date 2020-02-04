using System;
using System.Collections.Generic;

namespace Toolbox.Protocol
{
    public interface IMethod : IDisposable
    {
        byte[] Encode();
        bool Decode(byte[] data);
        IDictionary<string, object> Data();
    }
}
