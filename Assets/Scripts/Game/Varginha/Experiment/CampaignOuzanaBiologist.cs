using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Shares the existing atlas, lighting and ground-sorting systems.
    public sealed class CampaignOuzanaBiologist : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private EdelzioTopDownController _player;
        private int _row, _frame = -1;
        private float _nextPose;
        private void Awake() => _renderer = GetComponent<SpriteRenderer>();
        private void Start() => _player = FindAnyObjectByType<EdelzioTopDownController>();
        private void LateUpdate()
        {
            if (Time.time < _nextPose) return;
            _nextPose = Time.time + .35f;
            int row = 0;
            if (_player != null)
            {
                Vector2 toward = _player.transform.position - transform.position;
                if (toward.sqrMagnitude < 9 && toward.sqrMagnitude > .04f)
                    row = Mathf.Abs(toward.y) >= Mathf.Abs(toward.x) ? toward.y > 0 ? 3 : 0 : toward.x < 0 ? 1 : 2;
            }
            float cycle = Time.time % 16;
            int frame = cycle < 8 ? Mathf.FloorToInt(Time.time * 1.4f) % 2 : cycle < 12 ? 2 : 3;
            if (row == _row && frame == _frame) return;
            var sprite = CampaignStorySprites.Frame("OuzanaBiologist", row, frame);
            if (sprite != null) { _renderer.sprite = sprite; _row = row; _frame = frame; }
        }
    }
}
