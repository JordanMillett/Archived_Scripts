Shader "Custom/Raymarched"
{
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
        }

        Cull Off 
        ZWrite Off 
        ZTest Always

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewVector : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f output;
                output.pos = UnityObjectToClipPos(v.vertex);
                output.uv = v.uv;
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                output.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
                return output;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float3 boundsMin;
            float3 boundsMax;

            float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 invRaydir)
            {
                float3 t0 = (boundsMin - rayOrigin) * invRaydir;
                float3 t1 = (boundsMax - rayOrigin) * invRaydir;
                float3 tmin = min(t0, t1);
                float3 tmax = max(t0, t1);
                
                float dstA = max(max(tmin.x, tmin.y), tmin.z);
                float dstB = min(tmax.x, min(tmax.y, tmax.z));

                float dstToBox = max(0, dstA);
                float dstInsideBox = max(0, dstB - dstToBox);
                return float2(dstToBox, dstInsideBox);
            }

            fixed4 frag(v2f input) : SV_Target
            {
                float4 col = 1;
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(input.viewVector);

                float nonLinearDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, input.uv);
                float depth = LinearEyeDepth(nonLinearDepth) * length(input.viewVector);

                float2 rayBoxInfo = rayBoxDst(boundsMin, boundsMax, rayOrigin, rayDir);
                float dstToBox = rayBoxInfo.x;
                float dstInsideBox = rayBoxInfo.y;

                
                bool rayHitBox = dstInsideBox > 0 && dstToBox < depth;
                if(rayHitBox)
                {
                    col = 0;
                }
                
                //col = tex2D(_MainTex, input.uv);
                //return 1 - col;

                col = tex2D(_MainTex, input.uv);
                return col;
                //return 1;
            }
            
            ENDCG
        }
    }
}
