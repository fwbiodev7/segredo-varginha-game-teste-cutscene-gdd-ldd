using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    /// <summary>Assigned classroom work and short, collision-aware interval routes.</summary>
    [DefaultExecutionOrder(300)]
    public sealed class CampaignSchoolLife : MonoBehaviour
    {
        public string StudentName, Activity;
        public Vector2 Home, Away;
        public bool Walks;
        private readonly List<Vector2> _path = new();
        private Transform _environment;
        private CampaignMapPlan _plan;
        private Rigidbody2D _body;
        private SpriteRenderer _renderer;
        private float _nextRoute, _talkUntil;
        private bool _away;
        public void Configure(string name, string activity, Vector2 home, Vector2 away, bool walks)
        {
            StudentName = name; Activity = activity; Home = home; Away = away; Walks = walks;
            _body = GetComponent<Rigidbody2D>(); _renderer = GetComponent<SpriteRenderer>();
            _environment = GameObject.Find("Escola_3_Sistema_Ambiente")?.transform;
            _body.position = home; _body.linearVelocity = Vector2.zero;
            _body.bodyType = walks ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
            _nextRoute = Time.time + 12 + name.Length;
            transform.localScale = Vector3.one;
        }
        public void Greet() { _talkUntil = Time.time + 4; _path.Clear(); _body.linearVelocity = Vector2.zero; }
        private void Update()
        {
            if (!Walks || Time.timeScale <= 0 || VarginhaCampaignStage.IsModalOpen || Time.time < _talkUntil) return;
            if (Time.time >= _nextRoute && _path.Count == 0)
            {
                _away = !_away;
                _plan ??= CampaignMapPlan.Create(VarginhaCampaignStage.Active.phase);
                var footOffset=GetComponent<CircleCollider2D>().offset;
                _plan.Route(_body.position+footOffset,(_away?Away:Home)+footOffset,_path);
                for(int i=0;i<_path.Count;i++)_path[i]-=footOffset;
                _nextRoute = Time.time + 18;
            }
        }
        private void FixedUpdate()
        {
            if (Time.timeScale <= 0 || VarginhaCampaignStage.IsModalOpen || Time.time < _talkUntil || _path.Count == 0) { _body.linearVelocity = Vector2.zero; return; }
            while (_path.Count > 0 && Vector2.Distance(_body.position, _path[0]) < .07f) _path.RemoveAt(0);
            if (_path.Count > 0) _body.MovePosition(Vector2.MoveTowards(_body.position, _path[0], 1.35f * Time.fixedDeltaTime));
        }
        private void LateUpdate()
        {
            if (_renderer == null) return;
            bool walking = _path.Count > 0 && !VarginhaCampaignStage.IsModalOpen;
            Vector2 facing = walking ? _path[0] - _body.position : Time.time < _talkUntil && EdelzioTopDownController.Instance != null
                ? (Vector2)(EdelzioTopDownController.Instance.transform.position - transform.position) : Walks ? Vector2.down : Vector2.up;
            int direction = Mathf.Abs(facing.y) >= Mathf.Abs(facing.x) ? facing.y > 0 ? 3 : 0 : facing.x < 0 ? 1 : 2;
            _renderer.sprite = VarginhaStudentSprites.Frame(StudentName, direction, walking ? 1 + (int)(Time.time * 6) % 2 : 0); _renderer.sortingOrder = 6;
        }
    }
}
