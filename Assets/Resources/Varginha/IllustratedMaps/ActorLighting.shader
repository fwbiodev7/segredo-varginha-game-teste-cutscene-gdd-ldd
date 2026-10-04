Shader "Varginha/IllustratedActorLighting"
{
 Properties { [PerRendererData] _MainTex("Sprite",2D)="white"{} _Color("Color",Color)=(1,1,1,1) _SceneTint("Scene light",Color)=(1,1,1,1) _KeyLight("Direction",Vector)=(-.3,.7,.5,0) _Shine("Volume",Float)=.08 }
 SubShader {
 Tags {"Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="True"}
 Cull Off Lighting Off ZWrite Off Blend One OneMinusSrcAlpha
 Pass {
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_instancing
 #pragma multi_compile _ PIXELSNAP_ON
 #include "UnitySprites.cginc"
 float4 _SceneTint,_KeyLight;float _Shine,_GrayBag,_BagUVYMin;float4 _MainTex_TexelSize;
 v2f vert(appdata_t IN){v2f OUT;UNITY_SETUP_INSTANCE_ID(IN);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);OUT.vertex=UnityObjectToClipPos(UnityFlipSprite(IN.vertex,_Flip));OUT.texcoord=IN.texcoord;OUT.color=IN.color*_Color*_RendererColor;return OUT;}
 fixed4 frag(v2f IN):SV_Target{
 fixed4 c=SampleSpriteTexture(IN.texcoord)*IN.color;
 // Legacy equipped action sheets retain their artwork; only upper-body navy bag fabric changes.
 if(_GrayBag>.5 && IN.texcoord.y>_BagUVYMin && c.b>c.r*1.15 && c.g>c.r*1.05 && c.r<.3){float gray=dot(c.rgb,float3(.3,.59,.11));c.rgb=gray.xxx*1.6;}
 float2 dx=float2(_MainTex_TexelSize.x,0),dy=float2(0,_MainTex_TexelSize.y);
 float a=SampleSpriteTexture(IN.texcoord+dx).a-SampleSpriteTexture(IN.texcoord-dx).a;
 float b=SampleSpriteTexture(IN.texcoord+dy).a-SampleSpriteTexture(IN.texcoord-dy).a;
 float bevel=-dot(float2(a,b),_KeyLight.xy)*_Shine*_KeyLight.z;
 c.rgb=saturate(c.rgb*_SceneTint.rgb+bevel);c.rgb*=c.a;return c;}
 ENDCG
 }
 }
}
