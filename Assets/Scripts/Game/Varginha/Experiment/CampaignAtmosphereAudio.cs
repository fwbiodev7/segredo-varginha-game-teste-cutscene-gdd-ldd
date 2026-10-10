using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignAtmosphereAudio
    {
        private static readonly Dictionary<string, AudioClip> Cache = new();
        public static AudioClip Bed(string environment, int band)
        {
            band = Mathf.Clamp(band, 0, 2);
            string key = environment + "_Atmosfera_" + band;
            if (Cache.TryGetValue(key, out var clip) && clip != null) return clip;
            const int rate = 11025, seconds = 24;
            var samples = new float[rate * seconds * 2];
            // Stable across sessions, independent of Unity's gameplay random sequence.
            int seed = 1996; foreach (char c in key) seed = unchecked(seed * 31 + c);
            var random = new System.Random(seed);
            float left = 0, right = 0;
            bool outside = environment == "Forest" || environment == "Road";
            float fundamental = environment == "Church" ? 40 : environment == "Lab" ? 100 : environment == "Workshop" ? 75 : 50;
            for (int frame = 0; frame < rate * seconds; frame++)
            {
                float t = frame / (float)rate, cycle = t / seconds * Mathf.PI * 2;
                left = Mathf.Lerp(left, (float)random.NextDouble() * 2 - 1, outside ? .009f : .015f);
                right = Mathf.Lerp(right, (float)random.NextDouble() * 2 - 1, outside ? .009f : .015f);
                float air = .13f + .04f * Mathf.Sin(cycle * 3);
                float hum = Mathf.Sin(t * Mathf.PI * 2 * fundamental) * (band == 0 ? .008f : .025f);
                float unease = band == 0 ? 0 : (Mathf.Sin(t * Mathf.PI * 2 * 42) + Mathf.Sin(t * Mathf.PI * 2 * (band == 2 ? 43.5f : 44))) * (.008f + band * .006f);
                float bird = outside ? Mathf.Sin(t * Mathf.PI * 2 * 2200 + Mathf.Sin(cycle * 48) * 7)
                    * Mathf.Pow(Mathf.Max(0, Mathf.Sin(cycle * 5)), 24) * (band == 0 ? .016f : .004f) : 0;
                float metal = environment == "Lab" || environment == "Workshop" ? Mathf.Sin(t * Mathf.PI * 2 * 300) * Mathf.Pow(Mathf.Max(0, Mathf.Sin(cycle * 2)), 32) * .007f : 0;
                // Zero endpoints avoid discontinuities in the filtered noise at loop boundaries.
                float seam = Mathf.SmoothStep(0, 1, Mathf.Min(t, seconds - t) / .18f);
                samples[frame * 2] = Mathf.Clamp((left * air + hum + unease + bird + metal) * seam, -.18f, .18f);
                samples[frame * 2 + 1] = Mathf.Clamp((right * air + hum + unease * .9f + bird * .6f + metal) * seam, -.18f, .18f);
            }
            clip = AudioClip.Create(key, rate * seconds, 2, rate, false); clip.SetData(samples, 0); Cache[key] = clip;
            return clip;
        }
        public static AudioClip Detail()
        {
            const string key = "Atmosfera_Ressonancia";
            if (Cache.TryGetValue(key, out var clip) && clip != null) return clip;
            const int rate = 22050; var data = new float[rate * 4];
            for (int i = 0; i < data.Length; i++)
            {
                float t = i / (float)rate, envelope = Mathf.Pow(Mathf.Sin(Mathf.PI * t / 4), 2);
                data[i] = (Mathf.Sin(t * Mathf.PI * 2 * 63) * .045f + Mathf.Sin(t * Mathf.PI * 2 * 94.5f) * .022f) * envelope;
            }
            clip = AudioClip.Create(key, data.Length, 1, rate, false); clip.SetData(data, 0); Cache[key] = clip; return clip;
        }
    }
}
