using System.Collections.Generic;
using Game.Varginha;
using UnityEngine;

namespace Game.UI
{
    public sealed class PixelButtonHoverState
    {
        public const float Duration=.2f;
        public float Amount { get; private set; }
        public void Advance(bool focused,float delta,bool enabled=true)
        {Amount=enabled?Mathf.MoveTowards(Amount,focused?1:0,Mathf.Max(0,delta)/Duration):0;}
    }

    // Cache by interface context and original hit rectangle, not by changing label text.
    public static class PixelButtonHover
    {
        private sealed class State { public readonly PixelButtonHoverState motion=new();public int frame=-1;public float seen; }
        private static readonly Dictionary<(string,int),State> States=new();
        private static readonly Dictionary<GUIStyle,GUIStyle> Styles=new();
        private static readonly List<(string,int)> Expired=new();
        private static float _pruneAt;
        public static readonly Color Accent=new(.64f,.84f,.86f);
        public static readonly Color FocusBackground=new(.14f,.23f,.27f,.98f);
        public const float TextStrength=.6f,FillAlpha=.12f,BorderAlpha=.85f;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset(){States.Clear();Styles.Clear();Expired.Clear();_pruneAt=0;}
        public static bool Focused(Rect rect)
        {
            bool selected=VarginhaGamepadUI.Selected(rect,true);
            return GUI.enabled&&(VarginhaGamepadUI.PointerActive?rect.Contains(Event.current.mousePosition):selected);
        }
        public static float Amount(Rect rect,bool focused)
        {
            var key=(VarginhaGamepadUI.CurrentContext??"default",rect.GetHashCode()^GUI.matrix.GetHashCode());
            if(!States.TryGetValue(key,out var state))States[key]=state=new State();
            state.seen=Time.unscaledTime;
            if(Event.current.type==EventType.Repaint&&state.frame!=Time.frameCount)
            {state.frame=Time.frameCount;state.motion.Advance(focused,Time.unscaledDeltaTime,GUI.enabled);}
            if(Time.unscaledTime>=_pruneAt)
            {
                _pruneAt=Time.unscaledTime+3;Expired.Clear();
                foreach(var item in States)if(Time.unscaledTime-item.Value.seen>5)Expired.Add(item.Key);
                foreach(var expired in Expired)States.Remove(expired);
            }
            return GUI.enabled?state.motion.Amount:0;
        }
        public static GUIStyle SmoothStyle(GUIStyle source,float amount)
        {
            source??=GUI.skin.button;
            if(!Styles.TryGetValue(source,out var style))Styles[source]=style=new GUIStyle(source);
            // Existing screens resize their cached styles; follow those changes without reallocating.
            style.font=source.font;style.fontSize=source.fontSize;style.fontStyle=source.fontStyle;
            style.alignment=source.alignment;style.wordWrap=source.wordWrap;style.clipping=source.clipping;
            style.padding=source.padding;style.margin=source.margin;style.border=source.border;
            style.fixedWidth=source.fixedWidth;style.fixedHeight=source.fixedHeight;
            style.stretchWidth=source.stretchWidth;style.stretchHeight=source.stretchHeight;
            Color color=Color.Lerp(source.normal.textColor,Color.white,amount*TextStrength);
            style.normal.textColor=style.hover.textColor=style.focused.textColor=color;
            style.normal.background=style.hover.background=style.focused.background=source.normal.background;
            style.active.textColor=source.active.textColor;style.active.background=source.active.background;
            return style;
        }
        public static void Decorate(Rect rect,float amount)
        {
            if(amount<=0)return;
            var before=GUI.color;
            GUI.color=new Color(Accent.r,Accent.g,Accent.b,amount*FillAlpha);
            GUI.DrawTexture(new Rect(rect.x+1,rect.y+1,rect.width-2,rect.height-2),Texture2D.whiteTexture);
            GUI.color=new Color(Accent.r,Accent.g,Accent.b,amount*BorderAlpha);
            GUI.DrawTexture(new Rect(rect.x,rect.y,rect.width,1),Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x,rect.yMax-1,rect.width,1),Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x,rect.y,1,rect.height),Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax-1,rect.y,1,rect.height),Texture2D.whiteTexture);
            GUI.color=before;
        }
    }
}
