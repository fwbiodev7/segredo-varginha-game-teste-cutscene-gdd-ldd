using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    /// <summary>Every cinematic shot is rasterized at the existing travel resolution before display.</summary>
    public sealed class ExperimentFrameComposer
    {
        public const int Width = 384, Height = 216;
        private readonly Dictionary<string, Color32[]> _sources = new();
        private readonly Color32[] _pixels = new Color32[Width * Height];
        public Texture2D Frame { get; }
        public ExperimentFrameComposer()
        {
            Frame = new Texture2D(Width, Height, TextureFormat.RGBA32, false)
            { filterMode = FilterMode.Point, name = "Cutscene_384x216" };
        }
        public Texture2D Compose(ExperimentShot shot, float time, bool reducedMotion)
        {
            var texture = VarginhaExperimentArt.Load(shot.asset);
            if (texture == null) { Fill(new Color32(8, 15, 25, 255)); Apply(); return Frame; }
            if (!_sources.TryGetValue(shot.asset, out var source))
            { source = texture.GetPixels32(); _sources.Add(shot.asset, source); }
            Rect uv = shot.panel < 0 ? new Rect(0, 0, 1, 1) : VarginhaExperimentArt.CellUV(shot.panel);
            int sx = Mathf.RoundToInt(uv.x * texture.width), sy = Mathf.RoundToInt(uv.y * texture.height);
            int sw = Mathf.RoundToInt(uv.width * texture.width), sh = Mathf.RoundToInt(uv.height * texture.height);
            int tick = Mathf.FloorToInt(time * 12);
            int shift = !reducedMotion && shot.mode == "street" ? Mathf.Min(5, tick / 24) : 0;
            for (int y = 0; y < Height; y++) for (int x = 0; x < Width; x++)
            {
                int px = sx + Mathf.Clamp(x * sw / Width + shift, 0, sw - 1);
                int py = sy + Mathf.Clamp(y * sh / Height, 0, sh - 1);
                Color32 c = source[py * texture.width + px];
                if (!reducedMotion && (shot.mode == "interference" || shot.mode == "signal"))
                {
                    int noise = (x * 73 + y * 31 + tick * 47) % 101;
                    byte n = (byte)(noise + 30);
                    if (shot.mode == "signal" || noise > 89) c = new Color32(n, n, n, 255);
                }
                if (shot.mode == "timeskip") c = new Color32(8, 13, 20, 255);
                if (shot.mode == "flash" && !reducedMotion)
                {
                    // Slow single exposure rise, not repeated flashes.
                    float t = Mathf.Clamp01(time / 2.5f);
                    c = (Color32)Color.Lerp(c, new Color(.88f, .94f, .88f), t);
                }
                if ((y & 1) == 0 && shot.mode != "timeskip")
                { c.r = (byte)(c.r * .88f); c.g = (byte)(c.g * .88f); c.b = (byte)(c.b * .88f); }
                _pixels[y * Width + x] = c;
            }
            if(shot.mode=="news"&&time>.45f&&time<shot.duration-.7f)AnimateReporterMouth(time);
            Apply();
            return Frame;
        }
        private void AnimateReporterMouth(float time)
        {
            int[] cycle={0,1,2,1,0,2,1,0};
            int frame=cycle[Mathf.FloorToInt(time*7)%cycle.Length];if(frame==0)return;
            const string id="ReporterMouthV3";
            var sheet=Resources.Load<Texture2D>("Varginha/HouseFeedback/"+id);if(sheet==null)return;
            if(!_sources.TryGetValue(id,out var source)){source=sheet.GetPixels32();_sources.Add(id,source);}
            // Sample only the generated lips: keep the original journalist, head, camera
            // and backdrop perfectly still even if the generated atlas rows differ.
            int sx=812,top=frame==1?463:777,sw=27,sh=13;
            for(int y=0;y<4;y++)for(int x=0;x<10;x++)
            {
                var pixel=source[(sheet.height-top-sh+y*sh/4)*sheet.width+sx+x*sw/10];
                if(((124+y)&1)==0){pixel.r=(byte)(pixel.r*.88f);pixel.g=(byte)(pixel.g*.88f);pixel.b=(byte)(pixel.b*.88f);}
                _pixels[(124+y)*Width+189+x]=pixel;
            }
        }
        private void Fill(Color32 color) { for (int i = 0; i < _pixels.Length; i++) _pixels[i] = color; }
        private void Apply() { Frame.SetPixels32(_pixels); Frame.Apply(false); }
        public void Dispose()
        {
            if(Frame==null)return;
            if(Application.isPlaying)Object.Destroy(Frame);else Object.DestroyImmediate(Frame);
        }
    }
}
