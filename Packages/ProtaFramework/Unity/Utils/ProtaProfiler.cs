using System;
using UnityEngine;

namespace Prota.Unity
{
	/// <summary>
	/// 性能分析工具类，使用 Stopwatch 测量代码执行时间
	/// </summary>
	public static class ProtaProfiler
	{
		/// <summary>
		/// 性能分析作用域，支持 using 语句自动测量时间
		/// </summary>
		public struct Scope : IDisposable
		{
			private readonly string name;
			private readonly System.Diagnostics.Stopwatch stopwatch;
			private readonly bool enabled;
			
			internal Scope(string name, bool enabled)
			{
				this.name = name;
				this.enabled = enabled;
				this.stopwatch = enabled ? System.Diagnostics.Stopwatch.StartNew() : null;
			}
			
			public void Dispose()
			{
				if (enabled && stopwatch != null)
				{
					stopwatch.Stop();
					Debug.Log($"{name} 执行时间: {stopwatch.ElapsedMilliseconds}ms");
				}
			}
		}
		
		/// <summary>
		/// 创建一个性能分析作用域
		/// </summary>
		/// <param name="name">作用域名称，将显示在日志中</param>
		/// <returns>Scope 结构体，用于 using 语句</returns>
		public static Scope Begin(string name)
		{
			return new Scope(name, true);
		}
		
		/// <summary>
		/// 创建一个禁用的性能分析作用域（用于条件编译或运行时开关）
		/// </summary>
		/// <param name="name">作用域名称</param>
		/// <returns>Scope 结构体</returns>
		public static Scope BeginDisabled(string name)
		{
			return new Scope(name, false);
		}
	}

}