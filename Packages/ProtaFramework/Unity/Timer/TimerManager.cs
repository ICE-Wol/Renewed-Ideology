using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Unity
{
    public class TimerManager
    {
		// 这是一个方便取用的接口.
		// TimerManager 类并不会管理它的生命周期.
		public static TimerManager instance = null!;
		
		
        // 小间隔时间采用时间轮(仅游戏时间). 与 lua 配置一致.
        // 改为由外部通过配置接口设定构造参数, 这里提供默认值作为兜底.
        static float timingWheelTick = 0.004f;           // 默认 4ms
        static int timingWheelBucketCount = 1000;         // 默认 1000 个槽位 4s
        
        readonly TimingWheel gameWheel;
        readonly TimerQueue normalQueue;
        readonly TimerQueue realtimeQueue;
		
        public TimerLog timerLog { get; }
        
        public TimerManager(bool enableLog = false, string logFilePath = "timer.log.txt", bool enableConsole = false)
        {
			gameWheel = new TimingWheel("GameWheel", timingWheelTick, timingWheelBucketCount);
			normalQueue = new TimerQueue();
			realtimeQueue = new TimerQueue();
			timerLog = enableLog ? new TimerLog(logFilePath, enableConsole) : null;
			
			if(timerLog != null)
			{
				timerLog.enable = enableLog;
				gameWheel.SetTimerLog(timerLog);
				normalQueue.SetTimerLog(timerLog);
				realtimeQueue.SetTimerLog(timerLog);
			}
        }
        
        public void Update(float time, float realtime)
        {
            normalQueue.Update(time);
            realtimeQueue.Update(realtime);
			gameWheel.Update(time);
        }
        
		public void AddTimer(Timer timer)
		{
			timerLog?.RecordAdd(timer);
			if(timer.maxDelay < gameWheel.durationLimit)
			{
				gameWheel.AddTimer(timer);
			}
			else
			{
				if(timer.isRealtime)
				{
					realtimeQueue.Add(timer);
				}
				else
				{
					normalQueue.Add(timer);
				}
			}
		}

        void UpdateQueues()
        {
        }

        public void ClearAllQueues()
        {
            normalQueue.Clear();
            realtimeQueue.Clear();
			gameWheel.ClearAll();
        }
    }
}
