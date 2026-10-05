Shader "Varginha/ContactShadow"
{
    Properties { [PerRendererData] _MainTex("Sprite",2D)="white"{} _Color("Tint",Color)=(1,1,1,1) }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off Lighting Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            fixed4 _Color;
            struct input { float4 vertex:POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            struct output { float4 position:SV_POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
            output vert(input v) { output o; o.position=UnityObjectToClipPos(v.vertex); o.uv=v.uv; o.color=v.color*_Color; return o; }
            fixed4 frag(output i):SV_Target {
                float2 p=(floor(i.uv*24)+.5)/24*2-1;
                float alpha=saturate((1-dot(p,p))*3);
                return fixed4(i.color.rgb,i.color.a*alpha);
            }
            ENDCG
        }
    }
}
