// Unlit sprite with a noise dissolve, for Whitey's fade-out. Replaces the old Dissolve.shadergraph, whose legacy
// master node no longer compiles a usable pass in Unity 6.5 (the sprite rendered invisible).
//   _Fade 1 = fully visible, 0 = fully dissolved. Driven per renderer by `fader` through a MaterialPropertyBlock.
// Noise is sampled in world space so the pattern runs continuously across all of a DragonBones rig's slot meshes.
Shader "Promise/Sprite Dissolve"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HDR] _Color ("Tint", Color) = (1, 1, 1, 1)
        _Fade ("Fade", Range(0, 1)) = 1
        _Scale ("Noise Scale", Float) = 40
        _EdgeWidth ("Edge Width", Range(0, 0.2)) = 0.04
        [HDR] _EdgeColor ("Edge Color", Color) = (2, 1.8, 1.6, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _Color;
            half _Fade;
            float _Scale;
            half _EdgeWidth;
            half4 _EdgeColor;
        CBUFFER_END

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            half4 color : COLOR;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            half4 color : COLOR;
            float2 positionWS : TEXCOORD1;
        };

        // Value noise, same construction as Shader Graph's Simple Noise node.
        float Hash(float2 p)
        {
            return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
        }

        float ValueNoise(float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);
            float a = Hash(i);
            float b = Hash(i + float2(1, 0));
            float c = Hash(i + float2(0, 1));
            float d = Hash(i + float2(1, 1));
            return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
        }

        float SimpleNoise(float2 uv, float scale)
        {
            float n = 0;
            n += ValueNoise(uv * scale) * 0.125;
            n += ValueNoise(uv * scale / 2.0) * 0.25;
            n += ValueNoise(uv * scale / 4.0) * 0.5;
            return n / 0.875;
        }

        Varyings Vert(Attributes input)
        {
            Varyings output;
            float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
            output.positionCS = TransformWorldToHClip(positionWS);
            output.positionWS = positionWS.xy;
            output.uv = TRANSFORM_TEX(input.uv, _MainTex);
            output.color = input.color;
            return output;
        }

        half4 Frag(Varyings input) : SV_Target
        {
            half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color * _Color;

            // The old graph used Scale 40 on a different input; a quarter of it in world units gives a similar grain.
            float n = SimpleNoise(input.positionWS, _Scale * 0.25);
            half visible = step(n, _Fade);
            // Bright rim just inside the dissolving edge (only while dissolving).
            half edge = (_Fade < 0.999h) ? visible - step(n, _Fade - _EdgeWidth) : 0;

            half4 color = tex;
            color.rgb = lerp(color.rgb, _EdgeColor.rgb, edge);
            color.a *= visible;
            return color;
        }
        ENDHLSL

        Pass
        {
            Name "Sprite Dissolve 2D"
            Tags { "LightMode" = "Universal2D" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }

        Pass
        {
            Name "Sprite Dissolve Forward"
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }

    Fallback Off
}
