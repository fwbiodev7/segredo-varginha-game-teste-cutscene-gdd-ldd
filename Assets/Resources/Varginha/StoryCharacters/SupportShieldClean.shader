Shader "Varginha/SupportShieldClean"
{
 Properties { [PerRendererData] _MainTex ("Sprite",2D)="white"{} _Color ("Tint",Color)=(1,1,1,1) }
 SubShader
 {
  Tags { "Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
  Cull Off Lighting Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
  Pass
  {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   sampler2D _MainTex;float4 _MainTex_TexelSize;fixed4 _Color;
   struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 colour:COLOR;};
   struct v2f {float4 position:SV_POSITION;float2 uv:TEXCOORD0;fixed4 colour:COLOR;};
   v2f vert(appdata v){v2f o;o.position=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.colour=v.colour*_Color;return o;}
   fixed4 frag(v2f i):SV_Target
   {
    fixed4 c=tex2D(_MainTex,i.uv);float2 d=_MainTex_TexelSize.xy*2;
    float neighbour=min(min(tex2D(_MainTex,i.uv+float2(d.x,0)).a,tex2D(_MainTex,i.uv-float2(d.x,0)).a),min(tex2D(_MainTex,i.uv+float2(0,d.y)).a,tex2D(_MainTex,i.uv-float2(0,d.y)).a));
    float edge=saturate((c.a-neighbour)*3);c.rgb*=i.colour.rgb;c.a*=i.colour.a*lerp(.13f,.68f,edge);return c;
   }
   ENDCG
  }
 }
}
