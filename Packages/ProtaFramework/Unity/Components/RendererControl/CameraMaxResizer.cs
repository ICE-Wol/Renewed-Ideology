using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Unity
{
    // 这个组件调整相机视野, 使其视野总是不超过某个区域大小.
    // 对应的 CanvasScaler 建议调整为 ScaleWithScreenSize,
    // 并且 Reference Resolution 的长宽比和最大区域的长宽比一致.
    [ExecuteAlways]
    public class CameraMaxResizer : MonoBehaviour
    {
        // 如果是正交投影: 表示最大视野长宽.
        // 如果是透视投影: 表示最大视野角度.
        public Vector2 maxSize;
        
        // 投影面到相机的距离. 只有透视投影有用.
        public float planeDistance;
        
        public void Update()
        {
            var curAspect = (float)Screen.width / Screen.height;
            var targetAspect = maxSize.x / maxSize.y;
            
            if(this.TryGetComponent<Camera>(out var cam))
            {
                if(cam.orthographic)
                {
                    cam.orthographicSize = ComputeSize(curAspect, targetAspect);
                }
                else
                {
                    cam.fieldOfView = ComputeFov(curAspect, targetAspect);
                }
            }
            else if(this.GetComponent("CinemachineVirtualCamera").PassValue(out Component vcam))
            {
                // int a = 0;
                // void PrintLens(ProtaReflectionObject len)
                // {
                //     a++;
                //     print("====" + a + "====");
                //     print("type:" + len.target.GetType().ToString());
                //     print("fov:" + len.Get("FieldOfView"));
                //     print("size:" + len.Get("OrthographicSize"));
                //     print("orth:" + len.Get("Orthographic"));
                //     print("near:" + len.Get("NearClipPlane"));
                //     print("far:" + len.Get("FarClipPlane"));
                // }
                
                var p = vcam.ProtaReflection();
                var lens = p.Get("m_Lens").ProtaReflection();
                // PrintLens(lens);
                if((bool)lens.Get("Orthographic"))
                {
                    lens.Set("OrthographicSize", ComputeSize(curAspect, targetAspect));
                }
                else
                {
                    lens.Set("FieldOfView", ComputeFov(curAspect, targetAspect));
                }
                // PrintLens(lens);
                p.Set("m_Lens", lens.target);
                // PrintLens(p.Get("m_Lens").ProtaReflection());
            }
        }
        
        
        

        private float ComputeFov(float curAspect, float targetAspect)
        {
            if (targetAspect <= curAspect)
            {
                var curX = maxSize.x;
                var curY = maxSize.x / curAspect;
                return Mathf.Atan2(curY / 2, planeDistance) * Mathf.Rad2Deg * 2;
            }
            else
            {
                return Mathf.Atan2(maxSize.y / 2, planeDistance) * Mathf.Rad2Deg * 2;
            }
        }

        private float ComputeSize(float curAspect, float targetAspect)
        {
            if (targetAspect <= curAspect)
            {
                var curX = maxSize.x;
                var curY = maxSize.x / curAspect;
                // print($"{curX} : {curY} : {curAspect}");
                return curY / 2;
            }
            else
            {
                return maxSize.y / 2;
            }
        }

    }
}
