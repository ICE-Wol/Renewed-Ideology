using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Prota.Unity;
using UnityEngine.Rendering;
using UnityEditor;

namespace Prota.Unity
{
    
    // 集成渲染贴图, 代替 Camera 渲染挂在这个节点里面的物体, 并且可以显示出来.
    // 支持迭代渲染, 可以用来做特殊效果.
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class SurfaceRenderer : MonoBehaviour
    {
        private struct Orderer : IComparer<SurfaceRenderer>
        {
            public int Compare(SurfaceRenderer x, SurfaceRenderer y) => x.priority.CompareTo(y.priority);
        }

        public static SortedList<SurfaceRenderer, SurfaceRenderer> all = new(new Orderer());
        
        public int priority = 0;
        
        public string renderTag = "";
        
        public RenderTexture renderTexture;
        
        public bool shouldClear = true;
        [ShowWhen("shouldClear")] public Color clearColor = Color.clear;
        
        public float zNear = -1000;
        public float zFar = 1000;
        public float pixelPerUnit = 16;
        
        public bool preview = true;
        
        public string passToRendererName = "";
        
        public Renderer[] renderers = Array.Empty<Renderer>();
        
        // ====================================================================================================
        // ====================================================================================================
        
        [NonSerialized] RenderTexture handledRenderTexture;
        [NonSerialized] MaterialPropertyBlock _props;
        MaterialPropertyBlock props => _props ?? (_props = new MaterialPropertyBlock());
        [EditorButton] public bool fetchRenderers = false;
        
        CommandBuffer _cmd;
        CommandBuffer cmd => _cmd ?? (_cmd = new CommandBuffer() { name = "SurfaceRenderer::" + this.gameObject.name });
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        void OnValidate()
        {
            if(zNear >= zFar) Debug.LogError("zNear should be less than zFar.", this);
        }
        
        void OnEnable()
        {
            all.Add(this, this);
            RenderPipelineManager.beginContextRendering += this.OnStartRendering;
        }
        
        void OnDisable()
        {
            all.Remove(this);
            RenderPipelineManager.beginContextRendering -= this.OnStartRendering;
            
            ClearCmd();
            ClearHandledRenderTexture();
        }
        
        void ClearCmd()
        {
            if(_cmd != null)
            {
                _cmd.Dispose();
                _cmd = null;
            }
        }
        
        void ClearHandledRenderTexture()
        {
            if(handledRenderTexture != null)
            {
                DestroyImmediate(handledRenderTexture);
                handledRenderTexture = null;
            }
        }
        
        void Update()
        {
            if(fetchRenderers.SwapSet(false))
            {
                renderers = this.GetComponentsInChildren<Renderer>().ToArray();
                foreach(var rd in renderers) rd.enabled = false;
            }
            if(preview) Preview();
            SetRender();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        void OnStartRendering(ScriptableRenderContext context, List<Camera> cameras)
        {
            DoRendering(context, cameras);
        }
        
        
        void DoRendering(ScriptableRenderContext context, List<Camera> cameras)
        {
            PrepareRenderTexture();
            ResizeRenderTexture();
            PrepareRendererList();
            
            cmd.Clear();
            cmd.SetRenderTarget(renderTexture);
            if(shouldClear) cmd.ClearRenderTarget(true, true, clearColor);
            cmd.SetViewProj(this.RectTransform(), zNear, zFar);
            
            foreach(var rd in rendererList)
            {
                cmd.DrawRenderer(rd, rd.sharedMaterial, 0, 0);
            }
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        void PrepareRenderTexture()
        {
            if(renderTexture != handledRenderTexture && handledRenderTexture != null)
            {
                ClearHandledRenderTexture();
            }
            
            if(!renderTexture)
            {
                handledRenderTexture = renderTexture = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGB32);
                handledRenderTexture.name = "Generated Texture";
            }
        }
        
        void ResizeRenderTexture()
        {
            var expectedSize = (this.RectTransform().rect.size * pixelPerUnit).RoundToInt();
            if(renderTexture.texelSize == expectedSize) return;
            renderTexture.Resize(expectedSize);
        }
        
        readonly List<Renderer> rendererList = new();
        void PrepareRendererList()
        {
            rendererList.Clear();
            var rect = this.RectTransform().WorldRect();
            var thisBounds = new Bounds(rect.center.ToVec3(zNear), rect.size.ToVec3(zFar - zNear));
            
            foreach(var rd in renderers)
                if(rd && rd.bounds.Intersects(thisBounds))
                    rendererList.Add(rd);
            
            if(StandaloneRenderer.all.ContainsKey(renderTag))
                foreach(var srd in StandaloneRenderer.all[renderTag])
                {
                    if(!srd) continue;
                    if(!srd.rd.PassValue(out var rd)) continue;
                    if(!rd.bounds.Intersects(thisBounds)) continue;
                    rendererList.Add(srd.rd);
                }
        }
        
        void Preview()
        {
            var p = new RenderParams(ProtaUnityConstant.urpSpriteUnlitMat);
            p.matProps = new MaterialPropertyBlock();
            p.matProps.SetColor("_Color", Color.white);
            p.matProps.SetTexture("_MainTex", renderTexture);
            var localToWorld = this.RectTransform().ToWorldMatrix();
            Graphics.RenderMesh(p, ProtaUnityConstant.rectMesh, 0, localToWorld);
        }
        
        void SetRender()
        {
            if(!this.TryGetComponent(out Renderer rd)) return;
            rd.GetPropertyBlock(props);
            var texName = passToRendererName.NullOrEmpty() ? "_MainTex" : passToRendererName;
            props.SetTexture(texName, renderTexture);
            rd.SetPropertyBlock(props);
        }
        
        // ====================================================================================================
        // ====================================================================================================
        

    }
}
