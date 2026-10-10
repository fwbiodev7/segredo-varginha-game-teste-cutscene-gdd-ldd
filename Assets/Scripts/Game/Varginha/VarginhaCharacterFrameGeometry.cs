using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha
{
    // Mesh metadata isolates alpha-connected artwork without copying or modifying any atlas.
    public static class VarginhaCharacterFrameGeometry
    {
        [Serializable] private sealed class Region { public int[] rect, bands; public int area; public float anchorX; }
        [Serializable] private sealed class Sheet { public string name, path; public int width, height; public Region[] regions; }
        [Serializable] private sealed class Manifest { public Sheet[] sheets; }
        private static Dictionary<string, List<Sheet>> _sheets;
        private static readonly Dictionary<Texture2D, Sheet> Textures = new();
        private sealed class PendingMesh { public Sprite sprite; public Vector2[] vertices; public ushort[] triangles; }
        private static readonly List<PendingMesh> Pending = new();
        private static readonly List<PendingMesh> EditorPending = new();
        private static VarginhaCharacterFrameGeometryRunner _runner;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            _sheets=null; Textures.Clear(); Pending.Clear(); _runner=null;
            foreach (var mesh in EditorPending) if (mesh.sprite != null) Pending.Add(mesh);
            EditorPending.Clear();
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void PrepareCachedFrames() { if (Pending.Count > 0) EnsureRunner(); }
        private static void EnsureRunner()
        {
            if (_runner != null) return;
            var owner = new GameObject("Geometria_dos_personagens");
            _runner=owner.AddComponent<VarginhaCharacterFrameGeometryRunner>();
            UnityEngine.Object.DontDestroyOnLoad(owner);
        }

        public static void ApplyPending()
        {
            foreach (var mesh in Pending) if (mesh.sprite != null) mesh.sprite.OverrideGeometry(mesh.vertices,mesh.triangles);
            Pending.Clear();
        }

        private static Sheet Data(Texture2D texture)
        {
            if (Textures.TryGetValue(texture, out var sheet)) return sheet;
            if (_sheets == null)
            {
                _sheets = new();
                var json = Resources.Load<TextAsset>("Varginha/CharacterFrameGeometry");
                if (json != null) foreach (var entry in JsonUtility.FromJson<Manifest>(json.text).sheets)
                {
                    if (!_sheets.TryGetValue(entry.name,out var entries)) _sheets[entry.name] = entries = new();
                    entries.Add(entry);
                }
            }
            sheet = null;
            if (_sheets.TryGetValue(texture.name, out var candidates))
                sheet = candidates.Find(s => s.width == texture.width && s.height == texture.height);
            Textures[texture] = sheet; return sheet;
        }
        private static Rect Area(Sheet sheet, Texture2D texture, int[] rect)
        {
            float sx = (float)texture.width / sheet.width, sy = (float)texture.height / sheet.height;
            return new Rect(rect[0]*sx, texture.height-(rect[1]+rect[3])*sy, rect[2]*sx, rect[3]*sy);
        }
        private static Rect Intersection(Rect a, Rect b)
        {
            float left = Mathf.Max(a.xMin,b.xMin), right = Mathf.Min(a.xMax,b.xMax);
            float bottom = Mathf.Max(a.yMin,b.yMin), top = Mathf.Min(a.yMax,b.yMax);
            return new Rect(left,bottom,Mathf.Max(0,right-left),Mathf.Max(0,top-bottom));
        }
        private static Region Primary(Sheet sheet, Texture2D texture, Rect crop)
        {
            Region best = null; float score = 0;
            foreach (var region in sheet.regions)
            {
                var area = Area(sheet,texture,region.rect); var overlap = Intersection(crop,area);
                float weight = overlap.width*overlap.height/(area.width*area.height)*region.area;
                if (weight > score) { best = region; score = weight; }
            }
            return best;
        }
        public static bool TryBounds(Texture2D texture, Rect crop, out Rect bounds, out float anchorX)
        {
            bounds = crop; anchorX = crop.center.x;
            var sheet = Data(texture); if (sheet == null) return false;
            var main = Primary(sheet,texture,crop); if (main == null) return false;
            var area = Area(sheet,texture,main.rect);
            // A few authored seated cells touch: never expand a crop into another actor.
            bounds = area.width <= crop.width*1.25f && area.height <= crop.height*1.25f ? area : Intersection(area,crop);
            foreach (var region in sheet.regions)
            {
                if (region == main || region.area < Mathf.Max(16,main.area*.003f)) continue;
                var extra = Area(sheet,texture,region.rect); var overlap = Intersection(crop,extra);
                if (overlap.width*overlap.height < extra.width*extra.height*.98f) continue;
                bounds = Rect.MinMaxRect(Mathf.Min(bounds.xMin,extra.xMin),Mathf.Min(bounds.yMin,extra.yMin),Mathf.Max(bounds.xMax,extra.xMax),Mathf.Max(bounds.yMax,extra.yMax));
            }
            anchorX = main.anchorX*texture.width/sheet.width;
            return bounds.width > 0 && bounds.height > 0;
        }
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float ppu,
            uint extrude = 0, SpriteMeshType meshType = SpriteMeshType.FullRect)
        {
            var sheet = Data(texture);
            // Unity rejects OverrideGeometry on FullRect sprites; use an editable mesh
            // only for the character atlases that have authored geometry metadata.
            var sprite = Sprite.Create(texture,rect,pivot,ppu,extrude,sheet == null ? meshType : SpriteMeshType.Tight);
            if (sheet == null) return sprite;
            var main = Primary(sheet,texture,rect); if (main == null) return sprite;
            var vertices = new List<Vector2>(); var triangles = new List<ushort>();
            foreach (var region in sheet.regions)
            {
                if (region != main)
                {
                    var area = Area(sheet,texture,region.rect); var overlap = Intersection(rect,area);
                    // Preserve detached hands, balls and weapons belonging to this pose.
                    // Fragments of a neighbouring body are never wholly inside this frame.
                    if (region.area < Mathf.Max(16,main.area*.003f) || overlap.width*overlap.height < area.width*area.height*.98f) continue;
                }
                for (int i = 0; i < region.bands.Length; i += 4)
                {
                    float sx = (float)texture.width/sheet.width, sy = (float)texture.height/sheet.height;
                    var band = new Rect(region.bands[i]*sx,texture.height-(region.bands[i+1]+region.bands[i+3])*sy,region.bands[i+2]*sx,region.bands[i+3]*sy);
                    var visible = Intersection(rect,band); if (visible.width <= 0 || visible.height <= 0) continue;
                    if (vertices.Count > 65528) throw new InvalidOperationException("Character sprite mesh exceeds 16-bit indices: " + texture.name);
                    ushort first = (ushort)vertices.Count;
                    // OverrideGeometry consumes pixels in Sprite Rect space; Unity applies
                    // the pivot and pixels-per-unit conversion itself.
                    vertices.Add(new Vector2(visible.xMin,visible.yMin)-rect.position);
                    vertices.Add(new Vector2(visible.xMax,visible.yMin)-rect.position);
                    vertices.Add(new Vector2(visible.xMin,visible.yMax)-rect.position);
                    vertices.Add(new Vector2(visible.xMax,visible.yMax)-rect.position);
                    triangles.Add(first); triangles.Add((ushort)(first+1)); triangles.Add((ushort)(first+2));
                    triangles.Add((ushort)(first+2)); triangles.Add((ushort)(first+1)); triangles.Add((ushort)(first+3));
                }
            }
            // Unity permits overrides in the render update, but rejects them in editor
            // evaluation and some creation callbacks. Apply before shadows and rendering.
            if (vertices.Count > 0)
            {
                var mesh=new PendingMesh {sprite=sprite,vertices=vertices.ToArray(),triangles=triangles.ToArray()};
                if (Application.isPlaying)
                {
                    Pending.Add(mesh); EnsureRunner();
                }
                else EditorPending.Add(mesh);
            }
            return sprite;
        }
    }
}
