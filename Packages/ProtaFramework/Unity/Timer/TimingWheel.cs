using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Prota.Unity;

/// <summary>
/// 固定精度/固定时长的时间轮实现.
/// 仅支持将执行时间位于 [curTime, curTime + maxDuration - tick) 的定时器加入.
/// </summary>
public sealed class TimingWheel
{
	public readonly string wheelName;
	
	// 当前处于哪个桶. 从 0 开始. 当从桶i移动到桶i+1时, 触发桶i的定时器.
	public int currentBucket;
	
	// 当前桶剩余时间. 从 0 开始. 当从桶i移动到桶i+1时, 剩余时间减少 tick.
	public double currentBucketRestTime;
	
	// 桶步进时长. 每个桶的时长为 tick.
	public readonly double tick;
	
	// 一个轮回的时长. 一个轮回包含 bucketCount 个桶.
    public readonly double loopDuration;
	
	// 定时器能接收的最大延迟时间. 当定时器延迟时间超过此值时, 定时器无法加入时间轮.
	public readonly double durationLimit;
	
	public readonly int slotCount;
	readonly List<List<Prota.Unity.Timer>> slots;
	
	// 安全时间限制. 当时间步进量超过此值时, 认为时间步进量过大, 直接触发所有定时器.
	public readonly double safeTimeLimit;
	
	// 当前时间.
	public double curTime { get; private set; }
	
	/// <param name="wheelName">时间轮名称, 仅用于日志</param>
    /// <param name="tick">槽位步进时长, 必须为正数</param>
    /// <param name="bucketCount">槽位数量, 必须为正整数, maxDuration = tick * bucketCount</param>
    public TimingWheel(string wheelName, double tick, int bucketCount)
	{
        if(tick <= 1e-6) throw new ArgumentOutOfRangeException($"TimingWheel - tick [{tick}] is too small, must be greater than 1e-6");
        if(bucketCount <= 0) throw new ArgumentOutOfRangeException($"TimingWheel - bucketCount [{bucketCount}] is invalid, must be positive");
		if(bucketCount < 5) throw new ArgumentOutOfRangeException($"TimingWheel - bucketCount [{bucketCount}] is too small, must be greater than 5");
		if(bucketCount >= 100000) throw new ArgumentOutOfRangeException($"TimingWheel - bucketCount [{bucketCount}] is too large, must be less than 100");
		
		this.wheelName = wheelName;
		this.curTime = 0.0;
		this.currentBucket = 0;
		this.currentBucketRestTime = tick;
		this.tick = tick;
        this.loopDuration = tick * bucketCount;
        this.durationLimit = this.loopDuration - tick;
        this.slotCount = bucketCount;
		this.slots = new List<List<Prota.Unity.Timer>>(this.slotCount);
		for(var i = 0; i < this.slotCount; i++) this.slots.Add(new List<Prota.Unity.Timer>(4));
		this.safeTimeLimit = this.loopDuration - 5 * this.tick;
	}

	TimerLog timerLog = null;
	
	public void SetTimerLog(TimerLog log)
	{
		timerLog = log;
	}
	
	/// <summary>
	/// 添加定时器.要求 timer.nextTime - curTime 严格小于 (loopDuration - tick).
	/// </summary>
	public void AddTimer(Prota.Unity.Timer timer)
	{
		if(!timer.isAlive)
			throw new ArgumentException($"TimingWheel.AddTimer - timer [{timer}] is not alive");
		
		if(timer.maxDelay >= durationLimit)
			throw new ArgumentOutOfRangeException($"TimingWheel.AddTimer - timer maxDelay [{timer.maxDelay}] is too large, must be less than [{durationLimit}]");
		
		if(timer.minDelay < 0)
			throw new ArgumentOutOfRangeException($"TimingWheel.AddTimer - timer minDelay [{timer.minDelay}] is too small, must be greater than 0");
		
		// 这里根据 timer.delay 来保证插入槽位确实代表着 nextTime.
		var timeInLoop = timer.nextTime % loopDuration;
		var bucketIndex = (int)Math.Floor(timeInLoop / tick);
		slots[bucketIndex].Add(timer);
		timerLog?.RecordAdd(timer);
	}
	
	/// <summary>
	/// 内部添加定时器，不记录日志（用于重复定时器的重新添加）.
	/// </summary>
	void AddTimerInternal(Prota.Unity.Timer timer)
	{
		if(!timer.isAlive)
			throw new ArgumentException($"TimingWheel.AddTimerInternal - timer [{timer}] is not alive");
		
		if(timer.maxDelay >= durationLimit)
			throw new ArgumentOutOfRangeException($"TimingWheel.AddTimerInternal - timer maxDelay [{timer.maxDelay}] is too large, must be less than [{durationLimit}]");
		
		if(timer.minDelay < 0)
			throw new ArgumentOutOfRangeException($"TimingWheel.AddTimerInternal - timer minDelay [{timer.minDelay}] is too small, must be greater than 0");
		
		var timeInLoop = timer.nextTime % loopDuration;
		var bucketIndex = (int)Math.Floor(timeInLoop / tick);
		slots[bucketIndex].Add(timer);
	}
	
	public void Update(double time)
	{
		if(time < curTime)
		{
			// 当做输入了同一个时间.
			if(time > curTime - 1e-6f) return; 
			
			// 输入了过去的时间.
			throw new ArgumentOutOfRangeException($"TimingWheel.Update - time [{time}] is too small, must be greater than [{curTime}]");
		}
		
		if(time == curTime) return;
		
		var delta = time - curTime;
		Step(delta);
		curTime = time;
	}
	
	/// <summary>
	/// 推进时间并触发到期定时器.
	/// </summary>
	void Step(double delta)
	{
		if(delta == 0) return;
		
		if(delta < 0)
		{
			throw new ArgumentOutOfRangeException($"TimingWheel.Update - delta [{delta}] is too small, must be greater than 0");
		}
		
		if(delta >= safeTimeLimit)
		{
			Debug.LogWarning("时间步进量过大, 直接触发所有定时器");
			for(var i = 0; i < slotCount; i++)
			{
				var slot = slots[i];
				TriggerTimers(slot);
				slot.Clear();
			}
			// 大步进场景下, 认为直接清空, 保持当前位置与剩余时间不变
			return;
		}

		currentBucketRestTime += delta;
		while(currentBucketRestTime >= tick)
		{
			// 触发当前桶
			var currentSlotIndex = currentBucket;
			var currentSlot = slots[currentSlotIndex];
			if(currentSlot.Count > 0)
			{
				TriggerTimers(currentSlot);
				currentSlot.Clear();
			}
			// 推进到下一个桶
			currentBucket = (currentBucket + 1) % slotCount;
			currentBucketRestTime -= tick;
		}
	}

	void TriggerTimers(List<Prota.Unity.Timer> timers)
	{
		foreach(var timer in timers)
		{
			if(!timer.isAlive) continue;

			try
			{
				timer.callback?.Invoke();
				timer.callbackWithReceiver?.Invoke(timer.callbackReceiver);
			}
			catch(Exception e)
			{
				UnityEngine.Debug.LogError($"TimingWheel.TriggerTimers - timer [{timer.GetOwnerDesc()}] callback error: {e}");
			}
			
			// 注意在回调里 isAlive 可能已经变为 false.
			if(timer.repeat && timer.isAlive)
			{
				timer.NextRepeat();
				AddTimerInternal(timer); // 重新添加到时间轮, 时间轮添加规则保证不会添加到当前槽位，不记录日志
			}
			else
			{
				timerLog?.RecordRemove(timer);
				timer.Destroy();
			}
		}
		
		timers.Clear();
	}

	public int GetActiveCount()
	{
		var count = 0;
		for(var si = 0; si < slots.Count; si++)
		{
			var slot = slots[si];
			for(var ti = 0; ti < slot.Count; ti++)
			{
				var timer = slot[ti];
				if(!timer.isAlive) continue;
				count++;
			}
		}
		return count;
	}

	public void ClearAll()
	{
		for(var i = 0; i < slots.Count; i++)
		{
			foreach(var timer in slots[i])
			{
				timerLog?.RecordRemove(timer);
				timer.Destroy();
			}
			slots[i].Clear();
		}
	}
}


