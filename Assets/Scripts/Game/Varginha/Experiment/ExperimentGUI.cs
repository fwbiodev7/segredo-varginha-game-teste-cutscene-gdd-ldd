using Game.UI;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    internal static class ExperimentGUI
    {
        public static readonly Color Ink = new(.025f, .045f, .065f);
        public static readonly Color Paper = new(.91f, .87f, .73f);
        public static readonly Color Muted = new(.57f, .68f, .66f);
        public static readonly Color Accent = new(.43f, .77f, .67f);
        private static Texture2D _pixel;
        private static GUIStyle _title, _text, _small, _button;
        public static void Init()
        {
            if (_pixel == null) { _pixel = new Texture2D(1, 1); _pixel.SetPixel(0, 0, Color.white); _pixel.Apply(); }
            if (_title != null) return;
            _title = new GUIStyle(GUI.skin.label) { fontSize = 26, fontStyle = FontStyle.Normal, wordWrap = true };
            _text = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = true };
            _small = new GUIStyle(_text) { fontSize = 13 };
            _button = new GUIStyle(GUI.skin.button) { fontSize = 15, wordWrap = true };
            foreach (var style in new[] { _title, _text, _small, _button }) PixelUIFont.Apply(style);
            _title.normal.textColor = _text.normal.textColor = _button.normal.textColor = Paper;
            _small.normal.textColor = Muted;
        }
        public static void Box(Rect rect, Color color)
        { var before = GUI.color; GUI.color = color; GUI.DrawTexture(rect, _pixel); GUI.color = before; }
        public static void Panel(Rect rect)
        {
            PixelMenuTheme.Panel(rect);
        }
        public static void Label(Rect rect, string text, bool title = false, bool small = false)
        {
            var style = title ? _title : small ? _small : _text;
            int original = style.fontSize;
            var content = new GUIContent(VarginhaInputActions.Prompt(text));
            while (style.fontSize > 10 && style.CalcHeight(content, rect.width) > rect.height) style.fontSize--;
            GUI.Label(rect, content, style); style.fontSize = original;
        }
        public static bool Button(Rect rect, string text) => PixelMenuTheme.Button(rect, text, size: 13);
        public static bool Choice(Rect rect,string text,bool selected)
        {
            bool clicked=Button(rect,(selected?"• ":"")+text);
            if(selected)Box(new Rect(rect.x,rect.y,3,rect.height),Accent);
            return clicked;
        }
        public static void Objective(string chapter, string title, string goal,CampaignHudState state=null)
        {
            if(goal!=null&&goal.Length>105){int end=goal.LastIndexOf(' ',102);goal=goal.Substring(0,end>0?end:102)+"…";}
            float alpha=state?.IdentificationAlpha??1;
            var before=GUI.color;
            if(alpha>0)
            {
                GUI.color=new Color(before.r,before.g,before.b,before.a*alpha);
                Ribbon(new Rect(28,24,480,106));
                PixelMenuTheme.Label(new Rect(44,34,448,18),chapter,9,Muted);
                PixelMenuTheme.Label(new Rect(44,57,448,25),title,14,Paper);
                Label(new Rect(44,89,448,35),goal,small:true);
            }
            else if(state!=null&&state.GoalAlpha>0)
            {
                GUI.color=new Color(before.r,before.g,before.b,before.a*state.GoalAlpha);
                Ribbon(new Rect(28,24,440,62));
                Label(new Rect(44,36,408,42),goal,small:true);
            }
            GUI.color=before;
        }
        public static void Ribbon(Rect rect)
        {
            // Small flat surfaces leave the scene visible; pixel type and palette stay original.
            var before=GUI.color;
            Box(rect,new Color(before.r*.025f,before.g*.045f,before.b*.055f,before.a*.86f));
            Box(new Rect(rect.x,rect.y,2,rect.height),new Color(Accent.r,Accent.g,Accent.b,before.a*.7f));
        }
        public static void ContextPrompt(string interaction,string tutorial=null,CampaignHudState state=null)
        {
            bool training=string.IsNullOrEmpty(interaction);
            string text=training?tutorial:interaction;
            float alpha=training?(state?.TutorialAlpha??0):1;
            if(string.IsNullOrEmpty(text)||alpha<=0)return;
            text=VarginhaInputActions.Prompt(text);
            float width=Mathf.Clamp(text.Length*8+36,220,760);
            var before=GUI.color;GUI.color=new Color(before.r,before.g,before.b,before.a*alpha);
            Ribbon(new Rect(640-width/2,653,width,38));
            Label(new Rect(656-width/2,663,width-32,25),text,small:true);
            GUI.color=before;
        }
        // Shared pause navigation keeps every chapter in the same visual language.
        public static int PausePanel(string chapter)
        {
            CampaignCinematics.DrawPauseBackdrop(); Panel(new Rect(390, 140, 500, 440));
            PixelMenuTheme.Label(new Rect(430, 174, 420, 24), "INVESTIGAÇÃO EM PAUSA", 9, Muted, TextAnchor.MiddleCenter);
            PixelMenuTheme.Label(new Rect(430, 218, 420, 42), "PAUSADO", 25, Paper, TextAnchor.MiddleCenter);
            PixelMenuTheme.Label(new Rect(430, 274, 420, 32), chapter, 10, Muted, TextAnchor.MiddleCenter);
            int action = 0;
            if (Button(new Rect(430, 325, 420, 48), "CONTINUAR")) action = 1;
            if (Button(new Rect(430, 390, 420, 48), "CONFIGURAÇÕES")) action = 2;
            if (Button(new Rect(430, 455, 420, 48), "SALVAR E VOLTAR AO MENU")) action = 3;
            PixelMenuTheme.Label(new Rect(430, 530, 420, 22), VarginhaInputActions.PauseLabel+" • VOLTAR À HISTÓRIA", 9, Muted, TextAnchor.MiddleCenter);
            return action;
        }
        public static void Caption(Rect rect, string text, int size)
        {
            int before = _text.fontSize; _text.fontSize = size == 0 ? 16 : size == 2 ? 24 : 19;
            GUI.Label(rect, text, _text); _text.fontSize = before;
        }
        public static void Character(Rect rect, int cell)
        {
            var texture = VarginhaExperimentArt.Load("GameCast");
            if (texture == null) return;
            float side = Mathf.Min(rect.width, rect.height);
            var area = new Rect(rect.center.x - side / 2, rect.center.y - side / 2, side, side);
            GUI.DrawTextureWithTexCoords(area, texture, VarginhaExperimentArt.CellUV(cell));
        }
        public static Matrix4x4 BeginCanvas()
        {
            Matrix4x4 before = GUI.matrix;
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width - 1280 * scale) / 2, (Screen.height - 720 * scale) / 2),
                Quaternion.identity, Vector3.one * scale);
            return before;
        }
    }
}
