// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/GroundShader"
{
    Properties
    {
        _MainTex ("Height Map", 2D) = "white" {}
        _RoadWidth ("Road Width", Range(1.0, 50.0)) = 15.3
        _RoadLength ("Road Length", float) = 400
        _HeightOffset ("Height Offset", Range(-1, 0)) = -0.2
        _HeightScale ("Height Scale", Range(0, 100)) = 40
        _HeightRoadBlend ("Height-Road Blend", Range(1, 100)) = 15
        _ShoulderThickness ("Shoulder Thickness", Range(0, 1)) = 0.5
        _GridCells ("Grid Cells", int) = 3
        _GridThickness ("Grid Thickness", Range(0, 0.5)) = 0.1
        _LightVector("Directional Light Vector", Vector) = (0, -1, -1)
        _KeyLightBrightness("Key Light Brightness", Range(0, 1)) = 0.3
        _ClearingLocation("Clearing Location", float) = 50
        _ClearingXOffset("Clearing X Offset", Range(0, 50)) = 10
        _ClearingScaleX("Clearing Scale X", Range(0, 1)) = 0.55
        _ClearingScaleY("Clearing Scale Y", Range(0, 1)) = 0.3
        _ClearingLocationSize("Clearing Location Size", Range(0, 30)) = 10
        _ClearingBlendWidth("Clearing Blend Width", Range(0, 30)) = 5
        [Toggle(USE_CLEARING)] _HasClearingLocation("Has Clearing", Float) = 1
        [Toggle(FLIP_V)] _FlipV("Flip V", Float) = 0
        [Toggle(FADE_OUT_HEIGHTMAP)] _FadeoutHeightmap("Fadeout Heightmap", Float) = 0
        [Toggle(FLIP_FADE)] _FlipFade("Flip Fade", Float) = 0
        // InnerRadius where heightmap = 0
        // OuterRadius where (OuterRadius - InnerRadius) = Blend
        _BgGridDensity ("Bg Grid Density", Range(1, 100)) = 40
        _BgGridThickness ("Bg Grid Thickness", Range(0, 0.5)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma shader_feature USE_CLEARING
            #pragma shader_feature FLIP_V
            #pragma shader_feature FADE_OUT_HEIGHTMAP
            #pragma shader_feature FLIP_FADE
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            fixed3 sRGBToLinear(fixed3 srgbColor) {
                return pow(srgbColor, 2.2);
            }

            // + A +
            // D N B
            // + C +
            float3 computeNormals( float h_A, float h_B, float h_C, float h_D, float h_N, float heightScale )
            {
                //To make it easier we offset the points such that n is "0" height
                float3 va = { 0, 1, (h_A - h_N)*heightScale };
                float3 vb = { 1, 0, (h_B - h_N)*heightScale };
                float3 vc = { 0, -1, (h_C - h_N)*heightScale };
                float3 vd = { -1, 0, (h_D - h_N)*heightScale };
                //cross products of each vector yields the normal of each tri - return the average normal of all 4 tris
                float3 average_n = ( cross(va, vb) + cross(vb, vc) + cross(vc, vd) + cross(vd, va) ) / -4;
                float tmp = average_n.z;
                average_n.z = average_n.y;
                average_n.y = tmp;
                return normalize( average_n );
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 posWorldSpace : VAR_POSITION_WS;
                float height : VAR_HEIGHT;
                float heightMultiplier : VAR_HEIGHT_MULTIPLIER;
                float distanceToRoad : VAR_DISTANCE;
                float distanceToClearing : VAR_DISTANCE2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _RoadWidth;
            float _RoadLength;
            float _HeightOffset;
            float _HeightScale;
            float _HeightRoadBlend;
            float _ShoulderThickness;
            int _GridCells;
            float _GridThickness;
            float3 _LightVector;
            float _KeyLightBrightness;
            float _BgGridDensity;
            float _BgGridThickness;

            float _ClearingLocation;
            float _ClearingXOffset;
            float _ClearingScaleX;
            float _ClearingScaleY;
            float _ClearingLocationSize;
            float _ClearingBlendWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                #ifdef FLIP_V
                    o.uv = float2(o.uv.x, 1 - o.uv.y);
                #endif

                // If worldspace position is between [-roadWidth / 2, roadWidth / 2], we're on the road
                o.posWorldSpace = mul(unity_ObjectToWorld, v.vertex).xyz;
                float halfRoadWidth = _RoadWidth / 2;
                float distanceToRoad;
                if (o.posWorldSpace.x < 0) {
                    distanceToRoad = max(0, -halfRoadWidth - o.posWorldSpace.x);
                } else {
                    distanceToRoad = max(0, o.posWorldSpace.x - halfRoadWidth);
                }
                o.distanceToRoad = clamp(distanceToRoad / _HeightRoadBlend, 0, 1);

                #ifdef USE_CLEARING
                    float2 worldPos = float2(o.posWorldSpace.xz);
                    float2 clearingPos = float2(halfRoadWidth + _ClearingXOffset, _ClearingLocation);
                    float2 distanceToClearingXY = abs(clearingPos - worldPos) * float2(_ClearingScaleX, _ClearingScaleY); // Multiply distance in one axis to change the shape
                    o.distanceToClearing = smoothstep(_ClearingLocationSize, _ClearingLocationSize + _ClearingBlendWidth, length(distanceToClearingXY));
                    o.heightMultiplier = _HeightScale * o.distanceToRoad * o.distanceToClearing;
                #else
                    o.distanceToClearing = 0;
                    o.heightMultiplier = _HeightScale * o.distanceToRoad;
                #endif

                #ifdef FADE_OUT_HEIGHTMAP
                    #ifdef FLIP_FADE
                        o.heightMultiplier = lerp(o.heightMultiplier, 0, 1 - v.uv.y);
                    #else
                        o.heightMultiplier = lerp(o.heightMultiplier, 0, v.uv.y);
                    #endif
                #endif
            
                float heightSample = tex2Dlod(_MainTex, float4(o.uv, 0, 0)).r + _HeightOffset; // [0, 1] -> [-1, 1]
                o.height = heightSample * o.heightMultiplier;
                if ((o.posWorldSpace.x < -halfRoadWidth || o.posWorldSpace.x > halfRoadWidth) && heightSample > 0) {
                    v.vertex.y = v.vertex.y + o.height;
                }

                o.vertex = UnityObjectToClipPos(v.vertex);
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            // Pallet:
            // Dark Blue: 20, 16, 57 <0.078, 0.063, 0.223>
            // Light Blue: 33, 24, 245 <0.130  0.133  0.961>
            // Orchid: 0.85490  0.43922  0.83922
            // Light Purple: 234,173,231 <0.914, 0.676, 0.902>
            fixed4 frag (v2f i) : SV_Target
            {
                // Draw the road
                float3 darkBlue = sRGBToLinear(float3(0.078, 0.063, 0.223));
                float3 lightBlue = sRGBToLinear(float3(0.130, 0.133, 0.961));
                float3 lightPurple = sRGBToLinear(float3(0.855, 0.439, 0.839));
                float3 grey = sRGBToLinear(float3(1, 1, 1));
                float3 red = float3(1, 0, 0);
                float3 blue = float3(0, 0, 1);
                float3 black = float3(0, 0, 0);

                float halfRoadWidth = _RoadWidth / 2;
                float halfGridThickness = _GridThickness / 2;
                float gridStep = _RoadWidth / (_GridCells + 1);
                if ((i.posWorldSpace.x >= -halfRoadWidth && i.posWorldSpace.x <= halfRoadWidth)) {
                    float roadPos = i.posWorldSpace.x + halfRoadWidth; // [0, RoadWidth]
                    float roadPosNormalized = roadPos / _RoadWidth; // [0, 1]
                    if (roadPos < _ShoulderThickness || roadPos > (_RoadWidth - _ShoulderThickness)) {
                        return fixed4(lightBlue, 1);
                    } else {
                        // Different way of handling grid lines
                        // Horizontal Grid lines
                        float roadPosZNormalized = i.posWorldSpace.z / _RoadLength; // [0, 1]
                        float fracA = frac(roadPosZNormalized * 30);
                        float fracB = frac((1 - roadPosZNormalized) * 30);
                        float a = pow(max(fracA, fracB), 80);
                        // Vertical grid lines
                        float frac1 = frac(roadPosNormalized * _GridCells);
                        float frac2 = frac((1 - roadPosNormalized) * _GridCells);
                        float b = pow(max(frac1, frac2), 1 / _GridThickness);
                        return float4(lerp(darkBlue.rgb, lightBlue.rgb, a + b), 1.0);

                        // Draw grid lines. Seems like If check doesn't work too well on this.
                        // float minDistance = _RoadWidth;
                        // for (int gridIdx = 1; gridIdx <= _GridCells; gridIdx++) {
                        //     minDistance = min(abs(roadPos - gridIdx * gridStep), minDistance);
                        // }
                        // if (minDistance <= halfGridThickness) {
                        //     return fixed4(lightBlue, 1);
                        // } else {
                        //     return fixed4(darkBlue, 1);
                        // }
                    }
                }

                // Visualize clearing
                // return fixed4(lerp(0, 1, i.distanceToClearing), 0, 0, 1);

                // Draw the terrain
                if (i.height > 0) {
                    // Calculate normal
                    // + A +
                    // D N B
                    // + C +
                    float2 texelSize = _MainTex_TexelSize.xy;
                    float height = tex2D(_MainTex, i.uv).r;
                    float heightL = tex2D(_MainTex, i.uv + float2(-texelSize.x, 0)).r;
                    float heightR = tex2D(_MainTex, i.uv + float2(texelSize.x, 0)).r;
                    #ifdef FLIP_V
                        float heightB = tex2D(_MainTex, i.uv + float2(0, texelSize.y)).r;
                        float heightT = tex2D(_MainTex, i.uv + float2(0, -texelSize.y)).r;

                    #else
                        float heightB = tex2D(_MainTex, i.uv + float2(0, -texelSize.y)).r;
                        float heightT = tex2D(_MainTex, i.uv + float2(0, texelSize.y)).r;
                    #endif
                    float3 N = computeNormals(heightT, heightR, heightB, heightL, height, i.heightMultiplier);

                    float3 L = -normalize(_LightVector);
                    float NdotL = clamp(dot(N, L), 0, 1);
                    return fixed4(NdotL* grey * _KeyLightBrightness, 1);
                } else {
                    #ifdef USE_CLEARING
                        // Grid lines
                        float2 fracA = frac(i.uv * _BgGridDensity);
                        float2 fracB = frac((1 - i.uv) * _BgGridDensity);
                        float a = pow(max(fracA.x, fracB.x), 1 / _BgGridThickness);
                        float b = pow(max(fracA.y, fracB.y), 1 / _BgGridThickness);
                        float3 gridCol = lerp(black, lightPurple.rgb, a + b);
                        return fixed4(lerp(black, gridCol, 1 - i.distanceToClearing), 1);
                    #else
                        return fixed4(black, 1);
                    #endif
                }

                // // // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // // // apply fog
                // // UNITY_APPLY_FOG(i.fogCoord, col);
                // return float4(i.distanceToRoad, 0, 0, 1);
                // return col;
            }
            ENDCG
        }
    }
}
