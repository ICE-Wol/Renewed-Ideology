using System;
using System.Threading;

namespace Prota
{
    public struct ResLock
    {
        int _count;
        public int count => _count;
        public void Access()
        {
            if(Interlocked.CompareExchange(ref _count, 1, 0) != 0)
                throw new InvalidOperationException("ResLock is not reentrant");
        }
        public void Release()
        {
            if(Interlocked.CompareExchange(ref _count, 0, 1) != 1)
                throw new InvalidOperationException("ResLock is not reentrant");
        }
    }
}
