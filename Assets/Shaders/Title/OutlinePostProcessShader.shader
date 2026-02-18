Shader "Custom/OutlinePostProcessShader"
{
    // 描边后处理Shader - 对遮罩纹理进行边缘检测并叠加到屏幕
    Properties
    {
        _Color("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width", Range(0, 10)) = 2.0
        _OffsetValue("Offset Value", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline"="UniversalRenderPipeline" }

        Pass
        {
            Name "OutlinePostProcess"
            
            // 混合模式：叠加到屏幕
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _OutlineWidth;
                float4 _OffsetValue;
            CBUFFER_END

            // _BlitTexture 和 _BlitTexture_TexelSize 由 Blit.hlsl 定义

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                
                // 应用偏移
                uv += _OffsetValue.xy;

                // 采样中心点
                half4 center = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);
                
                // 边缘检测 - 采样周围像素
                float2 texelSize = _BlitTexture_TexelSize.xy * _OutlineWidth;
                
                half4 left = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(-texelSize.x, 0));
                half4 right = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(texelSize.x, 0));
                half4 up = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(0, texelSize.y));
                half4 down = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(0, -texelSize.y));
                
                // 对角线采样增强描边效果
                half4 leftUp = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(-texelSize.x, texelSize.y));
                half4 rightUp = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(texelSize.x, texelSize.y));
                half4 leftDown = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(-texelSize.x, -texelSize.y));
                half4 rightDown = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(texelSize.x, -texelSize.y));

                // 计算周围像素的最大alpha值
                half maxAlpha = max(max(max(left.a, right.a), max(up.a, down.a)), 
                                   max(max(leftUp.a, rightUp.a), max(leftDown.a, rightDown.a)));

                // 描边区域：周围有遮罩但中心没有遮罩的区域
                half outline = saturate(maxAlpha - center.a);

                // 输出描边颜色，非描边区域透明
                return half4(_Color.rgb, outline * _Color.a);
            }
            ENDHLSL
        }
    }
}
