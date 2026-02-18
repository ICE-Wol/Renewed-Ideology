using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Unity
{
    [ExecuteAlways]
    public class KeepInArea : MonoBehaviour
    {
        public RectTransform area;

        void Update()
        {
            if(Application.isPlaying) return;
            // LateUpdate();
        }

        void LateUpdate()
        {
            if (this.area == null) return;

            var tr = this.transform;
            var rectTransform = tr as RectTransform;
            var areaRect = this.area.rect;
            var area = this.area.WorldRect();
            
            if (rectTransform != null)
            {
                var rect = rectTransform.WorldRect();
                var offsetX = 0f;
                if (rect.xMin < area.min.x) offsetX = area.min.x - rect.xMin;
                if (rect.xMax > area.max.x) offsetX = area.max.x - rect.xMax;
                var offsetY = 0f;
                if (rect.yMin < area.min.y) offsetY = area.min.y - rect.yMin;
                if (rect.yMax > area.max.y) offsetY = area.max.y - rect.yMax;
                rectTransform.position += new Vector3(offsetX, offsetY, 0f);
            }
            else
            {
                var pos = tr.position;
                tr.position = new Vector3(
                    Mathf.Clamp(pos.x, area.min.x, area.max.x),
                    Mathf.Clamp(pos.y, area.min.y, area.max.y),
                    pos.z
                );
            }
        }
    }
}
