Shader "Unlit/circle"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    _radius("Radius",Range(0, 100)) = 100
        _radiusOffet("Offset",Range(0, 100)) = 72
        _discardedColor("Dicard Color", Color) = (0, 0, 0, 0) // color
    }
        SubShader
    {
        //Tags{ "Queue" = "Transparent" }
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
    {
        CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


    sampler2D _MainTex;
    float4 _discardedColor;

    float _radius;
    float _radiusOffet;

    float minMinSlide = 0; //0.314
    float minMaxSlide = 100.0;

    struct v2f
    {
        float2 uv : TEXCOORD0;
    };

    v2f vert(
        float4 vertex : POSITION, // vertex position input
        float2 uv : TEXCOORD0, // texture coordinate input
        out float4 outpos : SV_POSITION // clip space position output
    )
    {
        v2f o;
        o.uv = uv;
        outpos = UnityObjectToClipPos(vertex);
        return o;
    }

    float mapValue(float mainValue, float inValueMin, float inValueMax, float outValueMin, float outValueMax)
    {
        return (mainValue - inValueMin) * (outValueMax - outValueMin) / (inValueMax - inValueMin) + outValueMin;
    }

    fixed4 frag(v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
    {
        //Set default values
        minMinSlide = 0;
    minMaxSlide = 100;

    //fixed4 col = tex2D(_MainTex, i.uv);

    float4 fragColor = 0;
    //float2 fragCoord = i.vertex.xy;
    float2 fragCoord = screenPos;
    float2 screenCoord = fragCoord.xy;

    float minOutRadius = 0;

    //THIS IS WRONG? WHAT'S THE PROPER WAY OF DOING THIS?
    float maxOutRadius = (_ScreenParams.x / 2) + _radiusOffet;

    //Calcuate the scaled radius
    float rad = mapValue(_radius, minMinSlide, minMaxSlide, minOutRadius, maxOutRadius);

    float2 middlePoint = _ScreenParams.xy * 0.5 - screenCoord;
    float middleLength = middlePoint.x * middlePoint.x + middlePoint.y * middlePoint.y;
    if (middleLength > rad * rad)
    {
        //Use black color
        fragColor = _discardedColor;
    }
    else {
        //Use red color
        fragColor = float4(1, 0, 0, 0);
    }
    return fragColor;
    }
        ENDCG
    }
    }
}