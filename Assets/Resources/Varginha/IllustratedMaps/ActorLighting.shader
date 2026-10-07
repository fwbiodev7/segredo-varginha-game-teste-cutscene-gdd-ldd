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
 float4 _SceneTint,_KeyLight,_ActorBounds;float _Shine,_GrayBag,_BagUVYMin;float4 _MainTex_TexelSize;
 struct lit_v2f {float4 vertex:SV_POSITION;float2 texcoord:TEXCOORD0;fixed4 color:COLOR;float2 world:TEXCOORD1;float4 axes:TEXCOORD2;UNITY_VERTEX_OUTPUT_STEREO};
 lit_v2f vert(appdata_t IN){lit_v2f OUT;UNITY_SETUP_INSTANCE_ID(IN);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);float4 local=UnityFlipSprite(IN.vertex,_Flip);OUT.vertex=UnityObjectToClipPos(local);OUT.world=mul(unity_ObjectToWorld,local).xy;OUT.axes=float4(UnityObjectToWorldDir(float3(_Flip.x,0,0)).xy,UnityObjectToWorldDir(float3(0,_Flip.y,0)).xy);OUT.texcoord=IN.texcoord;OUT.color=IN.color*_Color*_RendererColor;return OUT;}
 fixed4 frag(lit_v2f IN):SV_Target{
 fixed4 c=SampleSpriteTexture(IN.texcoord)*IN.color;
 // Legacy equipped action sheets retain their artwork; only upper-body navy bag fabric changes.
 if(_GrayBag>.5 && IN.texcoord.y>_BagUVYMin && c.b>c.r*1.15 && c.g>c.r*1.05 && c.r<.3){float gray=dot(c.rgb,float3(.3,.59,.11));c.rgb=gray.xxx*1.6;}
 float2 dx=float2(_MainTex_TexelSize.x,0),dy=float2(0,_MainTex_TexelSize.y);
 float a=SampleSpriteTexture(IN.texcoord+dx).a-SampleSpriteTexture(IN.texcoord-dx).a;
 float b=SampleSpriteTexture(IN.texcoord+dy).a-SampleSpriteTexture(IN.texcoord-dy).a;
 float bevel=-dot(a*IN.axes.xy+b*IN.axes.zw,_KeyLight.xy)*_Shine*_KeyLight.z;
 // Broad, restrained body volume in world space. Animation/atlas UV changes and
 // mirrored poses cannot reverse the light. Authored pixels remain point sampled.
 float2 body=clamp((IN.world-_ActorBounds.xy)/max(_ActorBounds.zw,float2(.01,.01))*2,-1,1);
 float volume=dot(body,float2(_KeyLight.x*.75,_KeyLight.y*.35))*min(.14,_KeyLight.w*.22);
 c.rgb=saturate(c.rgb*(_SceneTint.rgb+volume)+bevel);c.rgb*=c.a;return c;}
 ENDCG
 }
 }
}
