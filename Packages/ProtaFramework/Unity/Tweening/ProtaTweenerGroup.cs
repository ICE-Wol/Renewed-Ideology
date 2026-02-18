using UnityEngine;
using System;

namespace Prota.Unity
{
    [ExecuteAlways]
    public class ProtaTweenerGroup : MonoBehaviour
    {
        [SerializeField] public ProtaTweener[] tweeners = Array.Empty<ProtaTweener>();

		void ThrowNoTweener(string animName)
		{
			throw new Exception($"No tweener found on {gameObject.GetNamePath()} with anim name {animName}");
		}
		
		public void SetToStart(string animName = "")
		{
            bool found = false;
            foreach(var t in tweeners)
            {
                if(t.animName != animName) continue;
                found = true;
                t.SetToStart();
            }
            if(!found) ThrowNoTweener(animName);
		}

		public void SetToFinish(string animName = "")
		{
			bool found = false;
			foreach (var t in tweeners)
			{
				if (t.animName != animName) continue;
				found = true;
				t.SetToFinish();
			}
			if (!found) ThrowNoTweener(animName);
		}
		
        public void PlayForwardRestart(string animName = "")
        {
            bool found = false;
            foreach(var t in tweeners)
            {
                if(t.animName != animName) continue;
                found = true;
                t.PlayForwardRestart();
            }
            if(!found) ThrowNoTweener(animName);
        }

        public void PlayBackwardRestart(string animName = "")
        {
            bool found = false;
            foreach(var t in tweeners)
            {
                if(t.animName != animName) continue;
                found = true;
                t.PlayBackwardRestart();
            }
            if(!found) ThrowNoTweener(animName);
        }

        public void PlayForward(string animName = "")
        {
            bool found = false;
            foreach(var t in tweeners)
            {
                if(t.animName != animName) continue;
                found = true;
                t.PlayForward();
            }
            if(!found) ThrowNoTweener(animName);
        }

        public void PlayBackward(string animName = "")
        {
            bool found = false;
            foreach(var t in tweeners)
            {
                if(t.animName != animName) continue;
                found = true;
                t.PlayBackward();
            }
            if(!found) ThrowNoTweener(animName);
        }
    }
}
