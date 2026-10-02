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
        private Vector2 _input;
        private float _titleTime, _stateTime, _saveTime;
        private bool _paused, _settings, _inspect;
        private string _feedback;
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
            Part("Rua", new Vector2(0, 68), new Vector2(10, 150), "Street_Campaign", new Color(.20f, .23f, .26f), 0);
            Part("Calçada_Esquerda", new Vector2(-6, 68), new Vector2(2, 150), "Driveway", new Color(.37f, .36f, .33f), 1);
            Part("Calçada_Direita", new Vector2(6, 68), new Vector2(2, 150), "Driveway", new Color(.37f, .36f, .33f), 1);
            for (int y = -5; y < 142; y += 8)
            {
                Part("Faixa_" + y, new Vector2(0, y), new Vector2(.15f, 2), "RoadMarking", new Color(.68f, .65f, .47f), 1);
                float side = y % 16 == 3 ? -1 : 1;
                var house = Part("Casa_" + y, new Vector2(side * 10, y), new Vector2(5, 5), "Wall_House", new Color(.33f, .28f, .25f), 2);
                house.AddComponent<BoxCollider2D>();
                Part("Telhado_" + y, new Vector2(side * 10, y + .6f), new Vector2(5.2f, 3.5f), "Wall_Roof", new Color(.36f, .17f, .13f), 3);
                Part("Janela_" + y, new Vector2(side * 9.2f, y - 1.6f), new Vector2(.85f, .65f), "Window", new Color(.67f, .69f, .46f), 3);
                Part("Árvore_" + y, new Vector2(-side * 8.2f, y + 2.5f), new Vector2(1.4f, 1.8f), "Tree_Campaign", new Color(.16f, .26f, .19f), 3);
                Part("Poste_" + y, new Vector2(5.7f, y + 3), new Vector2(.6f, 1.8f), "StreetLamp", new Color(.75f, .65f, .40f), 3);
            }
            var car = new GameObject("Fusca_TopView_Campanha"); car.transform.SetParent(transform); _car = car.transform;
            _car.position = new Vector3(-2, _progress.driveDistance);
            var texture = VarginhaExperimentArt.Load("FuscaTopView");
            if (texture == null) throw new System.InvalidOperationException("Arte topview do Fusca ausente.");
            int w = texture.width / 2, h = texture.height / 2; _carFrames = new Sprite[4];
            for (int i = 0; i < 4; i++) _carFrames[i] = Sprite.Create(texture, new Rect(i % 2 * w, i < 2 ? h : 0, w, h), new Vector2(.5f,.5f), h / 3.4f, 0, SpriteMeshType.FullRect);
            _carSprite = _carFrames[1];
            _carRenderer = car.AddComponent<SpriteRenderer>(); _carRenderer.sprite = _carSprite; _carRenderer.sortingOrder = 5;
            _body = car.AddComponent<Rigidbody2D>(); _body.gravityScale = 0; _body.constraints = RigidbodyConstraints2D.FreezeRotation;
            _body.interpolation = RigidbodyInterpolation2D.Interpolate; _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            car.AddComponent<BoxCollider2D>().size = new Vector2(1.05f, 2.35f);
            var cameraObject = new GameObject("Main Camera"); cameraObject.transform.SetParent(transform); cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>(); camera.orthographic = true; camera.orthographicSize = 7;
            camera.transform.position = new Vector3(0, _car.position.y + 3, -10); camera.backgroundColor = new Color(.025f, .04f, .06f);
            cameraObject.AddComponent<AudioListener>(); cameraObject.AddComponent<CameraFollow2D>().ConfigureTopDown(_car);
            VarginhaPixelPresentation.Configure(camera);
        }
        private GameObject Part(string name, Vector2 position, Vector2 size, string motif, Color tint, int order)
        {
            var item = new GameObject(name); item.transform.SetParent(transform); item.transform.position = position;
            var sr = item.AddComponent<SpriteRenderer>(); sr.sprite = VarginhaPixelArtSprites.Create(motif, tint); sr.drawMode = SpriteDrawMode.Tiled; sr.size = size; sr.sortingOrder = order;
            return item;
        }
        private void Update()
        {
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
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
                if (_progress.driveDistance >= 55 && _progress.inspection != 7) BreakDown();
                if (_progress.driveDistance >= 120) { _progress.arrival = true; _state = State.Arrived; _radio.Stop(); _sound.Engine(false); Save(); }
                _radio.volume = _progress.driveDistance < 25 ? .008f : .025f;
                _sound.Engine(_state == State.Drive, Mathf.Max(0, _input.y));
            }
            else if (_state == State.Breakdown)
            {
                _radio.Stop();
                if (VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact)) _inspect = true;
                _sound.Engine(false);
                if (_progress.inspection == 7 && !_inspect) { _state = State.Restart; _stateTime = 0; _frameTick = -1; Save(); }
            }
            else if (_state == State.Restart && _stateTime > 4) { _state = State.Drive; _radio.Play(); _sound.Play("Starter"); _sound.Engine(true); }
            _carRenderer.sprite = _carFrames[_state == State.Breakdown ? 0 : _state == State.Drive ? 1 : 3];
            _saveTime += Time.deltaTime; if (_saveTime > 5) { _saveTime = 0; Save(); }
        }
        private void FixedUpdate()
        {
            if (_titleTime < 3 || _paused || _inspect || _state != State.Drive) { _body.linearVelocity = Vector2.zero; return; }
            Vector2 target = _body.position + new Vector2(_input.x * 4, _input.y * 9) * Time.fixedDeltaTime;
            target.x = Mathf.Clamp(target.x, -3.8f, 3.8f); target.y = Mathf.Clamp(target.y, 0, 120);
            _body.MovePosition(target); _progress.driveDistance = target.y;
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
        public void TogglePause() { _paused = !_paused; _settings = false; Time.timeScale = _paused ? 0 : 1; _sound.Suspend(_paused); if (_paused) _radio.Pause(); else if (_state == State.Drive) _radio.UnPause(); }
        public void Save() { if (_progress != null) CampaignStorySave.Write(_progress); }
        private void OnGUI()
        {
            ExperimentGUI.Init(); GUI.depth = -3100;
            if (_titleTime < 3 || _state == State.Arrived || _inspect || _state == State.Restart) ExperimentGUI.Box(new Rect(0, 0, Screen.width, Screen.height), Color.black);
            var matrix = ExperimentGUI.BeginCanvas();
            if (_titleTime < 3)
            {
                ExperimentGUI.Label(new Rect(230, 230, 880, 60), "ATO II — O CHAMADO", true);
                ExperimentGUI.Label(new Rect(230, 325, 880, 70), "FASE 3 • NÃO DEIXA ELA SAIR\nO caminho até a Industrial.");
            }
            else if (_state == State.Arrived)
            {
                var facade = Resources.Load<Texture2D>(VarginhaIndustrialSchoolFacade.ResourcePath); if (facade != null) GUI.DrawTexture(new Rect(64, 110, 1152, 365), facade, ScaleMode.ScaleToFit);
                ExperimentGUI.Panel(new Rect(150, 460, 980, 145));
                ExperimentGUI.Label(new Rect(180, 481, 920, 90), "INDUSTRIAL • 2026\nO Fusca para em frente à escola. A rotina continua, mas você não consegue esquecer a pane.");
                if (ExperimentGUI.Button(new Rect(420, 627, 440, 45), "ENTRAR • FASE 4")) CampaignStorySave.GoTo(4);
            }
            else if (_inspect || _state == State.Restart) DrawInspection();
            else
            {
                ExperimentGUI.Panel(new Rect(24, 18, 940, 103));
                ExperimentGUI.Label(new Rect(46, 32, 890, 33), "FASE 3 • NÃO DEIXA ELA SAIR", true);
                ExperimentGUI.Label(new Rect(46, 76, 890, 37), _state == State.Breakdown ? "O motor parou. [E] examine ignição, rádio e painel."
                    : _state == State.Restart ? "Silêncio. O motor recomeça sozinho..." : "W / ↑: ACELERAR • A/D: DIREÇÃO • S / ↓: RECUAR • ESC: PAUSAR", small: true);
                if (ExperimentGUI.Button(new Rect(1080, 22, 175, 44), "PAUSAR")) TogglePause();
                string signal = _progress.driveDistance < 25 ? "RÁDIO: previsão do tempo e notícias locais."
                    : _progress.driveDistance < 45 ? "O rádio perde o sinal. O farol parece iluminar a mesma rua outra vez."
                    : "No retrovisor, a lembrança de uma criança: NÃO DEIXA ELA SAIR.";
                if (VarginhaGameSettings.Current.subtitles) { ExperimentGUI.Panel(new Rect(160, 630, 960, 65)); ExperimentGUI.Caption(new Rect(182, 643, 916, 44), signal, VarginhaGameSettings.Current.subtitleSize); }
            }
            if (_paused)
            {
                if (_settings) { GUI.matrix = matrix; if (VarginhaGameSettings.Draw()) _settings = false; return; }
                ExperimentGUI.Box(new Rect(0, 0, 1280, 720), new Color(0, 0, 0, .85f)); ExperimentGUI.Panel(new Rect(390, 195, 500, 345));
                ExperimentGUI.Label(new Rect(420, 215, 440, 50), "PAUSADO", true);
                if (ExperimentGUI.Button(new Rect(420, 290, 440, 45), "CONTINUAR")) TogglePause();
                if (ExperimentGUI.Button(new Rect(420, 355, 440, 45), "CONFIGURAÇÕES")) _settings = true;
                if (ExperimentGUI.Button(new Rect(420, 420, 440, 45), "SALVAR E VOLTAR AO MENU")) { Save(); Time.timeScale = 1; SceneManager.LoadScene("Menu_MisterioDeVarginha"); }
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
            if (ExperimentGUI.Button(new Rect(1070,60,150,43), "PAUSAR")) TogglePause();
            ExperimentGUI.Panel(new Rect(110,472,1060,142));
            ExperimentGUI.Caption(new Rect(135,488,1010,116), _state == State.Restart ? "O rádio está desligado. Ninguém toca a chave.\nO motor recomeça sozinho."
                : _feedback ?? "O Fusca parou no meio da rua. O silêncio é estranho.\nInspecione a ignição, o rádio e o painel.", VarginhaGameSettings.Current.subtitleSize);
            if (_state != State.Restart)
            {
                string[] parts={"IGNIÇÃO","RÁDIO","PAINEL"};
                for(int i=0;i<3;i++) if(ExperimentGUI.Button(new Rect(110+i*245,630,230,47),parts[i]+((_progress.inspection & 1<<i)!=0 ? " • VISTO" : ""))) Inspect(i);
                if(ExperimentGUI.Button(new Rect(875,630,295,47),_progress.inspection==7 ? "AGUARDAR O MOTOR" : "VOLTAR AO CARRO")) CloseInspection();
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
