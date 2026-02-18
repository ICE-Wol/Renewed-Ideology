using UnityEngine;
using System;

namespace Prota.Unity
{
    [DisallowMultipleComponent]
    public class DestroyAfter : MonoBehaviour
    {
        [Serializable]
        public enum DestroyAfterEvent
        {
            Start,
            Manually,
        }
        
        public DestroyAfterEvent destroyAfterEvent = DestroyAfterEvent.Start;
        
        public float delay = 0;
        
        [Inspect] public bool destroyTriggered;
        
        void Awake()
        {
            destroyTriggered = false;
        }
        
        void Start()
        {
            if(destroyAfterEvent == DestroyAfterEvent.Start)
            {
                DoDestroy();
            }
        }
        
        public void Trigger()
        {
            DoDestroy();
        }
        
        void DoDestroy()
        {
            if(destroyTriggered) return;
            destroyTriggered = true;
            if(delay > 0)
            {
                DestroyAfterManager.instance.Schedule(this.gameObject, delay);
                return;
            }
            this.gameObject.ActiveDestroy();
        }
    }
}
