Shader "Custom/PS1_Lit_Billboard"
{
    Properties
    {
        [MainColor] _BaseColor("Color", Color) = (0.5,0.5,0.5,1)
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        _LockAxis("Lock Axis", Float) = 0.0

        // Blending state
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalRenderPipeline"
        }
        LOD 200

        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM

            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            struct appdata
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv                       : TEXCOORD0;
                float3 world_position           : TEXCOORD1; // xyz: positionWS, w: vertex fog factor
                half3  normal                   : TEXCOORD2;
                float4 pixel_position           : SV_POSITION;
            };

            float _LockAxis;

            v2f LitPassVertex(appdata input)
            {
                v2f o;

                // VertexPositionInputs contains position in multiple spaces (world, view, homogeneous clip space)
                // Our compiler will strip all unused references (say you don't use view space).
                // Therefore there is more flexibility at no additional cost with this struct.
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                // Similar to VertexPositionInputs, VertexNormalInputs will contain normal, tangent and bitangent
                // in world space. If not used it will be stripped.
                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                //Affine Texture Mapping
                //float distance = length(mul(UNITY_MATRIX_MV,vertexInput.positionCS));
				//float4 affinePos = vertexInput.positionCS; //vertex;				
				//o.uv = TRANSFORM_TEX(input.uv, _BaseMap);
				//o.uv *= distance + (vertexInput.positionCS.w*(UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;
				//o.normal = distance + (vertexInput.positionCS.w*(UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;
                
                o.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                o.normal = _WorldSpaceCameraPos.xyz - mul(UNITY_MATRIX_M, input.positionOS);
                o.world_position = vertexInput.positionWS;


                      
                /*
                float4 model_origin = mul(UNITY_MATRIX_M, float4(0,0,0,1));
                float4 view_origin = mul(UNITY_MATRIX_V, model_origin);
                float4 world_pos = mul(UNITY_MATRIX_M, input.positionOS);
                float4 view_pos = world_pos - model_origin + view_origin;
                float4 clip_pos = mul(UNITY_MATRIX_P, view_pos);
                */

                float4 model_origin = mul(UNITY_MATRIX_M, float4(0,0,0,1));
                float4 view_origin = mul(UNITY_MATRIX_V, model_origin);

                float4 world_pos = mul(UNITY_MATRIX_M, input.positionOS);
                float4 view_pos = mul(UNITY_MATRIX_V, world_pos);

                float4 face_cam = world_pos - model_origin + view_origin;

                float4 clip_pos = mul(UNITY_MATRIX_P, face_cam);
                //float4 clip_pos = mul(UNITY_MATRIX_P, float4(view_pos.x,view_pos.y,face_cam.z,face_cam.w));


                //Pixel Snapping in Projection Space
                float4 snapToPixel = clip_pos;
                float4 vertex = snapToPixel;
				vertex.xyz = snapToPixel.xyz / snapToPixel.w;
                //vertex.x = floor(384 * vertex.x) / 384;
				//vertex.y = floor(216 * vertex.y) / 216;
				vertex.x = floor(256 * vertex.x) / 256;
				vertex.y = floor(144 * vertex.y) / 144;
				vertex.xyz *= snapToPixel.w;

                o.pixel_position = vertex;

                return o;
            }

            half4 LitPassFragment(v2f input) : SV_Target
            {
                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(input.uv, surfaceData);

                half3 normal = input.normal;
                normal = normalize(normal);
        
                float3 position = input.world_position;
                half3 viewDirection = SafeNormalize(GetCameraPositionWS() - position);

                BRDFData brdfData;
                InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

                if(surfaceData.alpha == 0)
                    discard;

                half3 color = half3(0,0,0);

                #ifdef _ADDITIONAL_LIGHTS
            
                    int additionalLightsCount = GetAdditionalLightsCount();
                    for (int i = 0; i < additionalLightsCount; ++i)
                    {
                        // Similar to GetMainLight, but it takes a for-loop index. This figures out the
                        // per-object light index and samples the light buffer accordingly to initialized the
                        // Light struct. If _ADDITIONAL_LIGHT_SHADOWS is defined it will also compute shadows.
                        Light light = GetAdditionalLight(i, position);

                        // Same functions used to shade the main light.
                        color += LightingPhysicallyBased(brdfData, light, normal, viewDirection);
                        //color += CustomShading(light, normalWS, viewDirectionWS);
                    }
                #endif

                return half4(color, 1);
            }
            ENDHLSL
        }
    }
}