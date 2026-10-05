using System.Collections.Generic;
using System.Collections;
using Game.Player;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // A fightable projection, not the injured entity itself. Reuses the existing hit/health pipeline.
    public sealed class CampaignManifestationCombat:MonoBehaviour
    {
        private static readonly Sprite[,,] Frames=new Sprite[2,4,8];
        private EdelzioTopDownController _player;
        private CampaignMapPlan _plan;
        private HealthSystem _health;
        private SpriteRenderer _renderer,_warning,_shock;
        private Rigidbody2D _body;
        private Vector2 _target;
        private float _timer,_nextSpawn,_slowUntil,_staggerUntil;
        private int _stage;
        private bool _minor;
        private bool _entering,_enraged,_attackHit,_waveAttack;
        private int _attackCycle;
        private float _hurtUntil,_shockUntil;
        private readonly List<CampaignManifestationCombat> _pool=new();
        public const int MaximumMinions=5;
        public bool IsMinor=>_minor;
        public bool Defeated=>_health!=null&&_health.IsDead;
        public bool EntrancePlaying=>_entering;
        public bool CanReceiveHit=>!_entering&&(_minor||_stage==2||Time.time<_staggerUntil);
        public int ActiveMinions {get{int n=0;foreach(var m in _pool)if(m!=null&&m.gameObject.activeSelf&&!m.Defeated)n++;return n;}}
        public float HealthPercent=>_health.HealthPercent;
        public static Sprite Frame(int direction,int pose,bool minor=false)
        {
            int size=minor?1:0;var sprite=Frames[size,direction,pose];if(sprite!=null&&sprite.texture!=null)return sprite;
            var texture=Resources.Load<Texture2D>("Varginha/StoryCharacters/EntityManifestation");
            float ppu=minor?100:34;
            sprite=Sprite.Create(texture,new Rect(pose*128,(3-direction)*144,128,144),new Vector2(.5f,(8+ppu*.58f)/144),ppu,0,SpriteMeshType.FullRect);
            sprite.name="Manifestation_"+direction+"_"+pose;return Frames[size,direction,pose]=sprite;
        }
        public static CampaignManifestationCombat Spawn(Transform owner,EdelzioTopDownController player,CampaignMapPlan plan,Vector2 feet,bool minor=false)
        {
            var go=new GameObject(minor?"Eco_da_manifestação":"Manifestação_do_selo");go.transform.SetParent(owner);go.transform.position=feet+Vector2.up*.58f;
            var sr=go.AddComponent<SpriteRenderer>();sr.sprite=Frame(0,0,minor);
            var body=go.AddComponent<Rigidbody2D>();body.gravityScale=0;body.constraints=RigidbodyConstraints2D.FreezeRotation;body.bodyType=RigidbodyType2D.Kinematic;
            var collider=go.AddComponent<CircleCollider2D>();collider.radius=minor?.22f:.55f;collider.offset=Vector2.down*.58f;
            // Movement uses the feet; punches use a separate, non-solid body hurtbox.
            var hurt=go.AddComponent<CapsuleCollider2D>();hurt.isTrigger=true;hurt.direction=CapsuleDirection2D.Vertical;
            hurt.size=minor?new Vector2(.65f,1.2f):new Vector2(1.8f,3.1f);hurt.offset=minor?new Vector2(0,.02f):new Vector2(0,.95f);
            var health=go.AddComponent<HealthSystem>();health.SetMaxHealth(minor?65:650,false);
            var target=go.AddComponent<VarginhaCombatTarget>();target.SetKind(minor?VarginhaCombatTarget.EnemyKind.MinorManifestation:VarginhaCombatTarget.EnemyKind.AlteredCreature);
            var combat=go.AddComponent<CampaignManifestationCombat>();combat._player=player;combat._plan=plan;combat._health=health;combat._renderer=sr;combat._body=body;combat._minor=minor;
            combat._nextSpawn=Time.time+5;
            VarginhaWorldDepth.Ensure(sr,ground:collider);
            var warning=new GameObject("Aviso_de_ataque");warning.transform.SetParent(owner);combat._warning=warning.AddComponent<SpriteRenderer>();
            combat._warning.sprite=VarginhaAllyAttackPresentation.MarkerSprite;combat._warning.color=new Color(1,.25f,.15f,.7f);combat._warning.sortingOrder=-930;combat._warning.enabled=false;
            var shock=new GameObject("Pulso_da_manifestação");shock.transform.SetParent(owner);combat._shock=shock.AddComponent<SpriteRenderer>();
            combat._shock.sprite=VarginhaAllyAttackPresentation.MarkerSprite;combat._shock.sortingOrder=30005;combat._shock.enabled=false;
            return combat;
        }
        public IEnumerator Enter()
        {
            _entering=true;_body.simulated=false;Vector3 origin=transform.position;
            var pulse=new GameObject("Ruptura_de_entrada");pulse.transform.SetParent(transform.parent);
            var fx=pulse.AddComponent<SpriteRenderer>();fx.sprite=CampaignFlashArt.Explosion[0];fx.sortingOrder=-920;
            pulse.transform.position=origin-Vector3.up*.58f;
            float t=0;
            while(t<3.4f)
            {
                t+=Time.deltaTime;float rise=Mathf.SmoothStep(0,1,Mathf.Clamp01((t-.8f)/1.8f));
                _renderer.sprite=Frame(0,t<2.3f?2:3);_renderer.color=new Color(1,1,1,rise);
                float scale=Mathf.Lerp(.4f,1,rise);transform.localScale=new Vector3(Mathf.Lerp(.75f,1,rise),scale,1);transform.position=origin+Vector3.up*.58f*(scale-1);
                pulse.transform.localScale=Vector3.one*(1.1f+Mathf.Sin(t*3)*.12f+rise*2);fx.color=new Color(.35f,.85f,1,(1-Mathf.Clamp01((t-2.5f)/.9f))*.85f);
                if(t>1.8f&&t<1.8f+Time.deltaTime)Camera.main?.GetComponent<VarginhaCameraShake>()?.Shake(.5f,.08f);
                yield return null;
            }
            transform.localScale=Vector3.one;transform.position=origin;_renderer.color=Color.white;_body.simulated=true;
            _entering=false;_timer=0;_nextSpawn=Time.time+5;Destroy(pulse);
        }
        public void ReactToHit(){_hurtUntil=Time.time+.22f;}
        public void Stagger(float seconds){_staggerUntil=Time.time+seconds;_stage=2;_timer=0;_warning.enabled=false;}
        public void Slow(float seconds){_slowUntil=Time.time+seconds;}
        public void Repel(Vector2 centre,float step){MoveTowards((Vector2)transform.position+((Vector2)transform.position-centre).normalized,step);}
        private void Update()
        {
            if(_player==null||_health==null||Defeated){if(_warning!=null)_warning.enabled=false;if(_shock!=null)_shock.enabled=false;return;}
            if(_entering)return;
            if(CampaignContinuationController.Active?.Modal==true||Time.timeScale<=0||_player.IsInputLocked)return;
            _timer+=Time.deltaTime;
            var delta=(Vector2)_player.transform.position-(Vector2)transform.position;
            int direction=Mathf.Abs(delta.x)>Mathf.Abs(delta.y)?delta.x<0?1:3:delta.y>0?2:0;
            if(!_minor&&!_enraged&&HealthPercent<=.5f){_enraged=true;_stage=5;_timer=0;_warning.enabled=false;Camera.main?.GetComponent<VarginhaCameraShake>()?.Shake(.35f,.06f);CampaignContinuationController.Active?.GetComponent<CampaignSoundscape>()?.Play("AlienBurst");}
            if(_stage==0)
            {
                _renderer.sprite=Frame(direction,(int)(Time.time*4)%2,_minor);
                if(delta.magnitude>1.3f)MoveTowards(_player.transform.position,(_minor?2.3f:_enraged?1.55f:1.2f)*Time.deltaTime);
                if(_timer>(_minor?2.2f:_enraged?2.35f:3f)){_stage=1;_timer=0;_waveAttack=!_minor&&_attackCycle++%2==1;_target=_waveAttack?(Vector2)transform.position:(Vector2)_player.transform.position;_warning.transform.position=_target-Vector2.up*.58f;_warning.transform.localScale=Vector3.one*(_waveAttack?5.9f:_minor?1.6f:2.6f);_warning.enabled=true;}
            }
            else if(_stage==1)
            {
                _renderer.sprite=Frame(direction,_timer<.4f?2:3,_minor);
                if(_timer>(_minor?.7f:1.05f))
                {
                    _timer=0;_attackHit=false;
                    if(_waveAttack){_shockUntil=Time.time+.6f;_shock.enabled=true;_shock.transform.position=transform.position-Vector3.up*.58f;Strike(2.6f);_stage=2;_warning.enabled=false;}
                    else _stage=3;
                }
            }
            else if(_stage==3)
            {
                _renderer.sprite=Frame(direction,4,_minor);MoveTowards(_target,(_minor?6:8.5f)*Time.deltaTime);Strike(_minor?.65f:1.1f);
                if(_timer>.36f){_stage=2;_timer=0;_warning.enabled=false;}
            }
            else if(_stage==5){_renderer.sprite=Frame(direction,_timer<.5f?2:3);if(_timer>1.1f){_stage=0;_timer=0;SpawnMinion();}}
            else
            { _renderer.sprite=Frame(direction,Time.time<_staggerUntil?6:5,_minor);if(_timer>1.6f&&Time.time>=_staggerUntil){_stage=0;_timer=0;} }
            if(Time.time<_hurtUntil){_renderer.sprite=Frame(direction,6,_minor);_renderer.color=Color.Lerp(new Color(1,.6f,.7f),Color.white,1-(_hurtUntil-Time.time)/.22f);}else _renderer.color=Color.white;
            if(_shock.enabled){float p=1-(_shockUntil-Time.time)/.6f;_shock.transform.localScale=Vector3.one*Mathf.Lerp(.3f,5.9f,p);_shock.color=new Color(.3f,.85f,1,1-p);if(Time.time>=_shockUntil)_shock.enabled=false;}
            if(!_minor&&Time.time>=_nextSpawn){_nextSpawn=Time.time+(_enraged?7:9);SpawnMinion();}
        }
        private void Strike(float radius)
        {
            if(_attackHit||Vector2.Distance(_player.transform.position,transform.position)>radius)return;
            var map=transform.parent.Find("Mapa_Campanha");
            foreach(var hit in Physics2D.LinecastAll(transform.position-Vector3.up*.58f,_player.transform.position-Vector3.up*.58f))
                if(map!=null&&hit.collider.transform.IsChildOf(map))return;
            _attackHit=true;_player.GetComponent<HealthSystem>().TakeDamage((_minor?10:_waveAttack?20:24)*(CampaignFinalAllies.Active?.DamageMultiplier??1));
        }
        private void MoveTowards(Vector2 target,float step)
        {
            if(Time.time<_slowUntil)step*=.4f;
            Vector2 now=transform.position,candidate=Vector2.MoveTowards(now,target,step),feet=candidate-Vector2.up*.58f;
            bool blocked=false;var from=now-Vector2.up*.58f;float distance=Vector2.Distance(from,feet);
            var map=transform.parent.Find("Mapa_Campanha");
            foreach(var hit in Physics2D.CircleCastAll(from,_minor?.23f:.56f,(feet-from).normalized,distance))
                if(map!=null&&hit.collider.transform.IsChildOf(map)){blocked=true;break;}
            if(!blocked&&_plan.IsClear(feet,_minor?.23f:.56f))_body.MovePosition(candidate);
        }
        public bool SpawnMinion()
        {
            if(Defeated||ActiveMinions>=MaximumMinions)return false;
            Vector2 feet=(Vector2)transform.position-Vector2.up*.58f;
            for(int i=0;i<12;i++)
            {
                float angle=(i+ActiveMinions)*Mathf.PI/6;Vector2 p=feet+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*1.7f;
                if(!_plan.IsClear(p,.25f)||Vector2.Distance(p+Vector2.up*.58f,_player.transform.position)<1.2f)continue;
                CampaignManifestationCombat item=_pool.Find(m=>m!=null&&!m.gameObject.activeSelf);
                if(item==null){item=Spawn(transform.parent,_player,_plan,p,true);_pool.Add(item);}
                else
                {
                    item.transform.position=p+Vector2.up*.58f;item._health.SetMaxHealth(65,false);
                    item.gameObject.SetActive(true);item._body.simulated=true;
                    foreach(var r in item.GetComponentsInChildren<SpriteRenderer>())r.enabled=true;
                    foreach(var c in item.GetComponentsInChildren<Collider2D>())c.enabled=true;
                    item._stage=0;item._timer=0;item._slowUntil=item._staggerUntil=item._hurtUntil=item._shockUntil=0;
                    item._attackHit=false;item._renderer.color=Color.white;item._warning.enabled=item._shock.enabled=false;
                }
                return true;
            }
            return false;
        }
        private void LateUpdate(){if(_minor&&Defeated)gameObject.SetActive(false);}
        public void ClearMinions(){foreach(var item in _pool)if(item!=null)item.gameObject.SetActive(false);}
        private void OnDisable(){if(_warning!=null)_warning.enabled=false;if(_shock!=null)_shock.enabled=false;}
        private void OnDestroy(){if(_warning!=null)Destroy(_warning.gameObject);if(_shock!=null)Destroy(_shock.gameObject);foreach(var item in _pool)if(item!=null)Destroy(item.gameObject);}
    }
}
