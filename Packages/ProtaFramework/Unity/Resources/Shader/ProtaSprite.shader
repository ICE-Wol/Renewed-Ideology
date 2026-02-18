Shader "Prota/Sprite"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 0.0
        [Enum(Off, 0, On, 1)] _ZWrite ("ZWrite", Float) = 0.0
        
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Blend Color Src", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Blend Color Dst", Float) = 10
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendASrc ("Blend Alpha Src", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendADst ("Blend Alpha Dst", Float) = 10
        
        _MainTex ("Texture", 2D) = "white" { }
        _MaskTex("Mask", 2D) = "white" { }
        
        [HDR] _MaskUsage("Mask Weight", Color) = (1, 1, 1, 1)
        
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        [HDR] _AddColor ("Add Color", Color) = (0,0,0,0)
        [HDR] _OverlapColor ("Overlap Color", Color) = (0,0,0,0)
        
        _AlphaClip ("Alpha Clip", float) = 0.5
        
		[Range(0, 1)] _HueOffset ("Hue Offset", float) = 0.0
        [Range(0, 1)] _SaturationOffset ("Saturation Offset", float) = 0.0
        [Range(0, 1)] _BrightnessOffset ("Brightness Offset", float) = 0.0
        [Range(0, 1)] _ContrastOffset ("Contrast Offset", float) = 0.0
        [Range(0, 1)] _HueConcentrate ("Hue Concentration", float) = 0.0
        
        [Range(0, 255)] _Stencil ("Stencil Ref", Float) = 0
        [Range(0, 255)] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [Range(0, 255)] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Compare", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Float) = 0
        
		_ColorMask ("Color Mask", Float) = 15
        _ColorClampMin ("Color Clamp Min", Vector) = (0,0,0,0)
        _ColorClampMax ("Color Clamp Max", Vector) = (100000000,100000000,100000000,100000000)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        
        Cull Off
        ZWrite [_ZWrite]
        ZTest [_ZTest]
        Blend [_BlendSrc] [_BlendDst], [_BlendASrc] [_BlendADst]
        ColorMask [_ColorMask]
        

        Pass
        {
            Stencil
            {
                Ref [_Stencil]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilOp]
            }

            HLSLPROGRAM
            
            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment
            #pragma shader_feature _HSL_CONVERSION
            
            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2  uv          : TEXCOORD0;
                float2 maskUV       : TEXCOORD3;
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                float4   color       : COLOR;
                float2  uv          : TEXCOORD0;
                float2   lightingUV  : TEXCOORD1;
                float2 maskUV      : TEXCOORD3;
            };
            
            #include "UnityCG.cginc"
            #include "./ProtaUtils.cginc"
            
            sampler2D _MainTex;
            sampler2D _MaskTex;
            
            CBUFFER_START(UnityPerMaterial)
                half4 _MainTex_ST;
                half4 _MaskTex_ST;
                float4 _Color;
                float4 _AddColor;
                float4 _OverlapColor;
                float _HueOffset;
                float _SaturationOffset;
                float _BrightnessOffset;
                float _ContrastOffset;
                float _AlphaClip;
                float4 _Flip;
                float4 _UVOffset;
                float4 _MaskUsage;
                float4 _ColorClampMin;
                float4 _ColorClampMax;
            CBUFFER_END

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;

                o.positionCS = UnityObjectToClipPos(v.positionOS);
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);

                o.color = v.color;
                
                o.maskUV = TRANSFORM_TEX(v.maskUV, _MaskTex);
                
                return o;
            }

            float4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                float4 main = i.color * tex2D(_MainTex, i.uv);
                const float4 mask = _MaskUsage * tex2D(_MaskTex, i.maskUV);
                
                if(main.a * mask.a <= _AlphaClip) discard;
                
                main *= _Color;
                
                #ifdef _HSL_CONVERSION
                float3 hsl;
                RGBtoHSL(main.rgb, hsl);
                HueOffsetHSL(hsl, _HueOffset);
                ContrastOffsetHSL(hsl, _ContrastOffset);
                SaturationOffsetHSL(hsl, _SaturationOffset);
                BrightnessOffsetHSL(hsl, _BrightnessOffset);
                HSLtoRGB(hsl, main.rgb);
                #endif
                
                main.rgb += _AddColor.rgb;
                main.rgb = _OverlapColor.a * _OverlapColor.rgb + (1 - _OverlapColor.a) * main.rgb;
                
                main *= mask;
                
                main = clamp(main, _ColorClampMin, _ColorClampMax);
                
                return main;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}

