using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Prota;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Prota.Unity
{

    public class EditorTimer : IComparable<EditorTimer>
    {
        public int id;
        public bool repeat;
        public float executeTime;
        public float startTime;
        public float duration;
        public Action callback;
        
        public int CompareTo(EditorTimer other)
        {
            if(executeTime != other.executeTime) return executeTime.CompareTo(other.executeTime);
            return this.id.CompareTo(other.id);
        }
        
        public EditorTimer(int id)
        {
            this.id = id;
        }
        
        public void SetStart(float currentTime, float duration, bool repeat, Action callback)
        {
            if(duration <= 1e-4f) throw new Exception($"duration is too short. {duration}");
            this.callback = callback ?? throw new Exception("callback is null.");
            startTime = currentTime;
            executeTime = currentTime + duration;
            this.duration = duration;
            this.repeat = repeat;
        }
        
        public void Remove()
        {
            EditorTimerManager.Remove(this);
        }
    }



    // ============================================================================
    // ============================================================================



    public class EditorTimerManager
    {
        static List<EditorTimer> timerPool = new();
        static HashSet<int> idleIds = new();
        static List<Action> onManagerInitializedCallbacks = new();
        static bool initialized = false;
        
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            EditorApplication.update += Update;
            stopwatch.Start();
            
            foreach(var callback in onManagerInitializedCallbacks)
                callback.Invoke();
            onManagerInitializedCallbacks.Clear();
            initialized = true;
        }
        
        public static void OnManagerInitialized(Action callback)
        {
            if(callback == null) return;
            if(initialized)
            {
                callback.Invoke();
                return;
            }
            onManagerInitializedCallbacks.Add(callback);
        }
        
        static readonly Stopwatch stopwatch = new();
        static readonly SortedSet<EditorTimer> runningTimers = new();
        
        
        static EditorTimer NewTimer()
        {
            if(idleIds.Count > 0)
            {
                var id = idleIds.First();
                idleIds.Remove(id);
                var timer = timerPool[id];
                timer.id = id;
                return timer;
            }
            else
            {
                var timer = new EditorTimer(timerPool.Count);
                timerPool.Add(timer);
                return timer;
            }
        }
        
        public static EditorTimer New(float duration, Action callback)
        {
            var timer = NewTimer();
            timer.SetStart(stopwatch.ElapsedMilliseconds / 1000f, duration, false, callback);
            runningTimers.Add(timer);
            return timer;
        }
        
        public static EditorTimer NewRepeat(float duration, Action callback)
        {
            var timer = NewTimer();
            timer.SetStart(stopwatch.ElapsedMilliseconds / 1000f, duration, true, callback);
            runningTimers.Add(timer);
            return timer;
        }
        
        public static void Remove(EditorTimer timer)
        {
            if(timer == null) return;
            Debug.Assert(timer.id != -1, "Timer is already removed.");
            idleIds.Add(timer.id);
            runningTimers.Remove(timer);
            timer.id = -1;
        }
        
        static void Update()
        {
            while(runningTimers.Count > 0)
            {
                var timer = runningTimers.Min;
                var currentTime = stopwatch.ElapsedMilliseconds / 1000f;
                if(timer.executeTime < currentTime)
                {
                    
                    Profiler.BeginSample($"EditorTimer.Update - [{timer.callback?.Method.ToInfoString() ?? "<unknown>"}]");
                    timer.callback?.Invoke();
                    Profiler.EndSample();
                    runningTimers.Remove(timer);
                    
                    if(timer.repeat)
                    {
                        timer.SetStart(currentTime, timer.duration, true, timer.callback);
                        runningTimers.Add(timer);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}