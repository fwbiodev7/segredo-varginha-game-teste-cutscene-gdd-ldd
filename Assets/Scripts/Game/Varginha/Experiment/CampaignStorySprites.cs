using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Real alpha assets share one GPU texture per sheet; manifests avoid CPU-readable texture copies.
    public static class CampaignStorySprites
    {
        [Serializable] private class FrameData { public int[] rect; public float anchorX, lift; }
        [Serializable] private class Manifest { public int width, height, columns, rows; public float[] pixelsPerUnit; public FrameData[] frames; }
        private sealed class Sheet { public Texture2D texture; public Manifest data; public readonly Dictionary<int,Sprite> sprites = new(); }
        private static readonly Dictionary<string,Sheet> Sheets = new();
        public static Sprite Frame(string name, int row, int column, bool portrait = false, bool churchScale = false)
        {
            if (!Sheets.TryGetValue(name, out var sheet) || sheet.texture == null)
            {
                var texture = Resources.Load<Texture2D>("Varginha/StoryCharacters/" + name);
                var json = Resources.Load<TextAsset>("Varginha/StoryCharacters/" + name);
                if (texture == null || json == null) return null;
                sheet = new Sheet { texture = texture, data = JsonUtility.FromJson<Manifest>(json.text) }; Sheets[name] = sheet;
            }
            if (row < 0 || row >= sheet.data.rows || column < 0 || column >= sheet.data.columns) return null;
            bool reduced = churchScale && IsEdelzio(name);
            int index = row * sheet.data.columns + column, key = index + (portrait ? 10000 : 0) + (reduced ? 20000 : 0);
            if (sheet.sprites.TryGetValue(key, out var sprite) && sprite != null) return sprite;
            var f = sheet.data.frames[index]; int[] crop = f.rect;
            float sx = (float)sheet.texture.width/sheet.data.width, sy = (float)sheet.texture.height/sheet.data.height;
            var body = new Rect(crop[0]*sx, (sheet.data.height-crop[1]-crop[3])*sy, crop[2]*sx, crop[3]*sy);
            float anchorX = f.anchorX*sx;
            if (VarginhaCharacterFrameGeometry.TryBounds(sheet.texture,body,out var visible,out var centre))
            { body = visible; anchorX = centre; }
            var rect = body;
            if (portrait)
            {
                float height = Mathf.Ceil(body.height*.47f);
                rect.y = body.yMax-height; rect.height = height;
            }
            float ppu = sheet.data.pixelsPerUnit[row]*sy;
            if (reduced) ppu /= CampaignTeamEdelzio.ChurchVisualScale;
            var pivot = new Vector2((anchorX-rect.x)/rect.width, portrait ? .5f : (.58f-f.lift)*ppu/rect.height);
            sprite = VarginhaCharacterFrameGeometry.Create(sheet.texture, rect, pivot, ppu, 0, SpriteMeshType.FullRect);
            sprite.name = "Team_V4_" + name + "_" + row + "_" + column + (portrait ? "_Head" : "");
            sheet.sprites[key] = sprite; return sprite;
        }
        private static bool IsEdelzio(string name) => name == "WalkGray" || name == "Life" || name == "LifeGray"
            || name == "Punch" || name == "Seated" || name == "SeatedGray" || name == "SeatedDeskNorth" || name == "Driving";
        public static int EightDirection(Vector2 face)
        {
            float angle = Mathf.Atan2(face.x,-face.y)*Mathf.Rad2Deg;
            return ((Mathf.RoundToInt(-angle/45)+8)%8);
        }
        public static void Portrait(Rect rect, string sheet, int row = 0, int frame = 0)
        {
            var sprite = Frame(sheet,row,frame,true); if (sprite == null) return;
            float scale = Mathf.Min(rect.width/sprite.rect.width,rect.height/sprite.rect.height);
            var area = new Rect(rect.center.x-sprite.rect.width*scale/2, rect.center.y-sprite.rect.height*scale/2, sprite.rect.width*scale,sprite.rect.height*scale);
            GUI.DrawTextureWithTexCoords(area,sprite.texture,new Rect(sprite.rect.x/sprite.texture.width,sprite.rect.y/sprite.texture.height,sprite.rect.width/sprite.texture.width,sprite.rect.height/sprite.texture.height));
        }
    }
}
