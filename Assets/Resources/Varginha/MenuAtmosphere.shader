Shader "Varginha/MenuAtmosphere"
{
    Properties
    {
        _MainTex("Original menu art", 2D) = "white" {}
        [NoScaleOffset] _UfoClean("Sky without saucer", 2D) = "black" {}
        [NoScaleOffset] _UfoSprite("Transparent saucer", 2D) = "black" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex, _UfoClean, _UfoSprite;
            float4 _MainTex_TexelSize;
            float _MenuTime, _Motion, _Gaze, _Interference;
            float _UfoLayered, _UfoPresence, _UfoTrail;
            float4 _UfoOffset;
            float Hash(float2 p) { return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453); }
            float Box(float2 p,float4 r) { return step(r.x,p.x)*step(p.x,r.z)*step(r.y,p.y)*step(p.y,r.w); }
            float Band(float v,float lo,float hi,float edge) { return smoothstep(lo,lo+edge,v)*(1-smoothstep(hi-edge,hi,v)); }
            float2 Pixel(float2 uv) { return (floor(uv*_MainTex_TexelSize.zw)+.5)*_MainTex_TexelSize.xy; }
            half4 Saucer(float2 uv,float2 offset)
            {
                float2 spriteUV=(uv-float2(.800,.850)-offset)/float2(144.0/1672,54.0/941)+.5;
                if(any(spriteUV<0)||any(spriteUV>1))return 0;
                return tex2D(_UfoSprite,spriteUV);
            }
            half4 frag(v2f_img input):SV_Target
            {
                float2 uv=Pixel(input.uv), size=_MainTex_TexelSize.xy;
                half4 original=tex2D(_MainTex,uv);
                if(_Motion<.001)return original;
                // The clean plate already preserves the scene outside the repaired sky.
                // Sample it directly so neither the baked halo nor an oval blend survives departure.
                if(_UfoLayered>.5)original=tex2D(_UfoClean,uv);
                float t=_MenuTime;
                // Masks are measured on the existing 1672 x 941 composition, in bottom-left UV.
                // All displacements are integral source pixels; no filtering or texture synthesis.
                float head=Box(uv,float4(.864,.375,.891,.419));
                float body=Box(uv,float4(.861,.304,.907,.383));
                float foliage=Band(uv.y,.015,.33,.015)*(step(uv.x,.58)+step(.916,uv.x));
                foliage*=step(original.r*1.16,original.g)*step(.035,original.g);
                float clouds=Band(uv.y,.53,.985,.025)*step(uv.x,.79)*step(.027,original.b);
                clouds*=1-Box(uv,float4(.74,.76,.85,.884))*(1-_UfoLayered);
                foliage*=1-Box(uv,float4(.803,.073,.988,.293));
                float2 displaced=uv;
                displaced.x+=size.x*round(sin(t*.28+uv.y*13)*7)*clouds;
                displaced.x+=size.x*round(sin(t*1.1+uv.y*19)*2.5)*foliage;
                float saucerMotion=Band(uv.x,.754,.844,.017)*Band(uv.y,.792,.906,.025);
                displaced.x+=size.x*round(sin(t*.68)*3)*saucerMotion*(1-_UfoLayered);
                displaced.y+=size.y*round(sin(t*.9)*2)*saucerMotion*(1-_UfoLayered);
                // The existing silhouette breathes; no new character frames are painted.
                displaced.y+=size.y*round(sin(t*1.6)*1.5)*body;
                displaced.x-=size.x*floor(_Gaze*3.9)*head;
                displaced.y+=size.y*round(sin(t*.65)*1.2)*head;
                half4 color=_UfoLayered>.5?tex2D(_UfoClean,Pixel(displaced)):tex2D(_MainTex,Pixel(displaced));
                float green=step(color.r*1.35,color.g)*step(color.b*.85,color.g);
                float ufo=Box(uv,float4(.74,.76,.85,.884));
                float pulse=.36*sin(t*.95)+_Interference*.24*sin(t*79);
                color.rgb*=1+ufo*green*pulse*(1-_UfoLayered);
                float presence=_UfoLayered>.5?_UfoPresence:1;
                float beam=Band(uv.x,.745,.926,.025)*Band(uv.y,.275,.865,.025)*green;
                // With a clean sky, departure removes the dynamic beam; it must not
                // blacken green cloud pixels or cut rectangular holes around the alien.
                color.rgb*=1+beam*(.18*sin(t*.85+uv.y*3)+_Interference*.12*sin(t*67))*presence;
                float beamDepth=saturate((.85-uv.y)/.55);
                float beamCenter=.80+beamDepth*.067+sin(t*.7+beamDepth*2)*.006+_UfoOffset.x*(1-beamDepth);
                float beamWidth=.016+beamDepth*.061;
                float shaft=(1-smoothstep(beamWidth*.45,beamWidth,abs(uv.x-beamCenter)))
                    *Band(uv.y,.295,.85,.04)*(1-head)*(1-body);
                float rays=.65+.35*sin((uv.x-beamCenter)*240+t*.8+uv.y*14);
                color.rgb+=float3(.026,.15,.076)*shaft*rays*(.18+.12*sin(t*.95))*presence;
                float eyes=head*step(color.g*1.2,color.r)*step(.065,color.r);
                color.rgb*=1+eyes*(.38+.3*sin(t*1.1)+_Gaze*.55);
                float city=Box(uv,float4(.35,.32,.83,.465))*step(color.g*1.2,color.r)*step(.12,color.r);
                float lamp=Hash(floor(uv/size/float2(5,3)));
                color.rgb*=1-city*.45*step(.965,sin(t*(.55+lamp)+lamp*43));
                float stars=Band(uv.y,.78,.99,.01)*step(.1,color.g)*step(uv.x,.74);
                color.rgb*=1+stars*.48*sin(t*1.35+Hash(floor(uv/size))*45);
                float puddles=Box(uv,float4(.32,.004,.79,.205))*green*step(.06,color.g);
                color.rgb*=1+puddles*.4*sin(t*1.6+floor(uv.y/size.y)*.22);
                // Two sparse, drifting fog layers; vehicle and alien silhouettes stay intact.
                float fog=Band(uv.y,.25,.5,.08)*(1-head)*(1-body)*(.25+.75*step(.025,max(original.g,original.b)));
                float mist=sin(uv.x*18+t*.42+sin(uv.y*39+t*.17))*.5+.5;
                mist*=sin(uv.x*31-t*.65+uv.y*17)*.5+.5;
                color.rgb+=float3(.12,.21,.23)*fog*mist*.72;
                float foreground=Band(uv.y,.12,.29,.06)*step(uv.x,.8);
                color.rgb+=float3(.11,.18,.19)*foreground*(sin(uv.x*23+t*.72+uv.y*19)*.5+.5)*.16;
                // A handful of low-contrast dust pixels, quantized to the original pixel grid.
                float2 cell=floor((uv+float2(t*.002,t*.0012))/size/2);
                float dust=step(.9997,Hash(cell))*Band(uv.y,.14,.7,.05);
                color.rgb+=dust*float3(.09,.15,.14);
                if(_UfoLayered>.5&&presence>.001)
                {
                    half4 saucer=Saucer(uv,_UfoOffset.xy);
                    color.rgb=lerp(color.rgb,saucer.rgb*(1+pulse),saucer.a*presence);
                    if(_UfoTrail>.01)
                    {
                        for(int i=1;i<=4;i++)
                        {
                            half4 trail=Saucer(uv,_UfoOffset.xy-float2(.015,.0058)*i);
                            color.rgb+=trail.rgb*trail.a*(.16/i)*_UfoTrail;
                        }
                    }
                }
                float vignette=saturate(dot((uv-.5)*float2(1,.8),(uv-.5)*float2(1,.8))-.15);
                color.rgb*=1-vignette*.035;
                color.rgb*=1+.018*sin(t*.25);
                return half4(color.rgb,original.a);
            }
            ENDHLSL
        }
    }
}
