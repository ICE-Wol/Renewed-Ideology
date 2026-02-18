Shader "Custom/TitleCullingShader"
{
    // 剔除Shader - 用于在描边遮罩上"挖洞"，使被遮挡区域不显示描边
    Properties
    {
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" }

        Pass
        {
            Name "Culling"
            
            Cull Back
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // 输出全透明，将遮罩对应区域清除
                return half4(0, 0, 0, 0);
            }
            ENDHLSL
        }
    }
}
