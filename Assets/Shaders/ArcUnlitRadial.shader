Shader "Custom/URP_ArcUnlitRadial"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color Tint", Color) = (1,1,1,1)
        _UVTiling("UV Tiling and Offset", Vector) = (1,1,0,0)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _Color;
            float4 _UVTiling;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv * _UVTiling.xy + _UVTiling.zw;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                return texColor * _Color;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
