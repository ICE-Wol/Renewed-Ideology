using UnityEngine;

namespace Prota.Unity
{
    // 让一个 rect mesh 的大小恰好覆盖相机.
    [ExecuteAlways]
    public class MatchCameraSize : MonoBehaviour
    {
        public Camera targetCamera;
        
        public bool overrideAspect = false;
        public float aspect = 1.0f;
        
        public float zOffset = 0;
        
        void Update()
        {
            if(targetCamera == null) return;
            var camtr = targetCamera.transform;
            var halfHeight = targetCamera.orthographicSize;
            var aspect = overrideAspect ? this.aspect : targetCamera.aspect;
            var halfWidth = halfHeight * aspect;
            this.transform.position = camtr.position + zOffset * camtr.forward;
            var scale = new Vector3(halfWidth * 2, halfHeight * 2, 1);
            this.transform.localScale = scale;
        }
        
        
    }
}
