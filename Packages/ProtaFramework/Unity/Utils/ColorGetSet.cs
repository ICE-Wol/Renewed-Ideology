using UnityEngine;
using Prota.Unity;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace Prota.Unity
{
    public static class ColorGetSet
    {
        static readonly List<Func<Component, Color>> getters = new();
        static readonly List<Action<Component, Color>> setters = new();
        static readonly Dictionary<Type, int> indexMap = new();
        
        static ColorGetSet()
        {
            void Add(Type type, Func<Component, Color> getter, Action<Component, Color> setter)
            {
                indexMap[type] = getters.Count;
                getters.Add(getter);
                setters.Add(setter);
            }
            
            Add(typeof(SpriteRenderer), c => ((SpriteRenderer)c).color, (c, v) => ((SpriteRenderer)c).color = v);
            Add(typeof(TextMeshPro), c => ((TextMeshPro)c).color, (c, v) => ((TextMeshPro)c).color = v);
            Add(typeof(UnityEngine.UI.Graphic), c => ((UnityEngine.UI.Graphic)c).color, (c, v) => ((UnityEngine.UI.Graphic)c).color = v);
            Add(typeof(CanvasGroup), c => ((CanvasGroup)c).alpha * Color.white, (c, v) => ((CanvasGroup)c).alpha = v.a);
            Add(typeof(ParticleSystem), c => ((ParticleSystem)c).main.startColor.color, (c, v) => {
                var main = ((ParticleSystem)c).main;
                main.startColor = v;
            });
            Add(typeof(LineRenderer), c => ((LineRenderer)c).startColor, (c, v) => {
                var lr = (LineRenderer)c;
                lr.startColor = v;
                lr.endColor = v;
            });
            Add(typeof(TrailRenderer), c => ((TrailRenderer)c).startColor, (c, v) => {
                var tr = (TrailRenderer)c;
                tr.startColor = v;
                tr.endColor = v;
            });
            Add(typeof(MeshRenderer), c => (c as MeshRenderer)!.material.color, (c, v) => (c as MeshRenderer)!.material.color = v);
        }
        
        public static bool GetColorComponent(this GameObject g, out Component colorTarget, out int typeIndex)
        {
            if(g.TryGetComponent<TextMeshPro>(out var tmp))
            {
                colorTarget = tmp;
                typeIndex = indexMap[typeof(TextMeshPro)];
                return true;
            } 
            
            if(g.TryGetComponent<SpriteRenderer>(out var sr))
            {
                colorTarget = sr;
                typeIndex = indexMap[typeof(SpriteRenderer)];
                return true;
            }
            
            // 注意: CanvasGroup 要优先于 Graphic.
			if(g.TryGetComponent<CanvasGroup>(out var cg))
            {
                colorTarget = cg;
                typeIndex = indexMap[typeof(CanvasGroup)];
                return true;
            }
			
            if(g.TryGetComponent<Graphic>(out var graphic))
            {
                colorTarget = graphic;
                typeIndex = indexMap[typeof(Graphic)];
                return true;
            }
            
            if(g.TryGetComponent<ParticleSystem>(out var ps))
            {
                colorTarget = ps;
                typeIndex = indexMap[typeof(ParticleSystem)];
                return true;
            }
            
            if(g.TryGetComponent<LineRenderer>(out var lr))
            {
                colorTarget = lr;
                typeIndex = indexMap[typeof(LineRenderer)];
                return true;
            }

            if(g.TryGetComponent<TrailRenderer>(out var tr))
            {
                colorTarget = tr;
                typeIndex = indexMap[typeof(TrailRenderer)];
                return true;
            }
            
            if(g.TryGetComponent<MeshRenderer>(out var mr))
            {
                colorTarget = mr;
                typeIndex = indexMap[typeof(MeshRenderer)];
                return true;
            }
            
            colorTarget = null;
            typeIndex = -1;
            return false;
        }
        
        public static bool GetColorComponent(this GameObject g, out Component colorTarget, out Func<Component, Color> getter, out Action<Component, Color> setter)
        {
            if(!g.GetColorComponent(out colorTarget, out var typeIndex))
            {
                getter = null;
                setter = null;
                return false;
            }
            
            getter = getters[typeIndex];
            setter = setters[typeIndex];
            return true;
        }
        
        public static Color GetColor(this Component c, int typeIndex)
        {
            return getters[typeIndex](c);
        }
        
        public static void SetColor(this Component c, int typeIndex, Color color)
        {
            setters[typeIndex](c, color);
        }
        
        
    }
    
}
