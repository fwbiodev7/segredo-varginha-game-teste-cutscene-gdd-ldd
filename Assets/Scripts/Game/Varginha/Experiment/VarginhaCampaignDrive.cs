using Game.Level;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    public sealed class VarginhaCampaignDrive : MonoBehaviour
    {
        public const string SceneName = "Ato2_Fase3_Nao_Deixa_Ela_Sair";
        public static VarginhaCampaignDrive Active { get; private set; }
        public static bool IsModalOpen => Active != null;
        private enum State { Drive, Breakdown, Restart, Arrived }
        private State _state;
        private CampaignStory _progress;
        private Transform _car;
        private Rigidbody2D _body;
        private BoxCollider2D _box;
        private readonly RaycastHit2D[] _hits=new RaycastHit2D[16];
        private readonly Collider2D[] _contacts=new Collider2D[16];
        [SerializeField] private float forwardSpeed=9,reverseSpeed=4,acceleration=5,coastDeceleration=4,brakeDeceleration=12;
        [SerializeField] private float maximumSteerAngle=32,steerResponse=3.5f,steerReturn=4,wheelbase=1.7f,lateralGrip=18;
        private float _speed,_steer,_yaw;
        private Vector2 _velocity;
        public float Speed=>_speed;
        public float Steering=>_steer;
        private Vector2 _input;
        private float _titleTime, _stateTime, _saveTime;
        private bool _paused, _settings, _inspect;
        private bool _headlights=true;
        private string _feedback;
        private readonly CampaignTimedSubtitle _drivingSubtitle = new(), _inspectionSubtitle = new();
        private AudioSource _radio;
        private AudioClip _static;
        private Sprite _carSprite;
        private CampaignSoundscape _sound;
        private ExperimentFrameComposer _composer;
        private Sprite[] _carFrames;
        private int _detail, _frameTick = -1;
        private SpriteRenderer _carRenderer;
        public CampaignStory Progress => _progress;
        public bool HasBrokenDown => _state == State.Breakdown;
        private void Awake()
        {
            Active = this; _progress = CampaignStorySave.Load(); _progress.phase = 3;
            BuildRoad();
            _state = _progress.arrival ? State.Arrived : _progress.inspection == 7 ? State.Drive : _progress.driveDistance >= 55 ? State.Breakdown : State.Drive;
            _static = VarginhaExperimentLab.CreateStatic(); _radio = gameObject.AddComponent<AudioSource>(); _radio.clip = _static; _radio.loop = true; _radio.volume = .008f;
            _radio.Play(); Save();
            _sound = gameObject.AddComponent<CampaignSoundscape>(); _sound.Configure("Road");
            _composer = new ExperimentFrameComposer();
        }
        private void BuildRoad()
        {
            var plan=CampaignMapPlan.Create(3);CampaignMapConstruction.Build(transform,plan);
            var car = new GameObject("Fusca_TopView_Campanha"); car.transform.SetParent(transform); _car = car.transform;
            var road=CampaignIllustratedMaps.Get(3);
            // The old map spawn is on the west pavement, outside its lane limits.
            // Start the physical car in the lane instead of casting through that curb.
            float laneX=road!=null?road.Position((road.laneBorders[0]+road.laneBorders[1])*.5f,0).x:plan.spawn.x;
            _car.position = new Vector3(laneX, _progress.driveDistance);
            _carFrames = new Sprite[4];
            for (int i = 0; i < 4; i++) _carFrames[i] = CampaignOriginalFusca.Top(CampaignWorkshopVehicle.Facing.North,2f);
            _carSprite = _carFrames[1];
            _carRenderer = car.AddComponent<SpriteRenderer>(); _carRenderer.sprite = _carSprite; _carRenderer.sortingOrder = 5;
            _body = car.AddComponent<Rigidbody2D>(); _body.gravityScale = 0; _body.bodyType=RigidbodyType2D.Kinematic;
            _body.interpolation = RigidbodyInterpolation2D.Interpolate; _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _box=car.AddComponent<BoxCollider2D>();_box.size = new Vector2(1.05f, 1.9f);
            CampaignFuscaLighting.Add(car.transform);
            var cameraObject = new GameObject("Main Camera"); cameraObject.transform.SetParent(transform); cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>(); camera.orthographic = true; camera.orthographicSize = 7;
            camera.transform.position = new Vector3(0, _car.position.y + 3, -10); camera.backgroundColor = new Color(.025f, .04f, .06f);
            cameraObject.AddComponent<AudioListener>(); cameraObject.AddComponent<CameraFollow2D>().ConfigureTopDown(_car);
            VarginhaPixelPresentation.Configure(camera);
        }
        private GameObject Part(string name, Vector2 position, Vector2 size, string motif, Color tint, int order)
        {
            var item = new GameObject(name); item.transform.SetParent(transform); item.transform.position = position;
            var sr = item.AddComponent<SpriteRenderer>(); sr.sprite = motif=="StreetLamp"?CampaignVisualAssets.Prop("Lamp")??VarginhaPixelArtSprites.Create(motif,tint):VarginhaPixelArtSprites.Create(motif,tint); sr.drawMode = SpriteDrawMode.Tiled; sr.size = size; sr.sortingOrder = order;
            return item;
        }
        private void Update()
        {
            if (CampaignCinematics.IsTransitioning) return;
            if ((VarginhaInputActions.PausePressed || VarginhaInputActions.CancelPressed && (_paused || _inspect)))
            { if (_inspect) _inspect = false; else TogglePause(); }
            if (_paused) return;
            if (_titleTime < 3) { _titleTime += Time.unscaledDeltaTime; return; }
            _stateTime += Time.deltaTime;
            _input = Vector2.zero;
            if (_state == State.Drive && !_inspect)
            {
                var keyboard = Keyboard.current;
                if (keyboard != null)
                {
                    _input.x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1 : 0);
                    _input.y = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1 : 0);
                }
                if(VarginhaInputActions.Move.sqrMagnitude>0)_input=VarginhaInputActions.Move;
                if (_progress.driveDistance >= 55 && _progress.inspection != 7) BreakDown();
                if (_progress.driveDistance >= 120) { _progress.arrival = true; _state = State.Arrived; _radio.Stop(); _sound.Engine(false); Save(); }
                _radio.volume = _progress.driveDistance < 25 ? .008f : .025f;
                _sound.Engine(_state == State.Drive, Mathf.Max(0, _input.y));
            }
            else if (_state == State.Breakdown)
            {
                _radio.Stop();
                if (VarginhaInputActions.CarPressed) _inspect = true;
                _sound.Engine(false);
                if (_progress.inspection == 7 && !_inspect) { _state = State.Restart; _stateTime = 0; _frameTick = -1; Save(); }
            }
            else if (_state == State.Restart && _stateTime > 4) { _state = State.Drive; _radio.Play(); _sound.Play("Starter"); _sound.Engine(true); }
            _carRenderer.sprite = _carFrames[_state == State.Breakdown ? 0 : _state == State.Drive ? 1 : 3];
            bool headlights=_state==State.Drive;
            if(headlights!=_headlights){_headlights=headlights;CampaignFuscaLighting.SetEnabled(_car,headlights);}
            _saveTime += Time.deltaTime; if (_saveTime > 5) { _saveTime = 0; Save(); }
        }
        private void FixedUpdate()
        {
            if (_titleTime < 3 || _paused || _inspect || _state != State.Drive || CampaignCinematics.IsTransitioning)
            { _speed=_steer=0;_velocity=Vector2.zero;_body.linearVelocity = Vector2.zero; return; }
            float dt=Time.fixedDeltaTime;
            _speed=CampaignVehiclePhysics.Speed(_speed,_input.y,forwardSpeed,reverseSpeed,acceleration,coastDeceleration,brakeDeceleration,dt);
            _steer=Mathf.MoveTowards(_steer,_input.x,Mathf.Max(.1f,Mathf.Abs(_input.x)<.01f?steerReturn:steerResponse)*dt);
            float nextYaw=_yaw-_speed/Mathf.Max(.5f,wheelbase)*Mathf.Tan(_steer*Mathf.Clamp(maximumSteerAngle,1,55)*Mathf.Deg2Rad)*Mathf.Rad2Deg*dt;
            var filter=new ContactFilter2D{useTriggers=false};filter.SetLayerMask(~0);
            int overlaps=Physics2D.OverlapBox(_body.position,_box.size,nextYaw,filter,_contacts);bool clear=true;
            for(int i=0;i<overlaps;i++)if(_contacts[i].attachedRigidbody!=_body){clear=false;break;}
            if(clear){_yaw=nextYaw;_body.MoveRotation(_yaw);}
            Vector2 forward=new(-Mathf.Sin(_yaw*Mathf.Deg2Rad),Mathf.Cos(_yaw*Mathf.Deg2Rad));
            _velocity=Vector2.Lerp(_velocity,forward*_speed,1-Mathf.Exp(-Mathf.Max(.1f,lateralGrip)*dt));
            Vector2 target=_body.position+_velocity*dt;
            var road=CampaignIllustratedMaps.Get(3);
            float left=road!=null?road.Position(road.laneBorders[0],0).x+.55f:-3.8f,right=road!=null?road.Position(road.laneBorders[1],0).x-.55f:3.8f;
            target.x = Mathf.Clamp(target.x,left,right); target.y = Mathf.Clamp(target.y, 0, 120);
            Vector2 delta=target-_body.position;
            if(delta.sqrMagnitude<=1e-12f&&_velocity.sqrMagnitude>.001f){_speed=0;_velocity=Vector2.zero;}
            float distance=CampaignVehiclePhysics.ClearDistance(_body,_box,_yaw,delta,_hits);
            if(distance<delta.magnitude){_speed=0;_velocity=Vector2.zero;}
            if(delta.sqrMagnitude>1e-12f)_body.MovePosition(_body.position+delta.normalized*distance);
            _progress.driveDistance=_body.position.y;
        }
        public void BreakDown() { _state = State.Breakdown; _input = Vector2.zero; _body.linearVelocity = Vector2.zero; _radio.Stop(); _sound.Engine(false); _sound.Play("Key"); _inspect = true; _detail = 0; _frameTick = -1; Save(); }
        public void Inspect(int index)
        {
            if (_state != State.Breakdown || index < 0 || index > 2) return;
            _progress.inspection |= 1 << index;
            _detail = index == 1 ? 2 : 1; _frameTick = -1;
            _sound.Play(index == 0 ? "Starter" : index == 1 ? "UI" : "Key");
            _feedback = index == 0 ? "IGNIÇÃO: o motor não responde. A chave gira sozinha de volta."
                : index == 1 ? "RÁDIO: desligado, mas o ruído continua. A lembrança de uma voz infantil: 'Não deixa ela sair'."
                : "PAINEL: o velocímetro indica movimento, embora o Fusca esteja parado. O relógio pisca 23:23.";
            Save();
        }
        public void CloseInspection() { _inspect = false; }
        public void TogglePause() { _paused = !_paused; _settings = false; Time.timeScale = _paused ? 0 : 1; CampaignCinematics.Pause(_paused); _sound.Suspend(_paused); if (_paused) _radio.Pause(); else if (_state == State.Drive) _radio.UnPause(); }
        public void Save() { if (_progress != null) CampaignStorySave.Write(_progress); }
        private void OnGUI()
        {
            VarginhaGamepadUI.Begin("drive:"+GetEntityId(),true,50);
            ExperimentGUI.Init(); GUI.depth = -3100;
            if(_titleTime<3){CampaignChapterPreview.Draw(3,_titleTime);return;}
            if(_state==State.Arrived)CampaignChapterPreview.Background(4);
            else if(_inspect||_state==State.Restart)ExperimentGUI.Box(new Rect(0,0,Screen.width,Screen.height),Color.black);
            var matrix=ExperimentGUI.BeginCanvas();
            if (_state == State.Arrived)
            {
                var facade = Resources.Load<Texture2D>(VarginhaIndustrialSchoolFacade.ResourcePath); if (facade != null) GUI.DrawTexture(new Rect(64, 110, 1152, 365), facade, ScaleMode.ScaleToFit);
                ExperimentGUI.Panel(new Rect(150, 460, 980, 145));
                ExperimentGUI.Label(new Rect(180, 481, 920, 90), "INDUSTRIAL • 2026\nO Fusca para em frente à escola. A rotina continua, mas você não consegue esquecer a pane.");
                if (VarginhaGamepadUI.CarButton(new Rect(420, 627, 440, 45), "ENTRAR • FASE 4")) CampaignStorySave.GoTo(4);
            }
            else if (_inspect || _state == State.Restart) DrawInspection();
            else
            {
                ExperimentGUI.Objective("ATO II • FASE 3 • 2026", "NÃO DEIXE ELA SAIR", _state == State.Breakdown ? "O motor parou. ["+VarginhaInputActions.CarLabel+"] examine ignição, rádio e painel."
                    : _state == State.Restart ? "Silêncio. O motor recomeça sozinho..." : "W / ↑: ACELERAR • A/D: DIREÇÃO • S / ↓: RECUAR");
                if (!_paused && CampaignHudIcons.Button(1174, CampaignHudIcons.Icon.Pause, "ESC", "Pausa")) TogglePause();
                string signal = _progress.driveDistance < 25 ? "RÁDIO: previsão do tempo e notícias locais."
                    : _progress.driveDistance < 45 ? "O rádio perde o sinal. O farol parece iluminar a mesma rua outra vez."
                    : "No retrovisor, a lembrança de uma criança: NÃO DEIXE ELA SAIR.";
                _drivingSubtitle.Draw(signal,_stateTime);
            }
            if (_paused)
            {
                if (_settings) { GUI.matrix = matrix; if (VarginhaGameSettings.Draw()) _settings = false; return; }
                int action = ExperimentGUI.PausePanel("FASE 3 • NÃO DEIXE ELA SAIR");
                if (action == 1) TogglePause();
                if (action == 2) _settings = true;
                if (action == 3) { Save(); Time.timeScale = 1; CampaignCinematics.Load("Menu_MisterioDeVarginha"); }
            }
            GUI.matrix = matrix;
        }
        private void DrawInspection()
        {
            int tick = Mathf.FloorToInt(Time.unscaledTime * 12);
            if (_frameTick != tick)
            {
                _composer.Compose(new ExperimentShot { asset="FuscaBreakdown", panel=_state == State.Restart ? 3 : _detail, mode=_detail == 2 ? "interference" : "still" }, _stateTime, VarginhaGameSettings.Current.reducedMotion);
                _frameTick = tick;
            }
            GUI.DrawTexture(new Rect(64,36,1152,648), _composer.Frame);
            ExperimentGUI.Panel(new Rect(90,55,940,62));
            ExperimentGUI.Label(new Rect(110,70,900,43), _state == State.Restart ? "QUATRO SEGUNDOS DE SILÊNCIO" : "FUSCA • ALGO ESTÁ ERRADO", true);
            if (!_paused && CampaignHudIcons.Button(1174, CampaignHudIcons.Icon.Pause, "ESC", "Pausa")) TogglePause();
            _inspectionSubtitle.Draw(_state == State.Restart ? "O rádio está desligado. Ninguém toca a chave.\nO motor recomeça sozinho."
                : _feedback ?? "O Fusca parou no meio da rua. O silêncio é estranho.\nInspecione a ignição, o rádio e o painel.",_stateTime);
            if (_state != State.Restart)
            {
                string[] parts={"IGNIÇÃO","RÁDIO","PAINEL"};
                for(int i=0;i<3;i++) if(VarginhaGamepadUI.CarButton(new Rect(110+i*245,630,230,47),parts[i]+((_progress.inspection & 1<<i)!=0 ? " • VISTO" : ""))) Inspect(i);
                if(VarginhaGamepadUI.CarButton(new Rect(875,630,295,47),_progress.inspection==7 ? "AGUARDAR O MOTOR" : "VOLTAR AO CARRO")) CloseInspection();
            }
        }
        private void OnApplicationPause(bool value) { if (value) Save(); }
        private void OnDestroy()
        {
            if (Active == this) Active = null; Time.timeScale = 1;
            if (_static != null) Destroy(_static); if (_carFrames != null) foreach(var sprite in _carFrames) if(sprite!=null) Destroy(sprite); _composer?.Dispose();
        }
    }
}
