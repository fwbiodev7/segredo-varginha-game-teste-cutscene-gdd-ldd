Shader "Varginha/PausePixelBlur"
{
    Properties { _MainTex("Frozen world",2D)="white"{} }
    SubShader {
        Cull Off ZWrite Off ZTest Always
        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex; float4 _MainTex_TexelSize;
            fixed4 frag(v2f_img i):SV_Target {
                float2 uv=(floor(i.uv*float2(160,90))+.5)/float2(160,90);
                float2 d=_MainTex_TexelSize.xy*1.5;
                fixed4 c=tex2D(_MainTex,uv)*.4;
                c+=(tex2D(_MainTex,uv+float2(d.x,0))+tex2D(_MainTex,uv-float2(d.x,0))
                   +tex2D(_MainTex,uv+float2(0,d.y))+tex2D(_MainTex,uv-float2(0,d.y)))*.15;
                return c;
            }
            ENDCG
        }
    }
}
