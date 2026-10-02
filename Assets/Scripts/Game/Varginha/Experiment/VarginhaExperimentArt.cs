using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class VarginhaExperimentArt
    {
        private static readonly Dictionary<string, Texture2D> Textures = new();
        private static readonly Dictionary<int, Sprite> Bodies = new();
        private static Texture2D _map;
        public static Texture2D Load(string name)
        {
            if (Textures.TryGetValue(name, out var existing) && existing != null) return existing;
            var texture = Resources.Load<Texture2D>("Varginha/Experiment/" + name);
            if (texture != null) { texture.filterMode = FilterMode.Point; Textures[name] = texture; }
            return texture;
        }
        public static Rect CellUV(int index) => new(index % 2 * .5f, index < 2 ? .5f : 0, .5f, .5f);
        public static Sprite Body(int cell)
        {
            if (Bodies.TryGetValue(cell, out var sprite) && sprite != null) return sprite;
            var sheet = Load("GameCast");
            if (sheet == null) return null;
            int w = sheet.width / 2, h = sheet.height / 2;
            // Same 64-pixel logical cells as the existing characters; no collider rescaling.
            sprite = Sprite.Create(sheet, new Rect(cell % 2 * w, cell < 2 ? h : 0, w, h),
                new Vector2(.5f, .18f), h / 1.8f, 0, SpriteMeshType.FullRect);
            sprite.name = "Experiment_Character_" + cell;
            Bodies[cell] = sprite;
            return sprite;
        }
        public static Texture2D Map()
        {
            if (_map != null) return _map;
            const int w = 160, h = 112;
            _map = new Texture2D(w, h, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, name = "Mapa_1996_Recortes" };
            var pixels = new Color32[w * h];
            for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
            {
                int noise = (x * 13 + y * 7) % 11;
                pixels[y * w + x] = new Color32((byte)(184 + noise), (byte)(169 + noise), (byte)(120 + noise), 255);
            }
            void Dot(int x, int y, Color32 c) { if (x >= 0 && x < w && y >= 0 && y < h) pixels[y * w + x] = c; }
            var river = new Color32(61, 107, 115, 255);
            var road = new Color32(99, 68, 48, 255);
            for (int y = 0; y < h; y++)
            {
                int x = 103 + Mathf.RoundToInt(Mathf.Sin(y * .07f) * 17);
                for (int d = -4; d <= 4; d++) Dot(x + d, y, river);
            }
            for (int x = 0; x < w; x++)
            {
                int y = 27 + x / 3;
                for (int d = -1; d <= 1; d++) Dot(x, y + d, road);
            }
            for (int tree = 0; tree < 48; tree++)
            {
                int tx = (tree * 43 + 18) % w, ty = (tree * 29 + 6) % h;
                if (tx < 56 && ty > 60) continue;
                for (int dy = 0; dy < 6; dy++) for (int dx = -dy / 2; dx <= dy / 2; dx++)
                    Dot(tx + dx, ty + dy, new Color32(94, 113, 66, 255));
            }
            // Church landmark northwest and wooden bridge across the river.
            for (int y = 74; y < 88; y++) for (int x = 19; x < 34; x++) Dot(x, y, new Color32(218, 208, 175, 255));
            for (int y = 88; y < 92; y++) for (int x = 16; x < 37; x++) Dot(x, y, road);
            for (int y = 90; y < 98; y++) Dot(26, y, road);
            for (int x = 23; x < 30; x++) Dot(x, 95, road);
            for (int x = 97; x < 120; x++) for (int y = 57; y < 60; y++) Dot(x, y, road);
            _map.SetPixels32(pixels); _map.Apply();
            return _map;
        }
        // Piece IDs are deliberately scrambled; only the river/road continuity locates them.
        public static Rect MapUV(int piece)
        {
            int[] visualCell = { 1, 3, 0, 2 };
            return CellUV(visualCell[Mathf.Clamp(piece, 0, 3)]);
        }
    }
}
