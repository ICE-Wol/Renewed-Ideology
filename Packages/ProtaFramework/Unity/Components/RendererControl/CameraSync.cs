using System.Collections;
using System.Collections.Generic;
using Prota.Unity;
using UnityEngine;

namespace Prota.Unity
{

    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class CameraSync : MonoBehaviour
    {
        public Camera from;
        
        public float zOffset = 0f;
        public float nearOffset = 0f;
        public float farOffset = 0f;
        public bool syncTargetSize = false;
        
        [ShowWhen("syncTargetSize")] public float targetSizeMult = 1f;
        
        void Update()
        {
            if(!from) return;
            var to = this.GetComponent<Camera>();
            SyncCameraParameter(to);
            SyncCameraTarget(to);
        }
        
        
        void SyncCameraParameter(Camera to)
        {
            to.transform.position = from.transform.position + Vector3.forward * zOffset;
            to.orthographicSize = from.orthographicSize;
            to.orthographic = from.orthographic;
            to.fieldOfView = from.fieldOfView;
            to.nearClipPlane = from.nearClipPlane + nearOffset;
            to.farClipPlane = from.farClipPlane + farOffset;
            to.aspect = from.aspect;
        }
        
        void SyncCameraTarget(Camera to)
        {
            if(!to.targetTexture) return;
            if(syncTargetSize)
            {
                var size = GetFromTextureSize(from.targetTexture) * targetSizeMult;
                to.targetTexture.Resize(size.RoundToInt());
            }
        }
        
        
        Vector2 GetFromTextureSize(RenderTexture targetTexture)
        {
            if(targetTexture)
                return new Vector2(targetTexture.width, targetTexture.height);
            else
                return new Vector2(Screen.width, Screen.height);
            
        }
    }
}
