using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.IO;

namespace Prota.Unity
{
    #region DualRenderBuffer
    public struct DualRenderBuffer
    {
        public string name;
        public RenderTexture rt1;
        public RenderTexture rt2;
        
        public int currentFrom;
        public RenderTexture from => currentFrom == 0 ? rt1 : rt2;
        public RenderTexture to => currentFrom == 0 ? rt2 : rt1;
        
        public DualRenderBuffer(string name, RenderTexture rt1, RenderTexture rt2)
        {
            this.name = name;
            this.rt1 = rt1;
            this.rt2 = rt2;
            currentFrom = 0;
        }
        
        public DualRenderBuffer Swap()
        {
            currentFrom = 1 - currentFrom;
            return this;
        }
    }
    #endregion
    
    #region Utils
    public static partial class UnityMethodExtensions
    {
        public static void SetViewProj(this CommandBuffer cmd, RectTransform tr, float zNear, float zFar)
        {
            var rect = tr.rect;
            var orthoViewMatrix = Matrix4x4.Ortho(rect.x, rect.x + rect.width, rect.y, rect.y + rect.height, zNear, zFar);
            cmd.SetViewProjectionMatrices(tr.worldToLocalMatrix, orthoViewMatrix);
        }
        
        public static RenderTexture Resize(this RenderTexture rt, int width, int height)
        {
            if(rt.width == width && rt.height == height) return rt;
            rt.Release();
            rt.width = width;
            rt.height = height;
            rt.Create();
            return rt;
        }
        
        public static RenderTexture Resize(this RenderTexture rt, Vector2Int size)
        {
            return rt.Resize(size.x, size.y);
        }
        
        public static RenderTexture Resize(this RenderTexture rt, Vector2 size)
        {
            return rt.Resize(size.x.CeilToInt(), size.y.CeilToInt());
        }
        
        public static Material CreateMaterialFromShader(this Shader shader)
        {
            var mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            return mat;
        }
        
        public static Material CreateMaterialFromShaderName(this string shaderName)
        {
            var shader = Shader.Find(shaderName);
            if(shader == null) throw new Exception($"Shader [{shaderName}] not found");
            return shader.CreateMaterialFromShader();
        }
        
        public static CullingResults Cull(this Camera camera, ScriptableRenderContext context, LayerMask? mask = null)
        {
            var cullingParams = new ScriptableCullingParameters();
            if (!camera.TryGetCullingParameters(out cullingParams)) return default;
            if(mask != null) cullingParams.cullingMask = mask.Value.value.ToUInt();
            return context.Cull(ref cullingParams);
        }
        
        public static RendererListDesc GetRenderListDesc(this Camera camera, ScriptableRenderContext context, string shaderTagId, LayerMask? mask = null)
        {
            var tagId = new ShaderTagId(shaderTagId);
            var cullResults = camera.Cull(context, mask);
            return new RendererListDesc(tagId, cullResults, camera) {
                renderQueueRange = RenderQueueRange.all,
                layerMask = mask ?? -1
            };
        }
        
        public static RendererListDesc GetRenderListDesc(this Camera camera, ScriptableRenderContext context, string shaderTagId, CullingResults cullResults, LayerMask? mask = null)
        {
            var tagId = new ShaderTagId(shaderTagId);
            return new RendererListDesc(tagId, cullResults, camera) {
                renderQueueRange = RenderQueueRange.all,
                layerMask = mask ?? -1
            };
        }
        
        public static void DrawRendererList(this CommandBuffer cmd, ScriptableRenderContext context, RendererListDesc listDesc)
        {
            var renderList = context.CreateRendererList(listDesc);
            cmd.DrawRendererList(renderList);
        }


        public struct CommandBufferPoolHandle : IDisposable
        {
            public CommandBuffer cmd;
            public ScriptableRenderContext cc;
            
            public void Dispose()
            {
                cc.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }

        public static CommandBufferPoolHandle WithCommandBuffer(this ScriptableRenderContext cc, out CommandBuffer cmd)
        {
            return new CommandBufferPoolHandle { cc = cc, cmd = cmd = CommandBufferPool.Get() };
        }
        
        public static CommandBufferPoolHandle WithCommandBuffer(this ScriptableRenderContext cc, string name, out CommandBuffer cmd)
        {
            return new CommandBufferPoolHandle { cc = cc, cmd = cmd = CommandBufferPool.Get(name) };
        }
        
        public static Color SetColorWithoutA(this SpriteRenderer rd, Color color)
        {
            var c = rd.color;
            c.r = color.r;
            c.g = color.g;
            c.b = color.b;
            rd.material.color = c;
            return c;
        }
        
        public static Color SetColorWithoutA(this Image rd, Color color)
        {
            var c = rd.color;
            c.r = color.r;
            c.g = color.g;
            c.b = color.b;
            rd.color = c;
            return c;
        }
        
        public static Rect GetNormalizedRect(this Sprite sprite)
        {
            var textureRect = sprite.textureRect;
            var texelSize = sprite.texture.texelSize;
            return new Rect(textureRect.min * texelSize, textureRect.size * texelSize);
        }
        // 
        // public static Vector2Int CameraSize(this CameraData cameraData, float scale = 1)
        // {
        //     var target = cameraData.cameraTargetDescriptor;
        //     return new Vector2(target.width * scale, target.height * scale).CeilToInt();
        // }
        // 
        // public static Vector2Int CameraSize(this RenderingData renderingData, float scale = 1)
        // {
        //     return renderingData.cameraData.CameraSize(scale);
        // }
        // 
        public static Matrix4x4 GetRectRTS(Camera camera, float drawDepth)
        {
            var worldRect = camera.GetCameraWorldView2D();
            var scale = new Vector2(worldRect.width, worldRect.height);
            var rotation = Quaternion.identity;
            var translation = new Vector3(worldRect.x, worldRect.y, drawDepth);
            var trs = Matrix4x4.TRS(translation, rotation, scale);
            return trs;
        }
        
        
        public static void DrawFullScreen(this CommandBuffer cmd,
            Camera camera,
            Material mat,
            float drawDepth = 0,
            MaterialPropertyBlock propertyBlock = null,
            int pass = 0)
        {
            var trs = GetRectRTS(camera, drawDepth);
            cmd.DrawMesh(ProtaUnityConstant.rectMesh, trs, mat, 0, pass, propertyBlock);
        }
        
        // public static void DrawFullScreen(this CommandBuffer cmd, ref RenderingData renderingData,
        //     Material mat, float drawDepth = 0, MaterialPropertyBlock propertyBlock = null, int pass = 0)
        // {
        //     cmd.DrawFullScreen(renderingData.cameraData.camera, mat, drawDepth, propertyBlock, pass);
        // }
        
        #endregion
    }
}
