using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Unity
{
    public enum CameraTargetResizerMode
    {
        Screen,
        CameraPPU,
    }

    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class CameraTargetResizer : MonoBehaviour
    {
        public CameraTargetResizerMode mode = CameraTargetResizerMode.Screen;
        public float ppu = 64f;
        public float aspect = 1f;   // width / height, used to define width.
        public float scale = 1f;

        [Inspect] public Camera cam;
        
        void Update()
        {
            TryGetComponent<Camera>(out cam);
            if(cam == null) throw new Exception("CameraTargetResizer requires a Camera component");
            var tgt = cam.targetTexture;
            if(tgt == null) return;
            var tgtSize = GetTargetSizeScaled();
            tgt.Resize(tgtSize.x, tgtSize.y);
        }

        Vector2Int GetTargetSizeScaled()
        {
            var tgtSize = GetTargetSize();
            return new Vector2(tgtSize.x * scale, tgtSize.y * scale).CeilToInt();
        }

        Vector2 GetTargetSize()
        {
            if(mode == CameraTargetResizerMode.Screen)
            {
                return new Vector2(Screen.width, Screen.height);
            }
            else if(mode == CameraTargetResizerMode.CameraPPU)
            {
                return new Vector2(cam.orthographicSize * ppu * 2 * aspect, cam.orthographicSize * ppu * 2);
            }
            return Vector2.zero;
        }
    }
}
