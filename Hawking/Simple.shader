Shader "Custom/Simple"
{
    Properties
    {
        [MainColor] _BaseColor("Tint", Color) = (0.5,0.5,0.5,1)
        [MainTexture] _BaseMap("Texture", 2D) = "white" {}
        _ReceiveShadows("Receive Shadows", Float) = 0.0
    }

    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
            "ShaderModel"="4.5"
        }
        LOD 100

        Pass
        {
            Name "Lit"
            Tags{ "LightMode" = "UniversalForward" }
            
			ZWrite On
			ZTest LEqual
           
            
            HLSLPROGRAM
            
            //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_instancing
            
            #pragma vertex VertexPass
            #pragma fragment FragPass
            
            #include "Common.hlsl"
            
            ENDHLSL
        }
        
        Pass 
        {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ColorMask 0
			ZWrite On
			ZTest LEqual
            Cull[_Cull]

			HLSLPROGRAM
            
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            
			ENDHLSL
		}
        
        Pass 
        {
			Name "DepthOnly"
			Tags { "LightMode" = "DepthOnly" }

			ColorMask 0
			ZWrite On
			ZTest LEqual
            Cull[_Cull]

			HLSLPROGRAM

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            
			ENDHLSL
		}
    }
}