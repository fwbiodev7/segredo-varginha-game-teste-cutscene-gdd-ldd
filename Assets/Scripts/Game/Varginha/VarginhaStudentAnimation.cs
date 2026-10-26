using UnityEngine;
using Game.Varginha.Experiment;

namespace Game.Varginha
{
    /// <summary>Distance-driven four-direction walk. Stops on a stable pose and never stretches pixels.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class VarginhaStudentAnimation : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private string _student;
        private Vector3 _previous;
        private float _stride;
        private int _direction;
        private int _artDirection;
        public int Facing => _direction;
        public int FrameIndex { get; private set; }

        public void Configure(string student)
        {
            Configure(student,Vector2.down);
        }
        public void Configure(string student,Vector2 initialFacing)
        {
            _student = student;
            _renderer = GetComponent<SpriteRenderer>();
            VarginhaContactShadow.Ensure(_renderer);
            _previous = transform.position;
            _stride = 0;
            _direction = VarginhaStudentSprites.Direction(initialFacing);
            _artDirection = CampaignStorySprites.EightDirection(initialFacing);
            Present(0);
        }
        private void OnEnable() => _previous = transform.position;

        private void LateUpdate()
        {
            var movement = (Vector2)(transform.position - _previous);
            _previous = transform.position;
            if (_renderer == null || !_renderer.enabled || string.IsNullOrEmpty(_student)) return;
            if (Time.timeScale <= 0f) return;
            float distance = movement.magnitude;
            if (distance > .0015f && distance < .75f)
            {
                // A small hysteresis prevents diagonal steering from flickering left/up.
                Vector2 facing = _direction == 1 ? Vector2.left : _direction == 2 ? Vector2.right
                    : _direction == 3 ? Vector2.up : Vector2.down;
                if (Vector2.Dot(facing, movement.normalized) < .55f)
                    _direction = VarginhaStudentSprites.Direction(movement);
                _artDirection=CampaignStorySprites.EightDirection(movement);
                _stride += Mathf.Max(Time.deltaTime * 8.5f, distance * 4.0f);
                Present(Mathf.FloorToInt(_stride) & 3);
            }
            else
            {
                _stride = 0;
                Present(0);
            }
        }

        private void Present(int frame)
        {
            var asset=VarginhaStudentSprites.AssetName(_student);
            var directional="Directional20261026/"+asset;
            var sprite=Resources.Load<TextAsset>("Varginha/StoryCharacters/"+directional)!=null
                ?CampaignStorySprites.Frame(directional,_artDirection,frame)
                :VarginhaStudentSprites.Frame(_student, _direction, frame);
            if (sprite == null) return;
            FrameIndex = frame;
            _renderer.sprite = sprite;
            _renderer.flipX = false;
        }
    }
}
