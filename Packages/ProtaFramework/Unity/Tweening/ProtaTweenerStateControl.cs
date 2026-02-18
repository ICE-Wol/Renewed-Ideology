using UnityEngine;
using System;

namespace Prota.Unity
{
    [ExecuteAlways]
    public class ProtaTweenerStateControl : MonoBehaviour
    {
        [Serializable]
        public struct TweenerState
        {
            public ProtaTweener tweener;
            public bool shouldPlay;
            public bool shouldPlayReverse;
        }
        
        [Serializable]
        public struct StateEntry
        {
            public string state;
            public TweenerState[] tweeners;
        }
        
        [SerializeField] string currentState;
        public StateEntry[] states = Array.Empty<StateEntry>();

        public string CurrentState => currentState;

        void Awake()
        {
            for (int i = 0; i < states.Length; i++)
            {
                ref var tweeners = ref states[i].tweeners;
                if (tweeners == null) tweeners = Array.Empty<TweenerState>();
            }
        }
        
        public ref readonly StateEntry GetStateEntry(string state)
        {
            for(int i = 0; i < states.Length; i++)
            {
                ref var s = ref states[i];
                if(s.state != state) continue;
                return ref s;
            }
            throw new Exception($"No state entry found: {state}");
        }

		public void SetState(string state)
		{
			if (currentState == state) return;
			currentState = state;

			ref readonly var entry = ref GetStateEntry(state);

			foreach (var t in entry.tweeners)
			{
				if (t.tweener == null) throw new Exception($"Null tweener in state entry: {state}");

				if (!t.shouldPlay)
				{
					t.tweener.play = false;
					t.tweener.running = false;
					continue;
				}

				t.tweener.playReversed = t.shouldPlayReverse;
				t.tweener.Play();
			}
		}
		
		public void SetStateInstantly(string state)
		{
			currentState = state;
			ref readonly var entry = ref GetStateEntry(state);
			foreach (var t in entry.tweeners)
			{
				if (t.tweener == null) throw new Exception($"Null tweener in state entry: {state}");
				if (!t.shouldPlay) continue;
				if (t.shouldPlayReverse) t.tweener.SetToStart();
				else t.tweener.SetToFinish();
			}
		}
    }
}

