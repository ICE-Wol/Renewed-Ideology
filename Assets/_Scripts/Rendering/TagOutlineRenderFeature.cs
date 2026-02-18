using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 自定义描边渲染特性 - 用于渲染注册到静态HashSet中的特定物体的描边效果
/// 渲染流程：1. 用纯白材质渲染物体到临时RT  2. 用描边材质对临时RT进行后处理
/// </summary>
public class TagOutlineRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class OutlineSettings
    {
        [Header("材质设置")]
        [Tooltip("用于渲染纯白遮罩的材质，留空时不渲染")]
        public Material maskMaterial;

        [Tooltip("用于描边后处理的材质，留空时不渲染")]
        public Material outlineMaterial;

        [Tooltip("用于剔除描边的材质（TitleCullingShader），留空时不剔除")]
        public Material cullingMaterial;

        [Header("描边参数")]
        [Tooltip("描边颜色")]
        public Color outlineColor = Color.black;

        [Tooltip("描边宽度")]
        [Range(0f, 0.1f)]
        public float outlineWidth = 0.02f;

        [Tooltip("描边偏移")]
        public Vector3 outlineOffset = Vector3.zero;

        [Header("渲染设置")]
        [Tooltip("渲染事件时机")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public OutlineSettings settings = new OutlineSettings();
    private TagOutlineRenderPass _renderPass;

    // Shader属性ID缓存
    private static readonly int OutlineColorId = Shader.PropertyToID("_Color");
    private static readonly int OutlineWidthId = Shader.PropertyToID("_OutlineWidth");
    private static readonly int OutlineOffsetId = Shader.PropertyToID("_OffsetValue");
    private static readonly int MaskTexId = Shader.PropertyToID("_MaskTex");

    public override void Create()
    {
        _renderPass = new TagOutlineRenderPass(settings);
        _renderPass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // 只有当两个材质都存在且有注册的渲染器时才添加渲染Pass
        if (settings.maskMaterial != null && settings.outlineMaterial != null && TagOutlineRegistry.RegisteredRenderers.Count > 0)
        {
            // 更新描边材质属性
            settings.outlineMaterial.SetColor(OutlineColorId, settings.outlineColor);
            settings.outlineMaterial.SetFloat(OutlineWidthId, settings.outlineWidth);
            settings.outlineMaterial.SetVector(OutlineOffsetId, settings.outlineOffset);

            renderer.EnqueuePass(_renderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        _renderPass?.Dispose();
    }
}

/// <summary>
/// 描边渲染Pass - 三步渲染流程：遮罩 -> 剔除 -> 描边后处理
/// </summary>
public class TagOutlineRenderPass : ScriptableRenderPass
{
    private TagOutlineRenderFeature.OutlineSettings _settings;
    private const string MaskPassTag = "TagOutlineMaskPass";
    private const string CullingPassTag = "TagOutlineCullingPass";
    private const string OutlinePassTag = "TagOutlinePass";
    private ProfilingSampler _maskProfilingSampler;
    private ProfilingSampler _cullingProfilingSampler;
    private ProfilingSampler _outlineProfilingSampler;

    // Shader属性ID
    private static readonly int MaskTexId = Shader.PropertyToID("_MaskTex");

    public TagOutlineRenderPass(TagOutlineRenderFeature.OutlineSettings settings)
    {
        _settings = settings;
        _maskProfilingSampler = new ProfilingSampler(MaskPassTag);
        _cullingProfilingSampler = new ProfilingSampler(CullingPassTag);
        _outlineProfilingSampler = new ProfilingSampler(OutlinePassTag);
    }

    // RenderGraph API 实现 (URP 14+)
    private class MaskPassData
    {
        public Material maskMaterial;
        public List<RendererData> renderers;
    }

    private class CullingPassData
    {
        public Material cullingMaterial;
        public List<RendererData> cullingRenderers;
    }

    private class OutlinePassData
    {
        public Material outlineMaterial;
        public TextureHandle maskTexture;
    }

    private struct RendererData
    {
        public Renderer renderer;
        public int submeshCount;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (_settings.maskMaterial == null || _settings.outlineMaterial == null)
            return;

        // 收集需要渲染的物体
        var renderers = new List<RendererData>();
        foreach (var renderer in TagOutlineRegistry.RegisteredRenderers)
        {
            if (renderer != null && renderer.gameObject.activeInHierarchy)
            {
                renderers.Add(new RendererData
                {
                    renderer = renderer,
                    submeshCount = renderer.sharedMaterials.Length
                });
            }
        }

        if (renderers.Count == 0)
            return;

        // 收集剔除物体
        var cullingRenderers = new List<RendererData>();
        foreach (var renderer in TagOutlineCullingRegistry.CullingRenderers)
        {
            if (renderer != null && renderer.gameObject.activeInHierarchy)
            {
                cullingRenderers.Add(new RendererData
                {
                    renderer = renderer,
                    submeshCount = renderer.sharedMaterials.Length
                });
            }
        }

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        // 创建临时遮罩纹理
        var maskTextureDesc = new TextureDesc(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height)
        {
            colorFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm,
            clearBuffer = true,
            clearColor = Color.clear,
            name = "_OutlineMaskTexture"
        };
        TextureHandle maskTexture = renderGraph.CreateTexture(maskTextureDesc);

        // Pass 1: 渲染纯白遮罩到临时RT
        using (var builder = renderGraph.AddRasterRenderPass<MaskPassData>(MaskPassTag, out var passData, _maskProfilingSampler))
        {
            builder.SetRenderAttachment(maskTexture, 0, AccessFlags.Write);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);

            passData.maskMaterial = _settings.maskMaterial;
            passData.renderers = renderers;

            builder.AllowPassCulling(false);

            builder.SetRenderFunc((MaskPassData data, RasterGraphContext context) =>
            {
                // 手动清除为全透明 (RGBA = 0,0,0,0)
                context.cmd.ClearRenderTarget(false, true, new Color(0, 0, 0, 0));

                foreach (var rendererData in data.renderers)
                {
                    for (int i = 0; i < rendererData.submeshCount; i++)
                    {
                        context.cmd.DrawRenderer(rendererData.renderer, data.maskMaterial, i, 0);
                    }
                }
            });
        }

        // Pass 2: 剔除Pass - 用剔除材质在遮罩上"挖洞"
        if (_settings.cullingMaterial != null && cullingRenderers.Count > 0)
        {
            using (var builder = renderGraph.AddRasterRenderPass<CullingPassData>(CullingPassTag, out var passData, _cullingProfilingSampler))
            {
                builder.SetRenderAttachment(maskTexture, 0, AccessFlags.Write);
                builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);

                passData.cullingMaterial = _settings.cullingMaterial;
                passData.cullingRenderers = cullingRenderers;

                builder.AllowPassCulling(false);

                builder.SetRenderFunc((CullingPassData data, RasterGraphContext context) =>
                {
                    // 用剔除材质渲染，将对应区域清除为透明
                    foreach (var rendererData in data.cullingRenderers)
                    {
                        for (int i = 0; i < rendererData.submeshCount; i++)
                        {
                            context.cmd.DrawRenderer(rendererData.renderer, data.cullingMaterial, i, 0);
                        }
                    }
                });
            }
        }

        // Pass 3: 使用描边材质对遮罩进行后处理并叠加到屏幕
        using (var builder = renderGraph.AddRasterRenderPass<OutlinePassData>(OutlinePassTag, out var passData, _outlineProfilingSampler))
        {
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);

            passData.outlineMaterial = _settings.outlineMaterial;
            passData.maskTexture = maskTexture;

            builder.UseTexture(maskTexture, AccessFlags.Read);
            builder.AllowPassCulling(false);

            builder.SetRenderFunc((OutlinePassData data, RasterGraphContext context) =>
            {
                // 设置遮罩纹理到描边材质
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                propertyBlock.SetTexture(MaskTexId, data.maskTexture);

                // 使用 Blitter 绘制全屏后处理
                // 注意：描边材质需要设置混合模式 (Blend SrcAlpha OneMinusSrcAlpha) 才能叠加而不是覆盖
                Blitter.BlitTexture(context.cmd, data.maskTexture, new Vector4(1, 1, 0, 0), data.outlineMaterial, 0);
            });
        }
    }

    public void Dispose()
    {
        // 清理资源（如果需要）
    }
}

/// <summary>
/// 描边物体注册表 - 静态HashSet用于存储需要描边的渲染器
/// </summary>
public static class TagOutlineRegistry
{
    /// <summary>
    /// 存储所有需要描边的Renderer
    /// </summary>
    public static HashSet<Renderer> RegisteredRenderers { get; } = new HashSet<Renderer>();

    /// <summary>
    /// 注册一个渲染器用于描边
    /// </summary>
    /// <param name="renderer">要注册的渲染器</param>
    public static void Register(Renderer renderer)
    {
        if (renderer != null)
        {
            RegisteredRenderers.Add(renderer);
        }
    }

    /// <summary>
    /// 注销一个渲染器
    /// </summary>
    /// <param name="renderer">要注销的渲染器</param>
    public static void Unregister(Renderer renderer)
    {
        if (renderer != null)
        {
            RegisteredRenderers.Remove(renderer);
        }
    }

    /// <summary>
    /// 清空所有注册的渲染器
    /// </summary>
    public static void Clear()
    {
        RegisteredRenderers.Clear();
    }
}

/// <summary>
/// 描边剔除物体注册表 - 用于存储会剔除描边的渲染器（如Title）
/// </summary>
public static class TagOutlineCullingRegistry
{
    /// <summary>
    /// 存储所有用于剔除描边的Renderer
    /// </summary>
    public static HashSet<Renderer> CullingRenderers { get; } = new HashSet<Renderer>();

    /// <summary>
    /// 注册一个渲染器用于剔除描边
    /// </summary>
    /// <param name="renderer">要注册的渲染器</param>
    public static void Register(Renderer renderer)
    {
        if (renderer != null)
        {
            CullingRenderers.Add(renderer);
        }
    }

    /// <summary>
    /// 注销一个渲染器
    /// </summary>
    /// <param name="renderer">要注销的渲染器</param>
    public static void Unregister(Renderer renderer)
    {
        if (renderer != null)
        {
            CullingRenderers.Remove(renderer);
        }
    }

    /// <summary>
    /// 清空所有注册的渲染器
    /// </summary>
    public static void Clear()
    {
        CullingRenderers.Clear();
    }
}
