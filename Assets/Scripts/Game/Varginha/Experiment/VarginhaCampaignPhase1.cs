using System;
using System.Collections;
using System.IO;
using System.Linq;
using Game.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    [Serializable]
    public sealed class CampaignMemory
    {
        public int version = 1, evidence;
        public bool openingSeen, powerFailed, complete,headHit,memoryLost;
        public float x = 6.15f, y = 2.8f;
    }
    public static class CampaignMemorySave
    {
        public static string Path => System.IO.Path.Combine(Application.persistentDataPath, "CampaignMemory1996.json");
        public static bool Exists => File.Exists(Path);
        public static CampaignMemory Load()
        {
            try
            {
                var value = File.Exists(Path) ? JsonUtility.FromJson<CampaignMemory>(File.ReadAllText(Path)) : null;
                if (value != null && value.version == 1)
                {
                    if (float.IsNaN(value.x) || float.IsInfinity(value.x) || float.IsNaN(value.y) || float.IsInfinity(value.y))
                    { value.x = 6.15f; value.y = 2.8f; }
                    value.x = Mathf.Clamp(value.x, -8, 26); value.y = Mathf.Clamp(value.y, -6, 6);
                    value.evidence &= 15; return value;
                }
            }
            catch (Exception e) when (e is IOException || e is ArgumentException || e is UnauthorizedAccessException)
            { Debug.LogWarning("Memória da campanha recuperada: " + e.GetType().Name); }
            return new CampaignMemory();
        }
        public static void Write(CampaignMemory value)
        {
            try
            {
                Directory.CreateDirectory(Application.persistentDataPath);
                string temp = Path + ".tmp"; File.WriteAllText(temp, JsonUtility.ToJson(value, true));
                if (File.Exists(Path)) File.Replace(temp, Path, null); else File.Move(temp, Path);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is PlatformNotSupportedException)
            { Debug.LogWarning("Não foi possível salvar a memória: " + e.GetType().Name); }
        }
    }
    public sealed class VarginhaCampaignPhase1 : MonoBehaviour
    {
        public const string SceneName = "Ato1_Fase1_O_Caso_de_Varginha";
        public static VarginhaCampaignPhase1 Active { get; private set; }
        public static bool IsModalOpen => Active != null && (Active._stage != Stage.Explore || Active._paused || Active._dialogue != null);
        private enum Stage { Opening, ActTitle, Explore, Encounter, Complete }
        private Stage _stage;
        private CampaignMemory _memory;
        private EdelzioTopDownController _player;
        private ExperimentTimeline _timeline;
        private ExperimentFrameComposer _composer;
        private AudioSource _voice, _ambience;
        private AudioClip _staticClip;
        private SpriteRenderer _child, _presence;
        private CampaignPreFlashGlow _preFlashGlow;
        private Sprite[,] _childFrames;
        private int _lastShot = -1, _frameTick = -1;
        private float _stageTime, _exploreTime, _saveTime;
        private bool _paused, _settings, _ready;
        private CampaignSoundscape _sound;
        private CampaignFlashEncounter _flash;
        private string _dialogue;
        private readonly string[] _names = { "TELEVISÃO", "JORNAL DE 1996", "DESENHO INFANTIL", "CAIXA DE BRINQUEDOS" };
        private readonly string[] _lines = {
            "A reportagem fala de uma criatura vista em Varginha. A imagem parece se repetir... mesmo depois de a voz parar.",
            "O jornal está aberto na mesa. As versões dos moradores são diferentes. Você só entende algumas palavras: luz, rua, olhos.",
            "Um desenho seu: a casa, uma árvore e uma figura sem rosto. Você não lembra de ter feito a última parte.",
            "Carrinhos, papéis e uma caixa velha. Você guarda um desenho aqui. Um dia, essa caixa voltará a ser importante."
        };
        private Vector2[] _points = { new(4.6f, 4.5f), new(6.2f, 4), new(-4.55f, 4.5f), new(-7.6f, 1.5f) };
        public CampaignMemory Memory => _memory;
        public bool IsExploring => _stage == Stage.Explore;
        public static void StartCampaign(bool continueGame)
        {
            if (!Application.CanStreamedLevelBeLoaded(SceneName)) { Debug.LogError("Cena inicial da campanha ausente."); return; }
            if (!continueGame) { CampaignMemorySave.Write(new CampaignMemory()); CampaignStorySave.Write(new CampaignStory()); }
            else if (CampaignStorySave.Load().phase > 1) { var saved=CampaignStorySave.Load();Time.timeScale = 1; GameManager.Instance?.StartGame(); CampaignCinematics.Load(saved.phase>=11?CampaignContinuationDefinition.SceneName(saved.phase,saved.continuation.area):CampaignStorySave.Scene(saved.phase)); return; }
            Time.timeScale = 1; GameManager.Instance?.StartGame(); CampaignCinematics.Load(SceneName);
        }
        private void Awake()
        {
            Active = this;
            var plan = CampaignMapPlan.Create(1);
            CampaignMapConstruction.Build(transform, plan);
            if (FindAnyObjectByType<EdelzioTopDownController>() == null) CampaignMapConstruction.CreatePlayer(transform, plan, true);
        }
        private IEnumerator Start()
        {
            _memory = CampaignMemorySave.Load(); _composer = new ExperimentFrameComposer();
            var glow=new GameObject("Brilho_antes_do_clarao");glow.transform.SetParent(transform,false);
            _preFlashGlow=glow.AddComponent<CampaignPreFlashGlow>();
            _player = FindAnyObjectByType<EdelzioTopDownController>();
            if (_player == null) throw new InvalidOperationException("Mapa da casa sem controlador do jogador.");
            QuietHouse();
            _player.HasBackpack = _player.HasResearchNotebook = _player.HasFuscaKey = false;
            _player.SetCombatLocked(false); _player.CanDodge = false;
            var housePlan=CampaignMapPlan.Create(1);
            _points=new[]{housePlan.points.Find(p=>p.id=="tv").position,housePlan.points.Find(p=>p.id=="paper").position,housePlan.points.Find(p=>p.id=="drawing").position,housePlan.points.Find(p=>p.id=="toys").position};
            _player.transform.position = housePlan.IsClear(new Vector2(_memory.x,_memory.y))?new Vector3(_memory.x,_memory.y):(Vector3)housePlan.spawn;
            var adult = _player.GetComponent<VarginhaPlayerSpriteAnimation>(); if (adult != null) adult.enabled = false;
            var action = _player.GetComponent<VarginhaPlayerActionAnimation>(); if (action != null) action.enabled = false;
            var attack = _player.GetComponent<VarginhaPlayerAttack>(); if (attack != null) attack.enabled = false;
            _player.transform.localScale = new Vector3(.72f, .72f, 1);
            foreach (Transform part in _player.transform) if (part.name.Contains("Beard") || part.name.Contains("Barba") || part.name.Contains("Backpack")) part.gameObject.SetActive(false);
            _child = _player.GetComponent<SpriteRenderer>(); _child.color = Color.white; _child.flipX = false; LoadChild(); CampaignPresentation.FootCollision(_player, true);
            var newspaper=housePlan.furniture.Find(p=>p.name=="Mesa do jornal");
            var drawing=housePlan.furniture.Find(p=>p.name=="Mesa de desenho");
            if(CampaignIllustratedMaps.Get(1)==null)
            {
                AddPaper("Jornal_1996",newspaper.position+Vector2.up*(newspaper.size.y*.25f),new Color(.86f,.8f,.62f),newspaper.name);
                AddPaper("Desenho_Infantil",drawing.position+Vector2.up*(drawing.size.y*.25f),new Color(.95f,.89f,.72f),drawing.name);
            }
            _voice = gameObject.AddComponent<AudioSource>(); _voice.volume = .7f;
            _ambience = gameObject.AddComponent<AudioSource>(); _ambience.volume = .025f; _ambience.loop = true;
            _staticClip = VarginhaExperimentLab.CreateStatic(); _ambience.clip = _staticClip;
            _sound = gameObject.AddComponent<CampaignSoundscape>(); _sound.Configure("House", _player);
            GameManager.Instance?.StartGame();
            yield return null; QuietHouse();
            if (_memory.complete) SetStage(Stage.Complete);
            else if (_memory.openingSeen) SetStage(Stage.Explore);
            else PlayOpening();
            _ready = true;
        }
        private void QuietHouse()
        {
            foreach (var prop in FindObjectsByType<InteractableProp>(FindObjectsInactive.Include)) prop.enabled = false;
            foreach (var enemy in FindObjectsByType<EntityManifestationAI>(FindObjectsInactive.Include)) enemy.gameObject.SetActive(false);
            foreach (var enemy in FindObjectsByType<VarginhaCombatEnemy>(FindObjectsInactive.Include)) enemy.gameObject.SetActive(false);
            foreach (var exit in FindObjectsByType<FuscaLevelExit>(FindObjectsInactive.Include)) exit.enabled = false;
            if (VarginhaGameHUD.Instance != null) VarginhaGameHUD.Instance.enabled = false;
            if (VarginhaGameHUD.Instance != null) VarginhaGameHUD.Instance.CloseDialogue();
            if (VarginhaNotebookQuiz.Instance != null) VarginhaNotebookQuiz.Instance.enabled = false;
            foreach (var item in FindObjectsByType<Transform>(FindObjectsInactive.Include))
                if (item.name == "Notebook_TI" || item.name == "Backpack_Prop" || item.name.Contains("Fusca") && item.GetComponent<SpriteRenderer>() != null)
                    item.gameObject.SetActive(false);
        }
        private void LoadChild()
        {
            var texture = VarginhaExperimentArt.Load("ChildEdelzio"); if (texture == null) return;
            int width = texture.width / 3, height = texture.height / 4;
            _childFrames = new Sprite[4, 3];
            for (int direction = 0; direction < 4; direction++) for (int frame = 0; frame < 3; frame++)
                _childFrames[direction, frame] = CampaignPresentation.AlignedFrame(texture,
                    new RectInt(frame * width, (3 - direction) * height, width, height), height / 1.8f, true);
            _child.sprite = _childFrames[0, 0];
        }
        private void AddPaper(string name, Vector2 position, Color color,string supportName)
        {
            var item = new GameObject(name); item.transform.SetParent(transform); item.transform.position = position;
            var renderer = item.AddComponent<SpriteRenderer>(); renderer.sprite = VarginhaPixelArtSprites.Create("Doc_Prop", color); renderer.sortingOrder = 8;
            item.transform.localScale = new Vector3(.48f, .48f, 1);
            var support=GameObject.Find(supportName);
            if(support!=null)VarginhaWorldDepth.Ensure(renderer,supportingObject:support.transform,offset:2);
        }
        public void PlayOpening()
        {
            // Stop at the child's television: 2026 and the school belong to subsequent acts.
            _timeline = new ExperimentTimeline(ExperimentDefinition.Load().shots.Take(6).ToArray());
            _lastShot = _frameTick = -1; SetStage(Stage.Opening); _ambience.Play();
        }
        public void FinishOpening()
        {
            _timeline.Seek(_timeline.Duration); _memory.openingSeen = true; Save(); SetStage(Stage.ActTitle);
        }
        public void DeliverControl() => SetStage(Stage.Explore);
        private void SetStage(Stage value)
        {
            _stage = value; _stageTime = 0; _player.SetInputLocked(value != Stage.Explore || _paused || _dialogue != null);
            if (_preFlashGlow!=null) _preFlashGlow.gameObject.SetActive(value==Stage.Explore && _memory.powerFailed);
            if (value != Stage.Opening) { _voice.Stop(); _ambience.Stop(); }
            if (value == Stage.Encounter)
            {
                var figure = new GameObject("Presença_Incompleta"); figure.transform.SetParent(transform);
                figure.transform.position = new Vector3(17.2f, 0);
                _presence = figure.AddComponent<SpriteRenderer>(); _presence.sprite = VarginhaPixelArtSprites.Create("ET_Subordinate_Entity", Color.black);
                _presence.sortingOrder = 9; _presence.color = new Color(.06f, .07f, .09f, .14f);
                _flash=gameObject.AddComponent<CampaignFlashEncounter>();_flash.Begin(_player,_child,_sound);
            }
        }
        private void Update()
        {
            if (CampaignCinematics.IsTransitioning) return;
            if (!_ready) return;
            if(_paused&&_settings&&VarginhaInputActions.CancelPressed&&!VarginhaInputActions.PausePressed){_settings=false;return;}
            if ((VarginhaInputActions.PausePressed || VarginhaInputActions.CancelPressed && (_paused || _stage!=Stage.Explore)))
            {
                if (_dialogue != null) CloseDialogue(); else TogglePause();
            }
            if (_paused || _dialogue != null) return;
            _stageTime += Time.unscaledDeltaTime;
            if (_stage == Stage.Opening)
            {
                _timeline.Tick(Time.unscaledDeltaTime);
                if (_lastShot != _timeline.ShotIndex)
                {
                    _lastShot = _timeline.ShotIndex; _voice.Stop();
                    // Opening uses captions and ambience; synthetic narration was removed.
                }
                if (_timeline.Finished) FinishOpening();
            }
            else if (_stage == Stage.ActTitle && _stageTime >= 3) DeliverControl();
            else if (_stage == Stage.Explore)
            {
                _exploreTime += Time.deltaTime;
                if (_childFrames != null)
                {
                    var face = _player.FacingDirection;
                    int direction = Mathf.Abs(face.x) > Mathf.Abs(face.y) ? face.x < 0 ? 1 : 2 : face.y > 0 ? 3 : 0;
                    int frame = _player.IsMoving ? 1 + Mathf.FloorToInt(Time.time * 7) % 2 : 0;
                    _child.sprite = _childFrames[direction, frame];
                }
                if (!_memory.powerFailed && _exploreTime > 15) TriggerPowerFailure();
                if (VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact))
                {
                    int closest = NearestEvidence(); if (closest >= 0) Examine(closest);
                }
                if (_memory.powerFailed && _player.transform.position.x > 13.5f) SetStage(Stage.Encounter);
                _saveTime += Time.deltaTime; if (_saveTime > 5) { _saveTime = 0; Save(); }
            }
            else if (_stage == Stage.Encounter)
            {
                if (_presence != null) _presence.color = new Color(.05f, .055f, .07f, Mathf.Lerp(.28f,0,Mathf.Clamp01((_stageTime-.4f)/1.2f)));
                _memory.headHit=_flash!=null&&_flash.HasHitHead;
                if (_stageTime > 5.8f) { _memory.complete = true;_memory.memoryLost=_memory.headHit; Save(); SetStage(Stage.Complete); }
            }
        }
        public void TriggerPowerFailure()
        {
            bool first = !_memory.powerFailed; _memory.powerFailed = true;
            if(_stage==Stage.Explore && _preFlashGlow!=null)_preFlashGlow.gameObject.SetActive(true);
            if (first)
            {
                CampaignAtmosphereLayer.Interference(transform);
                CampaignCameraDirector.Reveal(new Vector3(3.4f,5.55f), 3.5f);
                _sound?.CinematicDucking(.65f);
                StartCoroutine(RestorePowerFailureAmbience());
            }
            Save();
        }
        private IEnumerator RestorePowerFailureAmbience()
        {
            yield return new WaitForSeconds(3.5f);
            _sound?.CinematicDucking(0);
        }
        private int NearestEvidence()
        {
            int found = -1; float distance = 1.85f;
            for (int i = 0; i < _points.Length; i++)
            { float current = Vector2.Distance(_player.transform.position, _points[i]); if (current < distance) { found = i; distance = current; } }
            return found;
        }
        public void Examine(int index)
        {
            if (index < 0 || index >= _lines.Length) return;
            _memory.evidence |= 1 << index; _dialogue = _lines[index];
            _sound?.Play("Paper");
            CampaignCameraDirector.Reveal(_points[index]);
            _player.SetInputLocked(true); if (index == 0) TriggerPowerFailure(); Save();
        }
        private void CloseDialogue() { _dialogue = null; _player.SetInputLocked(_paused || _stage != Stage.Explore); }
        public void TogglePause()
        {
            _paused = !_paused; _settings = false; Time.timeScale = _paused ? 0 : 1; CampaignCinematics.Pause(_paused);
            _sound?.Suspend(_paused);
            _player.SetInputLocked(_paused || _stage != Stage.Explore || _dialogue != null);
            if (_paused) { _voice.Pause(); _ambience.Pause(); } else { _voice.UnPause(); _ambience.UnPause(); }
        }
        private void Save() { _memory.x = _player.transform.position.x; _memory.y = _player.transform.position.y; CampaignMemorySave.Write(_memory); }
        private void OnApplicationPause(bool paused) { if (paused && _memory != null) Save(); }
        private void OnGUI()
        {
            VarginhaGamepadUI.Begin("opening:"+GetEntityId()+":"+_stage+":"+_paused+":"+(_dialogue!=null),_paused||_dialogue!=null||_stage!=Stage.Explore,50);
            if (!_ready) return;
            ExperimentGUI.Init(); GUI.depth = -3100;
            if (_stage == Stage.Opening || _stage == Stage.ActTitle || _stage == Stage.Complete )
                ExperimentGUI.Box(new Rect(0, 0, Screen.width, Screen.height), _stage == Stage.ActTitle ? CampaignCinematics.ChapterBlack(_stageTime) : Color.black);
            var before = ExperimentGUI.BeginCanvas();
            var settings = VarginhaGameSettings.Current;
            if (_stage == Stage.Opening)
            {
                var shot = _timeline.Shots[_timeline.ShotIndex]; int tick = Mathf.FloorToInt(_timeline.Time * 12);
                if (_frameTick != tick) { _composer.Compose(shot, _timeline.ShotTime, settings.reducedMotion); _frameTick = tick; }
                ExperimentGUI.Box(new Rect(0, 0, 1280, 720), Color.black);
                GUI.DrawTexture(new Rect(64, 36, 1152, 648), _composer.Frame);
                ExperimentGUI.Panel(new Rect(80, 48, 730, 55)); ExperimentGUI.Label(new Rect(96, 59, 695, 34), shot.title, true);
                if (settings.subtitles) { ExperimentGUI.Panel(new Rect(100, 552, 1080, 75)); ExperimentGUI.Caption(new Rect(125, 565, 1030, 50), shot.subtitle, settings.subtitleSize); }
                if (!_paused && ExperimentGUI.Button(new Rect(916, 645, 270, 44), "PULAR ABERTURA")) FinishOpening();
            }
            else if (_stage == Stage.ActTitle)
            {
                ExperimentGUI.Label(new Rect(295, 245, 700, 60), "ATO 1 — A LEMBRANÇA VOLTA", true);
                ExperimentGUI.Label(new Rect(295, 322, 700, 60), "FASE 1 • O CASO DE VARGINHA\nEdelzio, seis anos. Varginha, 1996.");
            }
            else if (_stage == Stage.Explore)
            {
                float shade = _memory.powerFailed ? .22f : .10f;
                if (_memory.powerFailed && !settings.reducedMotion) shade += .035f * Mathf.Sin(Time.unscaledTime * .8f);
                ExperimentGUI.Box(new Rect(0, 0, 1280, 720), new Color(.035f, .075f, .17f, shade));
                ExperimentGUI.Objective("ATO I • FASE 1 • 1996", "A LEMBRANÇA", _memory.powerFailed ? "Uma luz vem do quintal. Siga pela porta à direita." : "Explore a casa e observe a televisão.");
                if (!_paused && CampaignHudIcons.Button(1174, CampaignHudIcons.Icon.Pause, "ESC", "Pausa")) TogglePause();
                if (settings.interactionHints && !_paused && _dialogue == null)
                {
                    int nearby = NearestEvidence();
                    ExperimentGUI.Panel(new Rect(250, 647, 780, 48));
                    ExperimentGUI.Label(new Rect(275, 658, 730, 30), nearby >= 0 ? "[E] " + _names[nearby] : "WASD / SETAS: ANDAR • E: EXAMINAR • ESC: PAUSAR", small: true);
                }
            }
            else if (_stage == Stage.Encounter)
            {
                float exposure=Mathf.Max(0,1-Mathf.Abs(_stageTime-.48f)/.35f)*(settings.reducedMotion?.12f:.25f);
                ExperimentGUI.Box(new Rect(0,0,1280,720),new Color(.93f,1,.95f,exposure));
                float blackout=Mathf.Clamp01((_stageTime-4.1f)/1.4f);
                if(blackout>0)ExperimentGUI.Box(new Rect(0,0,1280,720),new Color(.008f,.015f,.025f,blackout));
                if(settings.subtitles)ExperimentGUI.Caption(new Rect(265,610,750,58),_stageTime<1.13f?"O clarão explode. Uma força o lança para trás.":_stageTime<3.3f?"Sua cabeça atinge o chão. Tudo fica distante...":"Quando despertar, não lembrará daquela noite.",settings.subtitleSize);
            }
            else
            {
                ExperimentGUI.Box(new Rect(0, 0, 1280, 720), new Color(.008f, .015f, .025f, 1));
                ExperimentGUI.Label(new Rect(275, 210, 760, 60), "A LEMBRANÇA SE INTERROMPE", true);
                ExperimentGUI.Label(new Rect(275, 300, 760, 100), "Edelzio desperta sem lembrar do clarão ou da queda.\n\nTrinta anos depois, a caixa esquecida devolve a primeira pista.");
                ExperimentGUI.Label(new Rect(275, 445, 760, 70), "Trinta anos depois, uma chave desaparecida traz essa noite de volta.", small: true);
                if (ExperimentGUI.Button(new Rect(430, 550, 420, 50), "CONTINUAR • FASE 2")) CampaignStorySave.GoTo(2);
            }
            if (_dialogue != null)
            {
                ExperimentGUI.Panel(new Rect(170, 420, 940, 235));
                ExperimentGUI.Label(new Rect(205, 446, 870, 125), _dialogue);
                if (ExperimentGUI.Button(new Rect(810, 583, 265, 43), "CONTINUAR")) CloseDialogue();
            }
            if (_paused)
            {
                if (_settings) { GUI.matrix = before; if (VarginhaGameSettings.Draw()) _settings = false; return; }
                int action = ExperimentGUI.PausePanel("ATO I • A LEMBRANÇA • 1996");
                if (action == 1) TogglePause();
                if (action == 2) _settings = true;
                if (action == 3) { Save(); Time.timeScale = 1; CampaignCinematics.Load("Menu_MisterioDeVarginha"); }
            }
            GUI.matrix = before;
        }
        private void OnDestroy()
        {
            if (Active == this) Active = null;
            if (_childFrames != null) foreach (var sprite in _childFrames) if (sprite != null) Destroy(sprite);
            _composer?.Dispose(); if (_staticClip != null) Destroy(_staticClip); Time.timeScale = 1;
        }
    }
}
