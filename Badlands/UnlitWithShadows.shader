Shader "UnlitWithShadows"
{
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            
            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICSPECGLOSSMAP
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _OCCLUSIONMAP

            #pragma shader_feature _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _SPECULAR_SETUP
            #pragma shader_feature _RECEIVE_SHADOWS_OFF

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            
            #pragma multi_compile_instancing

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
                float2 uvLM         : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv                       : TEXCOORD0;
                float2 uvLM                     : TEXCOORD1;
                float4 positionWSAndFogFactor   : TEXCOORD2; // xyz: positionWS, w: vertex fog factor
                half3  normalWS                 : TEXCOORD3;
                float4 positionCS               : SV_POSITION;
            };

            Varyings LitPassVertex(Attributes input)
            {
                Varyings output;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.uvLM = input.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;

                output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
                output.normalWS = vertexNormalInput.normalWS;

                output.positionCS = vertexInput.positionCS;
                return output;
            }
  
            half4 LitPassFragment(Varyings input) : SV_Target
            {
                
                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(input.uv, surfaceData);
                
                float shadowStrength = 0;
                
                float3 positionWS = input.positionWSAndFogFactor.xyz;
                half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - positionWS);
                
                BRDFData brdfData;
                InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);
                
                half3 normalWS = input.normalWS;
                half3 bakedGI = SampleSH(normalWS);
                half3 color = GlobalIllumination(brdfData, bakedGI, surfaceData.occlusion, normalWS, viewDirectionWS);
                
                //half4 shadowMask = input.shadowMask;
                half4 shadowMask = half4(0,0,0,0);
                
                int additionalLightsCount = GetAdditionalLightsCount();
                
                float PixelColor = 0;
                for (int lightIndex = 0; lightIndex < additionalLightsCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, positionWS, shadowMask);
                    
                    if(light.color.r == 0)
                    {
                        if(light.distanceAttenuation != 0)
                        {
                            PixelColor = light.shadowAttenuation;
                            PixelColor = lerp(0, PixelColor, min(light.distanceAttenuation * 1000, 1));
                        }
                    }
                }
                
                return PixelColor;
            }
            
            ENDHLSL
        }
       
        Pass
        {
            Tags{ "LightMode" = "ShadowCaster" }
            CGPROGRAM
            #pragma vertex VSMain
            #pragma fragment PSMain
 
            float4 VSMain (float4 vertex:POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }
 
            float4 PSMain (float4 vertex:SV_POSITION) : SV_TARGET
            {
                return 0;
            }
           
            ENDCG
        }
    }
}