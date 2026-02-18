using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static Rect LocalRect(this RectTransform tr)
        {
            return tr.rect;
        }
        
        public static Rect WorldRect(this RectTransform tr)
        {
            if (tr == null) throw new ArgumentNullException(nameof(tr));
            var localRect = tr.rect;
            var min = tr.TransformPoint(localRect.min);
            var max = tr.TransformPoint(localRect.max);
            // Debug.Log($"{localRect} {min} {max}");
            var res = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            return res;
        }
    }
}
