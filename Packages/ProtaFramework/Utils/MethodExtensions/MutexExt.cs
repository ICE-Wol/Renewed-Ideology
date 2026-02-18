using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;


namespace Prota
{
    public struct MutexLock : IDisposable
    {
        public Mutex refMutex;
        public MutexLock(Mutex mutex)
        {
            refMutex = mutex;
            refMutex.WaitOne();
        }

        public void Dispose()
        {
            refMutex.ReleaseMutex();
        }
    }
    
    public static class MutexExt
    {
        public static MutexLock Lock(this Mutex mutex)
        {
            return new MutexLock(mutex);
        }
    }
    
}