Shader "Varginha/CharacterShadow"
{
    Properties { [PerRendererData] _MainTex("Sprite",2D)="white"{} _Color("Tint",Color)=(1,1,1,1) }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        Cull Off Lighting Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex; fixed4 _Color; float4 _Ground, _Projection;
            struct input { float4 vertex:POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            struct output { float4 position:SV_POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            output vert(input v) {
                output o; float4 p=mul(unity_ObjectToWorld,v.vertex);
                float h=saturate((p.y-_Ground.y)/_Projection.w);
                p.xy=_Ground.xy+float2((p.x-_Ground.x)*.85,0)+_Projection.xy*h*_Projection.z;
                o.position=mul(UNITY_MATRIX_VP,p); o.uv=v.uv; o.color=v.color*_Color; return o;
            }
            fixed4 frag(output i):SV_Target { return fixed4(i.color.rgb,tex2D(_MainTex,i.uv).a*i.color.a); }
            ENDCG
        }
    }
}
