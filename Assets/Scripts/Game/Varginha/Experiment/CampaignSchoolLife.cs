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
        public const float WalkSpeed=1.35f;
        private readonly List<Vector2> _path = new();
        private Transform _environment;
        private CampaignMapPlan _plan;
        private Rigidbody2D _body;
        private SpriteRenderer _renderer;
        private float _nextRoute, _talkUntil, _blockedFor;
        private float _lastDistance=float.PositiveInfinity;
        private bool _away;
        public static Rect RenanClearArea
        {
            get {var data=CampaignIllustratedMaps.Get(4);if(data==null)return new Rect(1.8f,-9.5f,2.4f,2.7f);var p=data.Objective("renan");return new Rect(p.x-.85f,p.y-.6f,1.7f,1.65f);}
        }
        public static CampaignMapPlan CreateRoutePlan(int phase)
        {
            var plan=CampaignMapPlan.Create(phase);
            // Keep roaming classmates outside Renan's whole silhouette, not just his feet.
            plan.walls.Add(RenanClearArea);return plan;
        }
        public void Configure(string name, string activity, Vector2 home, Vector2 away, bool walks)
        {
            StudentName = name; Activity = activity; Home = home; Away = away; Walks = walks;
            _body = GetComponent<Rigidbody2D>(); _renderer = GetComponent<SpriteRenderer>();
            _environment = GameObject.Find("Escola_3_Sistema_Ambiente")?.transform;
            _body.position = home; _body.linearVelocity = Vector2.zero;
            _lastDistance=float.PositiveInfinity;
            _body.bodyType = walks ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
            if(!walks && GetComponent<VarginhaStudentAnimation>()!=null)GetComponent<VarginhaStudentAnimation>().enabled=false;
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
                Repath();
                _nextRoute = float.PositiveInfinity;
            }
        }
        private void Repath()
        {
            _plan=CreateRoutePlan(VarginhaCampaignStage.Active.phase);
            var ownFeet=GetComponent<CircleCollider2D>();
            // Classmates and the player can occupy an otherwise open aisle.
            // Reserve their actual floor contacts when choosing a route.
            foreach(var feet in FindObjectsByType<CircleCollider2D>(FindObjectsSortMode.None))
            {
                if(feet==ownFeet||!feet.enabled||feet.isTrigger)continue;
                if(feet.GetComponent<EdelzioTopDownController>()==null&&feet.GetComponent<CampaignSchoolLife>()==null)continue;
                var b=feet.bounds;_plan.walls.Add(new Rect(b.min.x,b.min.y,b.size.x,b.size.y));
            }
            _plan.Route(_body.position+ownFeet.offset,(_away?Away:Home)+ownFeet.offset,_path);
            for(int i=0;i<_path.Count;i++)_path[i]-=ownFeet.offset;
            _blockedFor=0;_lastDistance=float.PositiveInfinity;
        }
        private void FixedUpdate()
        {
            if (Time.timeScale <= 0 || VarginhaCampaignStage.IsModalOpen || Time.time < _talkUntil) { _body.linearVelocity = Vector2.zero; return; }
            if(_path.Count==0)
            {
                _body.linearVelocity=Vector2.zero;
                if(float.IsPositiveInfinity(_nextRoute))
                {
                    if(Vector2.Distance(_body.position,_away?Away:Home)<.3f)_nextRoute=Time.time+18;
                    else if((_blockedFor+=Time.fixedDeltaTime)>1)Repath();
                }
                return;
            }
            while (_path.Count > 0 && Vector2.Distance(_body.position, _path[0]) < .07f){_path.RemoveAt(0);_lastDistance=float.PositiveInfinity;}
            if (_path.Count > 0)
            {
                float distance=Vector2.Distance(_body.position,_path[0]);
                // Being pushed sideways does not count as progress toward the waypoint.
                _blockedFor=distance<_lastDistance-.002f?0:_blockedFor+Time.fixedDeltaTime;
                _lastDistance=distance;if(_blockedFor>.8f){Repath();return;}
                _body.MovePosition(Vector2.MoveTowards(_body.position, _path[0], WalkSpeed * Time.fixedDeltaTime));
            }
        }
        private void LateUpdate()
        {
            if (_renderer == null) return;
            if(!Walks){_renderer.sprite=CampaignSeatingLayers.StudentPose(StudentName);return;}
            bool walking = _path.Count > 0 && !VarginhaCampaignStage.IsModalOpen;
            Vector2 facing = walking ? _path[0] - _body.position : Time.time < _talkUntil && EdelzioTopDownController.Instance != null
                ? (Vector2)(EdelzioTopDownController.Instance.transform.position - transform.position) : Walks ? Vector2.down : Vector2.up;
            int direction = Mathf.Abs(facing.y) >= Mathf.Abs(facing.x) ? facing.y > 0 ? 3 : 0 : facing.x < 0 ? 1 : 2;
            _renderer.sprite = VarginhaStudentSprites.Frame(StudentName, direction, walking ? 1 + (int)(Time.time * 6) % 2 : 0); _renderer.sortingOrder = 6;
        }
    }
}
