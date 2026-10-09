using System.Collections.Generic;
using Game.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    public sealed class CampaignExpansionController : MonoBehaviour
    {
        public int phase = 6;
        public static CampaignExpansionController Active { get; private set; }
        public static string SceneName(int number)
        {
            string[] names = { "Fragmentos", "A_Mata", "A_Ancora", "Ouzana", "O_Fusca_Marcado" };
            if (number < 6 || number > 10) throw new System.ArgumentOutOfRangeException(nameof(number));
            return "Ato" + (number <= 8 ? 3 : 4) + "_Fase" + number + "_" + names[number - 6];
        }
        public CampaignStory Progress { get; private set; }
        public CampaignMapPlan Plan { get; private set; }
        private CampaignExpansionState State => Progress.expansion;
        private EdelzioTopDownController _actor;
        private CampaignSoundscape _sound;
        private string _message, _panel;
        private Vector2 _journalScroll;
        private bool _paused, _settings, _hidden, _driving, _parking, _departing;
        private CampaignWorkshopVehicle _vehicle;
        private float _departureTime;
        private Vector2 _departureStart;
        private int _selected = -1, _year = 1898, _symbol, _record = 1;
        private float _title, _saveTimer, _routeTimer, _driveTime;
        private SpriteRenderer _entity;
        private readonly List<Vector2> _chasePath = new();
        private Vector2 _safePoint;
        private string _guideTarget;
        private bool Blocked => _paused || _title < 2.5f || _message != null || _panel != null || VarginhaGameHUD.Instance?.IsInventoryOpen == true || _actor?.GetComponent<VarginhaPlayerActionAnimation>()?.IsActing == true;
        private string Objective => phase == 6 ? "Examine livro, globo e painel na biblioteca da Industrial. Alinhe os fragmentos na mesa central."
            : phase == 7 ? "Árvore → rio → capela. Use o esconderijo durante a perseguição."
            : phase == 8 ? "Cruze ano, símbolo e número no Livro do Tombo."
            : phase == 9 ? "Compare as leituras com Ouzana e receba o reagente para a oficina."
            : _parking ? "Conduza o Fusca para a vaga na oficina. Pare voltado para cima e pressione "+VarginhaInputActions.CarLabel+"."
            : _departing ? "As marcas continuam. A investigação também."
            : "Use o reagente uma vez no Fusca para revelar as três marcas necessárias.";
        private void Awake()
        {
            Active = this; Plan = CampaignMapPlan.Create(phase); CampaignMapConstruction.Build(transform, Plan);
            _actor = CampaignMapConstruction.CreatePlayer(transform, Plan);
        }
        private void Start()
        {
            Progress = CampaignStorySave.Load(); Progress.phase = phase; _safePoint = phase == 7 && State.forestSigns > 0 ? Plan.points.Find(p=>p.id=="sign1").position : Plan.spawn;
            _year = State.anchorYear; _symbol = State.anchorSymbol; _record = State.anchorRecord;
            _actor.HasBackpack = _actor.HasResearchNotebook = _actor.HasFuscaKey = _actor.HasDecodedData = true;
            if (VarginhaGameHUD.Instance == null)
            {
                var hud = new GameObject("Campaign_HUD"); hud.transform.SetParent(transform); hud.AddComponent<VarginhaGameHUD>();
            }
            VarginhaGameHUD.Instance.CampaignInventoryOnly = true; VarginhaGameHUD.Instance.enabled = true;
            _actor.CanDodge = false; _actor.SetCombatLocked(false);
            if (Progress.positionPhase == phase && Plan.IsClear(new Vector2(Progress.x, Progress.y - .58f))) _actor.transform.position = new Vector3(Progress.x, Progress.y);
            _sound = gameObject.AddComponent<CampaignSoundscape>(); _sound.Configure(phase == 7 ? "Forest" : phase == 8 ? "Church" : phase == 9 ? "Lab" : phase == 10 ? "Workshop" : "House", _actor);
            if (phase == 7) CreateEntity();
            if(phase==9)CreateExitDoor();
            if (phase == 10)
            {
                var car=transform.Find("Mapa_Campanha/02_Mobilia_Colisoes/Fusca");
                _parking=!State.workshopParked;
                if(_parking)_actor.gameObject.SetActive(false);
                _vehicle=car.gameObject.AddComponent<CampaignWorkshopVehicle>();_vehicle.Configure(State);
                if(_parking){Camera.main.GetComponent<Game.Level.CameraFollow2D>().Target=car;_sound.Engine(true);}
                for (int i = 0; i < 3; i++) if ((State.sprayed & (1 << i)) != 0) RevealMark(i);
            }
            if (phase == 7 || phase == 8) CreateNPC("Padre Fábio", VarginhaReferenceSprites.PadreFabio(), Plan.points.Find(p=>p.id==(phase==7?"fabio":"trust")).position+new Vector2(phase==7?-.55f:.55f,.6f));
            if (phase == 9) CreateNPC("Ouzana", CampaignStorySprites.Frame("OuzanaBiologist",0,0),Plan.points.Find(p=>p.id=="ouzana").position+new Vector2(.9f,.6f)).AddComponent<CampaignOuzanaBiologist>();
            GameManager.Instance?.StartGame(); Save(); Lock();
        }
        private GameObject CreateNPC(string name, Sprite sprite, Vector2 position)
        {
            var go = new GameObject(name); go.transform.SetParent(transform); go.transform.position = position;
            var renderer = go.AddComponent<SpriteRenderer>(); renderer.sprite = sprite;
            var collider = go.AddComponent<CircleCollider2D>(); collider.radius = .23f; collider.offset = new Vector2(0,-.6f);
            VarginhaWorldDepth.Ensure(renderer, ground: collider);
            return go;
        }
        private void CreateExitDoor()
        {
            var point=Plan.points.Find(p=>p.id=="exit");point.label="PORTA DE SAÍDA • IR À OFICINA";
            var go=new GameObject("Porta_de_saida_Ouzana");go.transform.SetParent(transform,false);
            go.transform.position=point.position+Vector2.down*.22f;
            var sr=go.AddComponent<SpriteRenderer>();sr.sprite=CampaignVisualAssets.Prop("Door");
            float fit=1.1f/Mathf.Max(.01f,sr.sprite.bounds.size.x);go.transform.localScale=Vector3.one*fit;
            VarginhaWorldDepth.Ensure(sr,offset:2);
        }
        private void CreateEntity()
        {
            var go = new GameObject("Manifestação não combatível"); go.transform.SetParent(transform); go.transform.position = new Vector3(-8,2);
            _entity = go.AddComponent<SpriteRenderer>(); _entity.sprite = VarginhaPixelArtSprites.Create("ET_Subordinate_Entity", Color.gray);
            _entity.color = new Color(.19f,.23f,.22f,.6f); VarginhaWorldDepth.Ensure(_entity, true);
        }
        private void Update()
        {
            if (CampaignCinematics.IsTransitioning) return;
            if (Progress == null) return;
            if (VarginhaGameHUD.Instance?.IsInventoryOpen == true) return;
            if(_paused&&_settings&&VarginhaInputActions.CancelPressed&&!VarginhaInputActions.PausePressed){_settings=false;return;}
            if ((VarginhaInputActions.PausePressed || VarginhaInputActions.CancelPressed && (_paused || _message!=null || _panel!=null)))
            {
                if (_message != null && phase!=10) _message = null;
                else if (_panel != null && _panel != "complete") _panel = null;
                else TogglePause();
                Lock();
            }
            if (_paused) return;
            _title += Time.deltaTime;
            if (Blocked) { _vehicle?.StopInput(); Lock(); return; }
            if (_parking)
            {
                if(_vehicle.TickParking())
                {
                    State.workshopParked=true;_parking=false;_vehicle.ParkAtBay();_sound.Engine(false);
                    _actor.gameObject.SetActive(true);_actor.GetComponent<Rigidbody2D>().position=Plan.points.Find(p=>p.id=="drive").position+Vector2.up*.58f;
                    Camera.main.GetComponent<Game.Level.CameraFollow2D>().Target=_actor.transform;
                    Say("Fusca estacionado. O reagente pode revelar o que ficou escondido na lataria.");Save();
                }
                SaveVehiclePeriodically();Lock();return;
            }
            if (_departing) { Departure();return; }
            if (_driving) { Drive(); return; }
            if (Keyboard.current?.f1Key.wasPressedThisFrame == true) { OpenHints(); return; }
            if (VarginhaInputActions.JournalPressed) { _panel = "journal"; Lock(); return; }
            if (VarginhaInputActions.InventoryPressed) { OpenBackpack(); return; }
            var point=Nearest();
            if(point!=null && (phase==10 ? VarginhaInputActions.CarPressed : VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact))) Interact(point.id);
            if (_hidden && (_actor.transform.position - (Vector3)(Plan.points.Find(p=>p.id=="hide").position+Vector2.up*.58f)).sqrMagnitude > 2) _hidden = false;
            if (phase == 7) Chase();
            _saveTimer += Time.deltaTime; if (_saveTimer > 5) { _saveTimer = 0; Save(); }
            Lock();
        }
        private void SaveVehiclePeriodically()
        { _saveTimer+=Time.deltaTime;if(_saveTimer>5){_saveTimer=0;Save();} }
        private CampaignMapPlan.Point Nearest()
        {
            CampaignMapPlan.Point closest = null; float distance = 1.45f;
            var feet = (Vector2)_actor.transform.position + new Vector2(0,-.58f);
            foreach (var point in Plan.points)
            {
                float current = Vector2.Distance(feet, point.position);
                if (current >= distance || !Visible(feet, point.position)) continue;
                distance = current; closest = point;
            }
            return closest;
        }
        private bool Visible(Vector2 from, Vector2 to)
        {
            foreach (var hit in Physics2D.LinecastAll(from,to))
                if (hit.collider != null && hit.collider.transform.IsChildOf(transform.Find("Mapa_Campanha/01_Planta_Paredes_Divisoes"))) return false;
            return true;
        }
        public void Interact(string id)
        {
            if (Progress == null || _driving || _parking || _departing) return;
            if(id==_guideTarget)_guideTarget=null;
            _sound?.Play("Paper");
            if (id != "exit" && id != "fusca")
            {
                var point = Plan.points.Find(p => p.id == id);
                if (point != null) CampaignCameraDirector.Reveal(point.position);
            }
            if (id.StartsWith("truth"))
            {
                int bit = int.Parse(id.Substring(5)); State.truthClues |= 1 << bit;
                Say(bit == 0 ? "Um depoimento foi retirado da reportagem de 1996. Pista opcional registrada." : bit == 1
                    ? "O relatório cita um veículo sem placa na área do clarão. Pista opcional registrada."
                    : "1898: Zé Gomes registrou a contenção. O documento liga a inscrição antiga à região da mata. Pista opcional registrada.");
            }
            else if (id == "exit")
            {
                if (!State.Complete(phase)) Say(CampaignGuidance.Next(Progress,phase));
                else if(phase==10&&!State.workshopDeparted)BeginDeparture();
                else { _panel = "complete"; Lock(); }
            }
            else if (phase == 6)
            {
                if (id == "map") Open("map");
                else if (id == "archive") { State.visited |= 1; Say("O livro aberto da biblioteca guarda um registro da região: ÁRVORE, margem oeste. Terceiro fragmento de mapa recolhido. O carimbo de 23:23 corresponde à diocese.\nNota do levantamento: da árvore, a trilha segue para leste."); }
                else if (id == "school") { State.visited |= 2; Say("Uma anotação junto ao globo ajuda a orientar o levantamento: o RIO atravessa o centro, ligando a árvore à capela.\nNota do levantamento: a ponte cruza o rio de oeste para leste."); }
                else if (id == "square") { State.visited |= 4; Say("O painel conserva um relato de 1996: a CAPELA fica a leste. Alinhe ÁRVORE → RIO → CAPELA na mesa central, do oeste ao leste.\nNota do levantamento: a entrada da capela fica ao sul; siga para norte ao chegar."); }
                else if (id.StartsWith("library_shelf")) Say("Estantes da biblioteca da Industrial. Livros de história local, atlas e registros escolares. Os documentos da investigação estão no livro aberto, junto ao globo e no painel de avisos.");
                else if (id.StartsWith("library_plant")) Say("As plantas recebem a luz das janelas. Entre as estantes, a biblioteca continua silenciosa.");
            }
            else if (phase == 7)
            {
                if (id.StartsWith("sign"))
                {
                    int sign = int.Parse(id.Substring(4));
                    if (!State.mapSolved) Say("É preciso alinhar o mapa antes de seguir os símbolos.");
                    else if (sign == State.forestSigns) { State.forestSigns++; if (sign == 1) _safePoint = Plan.points.Find(p=>p.id=="sign1").position; Say(sign == 0 ? "A marca da árvore aponta para a clareira. O ruído acompanha seus passos."
                        : sign == 1 ? "A marca do rio aponta para a ponte. Há algo se movendo entre as árvores. Use o esconderijo ou siga até a capela."
                        : "A marca da capela confirma o caminho. Fábio espera dentro da ruína."); }
                    else Say(sign < State.forestSigns ? "Este símbolo já está no caderno." : "A direção não corresponde ao mapa. Procure o símbolo anterior.");
                }
                else if (id == "hide") { _hidden = !_hidden; Say(_hidden ? "Você se abriga e recupera o fôlego. A presença perde seu rastro enquanto você permanece aqui." : "Você sai do abrigo."); }
                else if (id == "fabio")
                {
                    if (State.forestSigns < 3) Say("Fábio: siga as marcas para entender por que veio até aqui.");
                    else { State.fabioMet = true; Say("Fábio: eu esperava que você se lembrasse. Venha ao arquivo; há um registro com seu nome."); }
                }
            }
            else if (phase == 8)
            {
                if (id == "index") { State.anchorClues |= 1; Say("Índice do Tombo: o registro de Edelzio pertence a 1996. O documento de 1898 é anterior e não substitui essa data."); }
                else if (id == "symbol") { State.anchorClues |= 2; Say("A inscrição do compartimento traz o símbolo ÂNCORA."); }
                else if (id == "record") { State.anchorClues |= 4; Say("Ficha de Edelzio: registro 23. Relacione ano, símbolo e número."); }
                else if (id == "anchor") Open("anchor");
                else if (id == "trust") { if (State.anchorFound) Open("trust"); else Say("Fábio: confira os arquivos antes de me cobrar uma resposta."); }
            }
            else if (phase == 9)
            {
                if (id == "ouzana") { State.evidencePresented = State.anchorFound; if(State.evidencePresented){State.labClues=7;State.SolveSamples();} Say(State.anchorFound ? "Ouzana: o controle é neutro, o resíduo reage. A comparação confirma as marcas de contenção. Leve o reagente à oficina; as leituras ficaram no caderno." : "Ouzana precisa do registro de Edelzio e das evidências da mata."); }
                else if (id == "control") { State.labClues |= 1; Say("Controle: leitura neutra. É o primeiro passo do protocolo."); }
                else if (id == "residue") { State.labClues |= 2; Say("Resíduo: leitura instável. Compare com o controle antes de aplicar o reagente."); }
                else if (id == "protocol") { State.labClues |= 4; Say("Protocolo: CONTROLE → RESÍDUO → REAGENTE. A reação deve ser observada depois da comparação."); }
                else if (id == "samples") Say("Controle neutro e resíduo instável: Ouzana confirma a análise e entrega o reagente ao receber o registro de Edelzio.");
                else if (id == "tutorial") Say(State.reagentUnlocked ? "O reagente revela uma marca invisível na amostra. Este teste não consome cargas. Ouzana entrega seis cargas para a oficina." : "Complete o teste da bancada antes de usar o reagente.");
            }
            else if (phase == 10)
            {
                if (id.StartsWith("spray"))
                {
                    int region = int.Parse(id.Substring(5));
                    if (State.Spray(region)) { for(int i=0;i<3;i++)RevealMark(i); Say("Uma aplicação revela ÁRVORE • I, RIO • II e CAPELA • III. O reagente confirma a estabilidade do Fusca; as três marcas estão no caderno."); }
                    else Say((State.sprayed & (1 << region)) != 0 ? "Esta marca já foi registrada." : "Reagente indisponível. Confira a bancada de reserva.");
                }
                else if (id == "seal") Say(State.stabilized ? "ÁRVORE • I, RIO • II e CAPELA • III registrados. O Fusca pode partir." : "Aplique o reagente no Fusca para revelar as marcas.");
                else if (id == "refill") { if (State.reagentUnlocked) State.reagentCharges = 6; Say("Reserva recarregada. Ouzana deixou reagente suficiente para repetir os testes sem bloquear a investigação."); }
                else if (id == "drive") { if (!State.stabilized) Say("Revele as marcas com o reagente antes de partir."); else BeginDeparture(); }
            }
            Save(); Lock();
        }
        private void RevealMark(int region)
        {
            var car=transform.Find("Mapa_Campanha/02_Mobilia_Colisoes/Fusca");
            var marks=car.GetComponent<CampaignReagentMarks>();if(marks==null)marks=car.gameObject.AddComponent<CampaignReagentMarks>();
            marks.Reveal(region,_title>=2.5f);
        }
        private void Chase()
        {
            bool active = State.forestSigns >= 2 && !State.fabioMet; _entity.gameObject.SetActive(active);
            if (!active || Blocked) return;
            if (_hidden) { _actor.RestoreSanity(Time.deltaTime * 7); return; }
            _routeTimer -= Time.deltaTime;
            Vector2 feet = (Vector2)_actor.transform.position + new Vector2(0,-.58f);
            if (_routeTimer <= 0) { _routeTimer = .8f; Plan.Route(_entity.transform.position, feet, _chasePath); }
            if (_chasePath.Count > 0)
            {
                _entity.transform.position = Vector2.MoveTowards(_entity.transform.position,_chasePath[0],Time.deltaTime * 2.15f);
                if (Vector2.Distance(_entity.transform.position,_chasePath[0]) < .08f) _chasePath.RemoveAt(0);
            }
            if (Vector2.Distance(_entity.transform.position,feet) < 3) _actor.DrainSanity(Time.deltaTime * 3);
            if (Vector2.Distance(_entity.transform.position,feet) < .7f || _actor.CurrentSanity <= 0) ReturnToCheckpoint();
        }
        public void ReturnToCheckpoint()
        {
            _actor.GetComponent<Rigidbody2D>().position = _safePoint + new Vector2(0,.58f); _actor.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            _actor.RestoreSanity(100); _entity.transform.position = new Vector3(-8,2); _chasePath.Clear();
            Say("A presença alcançou você. Retorno ao último ponto seguro; as pistas continuam no caderno."); Save();
        }
        private void BeginDrive()
        {
            _driving = true; _driveTime = 0; _actor.gameObject.SetActive(false);
            _vehicle.BeginTrack();_vehicle.SetPosition(new Vector2(-8,-4.5f));
            var camera = Camera.main; camera.GetComponent<Game.Level.CameraFollow2D>().enabled = false; camera.transform.position = new Vector3(0,-1,-10); camera.orthographicSize = 6;
            _sound.Engine(true); _sound.Play("Starter");
        }
        private void Drive()
        {
            _driveTime += Time.deltaTime;
            var car = transform.Find("Mapa_Campanha/02_Mobilia_Colisoes/Fusca");
            float movement = (Keyboard.current?.dKey.isPressed == true || Keyboard.current?.rightArrowKey.isPressed == true ? 1 : 0)
                - (Keyboard.current?.aKey.isPressed == true || Keyboard.current?.leftArrowKey.isPressed == true ? 1 : 0);
            if(VarginhaInputActions.Move.sqrMagnitude>0)movement=VarginhaInputActions.Move.x;
            _vehicle.SetPosition(new Vector2(Mathf.Clamp(car.position.x + movement * Time.deltaTime * 4,-8,8),-4.5f));
            if(movement!=0)_vehicle.SetFacing(movement<0?CampaignWorkshopVehicle.Facing.West:CampaignWorkshopVehicle.Facing.East);
            if (car.position.x < 7.5f) return;
            // Keep the parked collider off the exit where Edelzio leaves the vehicle.
            _vehicle.SetPosition(new Vector2(7.5f,-4.5f));_vehicle.FinishTrack();
            State.testDriven = true; _driving = false; _actor.gameObject.SetActive(true);
            _actor.GetComponent<Rigidbody2D>().position = Plan.points.Find(p=>p.id=="exit").position+Vector2.up*.58f;
            Camera.main.GetComponent<Game.Level.CameraFollow2D>().enabled = true; _sound.Engine(false);
            Say("O estabilizador resistiu ao teste. O rádio voltou, mas as marcas permanecem. A investigação continua."); Save();
        }
        private void BeginDeparture()
        {
            _departing=true;_departureTime=0;_departureStart=_vehicle.transform.position;
            _actor.gameObject.SetActive(false);_vehicle.BeginDeparture();
            Camera.main.GetComponent<Game.Level.CameraFollow2D>().enabled=false;
            Camera.main.transform.position=new Vector3(0,-1,-10);
            _sound.Engine(true);_sound.Play("Starter");Lock();
        }
        private void Departure()
        {
            _departureTime+=Time.deltaTime;
            float duration=VarginhaGameSettings.Current.reducedMotion?2:3.2f;
            float t=Mathf.Clamp01(_departureTime/duration);
            _vehicle.transform.position=Vector2.Lerp(_departureStart,new Vector2(Plan.bounds.xMax+4,_departureStart.y),Mathf.SmoothStep(0,1,t));
            if(t<1)return;
            State.workshopDeparted=true;_departing=false;_sound.Engine(false);_panel="complete";Save();Lock();
        }
        private void Open(string panel) { _panel = panel; _selected = -1; Lock(); }
        private void Say(string message) { _message = message; Lock(); }
        public void ClosePanel() { _message = _panel = null; Lock(); }
        private void Lock() => _actor?.SetInputLocked(Blocked || _driving || _parking || _departing);
        private void TogglePause()
        {
            _paused = !_paused; _settings = false; Time.timeScale = _paused ? 0 : 1;
            _vehicle?.StopInput();
            CampaignCinematics.Pause(_paused); _sound.Suspend(_paused); Lock();
        }
        public bool OpenBackpack()
        {
            if (Blocked || _driving || _parking || _departing) return false;
            return VarginhaGameHUD.Instance != null && VarginhaGameHUD.Instance.OpenBackpack();
        }
        public bool SubmitPuzzle()
        {
            bool result = _panel == "map" ? State.SolveMap() : _panel == "anchor" ? State.SolveAnchor(_year,_symbol,_record)
                : _panel == "samples" ? State.SolveSamples() : _panel == "seal" && State.Stabilize();
            if (result) { _panel = null; _sound.Play("Success"); Say(phase == 6 ? "As coordenadas revelam a entrada da mata. Destino registrado."
                : phase == 8 ? "SELO DA ENTIDADE: EDELZIO / CONTENÇÃO: ESTÁVEL. Você é o selo que mantém a entidade contida desde 1996."
                : phase == 9 ? "A reação contradiz as leituras normais. Ouzana aceita ajudar e entrega o reagente."
                : "As marcas reproduzem o trajeto árvore, rio e capela. As marcas estão registradas; o Fusca pode partir."); }
            else Say(_panel=="map"?"Confira os marcos e suas setas: a trilha precisa conectar árvore, ponte e entrada da capela. Os registros continuam no caderno.":"A combinação não corresponde às pistas. Confira os registros e tente novamente; nenhuma pista foi perdida.");
            Save(); return result;
        }
        public void Save()
        {
            if (Progress == null || _actor == null) return;
            // A later visit to the book must not overwrite its solved combination.
            if (State.anchorFound) { _year = 1996; _symbol = 1; _record = 23; }
            State.anchorYear = _year; State.anchorSymbol = _symbol; State.anchorRecord = _record;
            if(_parking)_vehicle.SaveParking(State);
            Progress.positionPhase = phase; Progress.x = _actor.transform.position.x; Progress.y = _actor.transform.position.y; CampaignStorySave.Write(Progress);
        }
        private void OnGUI()
        {
            VarginhaGamepadUI.Begin("expansion:"+GetEntityId()+":"+_panel+":"+_paused+":"+(_message!=null),_panel!=null||_message!=null||_paused,50,phase==10&&!_paused&&(_message!=null||_panel=="complete"||_panel=="seal"));
            if (Progress == null) return;
            if (VarginhaGameHUD.Instance?.IsInventoryOpen == true) return;
            GUI.depth = -3100;
            if (_title < 2.5f)
            {
                CampaignChapterPreview.Draw(phase,_title,2.5f);return;
            }
            ExperimentGUI.Init(); var matrix = ExperimentGUI.BeginCanvas();
            if(!_departing)
            {
                ExperimentGUI.Objective(CampaignSequence.Heading(phase) + " • 2026", CampaignSequence.Title(phase).ToUpperInvariant(), CampaignGuidance.Next(Progress,phase));
                if(!Blocked&&!_parking&&!_departing)CampaignGuidance.DrawMarker(Plan,Progress,phase,waypoint:_guideTarget);
                bool hudEnabled=GUI.enabled; GUI.enabled=hudEnabled&&!_paused&&_panel==null&&_message==null;
                if (CampaignHudIcons.Button(1010, CampaignHudIcons.Icon.Notebook, "TAB", "Caderno") && !Blocked && !_driving && !_parking) { _panel = "journal"; Lock(); }
                if (CampaignHudIcons.Button(1092, CampaignHudIcons.Icon.Backpack, "G", "Mochila")) OpenBackpack();
                if (CampaignHudIcons.Button(1174, CampaignHudIcons.Icon.Pause, "ESC", "Pausa")) TogglePause();
                GUI.enabled=hudEnabled;
            }
            if (_departing)
            {
                var canvas=GUI.matrix;GUI.matrix=matrix;
                float bar=Screen.height*75/720f;
                ExperimentGUI.Box(new Rect(0,0,Screen.width,bar),Color.black);ExperimentGUI.Box(new Rect(0,Screen.height-bar,Screen.width,bar),Color.black);
                float duration=VarginhaGameSettings.Current.reducedMotion?2:3.2f;
                float fade=Mathf.Clamp01((_departureTime/duration-.78f)/.22f);
                if(fade>0)ExperimentGUI.Box(new Rect(0,0,Screen.width,Screen.height),new Color(.008f,.015f,.025f,fade));
                GUI.matrix=canvas;
                ExperimentGUI.Caption(new Rect(200,661,880,45),"As marcas continuam. E a estrada também.",VarginhaGameSettings.Current.subtitleSize);
            }
            else if (_parking)
            {
                ExperimentGUI.Panel(new Rect(190,648,900,50));
                ExperimentGUI.Label(new Rect(210,660,870,30),_vehicle.CanPark?"["+VarginhaInputActions.CarLabel+"] ESTACIONAR E SAIR DO FUSCA":(VarginhaInputActions.UsingGamepad?"ANALÓGICO / D-PAD • CONDUZIR     A • ESTACIONAR     START • PAUSA":"WASD / SETAS • CONDUZIR     W • ESTACIONAR     ESC • PAUSA"),small:true);
            }
            else if (_driving) ExperimentGUI.Label(new Rect(200,620,950,70), "A / D OU SETAS • conduza até o fim da pista. ESC • pausa");
            else
            {
                var point = Nearest(); if(VarginhaGameSettings.Current.interactionHints)
                {
                ExperimentGUI.Panel(new Rect(190,648,900,50));
                ExperimentGUI.Label(new Rect(210,660,870,30), _hidden ? "ABRIGADO • E: sair • mover-se abandona o abrigo"
                    : point == null ? "WASD / SETAS • ANDAR     E • EXAMINAR" : "["+(phase==10?VarginhaInputActions.CarLabel:VarginhaInputActions.InteractLabel)+"] "+point.label, small:true);
                }
            }
            if (phase == 7) ExperimentGUI.Label(new Rect(35,158,610,40), "SANIDADE " + Mathf.RoundToInt(_actor.CurrentSanity) + " • MARCAS " + State.forestSigns + "/3",small:true);
            if (phase == 10 && !_departing && !_parking) ExperimentGUI.Label(new Rect(35,158,610,40), "REAGENTE " + State.reagentCharges + " • MARCAS " + Count(State.sprayed) + "/3",small:true);
            if (_panel != null) DrawPanel();
            if (_message != null)
            {
                ExperimentGUI.Panel(new Rect(150,395,980,235));
                ExperimentGUI.Caption(new Rect(180,420,920,145),_message,VarginhaGameSettings.Current.subtitleSize);
                if (phase==10 ? VarginhaGamepadUI.CarButton(new Rect(820,570,270,42),"CONTINUAR") : ExperimentGUI.Button(new Rect(820,570,270,42),"CONTINUAR")) { _message = null; Lock(); }
            }
            if (_paused)
            {
                if (_settings) { GUI.matrix = matrix; if (VarginhaGameSettings.Draw()) _settings = false; return; }
                int action = ExperimentGUI.PausePanel("FASE " + CampaignSequence.Chapter(phase) + " • " + Plan.title);
                if (action == 1) TogglePause();
                if (action == 2) _settings = true;
                if (action == 3) Menu();
            }
            GUI.matrix = matrix;
        }
        private static int Count(int bits) => (bits & 1) + ((bits >> 1) & 1) + ((bits >> 2) & 1);
        private int _hintLevel;
        public bool OpenHints()
        {
            if (Progress == null || _paused || _parking || _departing || _driving || _message != null || CampaignCinematics.IsTransitioning || VarginhaGameHUD.Instance?.IsInventoryOpen == true) return false;
            _hintLevel=0; _panel="hints"; Lock(); return true;
        }
        private void DrawPanel()
        {
            ExperimentGUI.Box(new Rect(0,115,1280,605),new Color(0,0,0,.8f)); ExperimentGUI.Panel(new Rect(125,145,1030,480));
            if (_panel == "complete")
            {
                ExperimentGUI.Label(new Rect(165,175,950,55),phase==9?"ANÁLISE CONCLUÍDA • IR À OFICINA":"FASE " + CampaignSequence.Chapter(phase) + " CONCLUÍDA",true);
                ExperimentGUI.Label(new Rect(165,250,950,155),phase == 10 ? "O rádio recebe uma ligação. Renan: Edelzio, preciso que você volte. A imagem está diferente da que imprimimos. A cópia em papel conservou o detalhe anterior."
                    : "Pistas e progresso salvos. Próxima etapa: " + CampaignSequence.Title(CampaignSequence.Next(phase)) + ".");
                if (phase==10 ? VarginhaGamepadUI.CarButton(new Rect(365,465,550,50),CampaignSequence.ContinueLabel(phase)) : ExperimentGUI.Button(new Rect(365,465,550,50),CampaignSequence.ContinueLabel(phase))) { Save(); CampaignStorySave.GoTo(CampaignSequence.Next(phase)); }
                if (ExperimentGUI.Button(new Rect(365,535,550,45),"SALVAR E VOLTAR AO MENU")) Menu(); return;
            }
            if (_panel == "hints") CampaignHints.Draw(Progress,phase,ref _hintLevel);
            else if (_panel == "journal")
            {
                ExperimentGUI.Label(new Rect(165,170,950,45),"CADERNO • REGISTROS",true);
                if (ExperimentGUI.Button(new Rect(710,175,405,40),"PRECISO DE UMA DICA • F1")) OpenHints();
                CampaignJournal.Draw(Progress,ref _journalScroll,new Rect(165,240,950,250));
                if (phase == 6)
                {
                    string[] names = { "LIVRO", "GLOBO", "PAINEL" }, ids = { "archive", "school", "square" };
                    for (int i = 0; i < 3; i++) if (ExperimentGUI.Button(new Rect(165+i*320,470,300,42),"MARCAR "+names[i])) { _guideTarget=ids[i]; ClosePanel(); Save(); }
                }
            }
            else if (_panel == "trust")
            {
                ExperimentGUI.Label(new Rect(165,170,950,45),"FÁBIO • A MEMÓRIA DE 1996",true);
                ExperimentGUI.Label(new Rect(165,245,950,150),"Fábio: mantive o registro porque sabia que você voltaria. Não tenho todas as respostas. Precisamos procurar Ouzana.");
                if (ExperimentGUI.Button(new Rect(165,435,455,55),"PEDIR UMA EXPLICAÇÃO")) { State.trust = 1; ClosePanel(); Save(); }
                if (ExperimentGUI.Button(new Rect(650,435,455,55),"CONFRONTAR FÁBIO")) { State.trust = -1; ClosePanel(); Save(); }
            }
            else if (_panel == "anchor")
            {
                ExperimentGUI.Label(new Rect(165,170,950,45),"LIVRO DO TOMBO",true);
                ExperimentGUI.Label(new Rect(165,245,950,100),"Combine as três pistas encontradas no arquivo e no subterrâneo. 1898 descreve a contenção anterior; a ficha de Edelzio é de 1996.");
                if (ExperimentGUI.Button(new Rect(165,365,280,55),"ANO: " + _year)) { _year = _year == 1898 ? 1996 : 1898; Save(); }
                if (ExperimentGUI.Button(new Rect(475,365,280,55),"SÍMBOLO: " + (_symbol == 0 ? "ÁRVORE" : "ÂNCORA"))) { _symbol = 1-_symbol; Save(); }
                if (ExperimentGUI.Button(new Rect(785,365,280,55),"REGISTRO: " + _record)) { _record = _record == 1 ? 23 : 1; Save(); }
                if (ExperimentGUI.Button(new Rect(785,475,280,50),"ABRIR COMPARTIMENTO")) SubmitPuzzle();
            }
            else if(_panel=="map")
                CampaignPuzzleDesign.DrawMap(State,ref _selected,()=>SubmitPuzzle(),Save);
            else
            {
                bool samples = _panel == "samples"; int[] order = samples ? State.sampleOrder : _panel == "map" ? State.mapOrder : State.sealOrder;
                string[] labels = samples ? new[] { "CONTROLE", "RESÍDUO", "REAGENTE" } : new[] { "ÁRVORE • OESTE", "RIO • CENTRO", "CAPELA • LESTE" };
                ExperimentGUI.Label(new Rect(165,170,950,45),samples ? "PROTOCOLO DA AMOSTRA" : _panel == "map" ? "SOBREPOSIÇÃO CARTOGRÁFICA" : "MARCAS DO FUSCA",true);
                ExperimentGUI.Label(new Rect(165,245,950,100),"Clique em duas peças para trocar suas posições. Consulte as pistas físicas antes de confirmar.");
                for (int i = 0; i < 3; i++) if (ExperimentGUI.Button(new Rect(165+i*320,365,290,85),(_selected == i ? "SELECIONADO\n" : "")+labels[order[i]]))
                {
                    if (_selected < 0) _selected = i; else { (order[_selected],order[i])=(order[i],order[_selected]); _selected=-1; Save(); }
                }
                if (ExperimentGUI.Button(new Rect(785,475,300,50),"CONFIRMAR")) SubmitPuzzle();
            }
            if (ExperimentGUI.Button(new Rect(165,555,300,45),"VOLTAR À EXPLORAÇÃO")) ClosePanel();
        }
        private void Menu() { Save(); Time.timeScale=1; CampaignCinematics.Load("Menu_MisterioDeVarginha"); }
        private void OnApplicationPause(bool pause) { if (pause) Save(); }
        private void OnDestroy() { if (Active == this) Active = null; Time.timeScale = 1; }
    }
}
