Shader "Unlit/Flat Sky"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SunTex ("Texture", 2D) = "white" {}
        _Gamma("Gamma", Range(1, 2.2)) = 1
        _Sunset ("Sunset", Range(0, 1)) = 0
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


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _SunTex;
            float4 _SunTex_ST;
            float _Sunset;
            float _Gamma;
            
            fixed4 sRGBToLinear(fixed4 srgbColor) {
                float gamma = lerp(_Gamma, 2.2, _Sunset);
                return pow(srgbColor, gamma);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv, _SunTex);
                o.uv2.y = lerp(o.uv2.y, o.uv2.y + 0.6, _Sunset);
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
                fixed4 col = sRGBToLinear(tex2D(_MainTex, i.uv));
                fixed4 sunColor = sRGBToLinear(tex2D(_SunTex, i.uv2));
                return fixed4(lerp(col.xyz, sunColor.xyz, sunColor.a), 1);
            }
            ENDCG
        }
    }
}