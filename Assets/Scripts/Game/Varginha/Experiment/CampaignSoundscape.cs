using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    /// <summary>Original ambient beds and tactile effects; no synthetic speech.</summary>
    public sealed class CampaignSoundscape : MonoBehaviour
    {
        private readonly Dictionary<string, AudioClip> _clips = new();
        private AudioSource _effects, _ambience, _engine;
        private EdelzioTopDownController _actor;
        private Vector3 _previous;
        private float _stride;
        private bool _school;
        public void Configure(string environment, EdelzioTopDownController actor = null)
        {
            _actor = actor; _school = environment == "School" || environment == "Lab" || environment == "Church" || environment == "Workshop";
            _effects = gameObject.AddComponent<AudioSource>(); _effects.playOnAwake = false; _effects.volume = .48f;
            _ambience = gameObject.AddComponent<AudioSource>(); _ambience.playOnAwake = false; _ambience.loop = true; _ambience.volume = .12f;
            _ambience.clip = Clip(environment + "Ambience"); _ambience.Play();
            if (_actor != null) _previous = _actor.transform.position;
            if (environment == "Road")
            {
                _engine = gameObject.AddComponent<AudioSource>(); _engine.playOnAwake = false; _engine.loop = true; _engine.volume = .22f;
                _engine.clip = Clip("Engine"); _engine.Play();
            }
        }
        public void Engine(bool running, float throttle = 0)
        {
            if (_engine == null) return;
            _engine.pitch = Mathf.Lerp(.85f, 1.35f, Mathf.Clamp01(throttle));
            if (running && !_engine.isPlaying) _engine.Play(); else if (!running && _engine.isPlaying) _engine.Stop();
        }
        public void Play(string sound) { if (_effects != null) _effects.PlayOneShot(Clip(sound), sound.StartsWith("Foot") ? .16f : 1f); }
        public void CinematicDucking(float weight) { if (_ambience != null) _ambience.volume = Mathf.Lerp(.12f, .025f, Mathf.Clamp01(weight)); }
        public void Suspend(bool value)
        {
            if (_ambience == null) return;
            if (value) { _ambience.Pause(); _engine?.Pause(); _effects.Pause(); }
            else { _ambience.UnPause(); _engine?.UnPause(); _effects.UnPause(); }
        }
        private void Update()
        {
            if (_actor == null) return;
            float distance = Vector2.Distance(_previous, _actor.transform.position); _previous = _actor.transform.position;
            if (Time.timeScale <= 0 || _actor.IsInputLocked || distance > .5f) return;
            _stride += distance;
            if (_stride > (_actor.IsRunning ? .85f : .65f)) { _stride = 0; Play(_school ? "FootTile" : "FootWood"); }
        }
        private AudioClip Clip(string id)
        {
            if (_clips.TryGetValue(id, out var clip)) return clip;
            const int rate = 22050;
            bool ambient = id.Contains("Ambience"), engine = id == "Engine";
            float duration = ambient ? 6 : engine ? 2
                : id == "AlienBurst"     ? .9f  : id == "Success"      ? 1.1f : id == "Starter"    ? 1.2f
                : id == "Typing"         ? .65f : id == "AllyDumbbell" ? .38f : id == "AllyVolleyball" ? .42f
                : id == "AllyPingPong"   ? .18f : id == "AllyGuitar"   ? .55f : id == "AllyPianoFall"  ? .70f
                : id == "AllyMicrophone" ? .50f : id == "AllySpray"    ? .45f : id == "AllyJiuJitsu"   ? .28f
                : id == "AllyBook"       ? .40f : id == "AllyArtSplash"? .35f : id == "AllyShield"     ? .80f
                : id == "AllyCommand"    ? .12f : id == "AllyReady"    ? .22f : id == "VictoryJingle"  ? 1.8f
                : .22f;
            if (id == "ManifestationRise") duration = 6.2f;
            if (id == "ManifestationCollapse") duration = 4.7f;
            if (id == "ManifestationTear" || id == "SealClose") duration = .85f;
            var data = new float[Mathf.RoundToInt(rate * duration)];
            var random = new System.Random(1996 + id.Length * 71); float filtered = 0;
            for (int i = 0; i < data.Length; i++)
            {
                float t = i / (float)rate, noise = (float)random.NextDouble() * 2 - 1;
                bool foot = id == "FootWood" || id == "FootTile";
                filtered = Mathf.Lerp(filtered, noise, ambient ? .02f : foot ? .09f : .22f);
                float envelope = Mathf.Min(1, t * 50) * Mathf.Clamp01((duration - t) * 10), value;
                if (engine)
                    value = Mathf.Sin(t * Mathf.PI * 2 * 44) * .25f + Mathf.Sin(t * Mathf.PI * 2 * 88) * .14f + filtered * .18f;
                else if (ambient)
                {
                    float bed = Mathf.Sin(t * Mathf.PI * 2 * 55) * .055f + Mathf.Sin(t * Mathf.PI * 2 * 82) * .026f;
                    float outdoor = Mathf.Sin(t * Mathf.PI * 2 * (2400 + 80 * Mathf.Sin(t * 3))) * Mathf.Pow(Mathf.Max(0, Mathf.Sin(t * 5)), 12) * .03f;
                    value = filtered * (id == "SchoolAmbience" ? .35f : .19f) + bed + outdoor;
                    if (id == "ChurchAmbience")   value = filtered * .12f + (Mathf.Sin(t * Mathf.PI * 2 * 38) * .07f + Mathf.Sin(t * Mathf.PI * 2 * 57) * .03f) * (.7f + .3f * Mathf.Sin(t * .8f));
                    if (id == "LabAmbience")      value = filtered * .25f + Mathf.Sin(t * Mathf.PI * 2 * 110) * .018f + (t % 2.4f < .06f ? Mathf.Sin(t * Mathf.PI * 2 * 660) * .025f : 0);
                    if (id == "ForestAmbience")   value = filtered * .3f + outdoor * 2;
                    if (id == "WorkshopAmbience") value = filtered * .2f + Mathf.Sin(t * Mathf.PI * 2 * 88) * .025f;
                }
                else if (id == "Success")
                {
                    int note = Mathf.Clamp((int)(t * 4), 0, 3); float[] notes = { 261.63f, 329.63f, 392, 523.25f };
                    value = Mathf.Sin(t * Mathf.PI * 2 * notes[note]) * .16f * Mathf.Exp(-(t % .25f) * 9);
                }
                else if (id == "Starter") value = filtered * .4f * (.5f + .5f * Mathf.Sin(t * 95)) + Mathf.Sin(t * Mathf.PI * 2 * (28 + t * 20)) * .25f;
                else if (id == "AlienBurst") value = (filtered * .32f + Mathf.Sin(t * Mathf.PI * 2 * (70 - t * 35)) * .24f + Mathf.Sin(t * Mathf.PI * 2 * 190) * .06f) * Mathf.Exp(-t * 4);
                else if (id == "ManifestationRise" || id == "ManifestationCollapse")
                {
                    float progress = t / duration, weight = id == "ManifestationRise" ? Mathf.SmoothStep(.08f, 1, progress) : 1 - progress;
                    float breath = .6f + .4f * Mathf.Sin(t * 4.5f) * Mathf.Sin(t * 1.7f);
                    value = (Mathf.Sin(t * Mathf.PI * 2 * 36) * .16f + Mathf.Sin(t * Mathf.PI * 2 * 53) * .08f + filtered * .22f * breath) * weight;
                    value += Mathf.Sin(t * Mathf.PI * 2 * (108 - t * 7)) * .035f * Mathf.Pow(Mathf.Max(0, Mathf.Sin(t * 2.1f)), 4) * weight;
                }
                else if (id == "ManifestationTear" || id == "SealClose") value = (filtered * .28f + Mathf.Sin(t * Mathf.PI * 2 * (64 - t * 24)) * .26f) * Mathf.Exp(-t * 5);
                else if (id == "BossClaw" || id == "EchoClaw") value = (filtered * .26f + Mathf.Sin(t * Mathf.PI * 2 * (id == "BossClaw" ? 72 : 145)) * .12f) * Mathf.Sin(Mathf.Clamp01(t / .22f) * Mathf.PI) * Mathf.Exp(-t * 8);
                else if (id == "Impact") value = (filtered * .12f + Mathf.Sin(t * Mathf.PI * 2 * 90) * .16f) * Mathf.Exp(-t * 24);
                else if (foot) value = filtered * .20f * Mathf.Exp(-t * 28) + Mathf.Sin(t * 2 * Mathf.PI * (id == "FootWood" ? 100 : 170)) * .13f * Mathf.Exp(-t * 35);
                else if (id == "Paper" || id == "Zip") value = filtered * .55f * Mathf.Sin(t * 160) * Mathf.Exp(-t * 11);
                else if (id == "Typing") value = filtered * .4f * Mathf.Pow(Mathf.Max(0, Mathf.Sin(t * 70)), 14);
                // ─── Ally attack SFX (procedural, no external files) ──────────────────
                else if (id == "AllyDumbbell")
                {
                    // Heavy metallic thud: low rumble + metallic ring
                    float rumble = filtered * .35f * Mathf.Exp(-t * 18);
                    float ping = Mathf.Sin(t * Mathf.PI * 2 * 380) * .18f * Mathf.Exp(-t * 22) + Mathf.Sin(t * Mathf.PI * 2 * 760) * .08f * Mathf.Exp(-t * 30);
                    value = (rumble + ping) * Mathf.Sin(Mathf.Clamp01(t / .01f) * Mathf.PI * .5f);
                }
                else if (id == "AllyVolleyball")
                {
                    // Hollow thump + air whoosh on impact
                    float thump = (filtered * .28f + Mathf.Sin(t * Mathf.PI * 2 * 170) * .22f) * Mathf.Exp(-t * 16);
                    float whoosh = filtered * .12f * Mathf.Exp(-t * 8) * (t < .12f ? t / .12f : 1);
                    value = thump + whoosh;
                }
                else if (id == "AllyPingPong")
                {
                    // Sharp plastic click
                    value = (filtered * .15f + Mathf.Sin(t * Mathf.PI * 2 * 920) * .26f) * Mathf.Exp(-t * 55);
                }
                else if (id == "AllyGuitar")
                {
                    // Power chord: root + fifth + octave with crunch
                    float root  = Mathf.Sin(t * Mathf.PI * 2 * 110) * .18f;
                    float fifth = Mathf.Sin(t * Mathf.PI * 2 * 165) * .12f;
                    float oct   = Mathf.Sin(t * Mathf.PI * 2 * 220) * .10f;
                    float crunch = Mathf.Clamp(root + fifth + oct, -1, 1);
                    value = (crunch + filtered * .08f) * Mathf.Exp(-t * 5.5f);
                }
                else if (id == "AllyPianoFall")
                {
                    // Descending cluster crash + low boom
                    float boom = (filtered * .40f + Mathf.Sin(t * Mathf.PI * 2 * (55 - t * 20)) * .28f) * Mathf.Exp(-t * 9);
                    float keys = Mathf.Sin(t * Mathf.PI * 2 * (440 + t * -200)) * .12f * Mathf.Exp(-t * 14) * (t > .15f ? 1 : t / .15f);
                    value = boom + keys;
                }
                else if (id == "AllyMicrophone")
                {
                    // Deep bass boom + high reverb tail (rap shout)
                    float bass = Mathf.Sin(t * Mathf.PI * 2 * 80) * .30f * Mathf.Exp(-t * 7);
                    float air  = filtered * .20f * Mathf.Pow(Mathf.Clamp01(1 - t / duration), 2);
                    float ring = Mathf.Sin(t * Mathf.PI * 2 * 640) * .06f * Mathf.Exp(-t * 12);
                    value = bass + air + ring;
                }
                else if (id == "AllySpray")
                {
                    // Aerosol hiss burst
                    float hiss = filtered * .42f * (t < .08f ? t / .08f : Mathf.Exp(-(t - .08f) * 10));
                    float whoosh = Mathf.Sin(t * Mathf.PI * 2 * 3800) * .06f * Mathf.Exp(-t * 18);
                    value = hiss + whoosh;
                }
                else if (id == "AllyJiuJitsu")
                {
                    // Fast air cut + body impact
                    float cut  = filtered * .18f * (t < .04f ? 1 : Mathf.Exp(-(t - .04f) * 25));
                    float thud = (filtered * .22f + Mathf.Sin(t * Mathf.PI * 2 * 190) * .16f) * Mathf.Exp(-t * 22) * (t > .06f ? 1 : t / .06f);
                    value = cut + thud;
                }
                else if (id == "AllyBook")
                {
                    // Magical swoosh + paper flutter + arcane shimmer
                    float swirl   = Mathf.Sin(t * Mathf.PI * 2 * (280 - t * 180)) * .18f * Mathf.Exp(-t * 6);
                    float flutter = filtered * .22f * Mathf.Pow(Mathf.Max(0, Mathf.Sin(t * 60)), 6) * Mathf.Exp(-t * 5);
                    float magic   = Mathf.Sin(t * Mathf.PI * 2 * 520) * .08f * Mathf.Exp(-t * 10);
                    value = swirl + flutter + magic;
                }
                else if (id == "AllyArtSplash")
                {
                    // Colorful wet splat + sparkle
                    float slap    = (filtered * .30f + Mathf.Sin(t * Mathf.PI * 2 * 240) * .20f) * Mathf.Exp(-t * 20);
                    float sparkle = Mathf.Sin(t * Mathf.PI * 2 * 1800) * .06f * Mathf.Exp(-t * 18);
                    value = slap + sparkle;
                }
                else if (id == "AllyShield")
                {
                    // Golden dome: rising hum + shimmer
                    float hum     = Mathf.Sin(t * Mathf.PI * 2 * 130) * .14f + Mathf.Sin(t * Mathf.PI * 2 * 195) * .07f;
                    float shimmer = filtered * .10f * Mathf.Sin(t * Mathf.PI * 2 * 2200) * Mathf.Exp(-t * 3);
                    float rise    = Mathf.SmoothStep(0, 1, t / (.4f * duration));
                    value = (hum * rise + shimmer) * Mathf.Exp(-t * 1.8f);
                }
                else if (id == "AllyCommand")
                {
                    // Short UI beep — ally selected
                    value = Mathf.Sin(t * Mathf.PI * 2 * 880) * .22f * Mathf.Exp(-t * 40);
                }
                else if (id == "AllyReady")
                {
                    // Two-tone ascending ready ping
                    float f = t < duration * .5f ? 660 : 880;
                    value = Mathf.Sin(t * Mathf.PI * 2 * f) * .20f * Mathf.Exp(-(t % (.5f * duration)) * 30);
                }
                else if (id == "VictoryJingle")
                {
                    // Upbeat fanfare: C major arpeggio then chord
                    float[] vNotes = { 261.63f, 329.63f, 392, 523.25f, 659.25f };
                    int seg   = Mathf.Clamp((int)(t * 5), 0, 4);
                    float tSeg = t - seg * .2f;
                    value = Mathf.Sin(tSeg * Mathf.PI * 2 * vNotes[seg]) * .22f * Mathf.Exp(-tSeg * 8);
                    if (t > 1f) value += (Mathf.Sin(t * Mathf.PI * 2 * 261.63f) + Mathf.Sin(t * Mathf.PI * 2 * 329.63f) + Mathf.Sin(t * Mathf.PI * 2 * 392f)) * .08f * Mathf.Exp(-(t - 1f) * 4);
                }
                // ─────────────────────────────────────────────────────────────────────
                else value = Mathf.Sin(t * 2 * Mathf.PI * (id == "Key" ? 1400 : 640)) * .17f * Mathf.Exp(-t * 28) + filtered * .12f * Mathf.Exp(-t * 40);
                data[i] = Mathf.Clamp(value * envelope, -.7f, .7f);
            }
            clip = AudioClip.Create("Campaign_" + id, data.Length, 1, rate, false); clip.SetData(data, 0); _clips[id] = clip; return clip;
        }
        private void OnDestroy() { foreach (var clip in _clips.Values) if (clip != null) Destroy(clip); }
    }
}
