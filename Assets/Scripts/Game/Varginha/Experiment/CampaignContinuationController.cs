using System.Collections.Generic;
using System.Collections;
using Game.Managers;
using Game.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Varginha.Experiment
{
    public sealed class CampaignContinuationController:MonoBehaviour
    {
        public int phase=11,area;
        public static CampaignContinuationController Active {get;private set;}
        public CampaignStory Progress {get;private set;}
        public CampaignMapPlan Plan {get;private set;}
        public EdelzioTopDownController Actor {get;private set;}
        public CampaignContinuationState State=>Progress.continuation;
        private CampaignContinuationDefinition _definition;
        private string _message,_panel;
        private Vector2 _journalScroll;
        private bool _paused,_settings;
        private int _selected=-1,_choice=-1;
        private int[] _order;
        private float _title,_saveTimer;
        private CampaignSoundscape _sound;
        private CampaignManifestationCombat _boss;
        private CampaignFinalAllies _allies;
        private SpriteRenderer _entity;
        private SpriteRenderer _portal;
        private float _endingFade;
        private bool _sequence,_resumeBattlePending;
        private Camera _shotCamera;
        private Game.Level.CameraFollow2D _shotFollow;
        private float _shotSize;
        private bool _shotFollowing;
        private bool _creditsPending,_creditsVisible;
        private float _creditsTime;
        private Texture2D _memoryImage;
        private string _memoryCaption;
        private float _memoryTime;
        public bool CreditsVisible=>_creditsVisible;
        private readonly Sprite[,] _childFrames=new Sprite[4,3];
        public bool Childhood=>phase==15||phase==19;
        public Vector2 Feet=>(Vector2)Actor.transform.position-(Childhood?Vector2.zero:Vector2.up*.58f);
        public bool Modal=>_creditsVisible||_sequence||_paused||_message!=null||_panel!=null||_title<2||CampaignCinematics.IsTransitioning||VarginhaGameHUD.Instance?.IsGameOver==true||VarginhaGameHUD.Instance?.IsInventoryOpen==true;
        private void Awake()
        {
            Active=this;_definition=CampaignContinuationDefinition.Get(phase,area);Plan=CampaignContinuationDefinition.Plan(phase,area);
            CampaignMapConstruction.Build(transform,Plan);Actor=CampaignMapConstruction.CreatePlayer(transform,Plan,Childhood);
        }
        private void Start()
        {
            Progress=CampaignStorySave.Load();Progress.phase=phase;bool sameArea=State.area==area;State.area=area;
            Actor.HasBackpack=Actor.HasResearchNotebook=!Childhood;Actor.HasFuscaKey=Actor.HasDecodedData=true;
            Actor.CanDodge=false;Actor.SetCombatLocked(false);
            if(Actor.GetComponent<VarginhaGameOverFlow>()==null)Actor.gameObject.AddComponent<VarginhaGameOverFlow>();
            if(sameArea&&Progress.positionPhase==phase&&Plan.IsClear(new Vector2(Progress.x,Progress.y-(Childhood?0:.58f))))Actor.transform.position=new Vector2(Progress.x,Progress.y);
            if(VarginhaGameHUD.Instance==null)new GameObject("Campaign_HUD").AddComponent<VarginhaGameHUD>();
            VarginhaGameHUD.Instance.CampaignInventoryOnly=true;VarginhaGameHUD.Instance.enabled=true;
            var attack=Actor.GetComponent<VarginhaPlayerAttack>();if(attack!=null)attack.enabled=false;
            _sound=gameObject.AddComponent<CampaignSoundscape>();_sound.Configure(phase==11||phase==21&&area==1?"School":phase==14?"Church":Childhood||phase==12||phase==13?"House":"Workshop",Actor);
            _order=new int[_definition.cards.Length];for(int i=0;i<_order.Length;i++)_order[i]=(i+1)%_order.Length;
            if(phase==11)School();
            if(phase==12&&State.serviceRevealed)MarkService();
            if(Childhood){LoadChild();CreateChildhoodPresence();}
            if(phase==14)NPC("Padre Fábio",VarginhaReferenceSprites.PadreFabio(),Plan.points.Find(p=>p.id=="fabio").position);
            if(phase==16){NPC("Padre Fábio",CampaignFinalAllies.PortraitForFinalScene(2),Plan.spawn+Vector2.left*.8f);NPC("Ouzana",CampaignFinalAllies.Portrait(1),Plan.spawn+Vector2.right*.8f);}
            if(phase==18||phase==21&&area==0)
            {
                var entityPoint=Plan.points.Find(p=>p.id==(phase==18?"wounds":"entity"));
                _entity=NPC("Entidade ferida",CampaignManifestationCombat.Frame(0,6),entityPoint.position+Vector2.up*.8f).GetComponent<SpriteRenderer>();
                var collider=_entity.GetComponent<Collider2D>();collider.enabled=false;
                if(phase==18)NPC("Ouzana",CampaignFinalAllies.Portrait(1),Plan.points.Find(p=>p.id=="reading").position+Vector2.left*2.1f+Vector2.down*.45f);
                else
                {
                    var friends=Plan.points.Find(p=>p.id=="friends").position;
                    for(int i=0;i<3;i++)NPC(CampaignFinalAllies.Names[i],CampaignFinalAllies.PortraitForFinalScene(i),friends+new Vector2((i-1)*1.25f,.6f));
                    if(State.finalStep>=2)_entity.enabled=false;
                    CreatePortal();
                }
            }
            if(phase==20){_allies=gameObject.AddComponent<CampaignFinalAllies>();_allies.Configure(Actor);foreach(int i in State.battleStudents)if(i>=0)_allies.Squad.SelectStudent(i);if(State.battleSupport>=0)_allies.SelectSupport(State.battleSupport);_resumeBattlePending=State.chambersPrepared==7&&!State.manifestationDispelled;}
            if(phase==21&&area==1)School();
            GameManager.Instance?.StartGame();Say(_definition.intro);Save();
            if(phase==21&&area==0&&State.CanReturn&&State.finalStep==3){CloseMessage();StartCoroutine(Release());}
        }
        private void LoadChild()
        {
            Actor.GetComponent<VarginhaPlayerSpriteAnimation>().enabled=false;
            var texture=VarginhaExperimentArt.Load("ChildEdelzio");int w=texture.width/3,h=texture.height/4;
            for(int d=0;d<4;d++)for(int f=0;f<3;f++)_childFrames[d,f]=CampaignPresentation.AlignedFrame(texture,new RectInt(f*w,(3-d)*h,w,h),h/1.8f,true);
        }
        private void LateUpdate()
        {
            if(!Childhood||Actor==null||_sequence)return;
            int d=VarginhaStudentSprites.Direction(Actor.FacingDirection),f=Actor.IsMoving&&!Modal?(int)(Time.time*6)%3:0;
            Actor.GetComponent<SpriteRenderer>().sprite=_childFrames[d,f];
        }
        private void School()
        {
            var data=CampaignIllustratedMaps.Get(11);
            var renan=NPC("Renan",CampaignStorySprites.Frame("RenanTeaching",3,0),data.Objective("renan"));
            renan.AddComponent<CampaignRenanTeacher>();CampaignRenanTeacher.DeskItems(transform,data);
            int[] indexes={0,2,3,4,6,7,8,10,11};var seats=CampaignIllustratedMaps.SchoolSeats(11);
            for(int i=0;i<indexes.Length;i++)
            {
                var point=seats[indexes[i]]-Vector2.up*.58f;
                var student=NPC(VarginhaPhase2Controller.StudentNames[i],VarginhaStudentSprites.Frame(VarginhaPhase2Controller.StudentNames[i],3,0),point,false);
                var chair=GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/Cadeira azul "+indexes[i])?.GetComponent<SpriteRenderer>();
                CampaignSeatingLayers.Attach(student.GetComponent<SpriteRenderer>(),chair,true);
                if(chair!=null){var feet=student.GetComponent<CircleCollider2D>();point=student.transform.TransformPoint(feet.offset);}
                Plan.points.Add(new CampaignMapPlan.Point("student"+i,VarginhaPhase2Controller.StudentNames[i]+" • CONVERSAR",point.x,point.y));
            }
            // Revisit the same frontage and classroom; night colour affects this new scene only.
            if(phase==11)foreach(var sr in transform.Find("Mapa_Campanha").GetComponentsInChildren<SpriteRenderer>())sr.color=new Color(.58f,.65f,.80f);
        }
        private GameObject NPC(string name,Sprite sprite,Vector2 feet,bool placeOnFloor=true)
        {
            if(placeOnFloor&&!Plan.IsClear(feet,.3f))
            {
                Vector2 original=feet;bool found=false;
                for(float radius=.25f;radius<=2&&!found;radius+=.25f)for(int i=0;i<16;i++)
                {
                    Vector2 candidate=original+new Vector2(Mathf.Cos(i*Mathf.PI/8),Mathf.Sin(i*Mathf.PI/8))*radius;
                    if(!Plan.IsClear(candidate,.3f))continue;feet=candidate;found=true;break;
                }
            }
            var go=new GameObject(name);go.transform.SetParent(transform);go.transform.position=feet+Vector2.up*.58f;
            var sr=go.AddComponent<SpriteRenderer>();sr.sprite=sprite;
            var collider=go.AddComponent<CircleCollider2D>();collider.radius=.23f;collider.offset=Vector2.down*.58f;
            VarginhaWorldDepth.Ensure(sr,ground:collider);return go;
        }
        private void Update()
        {
            if(Progress==null||CampaignCinematics.IsTransitioning)return;
            if(_creditsVisible)
            {
                _creditsTime+=Time.unscaledDeltaTime;Actor.SetInputLocked(true);
                if(_creditsTime>.8f&&(VarginhaInputActions.PausePressed||VarginhaInputActions.CancelPressed||VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact)))Menu();
                return;
            }
            if(VarginhaGameHUD.Instance?.IsGameOver==true){Actor.SetInputLocked(true);return;}
            if(VarginhaGameHUD.Instance?.IsInventoryOpen==true)return;
            if(_paused&&_settings&&VarginhaInputActions.CancelPressed&&!VarginhaInputActions.PausePressed){_settings=false;return;}
            if((VarginhaInputActions.PausePressed||VarginhaInputActions.CancelPressed && (_paused || _message!=null || _panel!=null)))
            {
                if(_message!=null)CloseMessage();else if(_panel!=null)_panel=null;else TogglePause();
            }
            if(_paused)return;_title+=Time.deltaTime;
            if(_portal!=null){_portal.enabled=State.finalStep>0&&State.finalStep<4;_portal.transform.localScale=new Vector3(2.8f+Mathf.Sin(Time.time*2)*.06f,3.4f,1);}
            if(_message!=null&&VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact)){CloseMessage();Actor.SetInputLocked(Modal);return;}
            if(!Modal)
            {
                if(_resumeBattlePending){_resumeBattlePending=false;StartBattle();Actor.SetInputLocked(true);return;}
                if(Keyboard.current?.f1Key.wasPressedThisFrame==true){OpenHints();return;}
                if(VarginhaInputActions.JournalPressed)_panel="journal";
                if(VarginhaInputActions.InventoryPressed)VarginhaGameHUD.Instance?.OpenBackpack();
                if(VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact)||VarginhaInputActions.CarPressed) {var point=Nearest();if(point!=null && (point.id=="car"?VarginhaInputActions.CarPressed:VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact)))Interact(point.id);}
                _saveTimer+=Time.deltaTime;if(_saveTimer>5){_saveTimer=0;Save();}
            }
            if(phase==20&&_boss!=null&&_boss.Defeated&&!State.manifestationDispelled)
            {
                _boss.ClearMinions();State.manifestationDispelled=true;State.solved[9]=State.finalCalibrated;_allies.CombatActive=false;
                Save();StartCoroutine(ExitBattle());
            }
            Actor.SetInputLocked(Modal);
        }
        public void StartBattle()
        {
            if(_boss!=null||State.manifestationDispelled||State.chambersPrepared!=7)return;
            StartCoroutine(EnterBattle());
        }
        public bool BattleEntrancePlaying=>phase==20&&_sequence&&_boss!=null&&!_boss.Defeated;
        public bool BattleExitPlaying=>phase==20&&_sequence&&_boss!=null&&_boss.Defeated;
        private IEnumerator EnterBattle()
        {
            _sequence=true;_message=null;_panel=null;Actor.SetInputLocked(true);_allies.CombatActive=false;
            _boss=CampaignManifestationCombat.Spawn(transform,Actor,Plan,Plan.bounds.center+Vector2.up*2);
            var camera=Camera.main;if(camera!=null&&camera.GetComponent<VarginhaCameraShake>()==null)camera.gameObject.AddComponent<VarginhaCameraShake>();
            yield return BossShot(false);
            _sequence=false;
            Actor.CanDodge=true;var attack=Actor.GetComponent<VarginhaPlayerAttack>()??Actor.gameObject.AddComponent<VarginhaPlayerAttack>();attack.enabled=true;
            _allies.CombatActive=true;
            Say("A manifestação protege o mecanismo. Desvie do aviso no chão; ataque durante a recuperação. Na mochila, equipe três ALUNOS e um APOIO. Use 1, 2 ou 3 para atacar com um aluno por vez. O comando de aluno alterna entre os equipados; o comando de professor aciona Renan, Ouzana ou Padre Fábio separadamente. No controle: LB aluno, RB professor.");
        }
        private IEnumerator ExitBattle()
        {
            _sequence=true;_message=null;_panel=null;Actor.SetInputLocked(true);
            Actor.GetComponent<VarginhaPlayerAttack>()?.EndHitstopForModal();_allies.CombatActive=false;
            yield return BossShot(true);
            _sequence=false;
            Say("A manifestação se desfaz. Recalibre o painel de retorno usando as três leituras das câmaras. O selo de Edelzio deve permanecer ativo até a travessia.");Save();
        }
        private IEnumerator BossShot(bool exiting)
        {
            _shotCamera=Camera.main;_shotFollow=_shotCamera?.GetComponent<Game.Level.CameraFollow2D>();
            _shotFollowing=_shotFollow!=null&&_shotFollow.enabled;
            _shotSize=_shotCamera!=null?_shotCamera.orthographicSize:0;
            bool reduced=VarginhaGameSettings.Current.reducedMotion;
            Vector3 shotStart=_shotCamera!=null?_shotCamera.transform.position:Vector3.zero;
            Vector3 shotGoal=new Vector3(_boss.transform.position.x,_boss.transform.position.y+.65f,shotStart.z);
            if(_shotFollow!=null)_shotFollow.enabled=false;
            var animation=exiting?_boss.Exit():_boss.Enter();float shotTime=0;
            _sound.CinematicDucking(1);
            try
            {
                while(animation.MoveNext())
                {
                    shotTime+=Time.deltaTime;
                    if(_shotCamera!=null&&Time.timeScale>0)
                    {
                        float push=reduced?1:Mathf.SmoothStep(0,1,shotTime/1.15f);
                        _shotCamera.orthographicSize=reduced?_shotSize:Mathf.Lerp(_shotSize,Mathf.Min(_shotSize,4.4f),push);
                        Vector3 position=Vector3.Lerp(shotStart,shotGoal,push);
                        _shotCamera.transform.position=_shotFollow!=null?_shotFollow.ConstrainPosition(position):position;
                    }
                    yield return animation.Current;
                }
                if(_shotCamera!=null)
                {
                    Vector3 returnStart=_shotCamera.transform.position;float closeSize=_shotCamera.orthographicSize,t=0;
                    while(t<(reduced?0:.6f))
                    {
                        if(Time.timeScale<=0){yield return null;continue;}
                        t+=Time.deltaTime;float p=Mathf.SmoothStep(0,1,t/.6f);
                        _shotCamera.orthographicSize=Mathf.Lerp(closeSize,_shotSize,p);
                        Vector3 position=Vector3.Lerp(returnStart,new Vector3(Actor.transform.position.x,Actor.transform.position.y,returnStart.z),p);
                        _shotCamera.transform.position=_shotFollow!=null?_shotFollow.ConstrainPosition(position):position;
                        yield return null;
                    }
                }
            }
            finally{(animation as System.IDisposable)?.Dispose();RestoreBossShot();}
        }
        private void RestoreBossShot()
        {
            if(_shotCamera!=null)_shotCamera.orthographicSize=_shotSize;
            if(_shotFollow!=null)_shotFollow.enabled=_shotFollowing;
            _shotCamera=null;_shotFollow=null;_sound?.CinematicDucking(0);
        }
        private CampaignMapPlan.Point Nearest()
        {
            var feet=Feet;CampaignMapPlan.Point best=null;float range=1.35f;
            foreach(var point in Plan.points)
            {
                float d=Vector2.Distance(feet,point.position);if(d>=range)continue;
                bool blocked=false;
                foreach(var hit in Physics2D.LinecastAll(feet,point.position))if(hit.collider.transform.IsChildOf(transform.Find("Mapa_Campanha/01_Planta_Paredes_Divisoes"))){blocked=true;break;}
                if(!blocked){range=d;best=point;}
            }
            return best;
        }
        public void Interact(string id)
        {
            if (Progress == null || _sequence || CampaignCinematics.IsTransitioning) return;
            if(phase==21){Finale(id);return;}
            if(phase==20)
            {
                if (State.manifestationDispelled)
                {
                    if (id == "procedure") { _panel = "calibration"; Actor.SetInputLocked(true); }
                    else if (id == "exit") { if (State.CanReturn) _panel = "complete"; else Say("A manifestação se dissipou. Calibre o retorno no mecanismo antes de abrir a passagem."); }
                    else Say("Os circuitos de combate estão preparados. Falta conferir a calibração do retorno.");
                    Save(); return;
                }
                int station=System.Array.IndexOf(_definition.ids,id);
                if(station>=0){State.chambersPrepared|=1<<station;ReadEvidence(station);if(State.chambersPrepared==7)StartBattle();}
                else if(id=="exit"||id=="procedure") {if(State.manifestationDispelled)_panel=State.finalCalibrated?"complete":"calibration";else Say("Prepare os três circuitos e dissipe a manifestação para alcançar a entidade com segurança.");}Save();return;
            }
            if(phase==14&&id=="fabio"){State.Read(14,7);Say("Fábio: Estes são os registros que escondemos. Compare o início do mecanismo, a transferência de 1996 e a falha atual. A página ocultada está no Tombo.");Save();return;}
            if(phase==14&&(id=="page"||id=="timeline")&&(State.clues[3]&128)==0){Say("Apresente os registros do casarão a Fábio primeiro.");return;}
            if(id=="timeline"){OpenPuzzle();return;}
            if(phase==12&&id=="entrance"){ChangeArea(1);return;}
            if(phase==12&&area==1&&id=="garden"){ChangeArea(0);return;}
            if(phase==13&&id=="ground"){Save();var entry=CampaignContinuationDefinition.Plan(12,1).points.Find(p=>p.id=="service").position+Vector2.down*.5f;CampaignStorySave.GoTo(12,1,entry);return;}
            if(phase==12&&id=="service")
            {
                if(!State.serviceRevealed||!State.serviceKey)Say("A planta indica uma porta aqui. Revele o contorno com reagente e procure a chave no escritório.");
                else{State.solved[1]=true;Save();CampaignStorySave.GoTo(13);}return;
            }
            if(phase==12&&id=="reveal")
            {
                if((State.clues[1]&1)==0){Say("Compare esta parede com a planta preservada primeiro.");return;}
                State.serviceRevealed=true;ReadEvidence(1);MarkService();return;
            }
            if(phase==12&&id=="key"){State.serviceKey=true;ReadEvidence(2);return;}
            if(id=="renan"){State.Read(11,7);Say("Renan: O papel e o original concordam. A cópia alterada apagou a indicação lateral e a data. Consulte as três versões no notebook.");}
            else if(id.StartsWith("student"))Say("A turma observou a tela escurecer durante a ausência de Edelzio. As cópias de Renan permaneceram guardadas; esta conversa é opcional.");
            else if(id=="notebook"&&phase==11){if((State.clues[0]&128)==0)Say("Converse com Renan junto ao quadro branco primeiro.");else _panel="evidence";}
            else if(id=="exit")
            {if(!(phase==18?State.AgreementComplete:State.solved[phase-11]))Say("Precisamos concluir a investigação desta fase. Consulte o caderno.");else _panel="complete";}
            else
            {
                int index=System.Array.IndexOf(_definition.ids,id);
                if(index>=0)ReadEvidence(index);
                else if(id=="puzzle")OpenPuzzle();
            }
            Save();Actor.SetInputLocked(Modal);
        }
        private void ChangeArea(int target)
        {
            Save();State.area=target;
            var next=CampaignContinuationDefinition.Plan(phase,target);
            Vector2 feet=target==0?next.points.Find(p=>p.id=="entrance").position+Vector2.down*.85f:next.spawn;
            Progress.positionPhase=phase;Progress.x=feet.x;Progress.y=feet.y+.58f;
            CampaignStorySave.Write(Progress);CampaignCinematics.Load(CampaignContinuationDefinition.SceneName(phase,target));
        }
        private void MarkService()
        {
            if(transform.Find("Marca_da_passagem")!=null)return;
            var point=Plan.points.Find(p=>p.id=="reveal");if(point==null)return;
            var go=new GameObject("Marca_da_passagem");go.transform.SetParent(transform);go.transform.position=point.position+Vector2.up*.5f;
            var sr=go.AddComponent<SpriteRenderer>();sr.sprite=CampaignReagentMarks.FrameSprite(2);sr.color=new Color(.55f,.9f,.8f,.75f);sr.sortingOrder=21000;go.transform.localScale=Vector3.one*.6f;
        }
        public void ReadEvidence(int index)
        {
            var point = Plan.points.Find(p => p.id == _definition.ids[index]);
            if (point != null) CampaignCameraDirector.Reveal(point.position);
            State.Read(phase,index);Say(_definition.documents[index]);Save();
        }
        public void OpenPuzzle()
        {if(!State.HasAll(phase,_definition.required))Say("Ainda faltam evidências: "+_definition.goal);else {_panel="puzzle";_selected=_choice=-1;}}
        public bool Submit(int choice,int[] order=null)
        {
            bool correct=State.HasAll(phase,_definition.required)&&(_definition.choice?choice==0:CampaignStory.Sequence(order,_identity()));
            if(!correct){Say("A combinação não corresponde aos registros. As pistas continuam no caderno.");return false;}
            State.solved[phase-11]=true;_panel=null;
            if(phase==18) { State.solved[8]=true; State.clues[8]|=15; }
            Save();
            if(phase==13||phase==15||phase==18||phase==19)StartCoroutine(Memory());else Say(_definition.success);
            return true;
        }
        public bool SubmitCalibration()
        {
            if(_sequence)return false;
            if(phase!=20||!State.CalibrateReturn())
            {Say(State.finalSealActive?"As três câmaras precisam atingir suas referências simultaneamente. Cada regulador afeta também a câmara seguinte.":"Sem o selo ativo, a ruptura perde a proteção durante a travessia. Reative a ligação de Edelzio.");return false;}
            State.finalCalibrated=State.solved[9]=true;_panel="complete";Save();return true;
        }
        private void Finale(string id)
        {
            if(!State.CanReturn) { Say("Dissipe a manifestação e calibre o retorno antes de abrir a passagem."); return; }
            if(area==1)
            {
                if(State.finalStep!=4) { Say("Aguarde o fechamento da ruptura antes de voltar à Industrial."); return; }
                if(id=="renan"){State.Read(21,0);Say("Renan entrega as cópias preservadas. A turma retomou suas atividades. Agora você conhece a história que estava investigando.");}
                else if(id=="notebook"){if((State.clues[10]&1)==0)Say("Receba as cópias de Renan primeiro.");else{State.Read(21,1);Say(_definition.success);}}
                else if(id=="fusca"||id=="exit")
                {if(!State.HasAll(21,2))Say("Converse com Renan e abra o caderno antes de partir.");else{State.finished=State.solved[10]=true;_sound.Play("Starter");_panel=null;_creditsPending=true;Say("O Fusca liga normalmente. O acordo foi cumprido; Edelzio está livre.\n\nEu sei o que vi. E agora sei por que voltei.");}}
                Save();return;
            }
            if(id=="friends"){Say(_definition.documents[2]);return;}
            if(id=="entity"){Say("A entidade ferida espera a passagem estável. Edelzio mantém o selo durante a travessia.");return;}
            if(id!="procedure"&&id!="exit")return;
            if(State.finalStep==0){State.finalStep=1;_sound.Play("AlienBurst");Say("A passagem de retorno está aberta. A ligação de Edelzio permanece enquanto a entidade se aproxima.");}
            else if(State.finalStep==1){StartCoroutine(Crossing());}
            else if(State.finalStep==2){State.finalStep=3;StartCoroutine(Release());}
            else if(State.finalStep==4){ChangeArea(1);return;}Save();
        }
        private IEnumerator Crossing()
        {
            _sequence=true;_sound.Play("AlienBurst");Vector3 start=_entity.transform.position;
            var goal=(Vector3)Plan.points.Find(p=>p.id=="procedure").position+Vector3.up*1.2f;
            float t=0;while(t<2.6f){t+=Time.deltaTime;_entity.transform.position=Vector3.Lerp(start,goal,Mathf.SmoothStep(0,1,t/2.6f));_entity.color=new Color(1,1,1,1-Mathf.Clamp01((t-1.8f)/.8f));yield return null;}
            State.finalStep=2;_sequence=false;_entity.enabled=false;Say("A entidade atravessou. Agora interrompa a ligação imposta pelo mecanismo.");Save();
        }
        private IEnumerator Release()
        {
            _sequence=true;_sound.Play("Success");float t=0;
            while(t<2.8f){t+=Time.deltaTime;_endingFade=t<1.4f?Mathf.Clamp01(t/1.4f):Mathf.Clamp01((2.8f-t)/1.4f);yield return null;}
            _endingFade=0;State.finalStep=4;State.finalSealActive=false;_sequence=false;if(_portal!=null)_portal.enabled=false;
            var friends=Plan.points.Find(p=>p.id=="friends").position;
            Actor.GetComponent<Rigidbody2D>().position=friends+Vector2.left*2.6f+Vector2.up*.58f;
            Say("A ruptura se fecha. A marca desaparece. Edelzio perde a consciência por alguns segundos e acorda ao lado dos amigos. O acordo terminou.");Save();
        }
        private void CreatePortal()
        {
            var go=new GameObject("Passagem_de_retorno");go.transform.SetParent(transform);
            go.transform.position=Plan.points.Find(p=>p.id=="procedure").position+Vector2.up*1.3f;
            _portal=go.AddComponent<SpriteRenderer>();_portal.sprite=CampaignFlashArt.Explosion[0];
            _portal.color=new Color(.42f,.9f,1,.78f);_portal.sortingOrder=22000;_portal.enabled=State.finalStep>0&&State.finalStep<4;
        }
        private void CreateChildhoodPresence()
        {
            var point=Plan.points.Find(p=>p.id==(phase==15?"presence":"memory"));
            if(point==null)return;
            var presence=NPC("Presença de 1996",CampaignManifestationCombat.Frame(0,6),point.position+Vector2.up*.8f);
            presence.transform.localScale=Vector3.one*.58f;
            presence.GetComponent<Collider2D>().enabled=false;
            presence.GetComponent<SpriteRenderer>().color=new Color(.62f,.81f,.86f,.8f);
        }
        private IEnumerator Memory()
        {
            _sequence=true;_message=null;_memoryTime=0;
            if(phase==19)
            {
                var point=Plan.points.Find(p=>p.id=="memory");
                if(point!=null&&Plan.IsClear(point.position))Actor.GetComponent<Rigidbody2D>().position=point.position;
                var flash=gameObject.AddComponent<CampaignFlashEncounter>();flash.Begin(Actor,Actor.GetComponent<SpriteRenderer>(),_sound);
                yield return new WaitForSeconds(2.3f);Destroy(flash);yield return null;
            }
            else
            {
                _memoryImage=phase==13?Resources.Load<Texture2D>("Varginha/Interface/Memory1898"):Resources.Load<Texture2D>("Varginha/IllustratedMaps/"+CampaignIllustratedMaps.Get(15).image);
                _memoryCaption=phase==13?"1898 • MÃOS ACIONAM O MECANISMO":phase==15?"1996 • UM PEDIDO DE AJUDA":"A ENTIDADE PROJETA A CASA DE 1996";
                while(_memoryTime<2.8f){_memoryTime+=Time.deltaTime;yield return null;}
                _memoryImage=null;
            }
            _sequence=false;Say(_definition.success);Save();
        }
        private int[] _identity(){var a=new int[_definition.cards.Length];for(int i=0;i<a.Length;i++)a[i]=i;return a;}
        public void Say(string text){_message=text;Actor?.SetInputLocked(true);}
        public void CloseMessage()
        {
            _message=null;
            if(_creditsPending){_creditsPending=false;_creditsVisible=true;_creditsTime=0;Save();}
            Actor?.SetInputLocked(Modal);
        }
        private void Save()
        {
            if(Progress==null)return;
            if(_allies!=null){for(int i=0;i<3;i++)State.battleStudents[i]=i<_allies.Squad.SelectedStudentIndices.Count?_allies.Squad.SelectedStudentIndices[i]:-1;State.battleSupport=_allies.SelectedSupport;}
            Progress.x=Actor.transform.position.x;Progress.y=Actor.transform.position.y;Progress.positionPhase=phase;CampaignStorySave.Write(Progress);
        }
        public void SaveTeamChoice()=>Save();
        public bool SubmitReturnCalibration()=>SubmitCalibration();
        public void TogglePause()
        {_paused=!_paused;_settings=false;Time.timeScale=_paused?0:1;CampaignCinematics.Pause(_paused);_sound?.Suspend(_paused);Actor.SetInputLocked(Modal);}
        private void OnGUI()
        {
            VarginhaGamepadUI.Begin("continuation:"+GetEntityId()+":"+_panel+":"+_paused+":"+(_message!=null),_panel!=null||_message!=null||_paused,50);
            if(Progress?.continuation==null||Actor==null||VarginhaGameHUD.Instance?.IsInventoryOpen==true)return;
            _definition??=CampaignContinuationDefinition.Get(phase,area);
            ExperimentGUI.Init();var matrix=ExperimentGUI.BeginCanvas();
            float hudScale=Mathf.Max(.01f,Mathf.Min(Screen.width/1280f,Screen.height/720f));
            float hudLeft=24-(Screen.width/hudScale-1280)*.5f;
            float hudTop=24-(Screen.height/hudScale-720)*.5f;
            if((BattleEntrancePlaying||BattleExitPlaying)&&!_paused)
            {
                float width=Screen.width/hudScale;
                float left=(1280-width)/2,height=Screen.height/hudScale,top=(720-height)/2;
                // Soft darkness at the edges holds attention on the apparition, without obscuring its eyes.
                for(int i=0;i<8;i++)
                {
                    float inset=i*18;var shade=new Color(0,.015f,.025f,Mathf.Lerp(.18f,.015f,i/7f));
                    ExperimentGUI.Box(new Rect(left,top+62+inset,width,18),shade);
                    ExperimentGUI.Box(new Rect(left,top+height-80-inset,width,18),shade);
                    ExperimentGUI.Box(new Rect(left+inset,top+62,18,height-124),shade);
                    ExperimentGUI.Box(new Rect(left+width-inset-18,top+62,18,height-124),shade);
                }
                ExperimentGUI.Box(new Rect(left,top,width,height),new Color(.35f,.8f,.9f,_boss.CinematicFlash));
                ExperimentGUI.Box(new Rect((1280-width)/2,hudTop-24,width,62),Color.black);
                ExperimentGUI.Box(new Rect((1280-width)/2,682-hudTop,width,62),Color.black);
                ExperimentGUI.Label(new Rect(310,624,660,32),_boss.CinematicCaption,small:true);
                GUI.matrix=matrix;return;
            }
            if(_creditsVisible){if(CampaignCredits.Draw(_creditsTime))Menu();GUI.matrix=matrix;return;}
            if(VarginhaGameHUD.Instance?.IsGameOver==true){GUI.matrix=matrix;return;}
            if(_title<2&&!_paused)CampaignChapterPreview.Draw(phase,_title,2);
            else if(!_paused)
            {
                string goal=phase==20&&State.manifestationDispelled&&!State.finalCalibrated?"Calibre o retorno no mecanismo central. A entidade ferida espera.":phase==21&&area==0&&State.finalStep==4?"O acordo terminou. Volte à Industrial.":(phase==18?State.AgreementComplete:State.solved[phase-11])?"Etapa concluída. Siga para a saída.":_definition.goal;
                if(phase!=20)ExperimentGUI.Objective(CampaignSequence.Heading(phase),CampaignSequence.Title(phase).ToUpperInvariant(),goal);
                else if(_boss==null||State.manifestationDispelled){ExperimentGUI.Panel(new Rect(345,hudTop,590,74));ExperimentGUI.Label(new Rect(365,hudTop+14,550,47),goal,small:true);}
                if(CampaignHudIcons.Button(1058,CampaignHudIcons.Icon.Notebook,"TAB","Caderno"))_panel="journal";
                if(CampaignHudIcons.Button(1116,CampaignHudIcons.Icon.Backpack,"G","Mochila"))VarginhaGameHUD.Instance?.OpenBackpack();
                if(CampaignHudIcons.Button(1174,CampaignHudIcons.Icon.Pause,"ESC","Pausa"))TogglePause();
                if(phase==20&&_boss!=null&&!State.manifestationDispelled)
                {
                    ExperimentGUI.Panel(new Rect(365,hudTop,550,84));ExperimentGUI.Label(new Rect(385,hudTop+10,510,23),"MANIFESTAÇÃO DA ENTIDADE",small:true);
                    ExperimentGUI.Box(new Rect(385,hudTop+40,510,16),new Color(.15f,.08f,.12f));ExperimentGUI.Box(new Rect(385,hudTop+40,510*_boss.HealthPercent,16),new Color(.73f,.19f,.24f));
                    ExperimentGUI.Label(new Rect(385,hudTop+61,510,18),_boss.CanReceiveHit?"VULNERÁVEL • ATAQUE AGORA":"OBSERVE O AVISO E DESVIE",small:true);
                    _allies?.DrawCommands(365,hudTop+94);
                    ExperimentGUI.Label(new Rect(365,hudTop+145,550,32),"ESQUIVA: "+VarginhaInputBindings.DisplayName(VarginhaInputAction.Dodge)+" • PRÓXIMO ALUNO: "+(VarginhaInputActions.UsingGamepad?"LB":VarginhaInputBindings.DisplayName(VarginhaInputAction.AllyCommand)),small:true);
                }
                if(phase==20)
                {
                    var health=Actor.GetComponent<HealthSystem>();
                    if(health!=null)
                    {
                        ExperimentGUI.Panel(new Rect(hudLeft,hudTop,270,84));
                        ExperimentGUI.Label(new Rect(hudLeft+20,hudTop+12,230,25),"EDELZIO",small:true);
                        ExperimentGUI.Label(new Rect(hudLeft+20,hudTop+36,230,20),"VIDA "+Mathf.CeilToInt(health.CurrentHealth)+" / "+Mathf.CeilToInt(health.MaxHealth),small:true);
                        ExperimentGUI.Box(new Rect(hudLeft+20,hudTop+62,230,12),new Color(.15f,.08f,.12f));ExperimentGUI.Box(new Rect(hudLeft+20,hudTop+62,230*health.HealthPercent,12),new Color(.23f,.74f,.51f));
                    }
                }
                if(!Modal&&VarginhaGameSettings.Current.interactionHints){var point=Nearest();if(point!=null){ExperimentGUI.Panel(new Rect(160,620,960,70));string label=phase==21&&area==0&&(point.id=="procedure"||point.id=="exit")?new[]{"ABRIR PASSAGEM DE RETORNO","AGUARDAR A TRAVESSIA","ENCERRAR O SELO","O ACORDO TERMINOU","VOLTAR À INDUSTRIAL"}[State.finalStep]:point.label;ExperimentGUI.Label(new Rect(180,639,920,36),"["+(point.id=="car"?VarginhaInputActions.CarLabel:VarginhaInputActions.InteractLabel)+"] "+label,small:true);}}
            }
            if(_paused)
            {
                int action=ExperimentGUI.PausePanel(CampaignSequence.Heading(phase)+" • "+CampaignSequence.Title(phase));
                if(action==1)TogglePause();if(action==2)_settings=!_settings;if(action==3)Menu();
                if(_settings){GUI.matrix=matrix;if(VarginhaGameSettings.Draw())_settings=false;return;}
            }
            if(_panel!=null&&!_paused)Panel();
            if(_message!=null&&!_paused)
            {ExperimentGUI.Panel(new Rect(90,440,1100,240));ExperimentGUI.Label(new Rect(120,465,1040,140),_message);if(ExperimentGUI.Button(new Rect(850,625,290,35),"CONTINUAR"))CloseMessage();}
            if(_endingFade>0)ExperimentGUI.Box(new Rect(0,0,1280,720),new Color(.025f,.06f,.08f,_endingFade));
            if(_memoryImage!=null)
            {
                GUI.DrawTexture(new Rect(0,0,1280,720),_memoryImage,ScaleMode.ScaleAndCrop);
                ExperimentGUI.Panel(new Rect(170,570,940,90));ExperimentGUI.Label(new Rect(200,591,880,45),_memoryCaption,true);
                float fade=Mathf.Max(1-Mathf.Clamp01(_memoryTime/.3f),Mathf.Clamp01((_memoryTime-2.4f)/.4f));
                ExperimentGUI.Box(new Rect(0,0,1280,720),new Color(.015f,.025f,.035f,fade));
            }
            GUI.matrix=matrix;
        }
        private int _hintLevel;
        public bool OpenHints()
        {
            if (Progress==null || _paused || _sequence || _message!=null || CampaignCinematics.IsTransitioning || VarginhaGameHUD.Instance?.IsInventoryOpen==true) return false;
            _hintLevel=0; _panel="hints"; Actor.SetInputLocked(true); return true;
        }
        private void Panel()
        {
            ExperimentGUI.Box(new Rect(0,155,1280,565),new Color(0,0,0,.7f));ExperimentGUI.Panel(new Rect(100,175,1080,465));
            ExperimentGUI.Label(new Rect(135,195,1010,40),_panel=="journal"?"CADERNO • EVIDÊNCIAS":_definition.title,true);
            if(_panel=="complete")
            {
                ExperimentGUI.Label(new Rect(145,260,990,160),_definition.success);
                if(ExperimentGUI.Button(new Rect(420,490,440,50),phase==21?"SALVAR E VOLTAR AO MENU":CampaignSequence.ContinueLabel(phase))){if(phase==21){Menu();return;}Save();Progress.continuation.area=0;Progress.positionPhase=0;CampaignStorySave.Write(Progress);CampaignStorySave.GoTo(CampaignSequence.Next(phase));}
            }
            else if(_panel=="hints") CampaignHints.Draw(Progress,phase,ref _hintLevel);
            else if(_panel=="journal")
            {
                if(ExperimentGUI.Button(new Rect(825,195,300,40),"PRECISO DE UMA DICA • F1")) OpenHints();
                CampaignJournal.Draw(Progress,ref _journalScroll,new Rect(145,250,990,300));
            }
            else if(_panel=="evidence")
            {
                for(int i=0;i<_definition.documents.Length;i++)
                {int index=i;bool seen=(State.clues[phase-11]&(1<<i))!=0;bool canRead=seen||_panel=="evidence"&&phase==11;GUI.enabled=canRead;if(ExperimentGUI.Button(new Rect(145,250+i*64,990,52),(seen?"✓ ":"EXAMINAR NO CENÁRIO • ")+_definition.labels[i]))ReadEvidence(index);GUI.enabled=true;}
                if(_panel=="evidence"&&ExperimentGUI.Button(new Rect(440,500,400,50),"COMPARAR AS CÓPIAS"))OpenPuzzle();
            }
            else if(_panel=="calibration")
            {
                ExperimentGUI.Label(new Rect(145,250,990,60),"Recalibre com as leituras ÁRVORE 3, RIO 1, CAPELA 2. Cada leitura soma duas partes do regulador local e uma do anterior, retornando a 0 após 3.");
                for(int i=0;i<3;i++)
                {
                    int index=i;string name=new[]{"ÁRVORE","RIO","CAPELA"}[i];
                    ExperimentGUI.Label(new Rect(145+i*320,330,300,45),name+" • LEITURA "+State.FinalReading(i));
                    if(ExperimentGUI.Button(new Rect(145+i*320,390,300,55),"REGULADOR "+State.finalRegulators[i])){State.TurnFinalRegulator(index);Save();}
                }
                if(ExperimentGUI.Button(new Rect(145,475,540,50),"SELO DE EDELZIO • "+(State.finalSealActive?"ATIVO":"DESLIGADO"))){State.SetFinalSeal(!State.finalSealActive);Save();}
                if(ExperimentGUI.Button(new Rect(755,500,360,45),"VALIDAR RETORNO SEGURO"))SubmitCalibration();
            }
            else if(_panel=="puzzle")
            {
                ExperimentGUI.Label(new Rect(145,250,990,95),_definition.question);
                for(int i=0;i<_order.Length;i++)
                {
                    int value=_definition.choice?i:_order[i];float w=960f/_order.Length;
                    if(ExperimentGUI.Button(new Rect(145+i*w,370,w-15,105),(_selected==i||_choice==i?"> ":"")+_definition.cards[value]))
                    {if(_definition.choice)_choice=i;else if(_selected<0)_selected=i;else{(_order[_selected],_order[i])=(_order[i],_order[_selected]);_selected=-1;}}
                }
                if(ExperimentGUI.Button(new Rect(755,500,360,45),"CONFIRMAR"))Submit(_choice,_order);
            }
            if(ExperimentGUI.Button(new Rect(145,570,360,40),"VOLTAR À EXPLORAÇÃO"))_panel=null;
        }
        private void Menu(){Save();Time.timeScale=1;CampaignCinematics.Load("Menu_MisterioDeVarginha");}
        private void OnApplicationPause(bool value){if(value)Save();}
        private void OnDestroy(){RestoreBossShot();if(Active==this)Active=null;Time.timeScale=1;}
    }
}
