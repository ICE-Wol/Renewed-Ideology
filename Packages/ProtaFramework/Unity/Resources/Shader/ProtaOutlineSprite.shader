Shader "Custom/OutlineOnly3x3"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Outline Color", Color) = (0,0,0,1)
        _Thickness ("Outline Thickness", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Thickness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 texelSize = _Thickness / _ScreenParams.xy;
                float alphaCenter = tex2D(_MainTex, i.uv).a;
                if (alphaCenter > 0.01)
                    discard;

                float alphaSum = 0;
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (x == 0 && y == 0) continue;
                        float2 offset = float2(x, y) * texelSize;
                        alphaSum += tex2D(_MainTex, i.uv + offset).a;
                    }
                }

                float outlineAlpha = saturate(alphaSum);
                
                if (outlineAlpha <= 0.01) discard;
                
                return float4(_Color.rgb, _Color.a * outlineAlpha);
            }
            ENDHLSL
        }
    }
}
