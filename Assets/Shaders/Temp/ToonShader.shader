Shader "Custom/ToonShader_URP"
{
    Properties
    {
        _MainColor ("Color", Color) = (0.8,0.8,0.8,1)
        _ShadowColor("Shadow Color", Color) = (0.5,0.5,0.5,1)
        _ShadowSoftness("Shadow Softness", Range(0, 1)) = 0.5
        _FogColor("Fog Color", Color) = (0.5,0.5,0.5,0.5)
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "Queue"="Geometry"
        }
        LOD 100

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Required URP includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float4 fogFactor : TEXCOORD2;
            };

            float4 _MainColor;
            float4 _ShadowColor;
            float _ShadowSoftness;
            float4 _FogColor;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.positionHCS = TransformWorldToHClip(o.positionWS);

                // Calculate fog factor
                o.fogFactor = ComputeFogFactor(o.positionWS.z);

                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 normal = normalize(i.normalWS);

                // Get main light direction and intensity in URP
                Light mainLight = GetMainLight(); 
                float3 lightDir = normalize(mainLight.direction);
                float NdotL = saturate(dot(normal, lightDir));
                
                // Apply shadow softness
                float lightIntensity = smoothstep(0, _ShadowSoftness, NdotL);

                // Apply toon shading by blending shadow color and main color based on light intensity
                float4 baseColor = lerp(_ShadowColor, _MainColor, lightIntensity);

                // Apply fog by blending the base color with fog color
                float4 finalColor = lerp(baseColor, _FogColor, i.fogFactor.a);

                return finalColor;
            }
            ENDHLSL
        }
    }
}
