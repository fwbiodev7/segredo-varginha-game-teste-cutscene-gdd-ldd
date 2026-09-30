using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha
{
    /// <summary>Classroom reference furniture, sampled on the same 32px world grid as the house.</summary>
    public static class VarginhaClassroomArt
    {
        public const string ResourcePath = "Varginha/ClassroomFurnitureV1";
        private static readonly Dictionary<string, Sprite> Cache = new();
        public static Sprite Surface(bool wall)
        {
            string key=wall?"Classroom_Wall":"Classroom_Floor";
            if(Cache.TryGetValue(key,out var cached) && cached!=null && cached.texture!=null) return cached;
            var pixels=new Color32[32*32];
            for(int y=0;y<32;y++)
            for(int x=0;x<32;x++)
            {
                Color shade;
                if(wall)
                {
                    shade=y<14?new Color(.63f,.29f,.16f):new Color(.77f,.73f,.60f);
                    if(y<14 && (x%8==0 || y%7==0)) shade=new Color(.48f,.26f,.17f);
                    if(y==14) shade=new Color(.43f,.23f,.14f);
                    if(y==31) shade=new Color(.89f,.84f,.71f);
                }
                else
                {
                    shade=((x/16+y/16)&1)==0?new Color(.59f,.57f,.43f):new Color(.64f,.61f,.47f);
                    if(x%16==0 || y%16==0) shade=new Color(.40f,.40f,.32f);
                    else if(x%16==1 || y%16==1) shade=Color.Lerp(shade,Color.white,.08f);
                    else if((x*7+y*11)%29==0) shade=Color.Lerp(shade,Color.black,.035f);
                }
                pixels[y*32+x]=shade;
            }
            var texture=new Texture2D(32,32,TextureFormat.RGBA32,false)
                { name=key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp };
            texture.SetPixels32(pixels); texture.Apply(false,false);
            var sprite=Sprite.Create(texture,new Rect(0,0,32,32),Vector2.one*.5f,32,0,SpriteMeshType.FullRect);
            sprite.name=key; Cache[key]=sprite; return sprite;
        }
        // Authored objects have unequal padding; these rectangles exclude neighbouring props.
        // Coordinates are normalized from the top-left of the generated atlas.
        public static readonly Rect[] Cells =
        {
            new(.015f,.060f,.245f,.300f), new(.269f,.100f,.250f,.260f),
            new(.530f,.078f,.145f,.280f), new(.700f,.070f,.300f,.310f),
            new(.012f,.397f,.297f,.238f), new(.309f,.381f,.181f,.248f),
            new(.490f,.407f,.248f,.211f), new(.740f,.403f,.245f,.220f),
            new(.036f,.643f,.190f,.303f), new(.290f,.689f,.193f,.170f),
            new(.532f,.670f,.145f,.270f), new(.740f,.668f,.235f,.250f)
        };

        public static Sprite Create(int cell, Vector2 size)
        {
            int width = Mathf.Max(4, Mathf.RoundToInt(size.x * 32));
            int height = Mathf.Max(4, Mathf.RoundToInt(size.y * 32));
            string key = cell + "_" + width + "x" + height;
            if (Cache.TryGetValue(key, out var cached) && cached != null && cached.texture != null) return cached;
            var source = Resources.Load<Texture2D>(ResourcePath);
            if (source == null || !source.isReadable) return null;
            Rect uv = Cells[Mathf.Clamp(cell, 0, Cells.Length - 1)];
            var area = new RectInt(Mathf.RoundToInt(uv.x * source.width),
                Mathf.RoundToInt((1 - uv.yMax) * source.height),
                Mathf.RoundToInt(uv.width * source.width), Mathf.RoundToInt(uv.height * source.height));
            var all = source.GetPixels32();
            RectInt crop = VisibleBounds(all, source.width, area);
            var pixels = new Color32[width * height];
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                int sx = crop.x + Mathf.Min(crop.width - 1, (int)((x + .5f) * crop.width / width));
                int sy = crop.y + Mathf.Min(crop.height - 1, (int)((y + .5f) * crop.height / height));
                pixels[y * width + x] = all[sy * source.width + sx];
            }
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
                { name = "Classroom_" + key, filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one * .5f, 32, 0, SpriteMeshType.FullRect);
            sprite.name = texture.name;
            Cache[key] = sprite;
            return sprite;
        }

        internal static RectInt VisibleBounds(Color32[] pixels, int width, RectInt area)
        {
            int left = area.xMax, right = area.x, bottom = area.yMax, top = area.y;
            for (int y = area.y; y < area.yMax; y++)
            for (int x = area.x; x < area.xMax; x++)
            {
                if (pixels[y * width + x].a < 64) continue;
                left = Mathf.Min(left, x); right = Mathf.Max(right, x + 1);
                bottom = Mathf.Min(bottom, y); top = Mathf.Max(top, y + 1);
            }
            return right > left && top > bottom ? new RectInt(left, bottom, right - left, top - bottom) : area;
        }
    }
}
