using Game.UI;
using UnityEngine;

namespace Game.Varginha
{
    // One cached state per main-menu button, advanced only on Repaint (IMGUI has multiple passes).
    public sealed class MenuButtonMotion
    {
        public const float FocusDuration=.2f, SubtitleDuration=.15f, GlitchDuration=.1f;
        private float _amount,_subtitle,_focusAt=-10,_glitchAt=-10,_nextGlitch;
        private int _frame=-1;
        private bool _focused;
        private readonly PixelButtonHoverState _hover=new();
        public float Amount => _amount;
        public float Subtitle => _subtitle;
        public bool Draw(Rect rect,string text,MenuCinematicAtmosphere atmosphere,bool reduced,int size=12)
        {
            bool selected=VarginhaGamepadUI.Selected(rect,true);
            bool focus=GUI.enabled&&(atmosphere.PointerActive?rect.Contains(Event.current.mousePosition):selected);
            float now=Time.unscaledTime;
            if(Event.current.type==EventType.Repaint && _frame!=Time.frameCount)
            {
                _frame=Time.frameCount;
                if(focus&&!_focused){_focusAt=now;_nextGlitch=now+13+rect.y*.013f;atmosphere.Navigate();}
                _focused=focus;
                _hover.Advance(focus,Time.unscaledDeltaTime,GUI.enabled);_amount=_hover.Amount;
                _subtitle=Mathf.MoveTowards(_subtitle,focus?1:0,Time.unscaledDeltaTime/SubtitleDuration);
                if(focus&&!reduced&&now>=_nextGlitch){_glitchAt=now;_nextGlitch=now+19+rect.y*.01f;}
            }
            Color accent=new(.64f,.84f,.86f);
            PixelHUDFrame.Draw(rect,Texture2D.whiteTexture,
                Color.Lerp(PixelMenuTheme.Background,PixelButtonHover.FocusBackground,_amount),
                Color.Lerp(PixelMenuTheme.Border,accent,_amount));
            var markerColor=Color.Lerp(PixelMenuTheme.Muted,accent,_amount);
            if(!reduced)markerColor.a*=1-_amount*.22f*(.5f+.5f*Mathf.Sin(now*3));
            PixelMenuTheme.Label(new Rect(rect.x+14+Mathf.Round(4*_amount),rect.y,26,rect.height),"›",9,markerColor);
            float glitch=!reduced&&focus&&now-_glitchAt<GlitchDuration?1:0;
            PixelMenuTheme.Label(new Rect(rect.x+48+glitch,rect.y,rect.width-60,rect.height),text,size,
                Color.Lerp(PixelMenuTheme.Paper,Color.white,_amount*PixelButtonHover.TextStrength));
            if(focus&&!reduced&&now-_focusAt<.8f)DrawSweep(rect,Mathf.Clamp01((now-_focusAt)/.8f),accent);
            // Register the original hit rectangle; visual motion never changes the target.
            return VarginhaGamepadUI.Button(rect,GUIContent.none,GUIStyle.none,false);
        }
        private static void DrawSweep(Rect rect,float progress,Color color)
        {
            float perimeter=2*(rect.width+rect.height), head=progress*perimeter;
            var before=GUI.color;color.a=.7f;GUI.color=color;
            for(int i=0;i<40;i++)
            {
                float d=head-i*2;if(d<0)continue;
                Vector2 point=d<rect.width?new Vector2(rect.x+d,rect.y):
                    d<rect.width+rect.height?new Vector2(rect.xMax,rect.y+d-rect.width):
                    d<2*rect.width+rect.height?new Vector2(rect.xMax-(d-rect.width-rect.height),rect.yMax):
                    new Vector2(rect.x,rect.yMax-(d-2*rect.width-rect.height));
                GUI.DrawTexture(new Rect(Mathf.Round(point.x),Mathf.Round(point.y),2,2),Texture2D.whiteTexture);
            }
            GUI.color=before;
        }
    }
}
