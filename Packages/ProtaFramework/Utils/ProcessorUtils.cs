using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;

namespace Prota
{
    /// <summary>
    /// 处理器信息工具类，提供获取 CPU/处理器相关信息的静态方法
    /// </summary>
    public static class ProcessorUtils
    {
        /// <summary>
        /// 获取当前线程正在运行的处理器核心 ID
        /// </summary>
        public static int GetCurrentThreadProcessorId()
        {
    		return Thread.GetCurrentProcessorId();
		}
		
#if UNITY_EDITOR_WIN
		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentProcessorNumber();
		
		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentThreadId();
		
		public static int GetCurrentThreadCore()
		{
			return (int)GetCurrentProcessorNumber();
		}
	
		public static void LockCurrentThreadToCore(uint coreMask)
		{
			uint osId = GetCurrentThreadId();
			var proc = Process.GetCurrentProcess();
			foreach (ProcessThread pt in proc.Threads)
			{
				if (pt.Id != osId) continue;
				pt.ProcessorAffinity = (IntPtr)coreMask;
				break;
			}
		}
#else

	public static int GetCurrentThreadCore()
	{
		throw new NotSupportedException();
	}
	
	public static void LockCurrentThreadToCore(uint coreMask)
	{
		throw new NotSupportedException();
	}

#endif	


    }
}

