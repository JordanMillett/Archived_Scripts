Shader "Unlit/MarchedClouds"                              //Shader lookup location
{
    Properties
    {
        _MAX_STEPS ("Max Steps", Float) = 100             //All exposed properties
        _MAX_DIST ("Max Distance", Float) = 100
        _SURF_DIST ("Surface Distance", Float) = 0.001
    }
    SubShader
    {
        LOD 100                                             
        CULL OFF                                            //Raymarching works inside shape
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
                float4 vertex : POSITION;                   //Vertex position data
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;                //Vertex position data
                float3 rayOrigin : TEXCOORD0;                      //Ray origin stored in TexCoord0
                float3 hitPos : TEXCOORD1;                  //Hit position stored in TexCoord1
            };
            
            float _MAX_STEPS;                               //Steps used to refine shape
            float _MAX_DIST;                                //Distance until pixel is invisible
            float _SURF_DIST;                               //Distance till collision is counted

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.rayOrigin = _WorldSpaceCameraPos;                                            //World Space
                o.hitPos = mul(unity_ObjectToWorld, v.vertex);
                //o.rayOrigin = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));    //Object Space
                //o.hitPos = v.vertex;
                return o;
            }
            
            float3 REPXYZ(float3 position, float3 factor)
            {
                float3 tmp = (position.xyz % factor) - (0.5 * factor);
                return float3(tmp.x, tmp.y, tmp.z);
            }

            float GetDist(float3 position)                         //Gets distance until collision
            {
                //float sin1 = (sin((position.x + (_Time.y * 5))/4))/2;
                //float sin2 = (sin((position.z + (_Time.y * 5))/4))/2;
                //float dist = position.y + sin1 + sin2;

                //position = REPXYZ(position, float3(10, 10, 10));

                //float d = length(float2(length(p.xz) - 0.25, p.y)) -.05;
                float3 z = position;
                float dr = 1;
                float r = 0;
                float _POWER = 5;
                
                for (int i = 0; i < 15 ; i++) 
                {
                    r = length(z);
                    if(r > 2) 
                        break;
                    
                    float theta = acos(z.z / r) * _POWER;
                    float phi = atan2(z.y , z.x) * _POWER;
                    float zr = pow(r , _POWER);
                    dr = pow(r , _POWER - 1) * _POWER * dr + 1;
                    
                    z = zr * float3(sin(theta) * cos(phi), sin(phi) * sin(theta), cos(theta));
                    z += position;
                }
                //return r / dr;
                return .3 * log(r) * r / dr;
                //return d;
                //float dist = length(position) - .5;

                //float sin1 = (sin((position.x + (_Time.y * 5))/4))/2;
                //float sin2 = (sin((position.z + (_Time.y * 5))/4))/2;
                //float dist = position.y + sin1 + sin2;
                

                //float dist = length(position % 1) - .25;
                /*
                float3 box = float3(1.5, 1.5, 1.5);
                float e = .1;

                //float3 p = length(1 % position) - 1;

                float3 p = abs(position) - box;

                
                float3 q = abs(p + e) - e;
                
                float dist =  min(min(
                    length(max(float3(p.x,q.y,q.z),0.0))+min(max(p.x,max(q.y,q.z)),0.0),
                    length(max(float3(q.x,p.y,q.z),0.0))+min(max(q.x,max(p.y,q.z)),0.0)),
                    length(max(float3(q.x,q.y,p.z),0.0))+min(max(q.x,max(q.y,p.z)),0.0));
                
                
                */
                //return dist;
            }

            float2 Raymarch(float3 rayOrigin, float3 rayDirection)            //Raymarches in a direction from an origin
            {
                float distFromOrigin = 0;
                float distToSurface;
                //float steps = 0;
                float trap = 0;
                float rad = 0;
                for(int i = 0; i < _MAX_STEPS; i++)
                {
                    //steps++;
                    float3 position = rayOrigin + distFromOrigin * rayDirection;
                    distToSurface = GetDist(position);
                    //trap += distToSurface;
                    //trap = distToSurface - distFromOrigin;
                    //rad = pow(sqrt(position.x*position.x+position.y*position.y+position.z*position.z), 2);
                    rad = i;
                    distFromOrigin += distToSurface;
                    if(distToSurface < _SURF_DIST || distFromOrigin > _MAX_DIST)
                        break;
                }

                trap += rad/_MAX_STEPS;
                
                //return float2(distFromOrigin, steps/_MAX_STEPS);
                return float2(distFromOrigin, trap);
            }

            float sampleDensity(float position)
            {
                return 0.001;
            }

            float Stepmarch(float rayOrigin, float rayDirection)
            {

                float totalDensity = 0;
                float distFromOrigin = 0;
                float stepSize = _MAX_DIST/_MAX_STEPS;
                while(distFromOrigin < _MAX_DIST)
                {

                    float3 position = rayOrigin + distFromOrigin * rayDirection;
                    totalDensity += sampleDensity(position);
                    distFromOrigin += stepSize;

                }

                return totalDensity;


            }

            float3 GetNormal(float3 position)                      //Gets normal for given point
            {
                float2 e = float2(1e-2, 0);             //?
                float3 normalDir = GetDist(position) - float3(
                    GetDist(position-e.xyy),
                    GetDist(position-e.yxy),
                    GetDist(position-e.yyx)
                );
                return normalize(normalDir);
            }

            float GetLight(float3 position)                        //Gets lighting for given point
            {

                float3 lightPos = float3(0, 5, 0);
                float3 lightDir = normalize(lightPos - position);
                float3 normalDir = GetNormal(position);

                float diffuse = dot(normalDir , lightDir);
                diffuse = clamp(diffuse, 0.25, 1);
                //diffuse *= ;
                return diffuse;

            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 rayOrigin = i.rayOrigin;                    //Ray Origin
                float3 rayDirection = normalize(i.hitPos - rayOrigin);       //Ray Direction

                
                float2 distanceANDColor = Raymarch(rayOrigin, rayDirection);                 //Ray Distance
                fixed4 col = 1;                        //Pixel Color

                if(distanceANDColor.x < _MAX_DIST)                           //If not exceeding max distance
                {
                    float3 position = rayOrigin + rayDirection * distanceANDColor.x;                 //Ray Point
                    //float diffuse = GetLight(position);                //Diffuse Lighting
                    //diffuse += 1;
                    //diffuse = 1 % diffuse;    
                    //Pixel equals Lighting
                    //col = lerp(float4(.5,.5,1,0), float4(0,0,0,0), (distanceANDColor.y/_MAX_STEPS));
                    //col = lerp(float4(1,0,0,0), float4(0,1,0,0), diffuse);
                    //col = lerp(col, float4(0,0,1,0), diffuse);
                    float diffuse = distanceANDColor.y;
                    //diffuse = 1 % diffuse;
                    //float2 InMinMax = float2(.1, 2);
                    //float2 OutMinMax = float2(0, 1);
                    //diffuse = OutMinMax.x + (diffuse - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
                    //diffuse = 5 % diffuse;
                    col = lerp(float4(1,0,0,0), float4(0,0,1,0), diffuse);
                    col *= (GetLight(position));
                    //col = lerp(col, float4(0,0,1,0), diffuse);

                }else
                {
                    discard;                                //Unused Pixels are invisible
                }
                /*
                float4 col = 0;
                col += float4(Stepmarch(rayOrigin, rayDirection).xxx, 0);   

                bool getRid = col == 0;
                if(getRid)
                    discard;
                */

                return col;                                 //This is the fragment color
            }
            ENDCG
        }
    }
}
