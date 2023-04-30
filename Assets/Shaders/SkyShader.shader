Shader "Unlit/Flat Sky"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Gamma("Gamma", Range(1, 2.2)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            float _Gamma;
            fixed3 sRGBToLinear(fixed3 srgbColor) {
                return pow(srgbColor, _Gamma);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
 
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                #if defined(UNITY_REVERSED_Z)
                // when using reversed-Z, make the Z be just a tiny
                // bit above 0.0
                o.vertex.z = 1.0e-9f;
                #else
                // when not using reversed-Z, make Z/W be just a tiny
                // bit below 1.0
                o.vertex.z = o.vertex.w - 1.0e-6f;
                #endif
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed3 col = sRGBToLinear(tex2D(_MainTex, i.uv).xyz);
                return fixed4(col, 1);
            }
            ENDCG
        }
    }
}