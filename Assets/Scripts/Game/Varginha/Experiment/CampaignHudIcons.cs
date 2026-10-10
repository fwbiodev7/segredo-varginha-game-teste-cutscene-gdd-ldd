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
            key=KeyLabel(icon);
            if (_texture == null) _texture = Resources.Load<Texture2D>("Varginha/Interface/StoryHudIcons");
            if (_atlas == null)
            {
                var json = Resources.Load<TextAsset>("Varginha/Interface/StoryHudIcons");
                if (json != null) _atlas = JsonUtility.FromJson<Atlas>(json.text);
            }
            // Existing callers pass the old anchors; map them to a compact, non-overlapping row.
            x=1120+(int)icon*48;
            var rect = new Rect(x, 24, 44, 48);
            bool hover = GUI.enabled && rect.Contains(Event.current.mousePosition);
            ExperimentGUI.Box(rect,hover?new Color(.08f,.13f,.14f,.92f):new Color(.02f,.04f,.05f,.65f));
            if (_texture != null && _atlas?.icons != null && (int)icon < _atlas.icons.Length)
            {
                int[] crop = _atlas.icons[(int)icon].rect;
                float scale = Mathf.Min(26f / crop[2], 27f / crop[3]);
                var draw = new Rect(rect.center.x - crop[2] * scale / 2, rect.y + 4 + (27 - crop[3] * scale) / 2,
                    crop[2] * scale, crop[3] * scale);
                var uv = new Rect((float)crop[0] / _atlas.width, 1f - (float)(crop[1] + crop[3]) / _atlas.height,
                    (float)crop[2] / _atlas.width, (float)crop[3] / _atlas.height);
                GUI.DrawTextureWithTexCoords(draw, _texture, uv);
            }
            PixelMenuTheme.Label(new Rect(x, 57, 44, 12), key, 7, PixelMenuTheme.Muted, TextAnchor.MiddleCenter);
            if (hover)
            {
                var tooltip = new Rect(Mathf.Min(x, 1100), 79, 146, 27);
                ExperimentGUI.Panel(tooltip);
                PixelMenuTheme.Label(tooltip, description, 9, PixelMenuTheme.Paper, TextAnchor.MiddleCenter);
            }
            return VarginhaGamepadUI.Button(rect, new GUIContent("", description), GUIStyle.none);
        }
        public static string KeyLabel(Icon icon) => icon switch
        {
            Icon.Notebook=>VarginhaInputActions.JournalLabel,
            Icon.Backpack=>VarginhaInputActions.InventoryLabel,
            _=>VarginhaInputActions.PauseLabel
        };
    }
}
