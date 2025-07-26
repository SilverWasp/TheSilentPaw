Shader "Custom/WallFadeTop"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _FadeHeight ("Fade Start Height", Float) = 1.5
        _SolidHeight ("Solid Base Height", Float) = 0.2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _FadeHeight;
            float _SolidHeight;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float worldY : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldY = worldPos.y;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 tex = tex2D(_MainTex, i.uv) * _Color;

                // Fully visible at or below solid base height
                if (i.worldY <= _SolidHeight)
                    return tex;

                // Start fading above solid height up to fade height
                float fade = saturate(1.0 - (i.worldY - _SolidHeight) / (_FadeHeight - _SolidHeight));
                tex.a *= fade;

                return tex;
            }
            ENDCG
        }
    }
}
