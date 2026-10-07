using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    /// <summary>Investigation chapters on the shipped house and Industrial maps.</summary>
    public sealed class VarginhaCampaignStage : MonoBehaviour
    {
        public int phase = 2;
        public static VarginhaCampaignStage Active { get; private set; }
        public static bool IsModalOpen => Active != null && Active.IsBlocked;
        private bool IsBlocked => !_ready || _titleTime < 3 || _paused || _busy || _dialogue != null || _view != View.World || VarginhaGameHUD.Instance?.IsInventoryOpen == true;
        private enum View { World, Pages, Photo, Code, Notebook, Hint, Cutscene, Complete }
        private View _view;
        private Vector2 _journalScroll;
        private CampaignStory _progress;
        private EdelzioTopDownController _player;
        private readonly List<Point> _points = new();
        private bool _ready, _paused, _settings, _busy;
        private CampaignSoundscape _sound;
        private readonly List<CampaignSchoolLife> _students = new();
        public bool IsActing => _busy;
        private float _titleTime, _saveTime, _cutTime;
        private bool _carContext;
        private string _dialogue, _speaker, _feedback, _cutTitle, _cutText;
        private int _selected = -1;
        private View _afterCut;
        private sealed class Point
        {
            public string id, label; public Vector2 position; public Transform target;
            public Vector2 Position => target != null ? (Vector2)target.position : position;
            public Point(string key, string name, Vector2 pos, Transform follow = null) { id = key; label = name; position = pos; target = follow; }
        }
        public CampaignStory Progress => _progress;
        private string PhaseTitle => CampaignSequence.Title(phase).ToUpperInvariant();
        private void Awake()
        {
            Active = this;
            if (phase >= 4)
            {
                var school=new GameObject("Escola_3_Sistema_Ambiente");school.transform.SetParent(transform);
                var plan=CampaignMapPlan.Create(phase);CampaignMapConstruction.Build(school.transform,plan);CampaignMapConstruction.CreatePlayer(transform,plan);
                foreach(string name in VarginhaPhase2Controller.StudentNames)
                {
                    var student=new GameObject("Refem_"+name.Replace(" ","_"));student.transform.SetParent(transform);
                    student.AddComponent<SpriteRenderer>().sprite=VarginhaStudentSprites.Frame(name,0,0);
                    var body=student.AddComponent<Rigidbody2D>();body.gravityScale=0;body.constraints=RigidbodyConstraints2D.FreezeRotation;
                    var feet=student.AddComponent<CircleCollider2D>();feet.radius=.22f;feet.offset=new Vector2(0,-.58f);
                    VarginhaWorldDepth.Ensure(student.GetComponent<SpriteRenderer>(),ground:feet);
                }
            }
        }
        private IEnumerator Start()
        {
            _progress = CampaignStorySave.Load(); _progress.phase = phase;
            yield return null;
            if (VarginhaGameHUD.Instance == null)
            {
                var hud = new GameObject("Campaign_HUD"); hud.transform.SetParent(transform); hud.AddComponent<VarginhaGameHUD>();
            }
            CampaignPresentation.QuietWorld();
            _player = FindAnyObjectByType<EdelzioTopDownController>();
            if (_player.GetComponent<CampaignTeamEdelzio>() == null) _player.gameObject.AddComponent<CampaignTeamEdelzio>();
            _player.SetCombatLocked(false); _player.CanDodge = false;
            CampaignPresentation.FootCollision(_player, false);
            if (phase == 2) { CampaignAdultHouse.Apply(); BuildHousePoints(); } else BuildSchoolPoints();
            if (_progress.positionPhase == phase && CampaignMapPlan.Create(phase).IsClear(new Vector2(_progress.x,_progress.y-.58f))) _player.transform.position = new Vector3(_progress.x, _progress.y);
            else _player.transform.position = (Vector3)(CampaignMapPlan.Create(phase).spawn+Vector2.up*.58f);
            RestoreInventory();
            _sound = gameObject.AddComponent<CampaignSoundscape>(); _sound.Configure(phase == 2 ? "House" : "School", _player);
            GameManager.Instance?.StartGame(); _ready = true; Lock(); Save();
        }
        private void RestoreInventory()
        {
            _player.HasBackpack = phase > 2 || _progress.boxFound || (_progress.routine & 8) != 0;
            _player.HasFuscaKey = _player.HasResearchNotebook = phase > 2 || _progress.pagesSolved;
            _player.HasDecodedData = phase > 2 || _progress.boxFound || (_progress.routine & 4) != 0;
            if(_progress.boxFound)GameObject.Find("Backpack_Prop")?.SetActive(false);
        }
        private void BuildHousePoints()
        {
            _points.Add(new Point("wash", "HIGIENE • LAVATÓRIO", CampaignAdultHouse.WashApproach));
            _points.Add(new Point("food", "CAFÉ DA MANHÃ", new(4.6f, -2.05f)));
            _points.Add(new Point("work", "PREPARAR NOTEBOOK • CADEIRA", new(-5, -2.3f)));
            _points.Add(new Point("bag", "MOCHILA", new(-3.35f, 2.65f)));
            _points.Add(new Point("box", "CAIXA SOB A CAMA", new(-6.3f, 3.15f)));
            _points.Add(new Point("car", "FUSCA • IR À INDUSTRIAL", new(20, 0)));
            if(CampaignIllustratedMaps.Get(2)!=null)foreach(var point in _points)point.position=CampaignMapPlan.Create(2).points.Find(p=>p.id==point.id).position+Vector2.up*.58f;
            foreach (var exit in FindObjectsByType<FuscaLevelExit>(FindObjectsInactive.Include)) exit.enabled = false;
            var bag = GameObject.Find("Backpack_Prop"); if (bag != null && (_progress.boxFound || (_progress.routine & 8) != 0)) bag.SetActive(false);
        }
        private void BuildSchoolPoints()
        {
            var school = GameObject.Find("Escola_3_Sistema_Ambiente");
            if (school != null&&CampaignIllustratedMaps.Get(phase)==null) CampaignMapConstruction.PreserveSchoolFacade(school.transform);
            foreach (var hostage in FindObjectsByType<VarginhaStudentHostage>(FindObjectsInactive.Include))
            {
                // These are ordinary classes in Act II; retain each student's existing sprite.
                hostage.enabled = false;
                foreach (Transform child in hostage.transform) if (child.name.Contains("Cage") || child.name.Contains("Grade") || child.name.Contains("Jaula")) child.gameObject.SetActive(false);
            }
            foreach (var sr in FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include))
                if (sr.name.Contains("HostageCage") || sr.name.Contains("Jaula") || sr.name.Contains("Cage")) sr.gameObject.SetActive(false);
            Vector2[] seats = { new(-5.85f,2.33f),new(3.75f,2.33f),new(6.55f,2.33f),new(-5.85f,-.07f),new(3.75f,-.07f),new(6.55f,-.07f),new(-5.85f,-2.47f),new(3.75f,-2.47f),new(6.55f,-2.47f) };
            if(CampaignIllustratedMaps.Get(phase)!=null){var all=CampaignIllustratedMaps.SchoolSeats(phase);seats=new[]{all[0],all[2],all[3],all[4],all[6],all[7],all[8],all[10],all[11]};}
            string[] activities = { "conferindo arquivos", "organizando o projeto", "comparando imagens", "anotando os resultados", "fazendo a pesquisa", "revisando documentos", "verificando a legenda", "levando material à aula", "conversando no intervalo" };
            for (int i = 0; i < VarginhaPhase2Controller.StudentNames.Length; i++)
            {
                string name = VarginhaPhase2Controller.StudentNames[i]; var actor = GameObject.Find("Refem_" + name.Replace(" ", "_"));
                if (actor == null) continue;
                var life = actor.AddComponent<CampaignSchoolLife>();
                var layout=CampaignIllustratedMaps.Get(phase);
                var away=layout!=null?layout.Position(i==7?740:1080,750)+Vector2.up*.58f:i == 7 ? new Vector2(.75f,-7.1f) : new Vector2(7.8f,-7.3f);
                life.Configure(name, activities[i], seats[i],away, false);
                if(layout!=null)
                {
                    int seatIndex=new[]{0,2,3,4,6,7,8,10,11}[i];
                    var chair=GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/Cadeira azul "+seatIndex)?.GetComponent<SpriteRenderer>();
                    CampaignSeatingLayers.Attach(actor.GetComponent<SpriteRenderer>(),chair,true);
                }
                _students.Add(life); _points.Add(new Point("student:" + i, name.ToUpperInvariant() + " • " + activities[i], seats[i], actor.transform));
            }
            var renan = new GameObject("Renan_Industrial_Campanha"); renan.transform.SetParent(transform); renan.transform.position = new Vector3(3,-7.7f);
            if(CampaignIllustratedMaps.Get(phase)!=null)renan.transform.position=CampaignIllustratedMaps.Get(phase).Objective("renan")+Vector2.up*.58f;
            var renderer = renan.AddComponent<SpriteRenderer>(); renderer.sprite = CampaignStorySprites.Frame("RenanTeaching",3,0); renderer.sortingOrder = 6;
            renan.AddComponent<CampaignRenanTeacher>();
            if(CampaignIllustratedMaps.Get(phase)!=null)CampaignRenanTeacher.DeskItems(transform,CampaignIllustratedMaps.Get(phase));
            var renanFeet=renan.AddComponent<CircleCollider2D>();renanFeet.radius=.24f;renanFeet.offset=Vector2.down*.58f;
            VarginhaWorldDepth.Ensure(renderer,ground:renanFeet);
            _points.Add(new Point("renan", "RENAN • QUADRO BRANCO", renan.transform.position, renan.transform));
            _points.Add(new Point("notebook", "NOTEBOOK DO PROFESSOR • FOTOGRAFIA", new(.75f,3.8f)));
            var layoutSchool=CampaignIllustratedMaps.Get(phase);
            if(layoutSchool!=null)_points.Find(point=>point.id=="notebook").position=layoutSchool.Objective("notebook")+Vector2.up*.58f;
        }
        private int _hintLevel;
        public bool OpenHints()
        {
            if (!_ready || _busy || _paused || _dialogue != null || _view == View.Cutscene || _view == View.Complete || VarginhaGameHUD.Instance?.IsInventoryOpen == true) return false;
            _hintLevel=0; Show(View.Hint); return true;
        }
        private void Update()
        {
            if (CampaignCinematics.IsTransitioning) return;
            if (!_ready) return;
            if (VarginhaGameHUD.Instance?.IsInventoryOpen == true) return;
            if(_paused&&_settings&&VarginhaInputActions.CancelPressed&&!VarginhaInputActions.PausePressed){_settings=false;return;}
            if ((VarginhaInputActions.PausePressed || VarginhaInputActions.CancelPressed && (_paused || _dialogue!=null || _view!=View.World)))
            {
                if (_dialogue != null && !_carContext) CloseDialogue();
                else if (_view != View.World && _view != View.Cutscene && _view != View.Complete) Show(View.World);
                else TogglePause();
            }
            if (_paused) return;
            if (_titleTime < 3) { _titleTime += Time.unscaledDeltaTime; Lock(); return; }
            if (_view == View.Cutscene)
            { _cutTime += Time.unscaledDeltaTime; if (VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact)) FinishCutscene(); return; }
            if (IsBlocked) return;
            if (Keyboard.current?.f1Key.wasPressedThisFrame == true) { OpenHints(); return; }
            if (VarginhaInputActions.InventoryPressed) { OpenBackpack(); return; }
            if (VarginhaInputActions.JournalPressed) Show(View.Notebook);
            int nearby = Nearest();
            if(nearby>=0 && (_points[nearby].id=="car" ? VarginhaInputActions.CarPressed : VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact))) Interact(_points[nearby].id);
            _saveTime += Time.deltaTime; if (_saveTime > 5) { _saveTime = 0; Save(); }
        }
        private int Nearest()
        {
            int best = -1; float distance = 1.5f;
            for (int i = 0; i < _points.Count; i++)
            { float d = Vector2.Distance(_player.transform.position, _points[i].Position); if (d < distance) { distance = d; best = i; } }
            return best;
        }
        public void Interact(string id)
        {
            if (!_ready || _busy) return;
            _carContext=id=="car";
            if (id == "car" && phase == 2 && _progress.CanLeaveHouse) { DepartForSchool(); return; }
            _sound?.Play(id == "box" || id == "notebook" ? "Paper" : "UI");
            if (id.StartsWith("student:")) StudentInteraction(int.Parse(id.Substring(8)));
            else if (phase == 2) HouseInteraction(id); else SchoolInteraction(id);
            RestoreInventory(); Save(); Lock();
        }
        private void DepartForSchool() { Save(); StartCoroutine(SchoolTransition()); }
        private IEnumerator SchoolTransition()
        {
            _busy = true; Lock(); _sound?.Play("Starter");
            _dialogue = "O Fusca atravessa Varginha. Uma pane breve, o rádio sem energia e uma voz: Não deixa ela sair. O motor volta a responder. A Industrial está próxima."; _speaker = "A CAMINHO DA INDUSTRIAL";
            yield return new WaitForSeconds(VarginhaGameSettings.Current.reducedMotion ? 2 : 3.5f);
            _progress.arrival = true; _progress.inspection = 7; _progress.driveDistance = 120; Save();
            CampaignStorySave.GoTo(4);
        }
        private void HouseInteraction(string id)
        {
            switch (id)
            {
                case "wash": StartCoroutine(RoutineAction("wash")); break;
                case "food": StartCoroutine(RoutineAction("food")); break;
                case "work": StartCoroutine(RoutineAction("work")); break;
                case "bag":
                    StartCoroutine(RoutineAction("bag")); break;
                case "box":

                    if (_progress.pagesSolved) { Say("Edelzio", "Guardei a chave, o caderno e o primeiro fragmento de mapa. A mensagem continua ali: ELA AINDA ESTÁ AQUI."); break; }
                    if (!_progress.boxFound)
                    {
                        _progress.boxFound = true;
                        Cutscene("TRINTA ANOS NA MESMA CAIXA", "Entre os carrinhos estão a chave, uma fotografia e páginas do caderno.\nA letra infantil para no meio. A anotação seguinte parece ter sido feita ontem.", View.Pages);
                    }
                    else Show(View.Pages);
                    break;
                case "car":
                    if (_progress.CanLeaveHouse) DepartForSchool();
                    else Say("Edelzio", "Investigue a caixa sob a cama e organize as páginas para encontrar a chave.");
                    break;
            }
        }
        private void SchoolInteraction(string id)
        {
            if(id=="renan")
            {
                _progress.renanMet=true;
                if(phase>=4&&_progress.codeSolved)
                {
                    _progress.renanConfirmed=true;
                    Cutscene("UM HORÁRIO QUE SE REPETE", "Renan: 23:23. Essa anotação também aparece nos registros da cidade.\nLeve a fotografia à biblioteca; pode haver outra parte dessa história lá.", View.Complete);
                }
                else Say("Renan",phase==4
                    ?"Edelzio, encontrei uma fotografia antiga ligada ao caso de 1996. Pode abrir meu notebook na mesa. Observe o portão: há algo estranho nela."
                    :"Reabra a mesma fotografia no meu notebook. Dois grupos iguais de números aparecem na margem, separados por dois pontos.");
            }
            else if(id=="notebook")
            {
                if(!_progress.renanMet){Say("Edelzio","Vou conversar com Renan antes de usar o notebook dele.");return;}
                if(!_progress.buildingSolved){_progress.photoOpened=true;Show(View.Photo);}

                else if(_progress.codeSolved)Say("Edelzio","23:23. Vou mostrar essa descoberta a Renan.");
                else{_progress.timeOpened=true;Show(View.Code);}
            }
        }
        private IEnumerator RoutineAction(string id)
        {
            _busy = true; Lock();
            var action = _player.GetComponent<VarginhaPlayerActionAnimation>();
            var visual = _player.GetComponent<VarginhaPlayerSpriteAnimation>();
            if (id == "bag")
            {
                var bag = GameObject.Find("Backpack_Prop");
                if (bag != null && !_player.HasBackpack)
                {
                    _sound.Play("Zip"); bag.GetComponent<BackpackPickupAnimation>()?.PlayPickup(_player);
                    float time = 0; while (!_player.HasBackpack && time < 2) { time += Time.deltaTime; yield return null; }
                    if (!_player.HasBackpack) { _player.EquipBackpack(); bag.SetActive(false); }
                }
                _progress.routine |= 8;
            }
            else if (id == "food" && action != null)
            {
                bool finished = false; action.PlayDrinkCoffee(GameObject.Find("Coffee_Cup")?.transform, () => finished = true);
                float time = 0; while (!finished && time < 5) { time += Time.deltaTime; yield return null; }
                _progress.routine |= 2;
            }
            else if (id == "work" && action != null)
            {
                bool finished = false;
                action.PlayNotebookSession(GameObject.Find("Notebook_TI")?.transform, () => { finished = true; _sound.Play("Typing"); });
                float time = 0; while (action.IsActing && time < 5) { time += Time.deltaTime; yield return null; }
                if (finished) _progress.routine |= 4;
                else { _busy = false; VarginhaGameHUD.Instance?.CloseDialogue(); Say("Edelzio", "Vou chegar pelo lado livre da cadeira para preparar o notebook."); yield break; }
            }
            else { if (action != null) yield return action.WashFaceRoutine(); _progress.routine |= 1; }
            _busy = false; VarginhaGameHUD.Instance?.CloseDialogue(); RestoreInventory(); Save();
            Say("Edelzio", id == "bag" ? "Mochila pronta. A chave sumiu. Escuto papel raspando debaixo da cama."
                : id == "work" ? "Notebook carregado e material de aula pronto. Por um instante, a tela mostrou o desenho de uma árvore."
                : id == "food" ? "Café e pão. São 07:10; preciso levar o notebook para a Industrial."
                : "Rosto lavado. Só mais um dia de trabalho... mas aquela lembrança de 1996 voltou no sonho.");
        }
        private void StudentInteraction(int index)
        {
            if (index < 0 || index >= _students.Count) return;
            var student = _students[index]; student.Greet(); _progress.studentsTalked |= 1 << index;
            string[] first = {
                "A fotografia que Renan encontrou é de 1996.",
                "O notebook do professor está na mesa junto ao quadro.",
                "Tem alguém naquelo portão? É difícil distinguir o rosto.",
                "O portão me chamou atenção antes de qualquer outra coisa.",
                "Uma imagem pode guardar uma história inteira.",
                "A sombra não parece combinar com a luz da fotografia.",
                "Os dois grupos na margem da foto são iguais.",
                "O projetor piscou com o cabo conectado. Renan também viu.",
                "Renan está mostrando documentos antigos à turma."
            };
            Say(student.StudentName, phase >= 4 && _progress.codeSolved
                ? "23:23... Guarde a foto original. Precisamos comparar se ela mudar de novo."
                : ("Estou " + student.Activity + ". ") + first[index]);
        }
        private void LateUpdate() { if (_ready) Lock(); }
        public bool OpenBackpack()
        {
            if (IsBlocked || !_player.HasBackpack) return false;
            _sound?.Play("Zip"); return VarginhaGameHUD.Instance != null && VarginhaGameHUD.Instance.OpenBackpack();
        }
        private void Say(string speaker, string line) { _speaker = speaker; _dialogue = line; Lock(); }
        public void CloseDialogue() { _dialogue = null; Lock(); }
        private void Show(View value) { _view = value; _selected = -1; _feedback = null; Lock(); }
        private void Cutscene(string title, string line, View next)
        { _cutTitle = title; _cutText = line; _afterCut = next; _cutTime = 0; Show(View.Cutscene); }
        public void FinishCutscene() => Show(_afterCut);
        public void TogglePause() { _paused = !_paused; _settings = false; Time.timeScale = _paused ? 0 : 1; CampaignCinematics.Pause(_paused); _sound?.Suspend(_paused); Lock(); }
        private void Lock() { if (_player != null) _player.SetInputLocked(IsBlocked); }
        public bool SubmitPages()
        {
            if (!_progress.SubmitPages()) { _feedback = "As páginas ainda não acompanham os elementos da fotografia."; return false; }
            Save(); RestoreInventory();
            _sound?.Play("Success");
            Cutscene("ELA AINDA ESTÁ AQUI", "Casa à esquerda. Árvore no centro. Figura à direita.\nAo alinhar as páginas, surge uma escrita recente: ELA AINDA ESTÁ AQUI.\nChave, caderno e FRAGMENTO DE MAPA 1 adicionados à mochila.", View.World); return true;
        }
        public bool SubmitPhotoClue(int choice)
        {
            if (!_progress.SubmitPhotoClue(choice)) { _feedback = "Observe o portão: o que parece estar fora do lugar?"; return false; }
            Save();_sound?.Play("Success");
            Cutscene("ALGUÉM NO PORTÃO", "Há um vulto no portão da fotografia de 1996. Edelzio salva uma cópia no caderno.\nNa margem, duas anotações iguais parecem formar um horário.", View.World); return true;
        }
        public bool SubmitCode()
        {
            if (!_progress.SubmitCode()) { _feedback = "A margem mostra 23 de cada lado dos dois pontos. Tente novamente."; return false; }
            Save();RestoreInventory();_sound?.Play("Success");
            Cutscene("23:23", "Os dois grupos formam 23:23. Ao ampliar o documento, aparece outro fragmento do mapa.\nA anotação menciona registros da cidade. Mostre a descoberta a Renan.", View.World); return true;
        }
        private string Objective()
        {
            if (phase == 2) return !_progress.pagesSolved ? "A chave sumiu. Investigue a caixa sob a cama e organize as páginas." : "Leve as pistas ao trabalho. Vá ao Fusca no quintal à direita.";
            if (!_progress.renanMet) return "Entre na sala e converse com Renan junto ao quadro branco.";
            if (!_progress.buildingSolved) return "Abra a fotografia no notebook do professor e observe o portão.";
            return !_progress.codeSolved ? "Reabra a fotografia no notebook e descubra o horário na margem." : "Mostre a descoberta das 23:23 a Renan junto ao quadro branco.";
        }
        private void OnGUI()
        {
            VarginhaGamepadUI.Begin("house-school:"+GetEntityId()+":"+_view+":"+_paused+":"+(_dialogue!=null),_view!=View.World||_dialogue!=null||_paused,50);
            if (!_ready) return;
            if (VarginhaGameHUD.Instance?.IsInventoryOpen == true) return;
            ExperimentGUI.Init(); GUI.depth = -3100;
            if(_titleTime<3){CampaignChapterPreview.Draw(phase,_titleTime);return;}
            if(_view==View.Complete)CampaignChapterPreview.Background(phase==4?5:6);
            else if(_view==View.Cutscene)CampaignChapterPreview.Background(phase,_cutTime);
            var matrix = ExperimentGUI.BeginCanvas();
            if (_view == View.Cutscene) DrawCutscene();
            else if (_view == View.Complete) DrawComplete();
            else
            {
                ExperimentGUI.Objective(CampaignSequence.Heading(phase) + " • 2026", PhaseTitle, Objective());
                bool hudEnabled=GUI.enabled; GUI.enabled=hudEnabled&&!_paused&&_view==View.World&&_dialogue==null;
                if (CampaignHudIcons.Button(1010, CampaignHudIcons.Icon.Notebook, "TAB", "Caderno")) Show(View.Notebook);
                bool enabled = GUI.enabled; GUI.enabled = enabled&&_player.HasBackpack;
                if (CampaignHudIcons.Button(1092, CampaignHudIcons.Icon.Backpack, "G", "Mochila")) OpenBackpack();
                GUI.enabled = enabled;
                if (CampaignHudIcons.Button(1174, CampaignHudIcons.Icon.Pause, "ESC", "Pausa")) TogglePause();
                GUI.enabled=hudEnabled;
                if (_view == View.World && _dialogue == null && VarginhaGameSettings.Current.interactionHints)
                {
                    int near = Nearest(); ExperimentGUI.Panel(new Rect(230, 654, 820, 44));
                    ExperimentGUI.Label(new Rect(250, 664, 780, 30), near < 0 ? "WASD / SETAS • ANDAR     E • EXAMINAR" : "[" + (_points[near].id=="car"?VarginhaInputActions.CarLabel:VarginhaInputActions.InteractLabel) + "] " + _points[near].label, small: true);
                }
                if (_view != View.World) DrawInvestigation();
            }
            if (_dialogue != null)
            {
                ExperimentGUI.Panel(new Rect(125, 405, 1030, 255));
                bool portrait = _speaker == "Renan" || _students.Exists(student => student.StudentName == _speaker);
                if (_speaker == "Renan") CampaignStorySprites.Portrait(new Rect(145,430,125,135),"RenanTeaching");
                else if (portrait)
                {
                    var head = VarginhaStudentSprites.Portrait(_speaker);
                    if (head != null) GUI.DrawTextureWithTexCoords(new Rect(150, 440, 120, 120), head.texture, new Rect(head.rect.x / head.texture.width, head.rect.y / head.texture.height, head.rect.width / head.texture.width, head.rect.height / head.texture.height));
                }
                float left = portrait ? 290 : 160;
                ExperimentGUI.Label(new Rect(left, 421, 815, 42), _speaker);
                ExperimentGUI.Caption(new Rect(left, 469, 815, 134), _dialogue, VarginhaGameSettings.Current.subtitleSize);
                if (_speaker == "Renan")
                {
                    if (ExperimentGUI.Button(new Rect(155,607,210,38), "FOTOGRAFIA")) DiscussWithRenan(0);
                    if (ExperimentGUI.Button(new Rect(385,607,210,38), "ANOTAÇÕES")) DiscussWithRenan(1);
                    if (ExperimentGUI.Button(new Rect(615,607,210,38), "PESQUISA")) DiscussWithRenan(2);
                }
                if (_carContext ? VarginhaGamepadUI.CarButton(new Rect(870,607,250,38),"CONTINUAR") : ExperimentGUI.Button(new Rect(870,607,250,38),"CONTINUAR")) CloseDialogue();
            }
            if (_paused)
            {
                if (_settings) { GUI.matrix = matrix; if (VarginhaGameSettings.Draw()) _settings = false; return; }
                int action = ExperimentGUI.PausePanel("FASE " + CampaignSequence.Chapter(phase) + " • " + PhaseTitle);
                if (action == 1) TogglePause();
                if (action == 2) _settings = true;
                if (action == 3) Menu();
            }
            GUI.matrix = matrix;
        }
        public void DiscussWithRenan(int topic)
        {
            _sound?.Play("UI");
            Say("Renan", topic == 0 ? "Observe a pessoa no portão. Não há registro de ninguém ali no momento da fotografia."
                : topic == 1 ? "As duas anotações na margem são iguais: 23 e 23. Com os dois pontos entre elas, parecem um horário."
                : "Os registros da biblioteca podem ajudar a entender a origem dessa imagem. Guarde a cópia no seu caderno.");
        }
        private void DrawInvestigation()
        {
            ExperimentGUI.Box(new Rect(0, 0, 1280, 720), new Color(0, 0, 0, .83f));
            ExperimentGUI.Panel(new Rect(105, 120, 1070, 525));
            if (_view == View.Pages)
            {
                ExperimentGUI.Label(new Rect(140, 143, 995, 55), "CADERNO • PÁGINAS DE 1996", true);
                ExperimentGUI.Label(new Rect(140, 204, 995, 78), "Fotografia: a CASA está à esquerda, a ÁRVORE ocupa o centro e a FIGURA ficou à direita.\nClique em duas páginas para trocá-las. Alinhe o desenho à foto.");
                string[] cards = { "CASA\n\nTelhado vermelho\nEscrita: ELA", "ÁRVORE\n\nGalhos sobre o muro\nEscrita: AINDA", "FIGURA\n\nSem rosto\nEscrita: ESTÁ AQUI" };
                for (int i = 0; i < 3; i++)
                    if (ExperimentGUI.Button(new Rect(145 + i * 335, 305, 315, 180), (_selected == i ? "[SELECIONADA]\n" : "") + cards[_progress.pages[i]]))
                    {
                        if (_selected < 0) _selected = i;
                        else { int old = _progress.pages[i]; _progress.pages[i] = _progress.pages[_selected]; _progress.pages[_selected] = old; _selected = -1; Save(); }
                    }
                if (ExperimentGUI.Button(new Rect(800, 550, 325, 45), "CONFERIR PÁGINAS")) SubmitPages();
            }
            else if (_view == View.Photo || _view == View.Code)
            {
                ExperimentGUI.Label(new Rect(140,143,995,55),_view==View.Photo?"NOTEBOOK • FOTOGRAFIA DE 1996":"NOTEBOOK • UM HORÁRIO NA MARGEM",true);
                CampaignSchoolEvidence.Draw(new Rect(150,212,540,300),_view==View.Code);
                ExperimentGUI.Label(new Rect(730,220,390,95),_view==View.Photo?"Renan: observe o portão. Qual detalhe parece estranho?":"Dois grupos iguais: 23 e 23. Qual horário eles formam?",small:true);
                string[] choices=_view==View.Photo?new[]{"PLACA DA ESCOLA","VULTO NO PORTÃO","COR DO MURO"}:new[]{"22:32","23:23","23:32"};
                for(int i=0;i<3;i++)if(ExperimentGUI.Button(new Rect(730,331+i*60,390,48),choices[i]))
                {if(_view==View.Photo)SubmitPhotoClue(i);else{_progress.schoolTimeChoice=i;SubmitCode();}}
            }
            else if (_view == View.Hint) CampaignHints.Draw(_progress,phase,ref _hintLevel);
            else DrawNotebook();
            if (!string.IsNullOrEmpty(_feedback)) ExperimentGUI.Label(new Rect(145, 506, 955, 42), _feedback, small: true);
            if (ExperimentGUI.Button(new Rect(145, 578, 270, 50), "VOLTAR")) { Save(); Show(View.World); }
        }
        private void DrawNotebook()
        {
            ExperimentGUI.Label(new Rect(140, 143, 995, 55), "CADERNO • EVIDÊNCIAS", true);
            CampaignJournal.Draw(_progress,ref _journalScroll,new Rect(140,206,675,335));
            ExperimentGUI.Label(new Rect(860, 207, 265, 42), "MAPA • " + _progress.MapFragments + "/3", true);
            for (int i = 0; i < _progress.MapFragments; i++) GUI.DrawTextureWithTexCoords(new Rect(860 + i * 130, 270, 125, 115), VarginhaExperimentArt.Map(), VarginhaExperimentArt.MapUV(i == 0 ? 2 : 0));
            ExperimentGUI.Label(new Rect(860, 420, 255, 90), "Os registros da cidade podem revelar o próximo fragmento.", small: true);
            if (ExperimentGUI.Button(new Rect(860,540,265,45),"PRECISO DE UMA DICA")) OpenHints();
        }
        private void DrawCutscene()
        {
            var image = phase == 2 ? VarginhaExperimentArt.Load("OpeningStoryboard") : Resources.Load<Texture2D>(VarginhaIndustrialSchoolFacade.ResourcePath);
            if (image != null)
            {
                var rect = new Rect(64, 36, 1152, 648);
                if (phase == 2) GUI.DrawTextureWithTexCoords(rect, image, VarginhaExperimentArt.CellUV(3));
                else GUI.DrawTexture(rect, image, ScaleMode.ScaleToFit);
            }
            ExperimentGUI.Box(new Rect(64, 36, 1152, 648), new Color(0, 0, 0, .32f));
            ExperimentGUI.Panel(new Rect(90, 65, 1100, 72));
            ExperimentGUI.Label(new Rect(113, 81, 1050, 49), _cutTitle, true);
            // The evidence itself is foregrounded in the box/notebook close-up.
            ExperimentGUI.Panel(new Rect(220, 185, 840, 380));
            if (phase == 2 && _progress.pagesSolved)
                GUI.DrawTextureWithTexCoords(new Rect(250, 203, 210, 145), VarginhaExperimentArt.Map(), VarginhaExperimentArt.MapUV(2));
            if (phase >= 4 && _progress.codeSolved) ExperimentGUI.Label(new Rect(480, 207, 500, 55), "23 : 23", true);
            ExperimentGUI.Caption(new Rect(250, 353, 780, 202), _cutText, VarginhaGameSettings.Current.subtitleSize);
            if (ExperimentGUI.Button(new Rect(890, 615, 275, 44), "CONTINUAR")) FinishCutscene();
        }
        private void DrawComplete()
        {
            ExperimentGUI.Panel(new Rect(180,130,930,510));
            ExperimentGUI.Label(new Rect(220, 165, 895, 55), "FASE " + CampaignSequence.Chapter(phase) + " CONCLUÍDA", true);
            ExperimentGUI.Label(new Rect(220, 270, 870, 155), "Renan confirmou o horário. A ampliação revelou outro fragmento. Leve a fotografia aos registros da cidade.\n\nPróxima fase: Fragmentos.");
            if (phase >= 4 && ExperimentGUI.Button(new Rect(370, 490, 540, 55), "CONTINUAR • FASE 4 • FRAGMENTOS")) CampaignStorySave.GoTo(6);
            if (ExperimentGUI.Button(new Rect(370, 570, 540, 45), "SALVAR E VOLTAR AO MENU")) Menu();
        }
        public void Save()
        {
            if (_progress == null || _player == null) return;
            _progress.positionPhase = phase; _progress.x = _player.transform.position.x; _progress.y = _player.transform.position.y; CampaignStorySave.Write(_progress);
        }
        private void Menu() { Save(); Time.timeScale = 1; CampaignCinematics.Load("Menu_MisterioDeVarginha"); }
        private void OnApplicationPause(bool value) { if (value) Save(); }
        private void OnDestroy() { if (Active == this) Active = null; Time.timeScale = 1; }
    }
}
