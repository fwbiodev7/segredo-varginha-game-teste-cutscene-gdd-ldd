Shader "Varginha/IllustratedMorning"
{
    Properties{[PerRendererData]_MainTex("Artwork",2D)="white"{} _Color("Tint",Color)=(1,1,1,1)}
    SubShader
    {
        Tags{"Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="True"}
        Cull Off Lighting Off ZWrite Off Blend One OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;fixed4 _Color;
            struct Input{float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
            struct Output{float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
            Output vert(Input v){Output o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;return o;}
            fixed4 frag(Output i):SV_Target
            {
                fixed4 c=tex2D(_MainTex,i.uv)*i.color;
                // Lift painted night shadows only in the adult morning map; no source pixel changes.
                c.rgb=pow(saturate(c.rgb),fixed3(.86,.86,.88));c.rgb*=c.a;return c;
            }
            ENDCG
        }
    }
}
