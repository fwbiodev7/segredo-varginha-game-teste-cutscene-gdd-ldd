Shader "Varginha/LightweightHeadlight"
{
 SubShader {Tags {"Queue"="Transparent" "RenderType"="Transparent"}Cull Off ZWrite Off Blend SrcAlpha One
 Pass {
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;};struct v2f{float4 position:SV_POSITION;float2 uv:TEXCOORD0;};
 v2f vert(appdata v){v2f o;o.position=UnityObjectToClipPos(v.vertex);o.uv=v.uv;return o;}
 fixed4 frag(v2f i):SV_Target {float2 p=floor(i.uv*64)/64;float edge=saturate((1-abs(p.x*2-1))*1.4);float a=edge*pow(1-p.y,1.5)*.27;return fixed4(1,.82,.46,a);}
 ENDCG
 }
 }
}
