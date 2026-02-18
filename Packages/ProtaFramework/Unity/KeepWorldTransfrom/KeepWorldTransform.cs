using UnityEngine;

namespace Prota.Unity
{

    public class KeepWorldTransform : MonoBehaviour
    {
        public bool keepPosition;
        public bool keepRotation;
        public bool keepScale;
        
        void Awake()
        {
            KeepWorldTransformManager.instance.Add(this);
        }
        
        void OnDestroy()
        {
            if (AppQuit.isQuitting) return;
            KeepWorldTransformManager.instance.Remove(this);
        }
    }
}
