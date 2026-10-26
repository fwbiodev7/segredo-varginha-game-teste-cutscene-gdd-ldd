using UnityEngine;
using Game.Varginha.Experiment;

namespace Game.Varginha
{
    /// <summary>Distance-driven eight-direction walk with a compact three-pose cycle.</summary>
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
                // Keep the original four-pose gait duration while presenting three
                // authored poses, matching the compact Edelzio walk cycle.
                Present(Mathf.FloorToInt(_stride * .75f) % 3);
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
            int sourceFrame=CampaignStorySprites.WalkColumn(frame);
            var sprite=Resources.Load<TextAsset>("Varginha/StoryCharacters/"+directional)!=null
                ?CampaignStorySprites.Frame(directional,_artDirection,sourceFrame)
                :VarginhaStudentSprites.Frame(_student, _direction, sourceFrame);
            if (sprite == null) return;
            FrameIndex = frame;
            _renderer.sprite = sprite;
            _renderer.flipX = false;
        }
    }
}
