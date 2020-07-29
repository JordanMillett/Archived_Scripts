Shader "Unlit/RayMarcher"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MAX_STEPS ("Max Steps", Float) = 100
        _MAX_DIST ("Max Distance", Float) = 100
        _SURF_DIST ("Surface Distance", Float) = 0.001
        _POWER ("POWER", Float) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        CULL OFF
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ro : TEXCOORD1;
                float3 hitPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _MAX_STEPS;
            float _MAX_DIST;
            float _SURF_DIST;
            float _POWER;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.ro = _WorldSpaceCameraPos;      //CLOUDS EFFECT, WORLD SPACE
                o.hitPos = mul(unity_ObjectToWorld, v.vertex);
                //o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));  
                //o.hitPos = v.vertex;
                return o;
            }
            
            float GetDist(float3 p)
            {   
                //float d = length(float2(length(p.xz) - 0.25, p.y)) -.05;
                float3 z = p;
                float dr = 1;
                float r = 0;
                //float power = 5;
                for (int i = 0; i < _MAX_STEPS ; i++) 
                {
                    r = length(z);
                    if(r > 2) 
                        break;
                    
                    float theta = acos(z.z / r) * _POWER;
                    float phi = atan2(z.y , z.x) * _POWER;
                    float zr = pow(r , _POWER);
                    dr = pow(r , _POWER - 1) * _POWER * dr + 1;
                    
                    z = zr * float3(sin(theta) * cos(phi), sin(phi) * sin(theta), cos(theta));
                    z += p;
                }
                return 0.5 * log(r) * r / dr;
                //return d;
            }

            float2 Raymarch(float3 ro, float3 rd)
            {
                float dO = 0;
                float dS;
                float steps = 0;
                for(int i = 0; i < _MAX_STEPS; i++)
                {
                    steps++;
                    float3 p = ro + dO * rd;
                    dS = GetDist(p);
                    dO += dS;
                    if(dS < _SURF_DIST || dO > _MAX_DIST)
                        break;
                }
                
                return float2(dO, steps);
            }

            float3 GetNormal(float3 p)
            {
                float2 e = float2(1e-2, 0);
                float3 n = GetDist(p) - float3(
                    GetDist(p-e.xyy),
                    GetDist(p-e.yxy),
                    GetDist(p-e.yyx)
                );
                return normalize(n);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv -0.5;
                float3 ro = i.ro;
                float3 rd = normalize(i.hitPos - ro);

                float2 stonk = Raymarch(ro, rd);
                //float d = Raymarch(ro, rd).x;
                fixed4 col = 0;

                if(stonk.x < _MAX_DIST)
                {
                    //float3 p = ro + rd * stonk.x;
                    //float3 n = GetNormal(p);
                    //col.rgb = n;
                    //col.rgb *= 1.5 - stonk.y/_MAX_STEPS;
                    col = lerp(float4(0,0,0,0), float4(.5,.5,1,0),(stonk.y/_MAX_STEPS));
                }else
                {
                    discard;
                }


                return col;
            }
            ENDCG
        }
    }
}
