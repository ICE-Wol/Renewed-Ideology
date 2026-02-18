using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Prota.Unity
{

    public class Timer
    {
        static ulong gid = 0;
		
        public string hint = null;
        public ulong id { get; private set; }
        public float nextTime { get; private set; }
		public Action callback { get; private set; }
		
        public Action<object> callbackWithReceiver { get; private set; }
        public object callbackReceiver { get; private set; }
        
		public bool repeat { get; private set; }
        public float delay { get; private set; }
        public float minDelay { get; private set; }
        public float maxDelay { get; private set; }
		
		public bool isRemoved { get; private set; }
		public bool isRealtime { get; private set; }
		
		public UnityEngine.Object owner { get; private set; }
		
		public bool isAlive => !isRemoved && owner != null;
        
		public Timer(float curTime, float minDelay, float maxDelay, bool repeat, bool realtime, UnityEngine.Object owner, string hint = null)
		{
			Initialize(curTime, minDelay, maxDelay, repeat, realtime, owner, hint);
		}
		
		void Initialize(float curTime, float minDelay, float maxDelay, bool repeat, bool realtime, UnityEngine.Object owner, string hint = null)
        {
            if(minDelay > maxDelay)
                throw new ArgumentOutOfRangeException($"Timer - minDelay [{minDelay}] must be less than or equal to maxDelay [{maxDelay}]");
            
			if(owner == null)
			{
				throw new ArgumentNullException($"Timer - owner is null");
			}
			
            this.hint = hint;
            this.id = unchecked(++gid);
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
            this.delay = minDelay == maxDelay ? minDelay : UnityEngine.Random.Range(minDelay, maxDelay);
            this.nextTime = curTime + this.delay;
            this.repeat = repeat;
			this.isRealtime = realtime;
			this.isRemoved = false;
			this.callback = null;
			this.callbackWithReceiver = null;
			this.callbackReceiver = null;
			this.owner = owner;
        }

        public void SetCallback(Action cb)
        {
            this.callbackWithReceiver = null;
            this.callbackReceiver = null;
            this.callback = cb;
        }

        public void SetCallback(Action<object> cb, object receiver)
        {
            this.callbackWithReceiver = cb;
            this.callbackReceiver = receiver;
            this.callback = () => cb?.Invoke(receiver);
        }
        
        internal bool NextRepeat()
        {
			if(!repeat) return false;
            this.delay = minDelay == maxDelay ? minDelay : UnityEngine.Random.Range(minDelay, maxDelay);
            nextTime += delay;
            return true;
        }
		
		public override string ToString()
		{
			var hintStr = hint ?? $"<none>";
			var ownerStr = GetOwnerDesc();
			return $"Timer[{hintStr}] id:{id} nextTime:{nextTime:F2} delay:{delay:F2} [{minDelay:F2}-{maxDelay:F2}] repeat:{repeat} realtime:{isRealtime} alive:{isAlive} removed:{isRemoved} owner:{ownerStr}";
		}

		public string GetOwnerDesc()
		{
			if (owner == null) return "<null>";
			if (owner is GameObject ownerGameObject) return $"{ownerGameObject.GetNamePath()}";
			if (owner is Component ownerComponent) return $"{ownerComponent.GetNamePath()}";
			return $"<{owner.GetType().Name}>";
		}

		public void SetDelay(float minDelay, float maxDelay)
		{
			if (minDelay > maxDelay)
				throw new ArgumentOutOfRangeException($"Timer.SetDelay - minDelay [{minDelay}] must be less than or equal to maxDelay [{maxDelay}]");
			this.minDelay = minDelay;
			this.maxDelay = maxDelay;
			// 会在下一次 repeat 生效.
		}
		
		public void SetDelay(float delay) => SetDelay(delay, delay);
		
		public void Destroy()
		{
			isRemoved = true;
			callback = null;
			callbackWithReceiver = null;
			callbackReceiver = null;
			owner = null;
			hint = null;
		}
    }
}
