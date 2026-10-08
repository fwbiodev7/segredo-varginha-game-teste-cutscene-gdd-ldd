using Game.Level;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // CameraFollow2D remains the sole writer of gameplay framing and map bounds.
    public sealed class CampaignCameraDirector : MonoBehaviour
    {
        private Vector3 _focus;
        private float _elapsed, _duration, _weight;
        public float Weight => _weight;
        public bool IsActive => _duration > 0;

        public static void Reveal(Vector3 point, float duration = 2.2f)
        {
            var camera = Camera.main;
            if (camera == null || VarginhaGameSettings.Current.reducedMotion) return;
            var follow = camera.GetComponent<CameraFollow2D>();
            if (follow == null || !follow.enabled) return;
            var director = camera.GetComponent<CampaignCameraDirector>();
            if (director == null) director = camera.gameObject.AddComponent<CampaignCameraDirector>();
            director.Focus(point, duration);
        }

        public void Focus(Vector3 point, float duration)
        {
            Cancel();
            if (VarginhaGameSettings.Current.reducedMotion) return;
            _focus = point; _duration = Mathf.Max(.8f, duration);
        }
        public void Cancel() { _duration = _elapsed = _weight = 0; }
        private void Update()
        {
            if (!IsActive || Time.timeScale <= 0) return;
            if (VarginhaGameSettings.Current.reducedMotion) { Cancel(); return; }
            _elapsed += Time.deltaTime;
            float edge = Mathf.Min(.65f, _duration * .3f);
            _weight = Mathf.SmoothStep(0, 1, Mathf.Min(_elapsed / edge, (_duration - _elapsed) / edge));
            if (_elapsed >= _duration) Cancel();
        }
        public Vector3 Compose(Vector3 gameplayPosition)
        {
            _focus.z = gameplayPosition.z;
            return Vector3.Lerp(gameplayPosition, _focus, _weight * .65f);
        }
        private void OnDisable() => Cancel();
    }
}
