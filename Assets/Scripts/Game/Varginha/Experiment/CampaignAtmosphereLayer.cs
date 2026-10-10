using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Modulate the already baked light; no texture upload or extra light/shadow pass per frame.
    public sealed class CampaignAtmosphereLayer : MonoBehaviour
    {
        private SpriteRenderer _light;
        private Color _baseColor;
        private float _time, _paranormal;
        public void Configure(SpriteRenderer light) { _light = light; _baseColor = light.color; }
        public static void Interference(Transform root)
        {
            CampaignAtmosphereDirector.Interference();
            var layers = root.GetComponentsInChildren<CampaignAtmosphereLayer>();
            foreach (var layer in layers) layer._paranormal = 1;
        }
        private void Update()
        {
            if (_light == null || Time.timeScale <= 0) return;
            _time += Time.deltaTime; _paranormal = Mathf.MoveTowards(_paranormal, 0, Time.deltaTime * .6f);
            bool reduced = VarginhaGameSettings.Current.reducedMotion;
            float tension = CampaignAtmosphereDirector.Active?.Tension ?? .2f;
            float breath = reduced ? 1 : 1 + (Mathf.Sin(_time * .73f) * .025f + Mathf.Sin(_time * 1.19f) * .012f) * Mathf.Lerp(.2f, 1.4f, tension);
            var color = Color.Lerp(_baseColor, new Color(.48f, .77f, .85f, _baseColor.a), _paranormal * .18f);
            color.a = _baseColor.a * breath; _light.color = color;
        }
        private void OnDisable() { if (_light != null) _light.color = _baseColor; }
    }
}
