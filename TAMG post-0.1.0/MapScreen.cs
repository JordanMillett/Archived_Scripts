﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScreen : MonoBehaviour
{
    Transform MapEmpty;
    Transform MapMarker;
    Transform Follow;

    Transform MapCam;

    float CurrentMapSize = 100f;
    //float MinSize = 20f;
    //float MaxSize = 200f;
    float MarkScale = 0f;

    void Start()
    {
        MapCam = this.transform;
        Follow = GameObject.FindGameObjectWithTag("Camera").transform;
        MapEmpty = GameObject.FindGameObjectWithTag("Target").transform.GetChild(0);
        MapMarker = MapEmpty.transform.GetChild(0);
    }

    void Update()
    {
        if(Input.mouseScrollDelta.y < 0f)
        {
            //if(CurrentMapSize < MaxSize)
                //CurrentMapSize += 10f;

            MarkScale = CurrentMapSize/5f;
            //MapMarker.localScale = new Vector3(MarkScale, MarkScale, MarkScale);
            MapMarker.localPosition = new Vector3(0f, 40f, MarkScale/2);
        }

        if(Input.mouseScrollDelta.y > 0f)
        {
            //if(CurrentMapSize > MinSize)
                //CurrentMapSize -= 10f;

            MarkScale = CurrentMapSize/5f;
            //MapMarker.localScale = new Vector3(MarkScale, MarkScale, MarkScale);
            MapMarker.localPosition = new Vector3(0f, 40f, MarkScale/2);
        }
    
        MapCam.transform.position = new Vector3(Follow.position.x, -10f, Follow.position.z);
        MapCam.transform.GetChild(0).transform.eulerAngles = new Vector3(0f, Follow.transform.eulerAngles.y, 0f);
        MapEmpty.transform.localEulerAngles = new Vector3(0f, Follow.transform.eulerAngles.y, 0f);
    }

    /*
    Transform MapEmpty;
    Transform MapMarker;
    Transform Follow;

    Transform MapCam;

    float CurrentMapSize = 100f;
    float MinSize = 20f;
    float MaxSize = 200f;
    float MarkScale = 0f;

    void Start()
    {
        MapCam = GameObject.FindGameObjectWithTag("MapCam").transform;
        Follow = GameObject.FindGameObjectWithTag("Player").transform;
        MapEmpty = GameObject.FindGameObjectWithTag("Target").transform.GetChild(0);
        MapMarker = MapEmpty.transform.GetChild(0);
    }

    void Update()
    {
        if(Input.mouseScrollDelta.y < 0f)
        {
            //if(CurrentMapSize < MaxSize)
                //CurrentMapSize += 10f;

            MarkScale = CurrentMapSize/5f;
            //MapMarker.localScale = new Vector3(MarkScale, MarkScale, MarkScale);
            MapMarker.localPosition = new Vector3(0f, 40f, MarkScale/2);
        }

        if(Input.mouseScrollDelta.y > 0f)
        {
            //if(CurrentMapSize > MinSize)
                //CurrentMapSize -= 10f;

            MarkScale = CurrentMapSize/5f;
            //MapMarker.localScale = new Vector3(MarkScale, MarkScale, MarkScale);
            MapMarker.localPosition = new Vector3(0f, 40f, MarkScale/2);
        }
    
        MapCam.transform.position = new Vector3(Follow.position.x, -10f, Follow.position.z);
        MapCam.transform.GetChild(0).transform.eulerAngles = new Vector3(0f, Follow.transform.eulerAngles.y, 0f);
        MapEmpty.transform.localEulerAngles = new Vector3(0f, Follow.transform.eulerAngles.y, 0f);
    }
    */
}

/*
Shader "UI/Unlit/MapUI"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
        
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                
                
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif
                

                color.a = 1;

                return color;
            }
        ENDCG
        }
    }
}
*/