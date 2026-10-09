Shader "Varginha/StainedGlassLighting"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _SceneTint("Scene light", Color) = (1,1,1,1)
        _GlassResponse("Glass response", Range(0,1)) = 1
        _KeyLight("Direction", Vector) = (0,0,0,0)
        _Shine("Volume", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" "CanUseSpriteAtlas"="True" }
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off ZWrite Off
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"
            struct Attributes { COMMON_2D_INPUTS half4 color : COLOR; };
            struct Varyings { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; half2 lightingUV:TEXCOORD1; float2 world:TEXCOORD2; half4 color:COLOR; UNITY_VERTEX_OUTPUT_STEREO };
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                half4 _Color, _SceneTint, _KeyLight, _ActorBounds;
                float _GlassResponse, _Shine, _GrayBag, _BagUVYMin;
            CBUFFER_END
            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input); UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.lightingUV = half2(ComputeScreenPos(output.positionCS / output.positionCS.w).xy);
                output.world = TransformObjectToWorld(input.positionOS).xy;
                output.uv = input.uv; output.color = input.color * _Color * unity_SpriteColor;
                return output;
            }
            half4 Frag(Varyings input):SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.uv) * input.color;
                if (_GrayBag>.5 && input.uv.y>_BagUVYMin && color.b>color.r*1.15 && color.g>color.r*1.05 && color.r<.3)
                    color.rgb = dot(color.rgb,half3(.3,.59,.11)).xxx*1.6;
                float2 body=clamp((input.world-_ActorBounds.xy)/max(_ActorBounds.zw,float2(.01,.01))*2,-1,1);
                float volume=dot(body,float2(_KeyLight.x*.75,_KeyLight.y*.35))*min(.14,_KeyLight.w*.22);
                color.rgb *= _SceneTint.rgb + volume;
                SurfaceData2D surface; InputData2D data;
                InitializeSurfaceData(color.rgb,color.a,half4(1,1,1,1),surface);
                InitializeInputData(input.uv,input.lightingUV,data);
                half4 lit = CombinedShapeLightShared(surface,data);
                return half4(lerp(color.rgb,lit.rgb,_GlassResponse),color.a);
            }
            ENDHLSL
        }
    }
}
