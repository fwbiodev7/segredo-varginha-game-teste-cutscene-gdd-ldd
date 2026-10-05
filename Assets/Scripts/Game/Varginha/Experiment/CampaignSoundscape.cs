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
        public void CinematicDucking(float weight){if(_ambience!=null)_ambience.volume=Mathf.Lerp(.12f,.025f,Mathf.Clamp01(weight));}
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
            float duration = ambient ? 6 : engine ? 2 : id == "AlienBurst" ? .9f : id == "Success" ? 1.1f : id == "Starter" ? 1.2f : id == "Typing" ? .65f : .22f;
            if(id=="ManifestationRise")duration=6.2f;
            if(id=="ManifestationCollapse")duration=4.7f;
            if(id=="ManifestationTear"||id=="SealClose")duration=.85f;
            var data = new float[Mathf.RoundToInt(rate * duration)];
            var random = new System.Random(1996 + id.Length * 71); float filtered = 0;
            for (int i = 0; i < data.Length; i++)
            {
                float t = i / (float)rate, noise = (float)random.NextDouble() * 2 - 1;
                bool foot=id=="FootWood"||id=="FootTile";
                filtered = Mathf.Lerp(filtered, noise, ambient ? .02f : foot ? .09f : .22f);
                float envelope = Mathf.Min(1, t * 50) * Mathf.Clamp01((duration - t) * 10), value;
                if (engine) value = Mathf.Sin(t * Mathf.PI * 2 * 44) * .25f + Mathf.Sin(t * Mathf.PI * 2 * 88) * .14f + filtered * .18f;
                else if (ambient)
                {
                    float bed = Mathf.Sin(t * Mathf.PI * 2 * 55) * .055f + Mathf.Sin(t * Mathf.PI * 2 * 82) * .026f;
                    float outdoor = Mathf.Sin(t * Mathf.PI * 2 * (2400 + 80 * Mathf.Sin(t * 3))) * Mathf.Pow(Mathf.Max(0, Mathf.Sin(t * 5)), 12) * .03f;
                    value = filtered * (id == "SchoolAmbience" ? .35f : .19f) + bed + outdoor;
                    if(id=="ChurchAmbience")value=filtered*.12f+(Mathf.Sin(t*Mathf.PI*2*38)*.07f+Mathf.Sin(t*Mathf.PI*2*57)*.03f)*(.7f+.3f*Mathf.Sin(t*.8f));
                    if(id=="LabAmbience")value=filtered*.25f+Mathf.Sin(t*Mathf.PI*2*110)*.018f+(t%2.4f<.06f?Mathf.Sin(t*Mathf.PI*2*660)*.025f:0);
                    if(id=="ForestAmbience")value=filtered*.3f+outdoor*2;
                    if(id=="WorkshopAmbience")value=filtered*.2f+Mathf.Sin(t*Mathf.PI*2*88)*.025f;
                }
                else if (id == "Success")
                {
                    int note = Mathf.Clamp((int)(t * 4), 0, 3); float[] notes = { 261.63f, 329.63f, 392, 523.25f };
                    value = Mathf.Sin(t * Mathf.PI * 2 * notes[note]) * .16f * Mathf.Exp(-(t % .25f) * 9);
                }
                else if (id == "Starter") value = filtered * .4f * (.5f + .5f * Mathf.Sin(t * 95)) + Mathf.Sin(t * Mathf.PI * 2 * (28 + t * 20)) * .25f;
                else if(id=="AlienBurst")value=(filtered*.32f+Mathf.Sin(t*Mathf.PI*2*(70-t*35))*.24f+Mathf.Sin(t*Mathf.PI*2*190)*.06f)*Mathf.Exp(-t*4);
                else if(id=="ManifestationRise"||id=="ManifestationCollapse")
                {
                    float progress=t/duration,weight=id=="ManifestationRise"?Mathf.SmoothStep(.08f,1,progress):1-progress;
                    float breath=.6f+.4f*Mathf.Sin(t*4.5f)*Mathf.Sin(t*1.7f);
                    value=(Mathf.Sin(t*Mathf.PI*2*36)*.16f+Mathf.Sin(t*Mathf.PI*2*53)*.08f+filtered*.22f*breath)*weight;
                    value+=Mathf.Sin(t*Mathf.PI*2*(108-t*7))*.035f*Mathf.Pow(Mathf.Max(0,Mathf.Sin(t*2.1f)),4)*weight;
                }
                else if(id=="ManifestationTear"||id=="SealClose")value=(filtered*.28f+Mathf.Sin(t*Mathf.PI*2*(64-t*24))*.26f)*Mathf.Exp(-t*5);
                else if(id=="BossClaw"||id=="EchoClaw")value=(filtered*.26f+Mathf.Sin(t*Mathf.PI*2*(id=="BossClaw"?72:145))*.12f)*Mathf.Sin(Mathf.Clamp01(t/.22f)*Mathf.PI)*Mathf.Exp(-t*8);
                else if(id=="Impact")value=(filtered*.12f+Mathf.Sin(t*Mathf.PI*2*90)*.16f)*Mathf.Exp(-t*24);
                else if (foot) value = filtered * .20f * Mathf.Exp(-t * 28) + Mathf.Sin(t * 2 * Mathf.PI * (id == "FootWood" ? 100 : 170)) * .13f * Mathf.Exp(-t * 35);
                else if (id == "Paper" || id == "Zip") value = filtered * .55f * Mathf.Sin(t * 160) * Mathf.Exp(-t * 11);
                else if (id == "Typing") value = filtered * .4f * Mathf.Pow(Mathf.Max(0, Mathf.Sin(t * 70)), 14);
                else value = Mathf.Sin(t * 2 * Mathf.PI * (id == "Key" ? 1400 : 640)) * .17f * Mathf.Exp(-t * 28) + filtered * .12f * Mathf.Exp(-t * 40);
                data[i] = Mathf.Clamp(value * envelope, -.7f, .7f);
            }
            clip = AudioClip.Create("Campaign_" + id, data.Length, 1, rate, false); clip.SetData(data, 0); _clips[id] = clip; return clip;
        }
        private void OnDestroy() { foreach (var clip in _clips.Values) if (clip != null) Destroy(clip); }
    }
}
