
using System;
using UnityEngine;

namespace Prota.Unity
{
	public static class TimerExt
	{
		public static Timer NewTimer(this UnityEngine.Object g, float delay, Action callback, string hint = null)
		{
			var timer = new Timer(Time.time, delay, delay, false, false, g, hint);
			timer.SetCallback(callback);
			TimerManager.instance.AddTimer(timer);
			return timer;
		}
		
		public static Timer NewTimer(this UnityEngine.Object g, float delay, Action<GameObject> callback, string hint = null)
	{
		var timer = new Timer(Time.time, delay, delay, false, false, g, hint);
		timer.SetCallback(a => callback?.Invoke(g as GameObject), g);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	
	// 随机延迟的一次性计时器
	public static Timer NewRandomTimer(this UnityEngine.Object g, float minDelay, float maxDelay, Action callback, string hint = null)
	{
		var timer = new Timer(Time.time, minDelay, maxDelay, false, false, g, hint);
		timer.SetCallback(callback);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	
	public static Timer NewRandomTimer(this UnityEngine.Object g, float minDelay, float maxDelay, Action<GameObject> callback, string hint = null)
	{
		var timer = new Timer(Time.time, minDelay, maxDelay, false, false, g, hint);
		timer.SetCallback(a => callback?.Invoke(g as GameObject), g);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	
	// 随机延迟的重复计时器
	public static Timer NewRandomTimerRepeat(this UnityEngine.Object g, float minDelay, float maxDelay, Action callback, string hint = null)
	{
		var timer = new Timer(Time.time, minDelay, maxDelay, true, false, g, hint);
		timer.SetCallback(callback);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	
	public static Timer NewRandomTimerRepeat(this UnityEngine.Object g, float minDelay, float maxDelay, Action<GameObject> callback, string hint = null)
	{
		var timer = new Timer(Time.time, minDelay, maxDelay, true, false, g, hint);
		timer.SetCallback(a => callback?.Invoke(g as GameObject), g);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	
	// 固定延迟的重复计时器
	public static Timer NewTimerRepeat(this UnityEngine.Object g, float delay, Action callback, string hint = null)
	{
		var timer = new Timer(Time.time, delay, delay, true, false, g, hint);
		timer.SetCallback(callback);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	
	public static Timer NewTimerRepeat(this UnityEngine.Object g, float delay, Action<GameObject> callback, string hint = null)
	{
		var timer = new Timer(Time.time, delay, delay, true, false, g, hint);
		timer.SetCallback(a => callback?.Invoke(g as GameObject), g);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	
	// Realtime 版本 - 随机延迟的一次性计时器
	public static Timer NewRandomTimerRealtime(this UnityEngine.Object g, float minDelay, float maxDelay, Action callback, string hint = null)
	{
		var timer = new Timer(Time.realtimeSinceStartup, minDelay, maxDelay, false, true, g, hint);
		timer.SetCallback(callback);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	
	public static Timer NewRandomTimerRealtime(this UnityEngine.Object g, float minDelay, float maxDelay, Action<GameObject> callback, string hint = null)
	{
		var timer = new Timer(Time.realtimeSinceStartup, minDelay, maxDelay, false, true, g, hint);
		timer.SetCallback(a => callback?.Invoke(g as GameObject), g);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	
	// Realtime 版本 - 随机延迟的重复计时器
	public static Timer NewRandomTimerRepeatRealtime(this UnityEngine.Object g, float minDelay, float maxDelay, Action callback, string hint = null)
	{
		var timer = new Timer(Time.realtimeSinceStartup, minDelay, maxDelay, true, true, g, hint);
		timer.SetCallback(callback);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	
	public static Timer NewRandomTimerRepeatRealtime(this UnityEngine.Object g, float minDelay, float maxDelay, Action<GameObject> callback, string hint = null)
	{
		var timer = new Timer(Time.realtimeSinceStartup, minDelay, maxDelay, true, true, g, hint);
		timer.SetCallback(a => callback?.Invoke(g as GameObject), g);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	
	// Realtime 版本 - 固定延迟的重复计时器
	public static Timer NewTimerRepeatRealtime(this UnityEngine.Object g, float delay, Action callback, string hint = null)
	{
		var timer = new Timer(Time.realtimeSinceStartup, delay, delay, true, true, g, hint);
		timer.SetCallback(callback);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	
	public static Timer NewTimerRepeatRealtime(this UnityEngine.Object g, float delay, Action<GameObject> callback, string hint = null)
	{
		var timer = new Timer(Time.realtimeSinceStartup, delay, delay, true, true, g, hint);
		timer.SetCallback(a => callback?.Invoke(g as GameObject), g);
		TimerManager.instance.AddTimer(timer);
		return timer;
	}
	}
}