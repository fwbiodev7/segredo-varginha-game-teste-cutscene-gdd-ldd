using System;
using Game.UI;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignHudIcons
    {
        public enum Icon { Notebook, Backpack, Pause }
        [Serializable] private class Entry { public string id; public int[] rect; }
        [Serializable] private class Atlas { public int width, height; public Entry[] icons; }
        private static Texture2D _texture;
        private static Atlas _atlas;

        public static bool Button(float x, Icon icon, string key, string description)
        {
            if (_texture == null) _texture = Resources.Load<Texture2D>("Varginha/Interface/StoryHudIcons");
            if (_atlas == null)
            {
                var json = Resources.Load<TextAsset>("Varginha/Interface/StoryHudIcons");
                if (json != null) _atlas = JsonUtility.FromJson<Atlas>(json.text);
            }
            var rect = new Rect(x, 24, 72, 76);
            bool hover = GUI.enabled && rect.Contains(Event.current.mousePosition);
            PixelHUDFrame.Draw(rect, Texture2D.whiteTexture,
                hover ? new Color(.10f, .15f, .17f, .98f) : PixelMenuTheme.Background,
                hover ? ExperimentGUI.Accent : PixelMenuTheme.Border);
            if (_texture != null && _atlas?.icons != null && (int)icon < _atlas.icons.Length)
            {
                int[] crop = _atlas.icons[(int)icon].rect;
                float scale = Mathf.Min(42f / crop[2], 43f / crop[3]);
                var draw = new Rect(rect.center.x - crop[2] * scale / 2, rect.y + 9 + (43 - crop[3] * scale) / 2,
                    crop[2] * scale, crop[3] * scale);
                var uv = new Rect((float)crop[0] / _atlas.width, 1f - (float)(crop[1] + crop[3]) / _atlas.height,
                    (float)crop[2] / _atlas.width, (float)crop[3] / _atlas.height);
                GUI.DrawTextureWithTexCoords(draw, _texture, uv);
            }
            PixelMenuTheme.Label(new Rect(x, 79, 72, 15), key, 8, PixelMenuTheme.Muted, TextAnchor.MiddleCenter);
            if (hover)
            {
                var tooltip = new Rect(Mathf.Min(x, 1100), 109, 146, 27);
                ExperimentGUI.Panel(tooltip);
                PixelMenuTheme.Label(tooltip, description, 9, PixelMenuTheme.Paper, TextAnchor.MiddleCenter);
            }
            return GUI.Button(rect, new GUIContent("", description), GUIStyle.none);
        }
    }
}
