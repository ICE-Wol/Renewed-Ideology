using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Unity
{
    // 配合 SurfaceRenderer 使用.
    // 打上标签的物体会被渲染到 SurfaceRenderer 的 RenderTexture 上.
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class StandaloneRenderer : MonoBehaviour
    {
        public readonly static HashMapSet<string, StandaloneRenderer> all = new();
        public readonly static HashSet<string> changed = new();
        
        
        [field:SerializeField] public string[] renderTag { get; private set; } = Array.Empty<string>();
        
        [NonSerialized] Renderer _rd;
        public Renderer rd => _rd ? _rd : (_rd = GetComponent<Renderer>());
        
        void OnValidate()
        {
            RemoveThisFromRecord();
            AddToList(renderTag);
        }
        
        void OnEnable()
        {
            AddToList(renderTag);
            rd.enabled = false;
        }
        
        void OnDisable()
        {
            RemoveFromList(renderTag);
        }
        
        public void AddRenderTag(string tag)
        {
            RemoveFromList(renderTag);
            renderTag = renderTag.Append(tag).ToArray();
            AddToList(renderTag);
        }
        
        public void RemoveRenderTag(string tag)
        {
            RemoveFromList(renderTag);
            renderTag = renderTag.Where(x => x != tag).ToArray();
            AddToList(renderTag);
        }
        
        
        void RemoveThisFromRecord()
        {
            all.RemoveByValue(this);
        }
        
        void AddToList(string[] tag)
        {
            foreach(var t in tag)
            {
                all.AddElement(t, this);
                changed.Add(t);
            }
        }
        
        void RemoveFromList(string[] tag)
        {
            foreach(var t in tag)
            {
                all.RemoveElement(t, this);
                changed.Add(t);
            }
        }
    }
    
    
}
