using UnityEngine;

namespace Game.Varginha.Experiment
{
    /// <summary>A dedicated speech channel, independent from effects and safe to pause/replace.</summary>
    public sealed class CampaignDialogueVoice : MonoBehaviour
    {
        private AudioSource _source;
        private bool _paused;
        public AudioSource Source => _source;
        public bool Owns(AudioSource source) => source == _source;
        public void Play(AudioClip clip)
        {
            if (clip == null) return;
            if (_source == null) { _source = gameObject.AddComponent<AudioSource>(); _source.playOnAwake = false; _source.loop = false; }
            _source.Stop(); _source.clip = clip; _source.volume = VarginhaGameSettings.Current.voice; _paused = false; _source.Play();
        }
        public void Pause(bool pause)
        {
            pause |= Time.timeScale <= 0;
            if (_source == null || _paused == pause) return;
            _paused = pause; if (pause) _source.Pause(); else _source.UnPause();
        }
        public void Seek(float seconds) { if (_source != null && _source.clip != null) _source.time = Mathf.Clamp(seconds, 0, Mathf.Max(0, _source.clip.length - .01f)); }
        public void Stop() { if (_source != null) _source.Stop(); _paused = false; }
        private void Update() { if (_source != null) _source.volume = VarginhaGameSettings.Current.voice; }
        private void OnDisable() => Stop();
    }
}
