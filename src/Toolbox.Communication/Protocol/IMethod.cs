using System;

namespace Toolbox.Communication.Protocol
{
    public interface IMethod : IDisposable
    {
        byte[] Encode();
        bool Decode(byte[] data);
    }
}
