Shader "Custom/VisionEffect"
{
	Properties 
	{
        _MainTex ("Main Texture", 2D) = "white" {}
        _VisionTex ("Vision Texture", 2D) = "white" {}
        _Intensity ("Intensity", Float) = 1
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
		
		Pass
		{
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            
			#pragma vertex vert
			#pragma fragment frag
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            TEXTURE2D(_VisionTex);
            SAMPLER(sampler_VisionTex);
            
			float _Intensity;
            
            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv        : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;
                output.uv = input.uv;
                
                return output;
            }
            
            float4 frag (Varyings input) : SV_Target 
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float shadowmask = SAMPLE_TEXTURE2D(_VisionTex, sampler_VisionTex, input.uv).r;
                float4 masked = lerp(0, original, shadowmask);

                //return lerp(original, shadowmask, _Intensity);
                return lerp(original, masked, _Intensity);
            }
            
			ENDHLSL
		}
	} 
}