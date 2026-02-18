using UnityEngine;
using System;

namespace Prota.Unity
{
    [Serializable]
    public class TweenDefinition
    {
        // 播放时长. 决定了播放速度.
        public float duration = 0.3f;
        
        // [Header("移动")]
        public bool move;
        public Vector3 moveFrom = Vector3.zero;
        public Vector3 moveTo = Vector3.zero;
        public TweenEaseEnum moveEase = TweenEaseEnum.Linear;
        public AnimationCurve moveCurve = new AnimationCurve();
        public Vector3 GetMove(float t)
        {
            float ratio;
            if(moveEase == TweenEaseEnum.None) ratio = moveCurve.Evaluate(t);
            else ratio = TweenEase.GetFromEnum(moveEase).Evaluate(t);
            return Vector3.Lerp(moveFrom, moveTo, ratio);
        }
        
        // [Header("旋转")]
        public bool rotate;
        public Vector3 rotateFrom = Vector3.zero;
        public Vector3 rotateTo = Vector3.zero;
        public TweenEaseEnum rotateEase = TweenEaseEnum.Linear;
        public AnimationCurve rotateCurve = new AnimationCurve();
        public bool useClosestRotate = true;
        public Vector3 GetRotate(float t)
        {
            float ratio;
            if(rotateEase == TweenEaseEnum.None) ratio = rotateCurve.Evaluate(t);
            else ratio = TweenEase.GetFromEnum(rotateEase).Evaluate(t);
            if(useClosestRotate) return (rotateFrom, rotateTo).LerpAngle(ratio);
            return Vector3.Lerp(rotateFrom, rotateTo, ratio); 
        }
        
        // [Header("缩放")]
        public bool scale;
        public Vector3 scaleFrom = Vector3.one;
        public Vector3 scaleTo = Vector3.one;
        public TweenEaseEnum scaleEase = TweenEaseEnum.Linear;
        public AnimationCurve scaleCurve = new AnimationCurve();
        public Vector3 GetScale(float t)
        {
            float ratio;
            if(scaleEase == TweenEaseEnum.None) ratio = scaleCurve.Evaluate(t);
            else ratio = TweenEase.GetFromEnum(scaleEase).Evaluate(t);
            return Vector3.Lerp(scaleFrom, scaleTo, ratio);
        }
        
        // [Header("颜色")]
        public bool color;
        [ColorUsage(true, true)] public Color colorFrom = Color.white;
        [ColorUsage(true, true)] public Color colorTo = Color.white;
        public TweenEaseEnum colorEase = TweenEaseEnum.Linear;
        public Gradient colorGradient = new Gradient();
        
        // [Header("透明度")]
        public bool alpha;
        public float alphaFrom = 1;
        public float alphaTo = 1;
        public TweenEaseEnum alphaEase = TweenEaseEnum.Linear;
        public AnimationCurve alphaCurve = new AnimationCurve();
        
        // [Header("尺寸")]
        public bool size;
        public Vector2 sizeFrom = Vector2.one;
        public Vector2 sizeTo = Vector2.one;
        public TweenEaseEnum sizeEase = TweenEaseEnum.Linear;
        public AnimationCurve sizeCurve = new AnimationCurve();
        
        // [Header("尺寸X")]
        public bool sizeX;
        public float sizeXFrom = 1;
        public float sizeXTo = 1;
        public TweenEaseEnum sizeXEase = TweenEaseEnum.Linear;
        public AnimationCurve sizeXCurve = new AnimationCurve();
        
        // [Header("尺寸Y")]
        public bool sizeY;
        public float sizeYFrom = 1;
        public float sizeYTo = 1;
        public TweenEaseEnum sizeYEase = TweenEaseEnum.Linear;
        public AnimationCurve sizeYCurve = new AnimationCurve();
		
        public ref Vector2 GetSizeXY(ref Vector2 size, float t)
        {
            if(this.size)
            {
                float ratio;
                if(sizeEase == TweenEaseEnum.None) ratio = sizeCurve.Evaluate(t);
                else ratio = TweenEase.GetFromEnum(sizeEase).Evaluate(t);
                size = Vector2.Lerp(sizeFrom, sizeTo, ratio);
            }
            
            if(sizeX)
            {
                float ratio;
                if(sizeXEase == TweenEaseEnum.None) ratio = sizeXCurve.Evaluate(t);
                else ratio = TweenEase.GetFromEnum(sizeXEase).Evaluate(t);
                size.x = Mathf.Lerp(sizeXFrom, sizeXTo, ratio);
            }
            
            if(sizeY)
            {
                float ratio;
                if(sizeYEase == TweenEaseEnum.None) ratio = sizeYCurve.Evaluate(t);
                else ratio = TweenEase.GetFromEnum(sizeYEase).Evaluate(t);
                size.y = Mathf.Lerp(sizeYFrom, sizeYTo, ratio);
            }
            
            return ref size;
        }
        
        public ref Color GetColorAlpha(ref Color color, float t)
        {
            if(this.color)
            {
                if(colorEase == TweenEaseEnum.None) color = colorGradient.Evaluate(t);
                else color = Color.Lerp(colorFrom, colorTo, TweenEase.GetFromEnum(colorEase).Evaluate(t));
            }
            
            if(alpha)
            {
                if(alphaEase == TweenEaseEnum.None) color.a = alphaCurve.Evaluate(t);
                else color.a = Mathf.Lerp(alphaFrom, alphaTo, TweenEase.GetFromEnum(alphaEase).Evaluate(t));
            }
            
            return ref color;
        }
        
    }
    
}
