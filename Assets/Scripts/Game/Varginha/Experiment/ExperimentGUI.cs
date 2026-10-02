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
            _title = new GUIStyle(GUI.skin.label) { fontSize = 26, fontStyle = FontStyle.Bold, wordWrap = true };
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
            Box(rect, Ink); Box(new Rect(rect.x, rect.y, rect.width, 2), Accent);
            Box(new Rect(rect.x, rect.yMax - 2, rect.width, 2), new Color(.18f, .28f, .29f));
        }
        public static void Label(Rect rect, string text, bool title = false, bool small = false)
            => GUI.Label(rect, text, title ? _title : small ? _small : _text);
        public static bool Button(Rect rect, string text) => GUI.Button(rect, text, _button);
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
