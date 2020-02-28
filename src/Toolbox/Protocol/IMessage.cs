using System;
using System.Collections.Generic;

namespace Toolbox.Protocol
{
    public interface IMessage : IDisposable
    {
        byte[] Encode();
        bool Decode(byte[] data);
        IDictionary<string, object> Data();
    }
}
