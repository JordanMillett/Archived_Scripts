Shader "Custom/PS1_Lit_REF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
        }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
			{
				fixed4 pos : SV_POSITION;
				half4 color : COLOR0;
				half4 colorFog : COLOR1;
				float2 uv_MainTex : TEXCOORD0;
				half3 normal : TEXCOORD1;
			};

            float4 _MainTex_ST;
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

            float3 CustomShading(float4 vertex, float3 normal)
            {
                float3 viewPosition = mul(UNITY_MATRIX_MV, vertex).xyz;
                float3 viewNormal = normalize(mul ((float3x3)UNITY_MATRIX_IT_MV, normal));

                float3 lightColor = float3(0,0,0); //ambient light color

                lightColor = float3(1.0, 1.0, 1.0);

                //lightColor = _WorldSpaceLightPos0.xyz;
                //lightColor = unity_LightPosition[0].xyz;

                /*
                for (int i = 0; i < 8; i++) 
                {
                    float3 toLight = unity_LightPosition[i].xyz - viewPosition.xyz * unity_LightPosition[i].w;
                    float lengthSq = dot(toLight, toLight);
                    toLight *= rsqrt(lengthSq);

                    float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);

                    float diff = max (0, dot (viewNormal, toLight));
                    lightColor += unity_LightColor[i].rgb * (diff * atten);
                }*/
                return lightColor;
            }

            v2f vert(appdata_full v)
			{
				v2f o;

				//Vertex snapping
				float4 snapToPixel = UnityObjectToClipPos(v.vertex);
				float4 vertex = snapToPixel;
				vertex.xyz = snapToPixel.xyz / snapToPixel.w;
				vertex.x = floor(256 * vertex.x) / 256;
				vertex.y = floor(144 * vertex.y) / 144;
				vertex.xyz *= snapToPixel.w;
				o.pos = vertex;

				//Vertex lighting 
				o.color = float4(CustomShading(v.vertex, v.normal), 1.0);
				o.color *= v.color;

				float distance = length(mul(UNITY_MATRIX_MV,v.vertex));

				//Affine Texture Mapping
				float4 affinePos = vertex; //vertex;				
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_MainTex *= distance + (vertex.w*(UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;
				o.normal = distance + (vertex.w*(UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;

				//Fog
				float4 fogColor = unity_FogColor;

				float fogDensity = (unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart);
				o.normal.g = fogDensity;
				o.normal.b = 1;

				o.colorFog = fogColor;
				o.colorFog.a = clamp(fogDensity,0,1);

				//Cut out polygons
				if (distance > unity_FogStart.z + unity_FogColor.a * 255)
				{
					o.pos.w = 0;
				}

				return o;
			}

            sampler2D _MainTex;
			float4 frag(v2f IN) : COLOR
			{
				half4 c = tex2D(_MainTex, IN.uv_MainTex / IN.normal.r)*IN.color;
				half4 color = c*(IN.colorFog.a);
				color.rgb += IN.colorFog.rgb*(1 - IN.colorFog.a);
				return color;
			}
            ENDCG
        }
    }
}
