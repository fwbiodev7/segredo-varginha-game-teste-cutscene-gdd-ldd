using UnityEngine;

namespace Game.Varginha
{
    /// <summary>Four-direction sitting with bent knees, stable feet and the current Edelzio skin.</summary>
    public static class VarginhaSeatedSprites
    {
        public const string ResourcePath = "Varginha/EdelzioSeatedV2";
        private static Sprite[][] _frames;

        public static Sprite Frame(int direction, int frame)
        {
            direction = Mathf.Clamp(direction, 0, 3);
            if (_frames == null || _frames[0][0] == null || _frames[0][0].texture == null)
            {
                var texture = Resources.Load<Texture2D>(ResourcePath);
                if (texture == null || !texture.isReadable) return null;
                texture.filterMode = FilterMode.Point;
                var pixels = texture.GetPixels32();
                int[] boundaries = RowBoundaries(pixels,texture.width,texture.height);
                _frames = new Sprite[4][];
                int[] rows = { 0, 3, 2, 1 }; // Game directions: down, left, right, up.
                var walk = VarginhaReferenceSprites.EdelzioWalkFrames()?[0][0];
                float worldHeight = 1.15f, footY = -.6f;
                if (walk != null && walk.texture.isReadable)
                {
                    var visible = VarginhaClassroomArt.VisibleBounds(walk.texture.GetPixels32(), walk.texture.width,
                        new RectInt((int)walk.rect.x, (int)walk.rect.y, (int)walk.rect.width, (int)walk.rect.height));
                    worldHeight = visible.height / walk.pixelsPerUnit;
                    footY = (visible.y - walk.rect.y - walk.pivot.y) / walk.pixelsPerUnit;
                }
                for (int d = 0; d < 4; d++)
                {
                    _frames[d] = new Sprite[4];
                    var bounds = new RectInt[4];
                    for (int f = 0; f < 4; f++)
                    {
                        int x = f * texture.width / 4, xMax = (f + 1) * texture.width / 4;
                        int y = boundaries[3-rows[d]], yMax = boundaries[4-rows[d]];
                        bounds[f] = VarginhaClassroomArt.VisibleBounds(pixels, texture.width, new RectInt(x, y, xMax - x, yMax - y));
                    }
                    float ppu = bounds[0].height / Mathf.Max(.1f, worldHeight);
                    for (int f = 0; f < 4; f++)
                    {
                        RectInt b = bounds[f];
                        var pivot = new Vector2(.5f, -footY * ppu / b.height);
                        var sprite = Sprite.Create(texture, new Rect(b.x,b.y,b.width,b.height), pivot, ppu, 0, SpriteMeshType.FullRect);
                        sprite.name = $"Edelzio_SeatedV2_{d}_{f}";
                        _frames[d][f] = sprite;
                    }
                }
            }
            return _frames[direction][Mathf.Clamp(frame, 0, 3)];
        }

        private static int[] RowBoundaries(Color32[] pixels,int width,int height)
        {
            int[] result={0,height/4,height/2,height*3/4,height};
            for(int boundary=1;boundary<4;boundary++)
            {
                int start=result[boundary]-height/8,end=result[boundary]+height/8;
                int gapStart=start,longest=0;
                for(int y=start;y<=end;y++)
                {
                    bool occupied=y==end;
                    for(int x=0;!occupied && x<width;x++) occupied=pixels[y*width+x].a>=64;
                    if(!occupied) continue;
                    if(y-gapStart>longest) { longest=y-gapStart; result[boundary]=(gapStart+y)/2; }
                    gapStart=y+1;
                }
            }
            return result;
        }
    }
}
