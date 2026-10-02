using System.Collections;
using Game.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    public sealed class VarginhaExperimentLab : MonoBehaviour
    {
        public const string SceneName = "Laboratorio_GDD_Cutscene";
        public static VarginhaExperimentLab Active { get; private set; }
        public static bool IsModalOpen => Active != null && (Active._view != View.World);
        private enum View { World, Opening, Puzzle, Characters, Dialogue }
        private ExperimentDefinition _data;
        private ExperimentProgress _progress;
        private ExperimentTimeline _timeline;
        private ExperimentFrameComposer _composer;
        private EdelzioTopDownController _player;
        private Transform _renan;
        private View _view;
        private int _puzzle, _selected = -1, _shot = -1, _lastFrame = -1;
        private string _feedback = "";
        private bool _hint, _reducedMotion, _sound = true;
        private AudioSource _voice, _ambience;
        private AudioClip _staticClip;
        public ExperimentProgress Progress => _progress;
        public ExperimentDefinition Definition => _data;
        private void Awake() { Active = this; }
        private IEnumerator Start()
        {
            _data = ExperimentDefinition.Load();
            _progress = ExperimentSave.Load(_data);
            _composer = new ExperimentFrameComposer();
            VarginhaPhase2RuntimeFactory.Build(transform);
            _player = FindAnyObjectByType<EdelzioTopDownController>();
            // Only the laboratory receives this peaceful version of the existing school.
            foreach (var enemy in FindObjectsByType<VarginhaCombatEnemy>(FindObjectsInactive.Include))
                enemy.gameObject.SetActive(false);
            foreach (var student in FindObjectsByType<VarginhaStudentHostage>(FindObjectsInactive.Include))
            {
                var cage = student.transform.Find("Jaula_ET"); if (cage != null) cage.gameObject.SetActive(false);
                student.enabled = false;
                var body = student.GetComponent<Rigidbody2D>(); if (body != null) body.bodyType = RigidbodyType2D.Static;
            }
            foreach (var prop in FindObjectsByType<InteractableProp>(FindObjectsInactive.Include)) prop.enabled = false;
            if (VarginhaGameHUD.Instance != null) VarginhaGameHUD.Instance.enabled = false;
            if (VarginhaNotebookQuiz.Instance != null) VarginhaNotebookQuiz.Instance.enabled = false;
            _player.HasBackpack = _player.HasResearchNotebook = _player.HasFuscaKey = true;
            _player.EquipBackpack();
            _player.SetCombatLocked(false); _player.CanDodge = false;
            var attack = _player.GetComponent<VarginhaPlayerAttack>(); if (attack != null) attack.enabled = false;
            _renan = new GameObject("Renan_Industrial").transform;
            _renan.SetParent(transform);
            _renan.position = new Vector3(2.8f, -7.8f);
            var renderer = _renan.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = VarginhaExperimentArt.Body(1); renderer.sortingOrder = 6;
            var collider = _renan.gameObject.AddComponent<CircleCollider2D>();
            collider.radius = .32f;
            collider.offset = new Vector2(0, .18f);
            _player.transform.position = new Vector3(.75f, -8.6f);
            _voice = gameObject.AddComponent<AudioSource>(); _voice.volume = .7f;
            _ambience = gameObject.AddComponent<AudioSource>(); _ambience.volume = .025f;
            _ambience.loop = true;
            _staticClip = CreateStatic(); _ambience.clip = _staticClip;
            GameManager.Instance?.StartGame();
            yield return null; // Let the existing animation/bootstrap components initialize.
            _player.SetCombatLocked(false);
            if (!_progress.openingSeen) PlayOpening();
            else SetView(View.World);
        }
        internal static AudioClip CreateStatic()
        {
            const int rate = 22050;
            var clip = AudioClip.Create("Estatica_Experimental", rate, 1, rate, false);
            var samples = new float[rate]; uint seed = 96;
            for (int i = 0; i < rate; i++) { seed = seed * 1664525 + 1013904223; samples[i] = ((seed >> 8) % 1000) / 1000f - .5f; }
            clip.SetData(samples, 0); return clip;
        }
        private void Update()
        {
            if (_data == null) return;
            if (_view == View.Opening)
            {
                _timeline.Tick(Time.unscaledDeltaTime);
                if (_timeline.ShotIndex != _shot)
                {
                    _shot = _timeline.ShotIndex; _voice.Stop();
                    var clip = Resources.Load<AudioClip>("Varginha/Experiment/Audio/" + _data.shots[_shot].voice);
                    if (_sound && clip != null) { _voice.clip = clip; _voice.Play(); }
                }
                if (_timeline.Finished) FinishOpening();
            }
            else if (_view == View.World && _player != null && _renan != null
                && Vector2.Distance(_player.transform.position, _renan.position) < 2.2f
                && VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact))
                SetView(View.Dialogue);
            if (_view != View.Opening && Keyboard.current?.escapeKey.wasPressedThisFrame == true && _view != View.World)
                SetView(View.World);
        }
        private void SetView(View view)
        {
            _view = view;
            _player?.SetInputLocked(view != View.World);
            if (view != View.Opening) { _voice?.Stop(); _ambience?.Stop(); }
            _selected = -1;
        }
        public void PlayOpening()
        {
            _timeline = new ExperimentTimeline(_data.shots); _shot = _lastFrame = -1;
            SetView(View.Opening);
            if (_sound) _ambience.Play();
        }
        public void FinishOpening()
        {
            _timeline.Finish(_progress);
            ExperimentSave.Write(_progress);
            SetView(View.World);
        }
        public bool OpenPuzzle(int index)
        {
            if (!_progress.CanOpen(index)) return false;
            _puzzle = index; _hint = false; _feedback = ""; SetView(View.Puzzle); return true;
        }
        public void Swap(int first, int second)
        {
            var order = _puzzle == 0 ? _progress.mapOrder : _progress.documentOrder;
            if (order == null || first < 0 || second < 0 || first >= order.Length || second >= order.Length) return;
            (order[first], order[second]) = (order[second], order[first]);
            ExperimentSave.Write(_progress);
        }
        private void OnGUI()
        {
            if (_data == null) return;
            ExperimentGUI.Init();
            GUI.depth = -3000;
            Matrix4x4 before = ExperimentGUI.BeginCanvas();
            switch (_view)
            {
                case View.Opening: DrawOpening(); break;
                case View.Puzzle: DrawPuzzle(); break;
                case View.Characters: DrawCharacters(); break;
                case View.Dialogue: DrawDialogue(); break;
                default: DrawWorld(); break;
            }
            GUI.matrix = before;
        }
        private void DrawWorld()
        {
            ExperimentGUI.Panel(new Rect(12, 10, 1256, 92));
            ExperimentGUI.Label(new Rect(28, 20, 640, 30), "INDUSTRIAL • INVESTIGAÇÃO EXPERIMENTAL", true);
            ExperimentGUI.Label(new Rect(28, 57, 680, 28), _progress.codeSolved ? "23:23 • A diocese é o próximo destino. Prévia concluída." : "Procure Renan na entrada. WASD para andar; E para conversar.", small: true);
            if (ExperimentGUI.Button(new Rect(745, 25, 170, 50), "REVER ABERTURA")) PlayOpening();
            if (ExperimentGUI.Button(new Rect(925, 25, 150, 50), "PERSONAGENS")) SetView(View.Characters);
            if (ExperimentGUI.Button(new Rect(1085, 25, 160, 50), "MENU")) SceneManager.LoadScene("Menu_MisterioDeVarginha");
            ExperimentGUI.Panel(new Rect(12, 624, 1256, 86));
            for (int i = 0; i < _data.puzzles.Length; i++)
            {
                GUI.enabled = _progress.CanOpen(i);
                string label = _progress.IsSolved(i) ? "✓ " : "";
                if (ExperimentGUI.Button(new Rect(28 + i * 310, 640, 300, 48), label + _data.puzzles[i].title)) OpenPuzzle(i);
            }
            GUI.enabled = true;
            if (ExperimentGUI.Button(new Rect(970, 640, 275, 48), "NOVO TESTE")) ResetProgress();
            if (_renan != null && Camera.main != null)
            {
                var point = Camera.main.WorldToScreenPoint(_renan.position + Vector3.up * 1.7f);
                float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
                float left = (Screen.width - 1280 * scale) / 2, top = (Screen.height - 720 * scale) / 2;
                ExperimentGUI.Label(new Rect((point.x - left) / scale - 90,
                    (Screen.height - point.y - top) / scale, 180, 36), "RENAN • [E]", small: true);
            }
        }
        private void DrawOpening()
        {
            var shot = _data.shots[_timeline.ShotIndex];
            int frame = Mathf.FloorToInt(_timeline.Time * 12);
            if (frame != _lastFrame)
            { _composer.Compose(shot, _timeline.ShotTime, _reducedMotion); _lastFrame = frame; }
            ExperimentGUI.Box(new Rect(0, 0, 1280, 720), Color.black);
            // Exact integer enlargement of the 384×216 cinematic framebuffer.
            GUI.DrawTexture(new Rect(64, 36, 1152, 648), _composer.Frame);
            ExperimentGUI.Box(new Rect(64, 36, 1152, 76), new Color(0, 0, 0, .88f));
            ExperimentGUI.Label(new Rect(92, 52, 1050, 44), shot.title, true);
            ExperimentGUI.Box(new Rect(64, 539, 1152, 145), new Color(0, 0, 0, .9f));
            ExperimentGUI.Label(new Rect(92, 548, 1070, 60), shot.subtitle);
            if (ExperimentGUI.Button(new Rect(90, 620, 125, 42), _timeline.Paused ? "CONTINUAR" : "PAUSAR"))
            {
                _timeline.Paused = !_timeline.Paused;
                if (_timeline.Paused) { _voice.Pause(); _ambience.Pause(); }
                else if (_sound) { _voice.UnPause(); _ambience.UnPause(); }
            }
            if (ExperimentGUI.Button(new Rect(225, 620, 135, 42), _sound ? "SOM: LIGADO" : "SOM: MUDO"))
            { _sound = !_sound; _voice.mute = _ambience.mute = !_sound; }
            bool reduced = GUI.Toggle(new Rect(385, 633, 360, 28), _reducedMotion, "REDUZIR DISTORÇÕES E CLARÃO");
            if (reduced != _reducedMotion) { _reducedMotion = reduced; _lastFrame = -1; }
            if (ExperimentGUI.Button(new Rect(750, 620, 195, 42), "PULAR PRÉVIA")) FinishOpening();
            ExperimentGUI.Label(new Rect(965, 630, 220, 30), Mathf.FloorToInt(_timeline.Time) + " / " + _timeline.Duration + " s", small: true);
        }
        private void DrawDialogue()
        {
            ExperimentGUI.Box(new Rect(0, 0, 1280, 720), new Color(0, 0, 0, .6f));
            ExperimentGUI.Panel(new Rect(100, 330, 1080, 320));
            ExperimentGUI.Character(new Rect(120, 350, 220, 250), 1);
            ExperimentGUI.Label(new Rect(370, 355, 760, 40), "RENAN • INDUSTRIAL", true);
            ExperimentGUI.Label(new Rect(370, 415, 760, 130), _progress.mapSolved
                ? "A estrada e o rio se encaixam. Agora precisamos conferir os documentos: arquivos digitais podem ter sido alterados."
                : "Edelzio, esses recortes parecem partes do mesmo mapa. Vamos comparar o rio, a estrada e a igreja antes de confiar nas coordenadas.");
            if (ExperimentGUI.Button(new Rect(370, 565, 340, 50), "ABRIR INVESTIGAÇÃO")) OpenPuzzle(_progress.mapSolved ? _progress.documentsSolved ? 2 : 1 : 0);
            if (ExperimentGUI.Button(new Rect(750, 565, 340, 50), "VOLTAR À ESCOLA")) SetView(View.World);
        }
        private void DrawPuzzle()
        {
            var puzzle = _data.puzzles[_puzzle];
            ExperimentGUI.Box(new Rect(0, 0, 1280, 720), new Color(0, 0, 0, .85f));
            ExperimentGUI.Panel(new Rect(40, 24, 1200, 672));
            ExperimentGUI.Label(new Rect(72, 48, 1110, 44), puzzle.title, true);
            ExperimentGUI.Label(new Rect(72, 100, 1110, 70), puzzle.description);
            if (_puzzle < 2)
            {
                var order = _puzzle == 0 ? _progress.mapOrder : _progress.documentOrder;
                for (int i = 0; i < order.Length; i++)
                {
                    var card = new Rect(72 + i % 2 * 330, 178 + i / 2 * 175, 315, 160);
                    ExperimentGUI.Box(card, _selected == i ? new Color(.22f, .4f, .34f) : new Color(.08f, .12f, .15f));
                    if (_puzzle == 0)
                    {
                        GUI.DrawTextureWithTexCoords(new Rect(card.x + 7, card.y + 7, 301, 128),
                            VarginhaExperimentArt.Map(), VarginhaExperimentArt.MapUV(order[i]));
                        ExperimentGUI.Label(new Rect(card.x + 10, card.y + 135, 295, 22), puzzle.items[order[i]], small: true);
                    }
                    else ExperimentGUI.Label(new Rect(card.x + 16, card.y + 18, 283, 128), puzzle.items[order[i]]);
                    if (GUI.Button(card, GUIContent.none, GUIStyle.none))
                    {
                        if (_selected < 0) _selected = i;
                        else { Swap(_selected, i); _selected = -1; _feedback = ""; }
                    }
                }
            }
            else
            {
                for (int i = 0; i < puzzle.items.Length; i++)
                    ExperimentGUI.Label(new Rect(90, 205 + i * 70, 565, 60), puzzle.items[i], i < 2);
                ExperimentGUI.Label(new Rect(450, 178, 260, 48), _progress.codeInput.PadRight(4, '_'), true);
                for (int digit = 0; digit < 10; digit++)
                {
                    if (ExperimentGUI.Button(new Rect(440 + digit % 3 * 80, 240 + digit / 3 * 60, 70, 50), digit.ToString())
                        && _progress.codeInput.Length < 4)
                    { _progress.codeInput += digit; ExperimentSave.Write(_progress); }
                }
                if (ExperimentGUI.Button(new Rect(440, 495, 230, 38), "APAGAR"))
                { _progress.codeInput = ""; ExperimentSave.Write(_progress); }
            }
            ExperimentGUI.Character(new Rect(805, 178, 345, 245), _puzzle == 1 ? 2 : 1);
            ExperimentGUI.Label(new Rect(800, 430, 365, 130), _hint ? puzzle.hint : _progress.IsSolved(_puzzle) ? puzzle.reward : "Compare as evidências. Nenhuma tentativa incorreta destrói uma pista.", small: true);
            ExperimentGUI.Label(new Rect(80, 540, 1090, 42), _feedback);
            if (ExperimentGUI.Button(new Rect(75, 600, 310, 52), "CONFERIR EVIDÊNCIAS"))
            {
                bool correct = _progress.Submit(_data, _puzzle);
                _feedback = correct ? puzzle.reward : "Ainda há uma contradição. Revise as conexões e tente novamente.";
                ExperimentSave.Write(_progress);
            }
            if (ExperimentGUI.Button(new Rect(405, 600, 205, 52), "DICA")) _hint = !_hint;
            if (ExperimentGUI.Button(new Rect(630, 600, 255, 52), "REORGANIZAR"))
            {
                if (_puzzle == 0) _progress.mapOrder = (int[])puzzle.initial.Clone();
                else if (_puzzle == 1) _progress.documentOrder = (int[])puzzle.initial.Clone();
                else _progress.codeInput = "";
                _selected = -1; _feedback = ""; ExperimentSave.Write(_progress);
            }
            if (ExperimentGUI.Button(new Rect(905, 600, 290, 52), "VOLTAR À ESCOLA")) SetView(View.World);
        }
        private void DrawCharacters()
        {
            ExperimentGUI.Box(new Rect(0, 0, 1280, 720), new Color(0, 0, 0, .95f));
            ExperimentGUI.Label(new Rect(40, 24, 1200, 50), "PERSONAGENS • REFERÊNCIAS REAIS EM PIXEL ART", true);
            for (int i = 0; i < _data.characters.Length; i++)
            {
                var character = _data.characters[i];
                var card = new Rect(34 + i * 310, 100, 295, 530);
                ExperimentGUI.Panel(card);
                ExperimentGUI.Character(new Rect(card.x + 15, card.y + 15, 265, 255), character.cell);
                ExperimentGUI.Label(new Rect(card.x + 15, card.y + 280, 265, 40), character.name, true);
                ExperimentGUI.Label(new Rect(card.x + 15, card.y + 325, 265, 50), character.role, small: true);
                ExperimentGUI.Label(new Rect(card.x + 15, card.y + 385, 265, 130), character.description, small: true);
            }
            if (ExperimentGUI.Button(new Rect(480, 648, 320, 50), "VOLTAR À ESCOLA")) SetView(View.World);
        }
        public void ResetProgress()
        {
            _progress = new ExperimentProgress(); _progress.Repair(_data);
            ExperimentSave.Write(_progress); SetView(View.World);
        }
        private void OnDestroy()
        {
            if (Active == this) Active = null;
            if (_player != null) _player.SetInputLocked(false);
            _composer?.Dispose();
            if (_staticClip != null) Destroy(_staticClip);
            Time.timeScale = 1f;
        }
    }
}
