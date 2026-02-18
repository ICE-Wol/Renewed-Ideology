using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Unity
{
    // 使用 SortedSet 存储 Timer
    public class TimerQueue
    {
        sealed class TimerComparer : IComparer<Timer>
        {
            public int Compare(Timer a, Timer b)
                => a.nextTime != b.nextTime ? a.nextTime.CompareTo(b.nextTime)
                    : a.id.CompareTo(b.id);
        }
        
        const int timersPerUpdate = 50000;
        
        public readonly SortedSet<Timer> timers = new SortedSet<Timer>(new TimerComparer());
        TimerLog timerLog = null;
        
        public void SetTimerLog(TimerLog log)
        {
            timerLog = log;
        }
        
        public void Update(float curTime)
        {
            int i = 0, n = Math.Min(timersPerUpdate, timers.Count);
            for(i = 0; i < n; i++)
            {
                var timer = timers.Min;
                if(timer == null) break;
                if(curTime < timer.nextTime) break;
                
				// 先删除, 如果有需要再添加回去.
                timers.Remove(timer);
                
                if(!timer.isAlive) continue;
				
				try
				{
					timer.callback?.Invoke();
					timer.callbackWithReceiver?.Invoke(timer.callbackReceiver);
				}
				catch(Exception e)
				{
					UnityEngine.Debug.LogError($"TimerQueue.Update - timer [{timer.GetOwnerDesc()}] callback error: {e}");
				}
				
				// 注意在回调里 isAlive 可能已经变为 false.
				if(timer.repeat && timer.isAlive)
				{
					timer.NextRepeat();
					timers.Add(timer);
					// 重复定时器在内部重新添加时不记录日志
				}
				else
				{
					timerLog?.RecordRemove(timer);
					timer.Destroy();
				}
            }
			
            if(i == timersPerUpdate)
			{
				throw new Exception($"TimerQueue.Update - 达到{ timersPerUpdate }/帧计时器处理上限");
			}
        }
        
        public void Clear()
		{
			foreach(var timer in timers)
			{
				timerLog?.RecordRemove(timer);
				timer.Destroy();
			}
			timers.Clear();
		}
		
		public Timer Add(Timer timer)
		{
			if(!timer.isAlive)
				throw new ArgumentException($"TimerQueue.Add - timer [{timer.GetOwnerDesc()}] is not alive");
			
			timers.Add(timer);
			timerLog?.RecordAdd(timer);
			
			return timer;
		}
        
    }
}


