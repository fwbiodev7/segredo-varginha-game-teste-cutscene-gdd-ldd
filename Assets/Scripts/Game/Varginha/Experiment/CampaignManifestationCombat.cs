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
        private static readonly Sprite[,,] CombatFrames=new Sprite[2,4,8];
        private static readonly Sprite[,,] FlowFrames=new Sprite[2,4,6];
        [System.Serializable] private sealed class PoseSize {public float width,height;}
        [System.Serializable] private sealed class PoseAtlas {public PoseSize[] frames;}
        private static readonly PoseAtlas[] CombatSizes=new PoseAtlas[2],FlowSizes=new PoseAtlas[2];
        private EdelzioTopDownController _player;
        private CampaignMapPlan _plan;
        private HealthSystem _health;
        private SpriteRenderer _renderer,_warning,_shock,_claws;
        private Rigidbody2D _body;
        private CircleCollider2D _feetCollider;
        private CapsuleCollider2D _hurtbox;
        private Vector2 _target;
        private Vector2 _movementTarget;
        private float _movementSpeed;
        private float _walkTime;
        private Vector2 _repulsion;
        private float _timer,_nextSpawn,_slowUntil,_staggerUntil;
        private int _stage;
        private float _healthScale=1,_damageScale=1,_telegraphScale=1,_intervalScale=1,_speedScale=1;
        private bool _minor;
        private bool _entering,_enraged,_attackHit,_waveAttack;
        private bool _exiting;
        private float _cinematicTime,_cinematicDuration;
        private GameObject _cinematicRoot;
        private static readonly Sprite[,] CinematicFrames=new Sprite[2,12];
        private static Sprite _ember;
        private int _attackCycle;
        private int _attackDirection;
        private Vector2 _attackFacing;
        private float _clawUntil;
        private static Sprite _clawSprite;
        private float _hurtUntil,_shockUntil;
        private readonly List<CampaignManifestationCombat> _pool=new();
        public const int MaximumMinions=5;
        public bool IsMinor=>_minor;
        public bool Defeated=>_health!=null&&_health.IsDead;
        public bool EntrancePlaying=>_entering;
        public bool ExitPlaying=>_exiting;
        public float CinematicProgress=>Mathf.Clamp01(_cinematicTime/Mathf.Max(.01f,_cinematicDuration));
        public string CinematicCaption=>_exiting?(_cinematicTime<1.4f?"A SOMBRA SE ROMPE":_cinematicTime<3.3f?"O SELO RESISTE":"O SILÊNCIO VOLTA. A LIGAÇÃO PERMANECE."):
            (_cinematicTime<1.6f?"O SILÊNCIO SE PARTE":_cinematicTime<3.8f?"ALGO RESPIRA SOB O SELO":"A MANIFESTAÇÃO DESPERTA");
        public float CinematicFlash=>VarginhaGameSettings.Current.reducedMotion?0:Mathf.Max(0,1-Mathf.Abs(_cinematicTime-(_exiting?1.45f:4.15f))/.25f)*.13f;
        public bool CanReceiveHit=>!_entering&&!_exiting&&!Defeated&&(_minor||_stage==2||Time.time<_staggerUntil);
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
        public static Sprite CombatFrame(int direction,int pose,bool minor=false)
        {
            int size=minor?1:0;var sprite=CombatFrames[size,direction,pose];if(sprite!=null&&sprite.texture!=null)return sprite;
            var texture=Resources.Load<Texture2D>("Varginha/StoryCharacters/"+(minor?"ManifestationChildCombatV2":"ManifestationBossCombatV2"));
            if(texture==null)return Frame(direction,pose,minor);
            int width=minor?96:160,height=minor?128:192;float ppu=minor?80:34;
            sprite=Sprite.Create(texture,new Rect(pose*width,(3-direction)*height,width,height),new Vector2(.5f,(8+ppu*.58f)/height),ppu,0,SpriteMeshType.FullRect);
            sprite.name=(minor?"EchoCombat_":"BossCombat_")+direction+"_"+pose;return CombatFrames[size,direction,pose]=sprite;
        }
        private static Sprite FlowFrame(int direction,int pose,bool minor)
        {
            int size=minor?1:0;var sprite=FlowFrames[size,direction,pose];if(sprite!=null&&sprite.texture!=null)return sprite;
            var texture=Resources.Load<Texture2D>("Varginha/StoryCharacters/Manifestation"+(minor?"Child":"Boss")+"FlowV1");
            if(texture==null)return CombatFrame(direction,pose<2?pose:pose==2?2:pose==3?3:pose==4?4:5,minor);
            int width=minor?96:160,height=minor?128:192;float ppu=minor?80:34;
            sprite=Sprite.Create(texture,new Rect(pose*width,(3-direction)*height,width,height),new Vector2(.5f,(8+ppu*.58f)/height),ppu,0,SpriteMeshType.FullRect);
            sprite.name=(minor?"EchoFlow_":"BossFlow_")+direction+"_"+pose;return FlowFrames[size,direction,pose]=sprite;
        }
        private void Pose(int direction,int pose,bool flow=false)
        {
            _renderer.sprite=flow?FlowFrame(direction,pose,_minor):CombatFrame(direction,pose,_minor);
            int size=_minor?1:0;string prefix="Varginha/StoryCharacters/Manifestation"+(_minor?"Child":"Boss");
            if(CombatSizes[size]==null){var data=Resources.Load<TextAsset>(prefix+"CombatV2");if(data!=null)CombatSizes[size]=JsonUtility.FromJson<PoseAtlas>(data.text);}
            if(flow&&FlowSizes[size]==null){var data=Resources.Load<TextAsset>(prefix+"FlowV1");if(data!=null)FlowSizes[size]=JsonUtility.FromJson<PoseAtlas>(data.text);}
            var atlas=flow?FlowSizes[size]:CombatSizes[size];var standing=CombatSizes[size];
            if(_hurtbox==null||atlas==null||standing==null)return;
            float ppu=_minor?80:34;var idle=standing.frames[direction*8];var current=atlas.frames[direction*(flow?6:8)+pose];
            // The core follows the crouch; raised claws, horns and cyan trails never enlarge the target.
            float width=idle.width*.62f/ppu,height=Mathf.Min(current.height,idle.height)*.88f/ppu;
            _hurtbox.size=new Vector2(width,Mathf.Max(width,height));
            _hurtbox.offset=new Vector2(0,-.58f+.06f+_hurtbox.size.y*.5f);
        }
        public static CampaignManifestationCombat Spawn(Transform owner,EdelzioTopDownController player,CampaignMapPlan plan,Vector2 feet,bool minor=false)
        {
            var go=new GameObject(minor?"Eco_da_manifestação":"Manifestação_do_selo");go.transform.SetParent(owner);go.transform.position=feet+Vector2.up*.58f;
            var sr=go.AddComponent<SpriteRenderer>();sr.sprite=CombatFrame(0,0,minor);
            var body=go.AddComponent<Rigidbody2D>();body.gravityScale=0;body.constraints=RigidbodyConstraints2D.FreezeRotation;body.bodyType=RigidbodyType2D.Kinematic;body.interpolation=RigidbodyInterpolation2D.Interpolate;
            var collider=go.AddComponent<CircleCollider2D>();collider.radius=minor?.22f:.55f;collider.offset=Vector2.down*.58f;
            // Movement uses the feet; punches use a separate, non-solid body hurtbox.
            var hurt=go.AddComponent<CapsuleCollider2D>();hurt.isTrigger=true;hurt.direction=CapsuleDirection2D.Vertical;
            hurt.size=minor?new Vector2(.65f,1.2f):new Vector2(1.8f,3.1f);hurt.offset=minor?new Vector2(0,.02f):new Vector2(0,.95f);
            var health=go.AddComponent<HealthSystem>();health.SetMaxHealth((minor?65:650)*VarginhaDifficulty.EnemyHealth/1.55f,false);
            var target=go.AddComponent<VarginhaCombatTarget>();target.SetKind(minor?VarginhaCombatTarget.EnemyKind.MinorManifestation:VarginhaCombatTarget.EnemyKind.AlteredCreature);
            var combat=go.AddComponent<CampaignManifestationCombat>();combat._player=player;combat._plan=plan;combat._health=health;combat._renderer=sr;combat._body=body;combat._feetCollider=collider;combat._hurtbox=hurt;combat._minor=minor;
            combat.Pose(0,0);if(CombatSizes[minor?1:0]!=null)collider.radius=Mathf.Clamp(CombatSizes[minor?1:0].frames[0].width/(minor?80f:34f)*.19f,minor?.12f:.35f,minor?.23f:.56f);
            combat._healthScale=VarginhaDifficulty.EnemyHealth/1.55f;combat._damageScale=VarginhaDifficulty.EnemyDamage;
            combat._telegraphScale=VarginhaDifficulty.TelegraphSeconds/.6f;combat._intervalScale=VarginhaDifficulty.EnemyAttackInterval/1.15f;combat._speedScale=VarginhaDifficulty.EnemySpeed/1.1f;
            combat._nextSpawn=Time.time+5;
            VarginhaWorldDepth.Ensure(sr,ground:collider);
            var warning=new GameObject("Aviso_de_ataque");warning.transform.SetParent(owner);combat._warning=warning.AddComponent<SpriteRenderer>();
            combat._warning.sprite=VarginhaAllyAttackPresentation.MarkerSprite;combat._warning.color=new Color(1,.25f,.15f,.7f);combat._warning.sortingOrder=-930;combat._warning.enabled=false;
            var shock=new GameObject("Pulso_da_manifestação");shock.transform.SetParent(owner);combat._shock=shock.AddComponent<SpriteRenderer>();
            combat._shock.sprite=VarginhaAllyAttackPresentation.MarkerSprite;combat._shock.sortingOrder=30005;combat._shock.enabled=false;
            var claws=new GameObject("Rastro_das_garras");claws.transform.SetParent(go.transform,false);combat._claws=claws.AddComponent<SpriteRenderer>();
            combat._claws.sprite=ClawSprite();combat._claws.sortingOrder=4;combat._claws.sharedMaterial=CampaignFlashArt.Material;combat._claws.enabled=false;
            return combat;
        }
        private static Sprite ClawSprite()
        {
            if(_clawSprite!=null)return _clawSprite;
            var tex=new Texture2D(64,64,TextureFormat.RGBA32,false){filterMode=FilterMode.Point};tex.SetPixels(new Color[64*64]);
            for(int stroke=0;stroke<3;stroke++)for(int x=7;x<57;x++)
            {
                float p=(x-7)/50f;int y=10+stroke*13+Mathf.RoundToInt(Mathf.Sin(p*Mathf.PI)*12);
                tex.SetPixel(x,y,new Color(.5f,.95f,1));if(x%4!=0)tex.SetPixel(x,y-1,new Color(.08f,.45f,.55f));
            }
            tex.Apply();_clawSprite=Sprite.Create(tex,new Rect(0,0,64,64),Vector2.one*.5f,34);_clawSprite.name="ManifestationClawTrail";return _clawSprite;
        }
        private void BeginStrikeVisual()
        {
            _clawUntil=Time.time+(_minor?.18f:.28f);
            CampaignContinuationController.Active?.GetComponent<CampaignSoundscape>()?.Play(_minor?"EchoClaw":"BossClaw");
        }
        public IEnumerator Enter()
        {
            return Manifest(false);
        }
        public IEnumerator Exit()=>Manifest(true);
        private static Sprite CinematicFrame(bool rift,int frame)
        {
            int sheet=rift?1:0;var sprite=CinematicFrames[sheet,frame];if(sprite!=null)return sprite;
            var texture=Resources.Load<Texture2D>("Varginha/StoryEffects/Boss"+(rift?"Rift":"Manifestation")+"CinematicV1");
            int width=rift?160:128;
            sprite=Sprite.Create(texture,new Rect((frame%6)*width,(1-frame/6)*160,width,160),new Vector2(.5f,rift?.22f:8f/160),34,0,SpriteMeshType.FullRect);
            sprite.name=(rift?"BossRift_":"BossManifestation_")+frame;return CinematicFrames[sheet,frame]=sprite;
        }
        private SpriteRenderer CinematicLayer(string name,int order)
        {
            var go=new GameObject(name);go.transform.SetParent(_cinematicRoot.transform,false);
            var sr=go.AddComponent<SpriteRenderer>();sr.sortingOrder=order;sr.sharedMaterial=CampaignFlashArt.Material;return sr;
        }
        private IEnumerator Manifest(bool exiting)
        {
            _entering=!exiting;_exiting=exiting;_cinematicTime=0;_cinematicDuration=exiting?4.7f:6.2f;
            _body.simulated=false;_renderer.enabled=false;_warning.enabled=_shock.enabled=false;
            if(exiting){ClearMinions();foreach(var collider in GetComponents<Collider2D>())collider.enabled=false;}
            bool reduced=VarginhaGameSettings.Current.reducedMotion;
            _cinematicRoot=new GameObject("Cinematica_da_manifestacao");_cinematicRoot.transform.SetParent(transform,false);
            _cinematicRoot.transform.localPosition=Vector3.down*.58f;
            var rift=CinematicLayer("Fenda_do_selo",-2);var silhouette=CinematicLayer("Corpo_em_pixelart",2);
            var embers=new SpriteRenderer[reduced?0:24];
            if(_ember==null)
            {
                var tex=new Texture2D(2,2,TextureFormat.RGBA32,false){filterMode=FilterMode.Point};tex.SetPixels(new[]{Color.white,Color.white,Color.white,Color.white});tex.Apply();
                _ember=Sprite.Create(tex,new Rect(0,0,2,2),Vector2.one*.5f,34);
            }
            for(int i=0;i<embers.Length;i++){embers[i]=CinematicLayer("Cinza_cian_"+i,3);embers[i].sprite=_ember;}
            var sound=CampaignContinuationController.Active?.GetComponent<CampaignSoundscape>();
            sound?.Play(exiting?"ManifestationCollapse":"ManifestationRise");
            bool impact=false,completed=false;
            try
            {
                while(_cinematicTime<_cinematicDuration)
                {
                    // All animation clocks use scaled time: pause freezes poses, dust and sound cues.
                    if(Time.timeScale<=0){yield return null;continue;}
                    _cinematicTime+=Time.deltaTime;float t=_cinematicTime,p=CinematicProgress;
                    int bodyFrame=exiting?Mathf.Min(5,(int)(t/ .68f)):t<1.6f?0:t<2.4f?1:t<3.2f?2:t<4.1f?3:t<5?4:5;
                    int riftFrame=exiting?Mathf.Min(5,(int)(t/.72f)):Mathf.Min(5,(int)(t/.82f));
                    silhouette.sprite=CinematicFrame(false,bodyFrame+(exiting?6:0));silhouette.enabled=exiting||t>.85f;
                    rift.sprite=CinematicFrame(true,riftFrame+(exiting?6:0));
                    float settle=exiting?Mathf.Clamp01((t-4)/.7f):Mathf.Clamp01((t-5.65f)/.55f);
                    silhouette.color=new Color(1,1,1,1-settle);rift.color=new Color(1,1,1,1-settle);
                    if(!exiting&&settle>0){_renderer.enabled=true;_renderer.sprite=CombatFrame(0,0);_renderer.color=new Color(1,1,1,settle);}
                    if(!reduced)silhouette.transform.localPosition=Vector3.up*(Mathf.Round(Mathf.Sin(t*2.7f)*.8f)/34);
                    for(int i=0;i<embers.Length;i++)
                    {
                        float life=Mathf.Repeat(t*.55f+i*.137f,1),angle=i*2.39996f;
                        float radius=exiting?Mathf.Lerp(2.2f,.08f,life):Mathf.Lerp(.2f,2,life);
                        Vector3 point=new Vector3(Mathf.Cos(angle)*radius,Mathf.Sin(angle)*radius*.3f+Mathf.Sin(life*Mathf.PI)*.7f,0);
                        point.x=Mathf.Round(point.x*34)/34;point.y=Mathf.Round(point.y*34)/34;embers[i].transform.localPosition=point;
                        embers[i].color=new Color(i%4==0?.65f:.25f,i%4==0?.18f:.8f,i%4==0?.12f:1,Mathf.Sin(life*Mathf.PI)*(1-settle)*Mathf.Clamp01(t-.6f));
                    }
                    float impactAt=exiting?1.45f:4.15f;
                    if(!impact&&t>=impactAt){impact=true;sound?.Play(exiting?"SealClose":"ManifestationTear");if(!reduced)Camera.main?.GetComponent<VarginhaCameraShake>()?.Shake(.38f,exiting?.065f:.09f);}
                    yield return null;
                }
                completed=true;
            }
            finally
            {
                if(_cinematicRoot!=null){_cinematicRoot.SetActive(false);Destroy(_cinematicRoot);_cinematicRoot=null;}
                _entering=_exiting=false;
                if(!exiting&&completed&&!Defeated){_renderer.enabled=true;_renderer.color=Color.white;_body.simulated=true;_timer=0;_nextSpawn=Time.time+5;}
                else _renderer.enabled=false;
            }
        }
        public void ReactToHit(){_hurtUntil=Time.time+.22f;}
        public void Stagger(float seconds){_staggerUntil=Time.time+seconds;_stage=2;_timer=0;_warning.enabled=false;}
        public void Slow(float seconds){_slowUntil=Time.time+seconds;}
        public void Repel(Vector2 centre,float step)
        {
            if(_body==null||!isActiveAndEnabled||Defeated||_entering||_exiting||Time.timeScale<=0)return;
            _repulsion+=(_body.position-centre).normalized*Mathf.Max(0,step);
        }
        private void Update()
        {
            _movementSpeed=0;
            if(_player==null||_health==null||Defeated){if(_warning!=null)_warning.enabled=false;if(_shock!=null)_shock.enabled=false;if(_claws!=null)_claws.enabled=false;return;}
            if(_entering)return;
            if(CampaignContinuationController.Active?.Modal==true||Time.timeScale<=0||_player.IsInputLocked)return;
            _timer+=Time.deltaTime;
            var delta=(Vector2)_player.transform.position-(Vector2)transform.position;
            int direction=Mathf.Abs(delta.x)>Mathf.Abs(delta.y)?delta.x<0?1:3:delta.y>0?2:0;
            if(_stage==1||_stage==3||_stage==2&&_timer<.45f)direction=_attackDirection;
            if(!_minor&&!_enraged&&HealthPercent<=.5f){_enraged=true;_stage=5;_timer=0;_warning.enabled=false;Camera.main?.GetComponent<VarginhaCameraShake>()?.Shake(.35f,.06f);CampaignContinuationController.Active?.GetComponent<CampaignSoundscape>()?.Play("AlienBurst");}
            if(_stage==0)
            {
                if(delta.magnitude>1.3f)_walkTime+=Time.deltaTime;
                int step=(int)(_walkTime*(_minor?10:7))%4;
                if(delta.magnitude>1.3f)Pose(direction,step/2,step%2==1);else Pose(direction,0);
                if(delta.magnitude>1.3f){_movementTarget=_player.transform.position;_movementSpeed=(_minor?2.3f:_enraged?1.55f:1.2f)*_speedScale;}
                if(_timer>(_minor?2.2f:_enraged?2.35f:3f)*_intervalScale){_stage=1;_timer=0;_attackDirection=direction;_attackFacing=delta.normalized;_waveAttack=!_minor&&_attackCycle++%2==1;_target=_waveAttack?(Vector2)transform.position:(Vector2)_player.transform.position;_warning.transform.position=_target-Vector2.up*.58f;_warning.transform.localScale=Vector3.one*(_waveAttack?5.9f:_minor?1.6f:2.6f);_warning.enabled=true;}
            }
            else if(_stage==1)
            {
                int anticipation=Mathf.Min(3,(int)(_timer/(_minor?.7f:1.05f)*4));
                Pose(direction,anticipation<2?2:3,anticipation%2==1);
                float warningPulse=VarginhaGameSettings.Current.reducedMotion?.7f:.6f+Mathf.Sin(_timer*9)*.12f;
                _warning.color=new Color(1,.25f,.15f,warningPulse);
                if(_timer>(_minor?.7f:1.05f)*_telegraphScale)
                {
                    _timer=0;_attackHit=false;
                    BeginStrikeVisual();
                    if(_waveAttack){_shockUntil=Time.time+.6f;_shock.enabled=true;_shock.transform.position=transform.position-Vector3.up*.58f;Strike(2.6f);_stage=2;_warning.enabled=false;}
                    else _stage=3;
                }
            }
            else if(_stage==3)
            {
                if(_timer<.07f)Pose(direction,4,true);else if(_timer<.23f)Pose(direction,4);else if(_timer<.3f)Pose(direction,5,true);else Pose(direction,5);
                float dash=_minor?6:8.5f;dash*=Mathf.Lerp(.85f,1.1f,Mathf.Sin(Mathf.Clamp01(_timer/.36f)*Mathf.PI));
                _movementTarget=_target;_movementSpeed=dash;Strike(_minor?.65f:1.1f);
                if(_timer>.36f){_stage=2;_timer=0;_movementSpeed=0;_warning.enabled=false;}
            }
            else if(_stage==5){Pose(direction,_timer<.5f?2:3);if(_timer>1.1f){_stage=0;_timer=0;SpawnMinion();}}
            else
            { if(Time.time<_staggerUntil)Pose(direction,6);else if(_waveAttack&&_timer<.2f)Pose(direction,4);else if(_timer<.65f)Pose(direction,5);else if(_timer<1.05f)Pose(direction,5,true);else Pose(direction,0);
                if(_timer>1.6f&&Time.time>=_staggerUntil){_stage=0;_timer=0;} }
            if(Time.time<_hurtUntil){Pose(direction,6);_renderer.color=Color.Lerp(new Color(1,.6f,.7f),Color.white,1-(_hurtUntil-Time.time)/.22f);}else _renderer.color=Color.white;
            _claws.enabled=!VarginhaGameSettings.Current.reducedMotion&&Time.time<_clawUntil;
            if(_claws.enabled)
            {
                float duration=_minor?.18f:.28f,p=1-(_clawUntil-Time.time)/duration;
                _claws.transform.localPosition=(Vector3)(_attackFacing*(_minor?.32f:.75f))+Vector3.up*(_minor?.12f:.65f);
                _claws.transform.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(_attackFacing.y,_attackFacing.x)*Mathf.Rad2Deg-90);
                _claws.transform.localScale=Vector3.one*(_minor?.4f:1.1f);_claws.color=new Color(.6f,.95f,1,(1-p)*.8f);
            }
            if(_shock.enabled){float p=1-(_shockUntil-Time.time)/.6f;_shock.transform.localScale=Vector3.one*Mathf.Lerp(.3f,5.9f,p);_shock.color=new Color(.3f,.85f,1,1-p);if(Time.time>=_shockUntil)_shock.enabled=false;}
            if(!_minor&&Time.time>=_nextSpawn){_nextSpawn=Time.time+(_enraged?7:9);SpawnMinion();}
        }
        private void FixedUpdate()
        {
            if(_entering||_exiting||Defeated||_player==null||_player.IsInputLocked||CampaignContinuationController.Active?.Modal==true||!_body.simulated){_repulsion=Vector2.zero;return;}
            // Render interpolation must never become the next physics position. The shield
            // displacement is consumed once here, so pursuit cannot overwrite it this tick.
            if(_repulsion.sqrMagnitude>0)
            {
                MoveTowards(_body.position+_repulsion,_repulsion.magnitude);_repulsion=Vector2.zero;
            }
            else if(_movementSpeed>0)MoveTowards(_movementTarget,_movementSpeed*Time.fixedDeltaTime);
        }
        private void Strike(float radius)
        {
            if(_attackHit||Vector2.Distance(_player.transform.position,transform.position)>radius)return;
            var map=transform.parent.Find("Mapa_Campanha");
            foreach(var hit in Physics2D.LinecastAll(transform.position-Vector3.up*.58f,_player.transform.position-Vector3.up*.58f))
                if(map!=null&&hit.collider.transform.IsChildOf(map))return;
            _attackHit=true;_player.GetComponent<HealthSystem>().TakeDamage((_minor?10:_waveAttack?20:24)*_damageScale*(CampaignFinalAllies.Active?.DamageMultiplier??1));
        }
        private void MoveTowards(Vector2 target,float step)
        {
            if(Time.time<_slowUntil)step*=.4f;
            Vector2 now=_body.position,candidate=Vector2.MoveTowards(now,target,step),feet=candidate-Vector2.up*.58f;
            bool blocked=false;var from=now-Vector2.up*.58f;float distance=Vector2.Distance(from,feet);
            var map=transform.parent.Find("Mapa_Campanha");
            float radius=_feetCollider.radius;
            foreach(var hit in Physics2D.CircleCastAll(from,radius,(feet-from).normalized,distance))
                if(map!=null&&hit.collider.transform.IsChildOf(map)){blocked=true;break;}
            if(!blocked&&_plan.IsClear(feet,radius))_body.MovePosition(candidate);
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
                if(item==null)
                {
                    item=Spawn(transform.parent,_player,_plan,p,true);item._healthScale=_healthScale;item._damageScale=_damageScale;
                    item._telegraphScale=_telegraphScale;item._intervalScale=_intervalScale;item._speedScale=_speedScale;
                    item._health.SetMaxHealth(65*_healthScale,false);_pool.Add(item);
                }
                else
                {
                    item._body.position=p+Vector2.up*.58f;item.transform.position=p+Vector2.up*.58f;item._health.SetMaxHealth(65*item._healthScale,false);
                    item.gameObject.SetActive(true);item._body.simulated=true;
                    foreach(var r in item.GetComponentsInChildren<SpriteRenderer>())r.enabled=true;
                    foreach(var c in item.GetComponentsInChildren<Collider2D>())c.enabled=true;
                    item._stage=0;item._timer=item._walkTime=item._movementSpeed=0;item._repulsion=Vector2.zero;item._slowUntil=item._staggerUntil=item._hurtUntil=item._shockUntil=0;
                    item._attackHit=false;item._clawUntil=0;item.Pose(0,0);item._renderer.color=Color.white;item._warning.enabled=item._shock.enabled=item._claws.enabled=false;
                }
                return true;
            }
            return false;
        }
        private void LateUpdate(){if(_minor&&Defeated)gameObject.SetActive(false);}
        public void ClearMinions(){foreach(var item in _pool)if(item!=null)item.gameObject.SetActive(false);}
        private void OnDisable(){_movementSpeed=0;_repulsion=Vector2.zero;if(_warning!=null)_warning.enabled=false;if(_shock!=null)_shock.enabled=false;if(_claws!=null)_claws.enabled=false;}
        private void OnDestroy(){if(_warning!=null)Destroy(_warning.gameObject);if(_shock!=null)Destroy(_shock.gameObject);foreach(var item in _pool)if(item!=null)Destroy(item.gameObject);}
    }
}
