using System;

namespace Toolbox.Protocol
{
    public interface IMethod : IDisposable
    {
        byte[] Encode();
        bool Decode(byte[] data);


    }
}
