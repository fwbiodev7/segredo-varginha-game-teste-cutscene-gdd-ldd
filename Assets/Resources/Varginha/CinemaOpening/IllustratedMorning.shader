Shader "Varginha/IllustratedMorning"
{
    Properties{[PerRendererData]_MainTex("Artwork",2D)="white"{} _Color("Tint",Color)=(1,1,1,1) _ReferenceMap("Original map UV",Float)=1 _OfficeFloor("Repair original office floor",Float)=0}
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
            sampler2D _MainTex;fixed4 _Color;float _ReferenceMap,_OfficeFloor;
            float roundedBoxDistance(float2 p,float2 center,float2 halfSize,float radius)
            {
                float2 q=abs(p-center)-halfSize+radius;
                return length(max(q,0.0))+min(max(q.x,q.y),0.0)-radius;
            }
            struct Input{float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
            struct Output{float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
            Output vert(Input v){Output o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;return o;}
            fixed4 frag(Output i):SV_Target
            {
                fixed4 c=tex2D(_MainTex,i.uv)*i.color;
                float2 p=float2(i.uv.x*1889,(1-i.uv.y)*833);
                if(_OfficeFloor>.5)
                {
                    // Remove the old chair silhouette with a rounded edge while
                    // retaining enough margin to cover its legs and floor shadow.
                    float distance=roundedBoxDistance(p,float2(85,530),float2(32,38),18.0);
                    float coverage=1-smoothstep(-4.0,20.0,distance);
                    // Sample from nearby uncovered floor so its plank lighting blends
                    // with the repaired seat area instead of importing a distant patch.
                    fixed4 floor=tex2D(_MainTex,i.uv+float2(80.0/1889,0))*i.color;
                    c=lerp(c,floor,coverage);
                }
                bool yard=p.x>960;
                bool plant=p.x>25&&p.x<110&&p.y>280&&p.y<337;
                bool green=c.g>c.r*1.16&&c.g>c.b*1.12;
                if(green&&(_ReferenceMap<.5||yard||plant))
                {
                    // Match teal foliage to the tree's olive hue while retaining
                    // every luminance detail. Existing olive tree colors stay intact;
                    // there are no rectangular exclusion masks around their crowns.
                    float teal=saturate((c.b/max(c.g,.001)-.38)*4);
                    float cold=saturate((.69-c.r/max(c.g,.001))*4);
                    float blend=max(teal,cold)*.85;
                    c.r=lerp(c.r,c.g*.73,blend);
                    c.b=lerp(c.b,c.g*.38,blend);
                }
                // Lift painted night shadows only in the adult morning map; no source pixel changes.
                c.rgb=pow(saturate(c.rgb),fixed3(.86,.86,.88));c.rgb*=c.a;return c;
            }
            ENDCG
        }
    }
}
