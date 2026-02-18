
using System;
using UnityEngine;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        
        public static Rect GetCameraWorldView2D(this Camera camera)
        {
            var bottomLeft = OrthoViewportToWorldPoint(camera, Vector2.zero);
            var topRight = OrthoViewportToWorldPoint(camera, Vector2.one);
            return new Rect(bottomLeft, topRight - bottomLeft);
        }
        
        
        static Vector3 OrthoViewportToWorldPoint(Camera camera, Vector3 viewportPoint)
        {
            if (!camera.orthographic)
            {
                Debug.LogError("Camera must be orthographic.");
                return Vector3.zero;
            }
            
            float halfWidth = camera.orthographicSize * camera.aspect;
            float halfHeight = camera.orthographicSize;
            
            float worldX = (viewportPoint.x - 0.5f) * 2 * halfWidth;
            float worldY = (viewportPoint.y - 0.5f) * 2 * halfHeight;
            
            Vector3 worldPoint = new Vector3(worldX, worldY, viewportPoint.z) + camera.transform.position;
            
            return worldPoint;
        }

        public static float PixelPerUnit(this Camera camera)
        {
            return camera.orthographicSize * 2 / Screen.height;
        }

        public static Vector2 WorldPosSnapToPixel(this Camera camera, Vector2 worldPos)
        {
            var ppu = camera.PixelPerUnit();
            return new Vector2(
                Mathf.Round(worldPos.x * ppu) / ppu,
                Mathf.Round(worldPos.y * ppu) / ppu
            );
        }
        
    }
}
