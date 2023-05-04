Shader "Custom/Wireframe Shader (WebGL)" {
    Properties {
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1)
        _LineWidth ("Line Width", Range(0, 0.1)) = 0.005
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 barycentric: TEXCOORD1;
            };

            struct v2f {
                float3 barycentric : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            float _LineWidth;
            fixed4 _LineColor;
            fixed4 _BackgroundColor;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.barycentric = v.barycentric;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // Calculate the distance to the nearest edge
                float minDist = min(min(i.barycentric.x, i.barycentric.y), i.barycentric.z);

                // Render the wireframe based on the distance to the nearest edge
                float edge = smoothstep(_LineWidth, 2 * _LineWidth, minDist);
                return lerp(_LineColor, _BackgroundColor, edge);
            }
            ENDCG
        }
    }
}