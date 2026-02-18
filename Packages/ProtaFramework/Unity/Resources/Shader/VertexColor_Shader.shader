Shader "Debug/VertexColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }      // SpriteRenderer needs this
        _Color ("Color", Color) = (1,1,1,1)         // SpriteRenderer needs this
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        
        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct Attributes
            {
                float3 position   : POSITION;
                float4 color      : COLOR;
            };
            
            struct v2f
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
            };
            
            float4 _Color;
            
            v2f vert(Attributes v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.position);
                o.color = v.color * _Color;
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            
            ENDCG
        }
    }
}
