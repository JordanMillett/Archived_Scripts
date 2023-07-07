#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

float _ReceiveShadows;

struct VertData
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    float2 uv           : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct FragData
{
    float2 uv                       : TEXCOORD0;
    float3 world_position           : TEXCOORD1; // xyz: positionWS, w: vertex fog factor
    half3  normal                   : TEXCOORD2;
    float4 pixel_position           : SV_POSITION;
    float4 shadowCoord              : TEXCOORD6;
};

FragData VertexPass(VertData input)
{
    FragData o;

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    o.world_position = vertexInput.positionWS;

    float4 snapToPixel = vertexInput.positionCS;
    float4 vertex = snapToPixel;
    vertex.xyz = snapToPixel.xyz / snapToPixel.w;
    vertex.x = floor(256 * vertex.x) / 256;
    vertex.y = floor(144 * vertex.y) / 144;
    vertex.xyz *= snapToPixel.w;
    
    o.pixel_position = vertex;
    o.uv = TRANSFORM_TEX(input.uv, _BaseMap);
    o.normal = vertexNormalInput.normalWS;
    o.shadowCoord = GetShadowCoord(vertexInput);

    return o;
}

half4 FragPass(FragData input) : SV_Target
{
    Light mainLight = GetMainLight(input.shadowCoord);
    half3 lightDir = mainLight.direction;
    half3 lightColor = mainLight.color;
    
    half dotProd = (dot(input.normal, lightDir) + 1.2)/2.0;

    half3 color = _BaseMap.Sample(sampler_BaseMap, input.uv).xyz;
    color *= _BaseColor.xyz;
    color *= lightColor * dotProd;
    
    half3 color_shadowed = color * 0.5;
    
    half fade = saturate(-TransformWorldToView(input.world_position).z/200);
    
    half3 shadows = lerp(1, lerp(mainLight.shadowAttenuation, 1, fade), _ReceiveShadows);
    shadows *= saturate(lightColor);
    
    return half4(lerp(color_shadowed, color, shadows), 1);
}