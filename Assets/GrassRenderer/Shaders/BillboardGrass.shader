Shader "Unlit/BillboardGrass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WindStrength ("Wind Strength", Range(0.0, 50.0)) = 1
        _CullingBias ("Cull Bias", Range(0.1, 1.0)) = 0.5
        _LODCutoff ("LOD Cutoff", Range(10.0, 500.0)) = 100
    }
    SubShader
    {
        Cull Off
        Zwrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma target 4.5

            #include "UnityPBSLighting.cginc"
            #include "AutoLight.cginc"
            #include "../Resources/Random.cginc"

            // Input structure for the vertex shader
            struct mesh_data
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Interpolators for passing data from vertex to fragment shader
            struct interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float saturation_level : TEXCOORD1;
            };

            struct grass_data
            {
                float4 position;
                float2 uv;
            };

            float4 _MainTex_ST;
            sampler2D _MainTex;
            sampler2D _HeightMap;

            StructuredBuffer<grass_data> _PositionBuffer;

            float _WindStrength;
            float _CullingBias;
            float _DisplacementStrength;
            float _LODCutoff;
            float _QuadScale;
            float _Rotation;

            float4 rotate_around_y_in_degrees (float4 vertex, float degrees)
            {
                float alpha = degrees * UNITY_PI / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);

                return float4(mul(m, vertex.xz), vertex.yw).xzyw;
            }

            bool vertex_is_below_clip_plane(float3 p, int planeIndex, float bias) {
                float4 plane = unity_CameraWorldClipPlanes[planeIndex];

                return dot(float4(p, 1), plane) < bias;
            }

            bool cull_vertex(float3 p, float bias)
            {
                return  distance(_WorldSpaceCameraPos, p) > _LODCutoff ||
                        vertex_is_below_clip_plane(p, 0, bias) ||
                        vertex_is_below_clip_plane(p, 1, bias) ||
                        vertex_is_below_clip_plane(p, 2, bias) ||
                        vertex_is_below_clip_plane(p, 3, -max(1.0f, _DisplacementStrength));
            }

            interpolators vert (mesh_data v, uint instanceID : SV_INSTANCEID)
            {
                interpolators o;

                float3 local_position = rotate_around_y_in_degrees(v.vertex, _Rotation).xyz;

                float local_wind_variance = min(max(0.4f, randValue(instanceID)), 0.75f);

                float4 grassPosition = _PositionBuffer[instanceID].position;

                float cos_time;
                if (local_wind_variance > 0.6f)
                {
                    cos_time = cos(_Time.y * (_WindStrength - (grassPosition.w - 1.0f)));
                }
                else
                {
                    cos_time = cos(_Time.y * (_WindStrength - (grassPosition.w - 1.0f) + local_wind_variance * 0.1f));
                }

                float trig_value = cos_time * cos_time * 0.65f - local_wind_variance * 0.5f;

                local_position.x += v.uv.y * trig_value * grassPosition.y * local_wind_variance * 0.6f;
                local_position.z += v.uv.y * trig_value * grassPosition.y * 0.4f;
                local_position *= _QuadScale;

                float4 worldPosition = float4(grassPosition.xyz + local_position, 1.0f);

                if (cull_vertex(worldPosition, -_CullingBias * max(1.0f, _DisplacementStrength)))
                {
                    o.vertex = 0.0f;
                }
                else
                {
                    o.vertex = UnityObjectToClipPos(worldPosition);
                }

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.saturation_level = 1.0 - (_PositionBuffer[instanceID].position.w - 1.0f) / 1.5f;
                o.saturation_level = max(o.saturation_level, 0.5f);

                return o;
            }

            fixed4 frag (interpolators i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(-(0.5 - col.a));

                float saturation = lerp(1.0f, i.saturation_level, i.uv.y * i.uv.y * i.uv.y);
                col.r /= saturation;

                float3 light_direction = _WorldSpaceLightPos0.xyz;
                float ndotl = DotClamped(light_direction, normalize(float3(0, 1, 0)));

                return col * ndotl;
            }

            ENDCG
        }
    }
}