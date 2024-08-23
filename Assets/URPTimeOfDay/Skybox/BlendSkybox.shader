Shader "Custom/BlendSkybox"
{
    Properties
    {
        _Day("Day (HDR Cubemap)", Cube) = "" {}
        _Night("Night (HDR Cubemap)", Cube) = "" {}
        _LerpValue("Lerp Value", Range(0, 1)) = 0.5
        _DayExposure("Day Exposure", Range(-8, 8)) = 0 
        _NightExposure("Night Exposure", Range(-8, 8)) = 0
        _DayRotation("Day Rotation", Range(0, 360)) = 0 
        _NightRotation("Night Rotation", Range(0, 360)) = 0 
    }

        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float3 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                samplerCUBE _Day;
                samplerCUBE _Night;
                float _LerpValue;
                float _DayExposure;
                float _NightExposure;
                float _DayRotation;
                float _NightRotation;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = normalize(v.vertex.xyz);
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    float dayRotationRad = _DayRotation * (3.14159265 / 180.0);
                    float nightRotationRad = _NightRotation * (3.14159265 / 180.0);

                    float3x3 dayRotationMat = float3x3(
                        cos(dayRotationRad), 0, -sin(dayRotationRad),
                        0, 1, 0,
                        sin(dayRotationRad), 0, cos(dayRotationRad)
                    );

                    float3x3 nightRotationMat = float3x3(
                        cos(nightRotationRad), 0, -sin(nightRotationRad),
                        0, 1, 0,
                        sin(nightRotationRad), 0, cos(nightRotationRad)
                    );

                    float3 dayRotatedUV = mul(dayRotationMat, i.uv);
                    float3 nightRotatedUV = mul(nightRotationMat, i.uv);

                    half4 dayColor = texCUBE(_Day, dayRotatedUV);
                    half4 nightColor = texCUBE(_Night, nightRotatedUV);

                    dayColor.rgb *= pow(2, _DayExposure);
                    nightColor.rgb *= pow(2, _NightExposure);

                    return lerp(nightColor, dayColor, _LerpValue);
                }
                ENDCG
            }
        }
}