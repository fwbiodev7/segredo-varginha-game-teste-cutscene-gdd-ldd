using UnityEngine;

namespace Game.Varginha
{
    /// <summary>Small cached colour/position changes; no textures or materials allocated per frame.</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class VarginhaAmbientPixelEffect : MonoBehaviour
    {
        [SerializeField] private bool drift;
        [SerializeField] private float phase;
        private SpriteRenderer _renderer;
        private Color _baseColor;
        private Vector3 _origin;

        public void Configure(bool drifting, float seed) { drift = drifting; phase = seed; }

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _baseColor = _renderer.color;
            _origin = transform.position;
        }

        private void Update()
        {
            if (_renderer == null || !_renderer.isVisible) return;
            if (Experiment.VarginhaGameSettings.Current.reducedMotion)
            { _renderer.color = _baseColor; if (drift) transform.position = _origin; return; }
            float t = Time.time + phase;
            float tension = Experiment.CampaignAtmosphereDirector.Active?.Tension ?? .2f;
            float instability = .012f + .06f * tension;
            float pulse = drift ? .55f + .35f * Mathf.Sin(t * .8f) : 1 - instability + instability * (.67f * Mathf.Sin(t * 3.1f) + .33f * Mathf.Sin(t * 7.3f));
            _renderer.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, _baseColor.a * pulse);
            if (!drift) return;
            var position = _origin + new Vector3(Mathf.Sin(t * .37f) * .12f, Mathf.Sin(t * .51f) * .16f, 0);
            transform.position = new Vector3(Mathf.Round(position.x * 32) / 32f, Mathf.Round(position.y * 32) / 32f, _origin.z);
        }
    }
}
