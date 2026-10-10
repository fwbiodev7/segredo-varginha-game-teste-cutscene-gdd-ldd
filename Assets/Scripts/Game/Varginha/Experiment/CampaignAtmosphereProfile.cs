using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Chapter numbers are player-facing; serialized scene IDs remain unchanged.
    public struct CampaignAtmosphereProfile
    {
        public float tension, temperature, contrast, saturation, vignette, dust;
        private static readonly float[] Curve = { .025f, .07f, .12f, .21f, .32f, .36f, .43f, .51f, .57f, .64f, .73f, .83f, .95f, .86f };
        public int AudioBand => tension < .27f ? 0 : tension < .69f ? 1 : 2;
        public static CampaignAtmosphereProfile For(int phase, int area = 0, CampaignStory story = null, CampaignMemory memory = null)
        {
            float t = Curve[CampaignSequence.Chapter(phase) - 1];
            if (phase == 1 && memory?.powerFailed == true) t += .055f;
            if (phase == 2 && story?.boxFound == true) t += .04f;
            if (phase == 4 && story?.codeSolved == true) t += .04f;
            if (phase == 6 && story?.expansion.mapSolved == true) t += .035f;
            if (phase == 7 && story != null) t += Mathf.Min(.06f, Bits(story.expansion.forestSigns) * .02f);
            if (phase == 8 && story?.expansion.anchorFound == true) t += .035f;
            if ((phase == 9 || phase == 10) && story != null) t += Mathf.Min(.05f, Bits(story.expansion.labClues) * .015f);
            if (phase == 12) t += area > 0 ? .055f : 0;
            if (phase == 13) t = .68f;
            if (phase >= 11 && story != null) t += Mathf.Min(.035f, Bits(story.continuation.clues[Mathf.Clamp(phase - 11, 0, 10)]) * .008f);
            if (phase == 20 && story?.continuation.manifestationDispelled == true) t = .73f;
            if (phase == 21) t = area == 1 ? .055f : .86f - (story?.continuation.finalStep ?? 0) * .035f;
            t = Mathf.Clamp01(t);
            bool refuge = phase == 8 || phase == 14;
            return new CampaignAtmosphereProfile {
                tension = t, temperature = Mathf.Lerp(3, refuge ? -5 : -15, t),
                contrast = Mathf.Lerp(0, 11, t), saturation = Mathf.Lerp(0, -13, t),
                vignette = Mathf.Lerp(.025f, .21f, t), dust = phase == 7 ? .11f : Mathf.Lerp(.035f, .105f, t)
            };
        }
        private static int Bits(int bits)
        { int count = 0; for (uint value = (uint)bits; value != 0; value &= value - 1) count++; return count; }
        public static int ScenePhase(string scene)
        {
            if (string.IsNullOrEmpty(scene)) return 0;
            int start = scene.IndexOf("_Fase", System.StringComparison.Ordinal);
            if (start < 0) return 0;
            start += 5; int phase = 0;
            while (start < scene.Length && scene[start] >= '0' && scene[start] <= '9') phase = phase * 10 + scene[start++] - '0';
            return phase >= 1 && phase <= 21 ? phase : 0;
        }
        public static string Environment(int phase, int area = 0)
            => phase == 3 ? "Road" : phase == 7 ? "Forest" : phase == 8 || phase == 14 ? "Church"
            : phase == 9 ? "Lab" : phase == 4 || phase == 5 || phase == 11 || phase == 21 && area == 1 ? "School"
            : phase == 10 || phase == 16 || phase == 17 || phase == 18 || phase == 20 || phase == 21 ? "Workshop" : "House";
    }
}
