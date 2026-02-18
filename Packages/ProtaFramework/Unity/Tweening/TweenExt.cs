using System;
using System.Diagnostics.CodeAnalysis;
using Prota;
using UnityEngine;

namespace Prota.Unity
{
	public static class TweenExt
	{
        public static TweenHandle NewTween(this UnityEngine.Object g, TweenId tid, ValueTweeningUpdate onUpdate)
        {
            return ProtaTweenManager.instance.New(tid, g, onUpdate);
        }
        
        public static TweenHandle TakeupTween(this UnityEngine.Object g, TweenId id)
        {
            return ProtaTweenManager.instance.New(id, g, ProtaTweenManager.doNothingValueTweening);
        }
        
        public static void TakeupTween(this UnityEngine.Object g, TweenId id, params TweenId[] xid)
        {
            g.TakeupTween(id);
            foreach(var i in xid) g.TakeupTween(i);
        }
        
        public static void ClearTween(this UnityEngine.Object g, TweenId id)
        {
            ProtaTweenManager.instance.Remove(g, id);
        }
        
        public static void ClearAllTween(this UnityEngine.Object g)
        {
            ProtaTweenManager.instance.RemoveAll(g);
        }
		
		
		// ============================================================================
		// ============================================================================
		
		
		public static bool TryGetTweener(this GameObject g, out ProtaTweener tweener)
		{
			var compCount = g.GetComponentCount();
			
			// 优先找到没有名字的 tweener.
			for(int i = 0; i < compCount; i++)
			{
				var comp = g.GetComponentAtIndex(i);
				if(comp is ProtaTweener t && t.animName.NullOrEmpty())
				{
					tweener = t;
					return true;
				}
			}
			
			// 其次找到任意一个.
			for(int i = 0; i < compCount; i++)
			{
				var comp = g.GetComponentAtIndex(i);
				if(comp is ProtaTweener t && !t.animName.NullOrEmpty())
				{
					tweener = t;
					return true;
				}
			}
			
			tweener = null!;
			return false;
		}
		
		public static bool TryGetTweener(this Component c, [NotNullWhen(true)] out ProtaTweener tweener)
		{
			return c.gameObject.TryGetTweener(out tweener);
		}
		
		public static bool TryGetTweener(this GameObject g, string animName, [NotNullWhen(true)] out ProtaTweener tweener)
		{
			var compCount = g.GetComponentCount();
			for(int i = 0; i < compCount; i++)
			{
				var comp = g.GetComponentAtIndex(i);
				if(comp is ProtaTweener t && t.animName == animName)
				{
					tweener = t;
					return true;
				}
			}
			tweener = null;
			return false;
		}
		
		public static bool TryGetTweener(this Component c, string animName, [NotNullWhen(true)] out ProtaTweener tweener)
		{
			return c.gameObject.TryGetTweener(animName, out tweener);
		}
		
		public static ProtaTweener GetTweener(this GameObject g)
		{
			if(g.TryGetTweener(out var tweener)) return tweener;
			throw new Exception($"No tweener found on {g.GetNamePath()}");
		}
		
		public static ProtaTweener GetTweener(this Component c)
		{
			if(c.gameObject.TryGetTweener(out var tweener)) return tweener;
			throw new Exception($"No tweener found on {c.GetNamePath()}");
		}
		
		public static ProtaTweener GetTweener(this GameObject g, string animName)
		{
			if(g.TryGetTweener(animName, out var tweener)) return tweener;
			throw new Exception($"No tweener found on {g.GetNamePath()} with anim name {animName}");
		}
		
		public static ProtaTweener GetTweener(this Component c, string animName)
		{
			if(c.gameObject.TryGetTweener(animName, out var tweener)) return tweener;
			throw new Exception($"No tweener found on {c.GetNamePath()} with anim name {animName}");
		}
	}	
}