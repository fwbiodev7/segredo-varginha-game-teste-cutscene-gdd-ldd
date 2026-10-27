using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    /// <summary>Investigation chapters on the shipped house and Industrial maps.</summary>
    public sealed partial class VarginhaCampaignStage : MonoBehaviour
    {
        public int phase = 2;
        public static VarginhaCampaignStage Active { get; private set; }
        public static bool IsModalOpen => Active != null && Active.IsBlocked;
        private bool IsBlocked => !_ready || _titleTime < 3 || _paused || _busy || _dialogue != null || _view != View.World || VarginhaGameHUD.Instance?.IsInventoryOpen == true;
        private enum View { World, Pages, Notebook, Hint, Cutscene, Complete, Deduction, School, Document }
        private View _view;
        private Vector2 _journalScroll;
        private CampaignStory _progress;
        private EdelzioTopDownController _player;
        private readonly List<Point> _points = new();
        private bool _ready, _paused, _settings, _busy;
        private CampaignSoundscape _sound;
        private CampaignHouseRoutineItems _routineItems;
        private CampaignMapPlan _guidancePlan;
        private readonly List<CampaignSchoolLife> _students = new();
        public bool IsActing => _busy;
        private float _titleTime, _saveTime, _cutTime;
        private bool _carContext;
        private bool _schoolTransition, _radioReducedMotion;
        private float _radioCueTime = -1;
        private string _dialogue, _speaker, _feedback, _cutTitle, _cutText;
        private readonly CampaignMessagePresentation _textMessage = new();
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
            if (phase == 2) { CampaignAdultHouse.Apply(); _routineItems=gameObject.AddComponent<CampaignHouseRoutineItems>();_routineItems.Build();BuildHousePoints(); } else BuildSchoolPoints();
            if (_progress.positionPhase == phase && CampaignMapPlan.Create(phase).IsClear(new Vector2(_progress.x,_progress.y-.58f))) _player.transform.position = new Vector3(_progress.x, _progress.y);
            else _player.transform.position = (Vector3)(CampaignMapPlan.Create(phase).spawn+Vector2.up*.58f);
            RestoreInventory();
            _sound = gameObject.AddComponent<CampaignSoundscape>(); _sound.Configure(phase == 2 ? "House" : "School", _player);
            _guidancePlan=CampaignMapPlan.Create(phase);
            if (phase == 2) BuildKeyInvestigation();
            GameManager.Instance?.StartGame(); _ready = true; Lock(); Save();
        }
        private void RestoreInventory()
        {
            _player.HasBackpack = phase > 2 || _progress.boxFound || (_progress.routine & 8) != 0;
            _player.HasFuscaKey = _player.HasResearchNotebook = phase > 2 || _progress.pagesSolved;
            _player.HasDecodedData = phase > 2 || _progress.boxFound || (_progress.routine & 4) != 0;
            _routineItems?.Restore(_progress.routine);
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
            _points.Add(new Point("notebook", "MESA DE INVESTIGAÇÃO • MEUS DOCUMENTOS", new(.75f,3.8f)));
            var layoutSchool=CampaignIllustratedMaps.Get(phase);
            if(layoutSchool!=null)_points.Find(point=>point.id=="notebook").position=layoutSchool.Objective("notebook")+Vector2.up*.58f;
            _points.Add(new Point("mural","MURAL • CONSULTA AO ACERVO",layoutSchool!=null?layoutSchool.Position(1080,105)+Vector2.up*.58f:new Vector2(7.5f,-4.55f)));
        }
        private int _hintLevel;
        public bool OpenHints()
        {
            if (!_ready || _busy || _paused || _dialogue != null || _view == View.Cutscene || _view == View.Complete || VarginhaGameHUD.Instance?.IsInventoryOpen == true) return false;
            if(phase>=4)_hintLevel=_progress.school.hintLevel;
            Show(View.Hint); return true;
        }
        private void Update()
        {
            if (CampaignCinematics.IsTransitioning) return;
            if (!_ready) return;
            if(_houseExitGate!=null)_houseExitGate.enabled=!_progress.CanLeaveHouse;
            if (VarginhaGameHUD.Instance?.IsInventoryOpen == true) return;
            if(_paused&&_settings&&VarginhaInputActions.CancelPressed&&!VarginhaInputActions.PausePressed){_settings=false;return;}
            if ((VarginhaInputActions.PausePressed || VarginhaInputActions.CancelPressed && (_paused || _dialogue!=null || _view!=View.World)))
            {
                if (_dialogue != null && !_carContext) CloseDialogue();
                else if(_view==View.School&&_schoolHints)_schoolHints=false;
                else if(_view==View.Document)CloseSchoolDocument();
                else if (_view != View.World && _view != View.Cutscene && _view != View.Complete) Show(View.World);
                else TogglePause();
            }
            _textMessage.Tick(Time.unscaledDeltaTime, _paused);
            if (_textMessage.FinishedByVoice) { if (_view == View.Cutscene) FinishCutscene(); else CloseDialogue(); }
            if (_paused) return;
            if (_dialogue != null)
            {
                if (_carContext ? VarginhaInputActions.CarPressed : VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact)) AdvanceDialogue();
                return;
            }
            if(_cinematicShot!=null)
            {
                Lock();
                if(VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact))TrySkipMissionCinema();
                return;
            }
            if (_titleTime < 3) { _titleTime += Time.unscaledDeltaTime; Lock(); return; }
            if (_view == View.Cutscene)
            { _cutTime += Time.unscaledDeltaTime; if (VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact)) { if (!_cutIsSpeech || _textMessage.Advance()) FinishCutscene(); } return; }
            if (IsBlocked) return;
            if(phase==2&&_progress.house.KeyOnCounter&&!_progress.house.bagReminder)
            {
                var exit=_points.Find(p=>p.id=="exitclue");
                if(exit!=null&&Vector2.Distance(_player.transform.position,exit.Position)<1.1f)
                {HandleKeyInteraction("exitclue");return;}
            }
            CampaignHud.For(phase).Tick(Time.unscaledDeltaTime,true,CampaignGuidance.Next(_progress,phase));
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
            { if (!PointAvailable(_points[i].id)) continue; float d = Vector2.Distance(_player.transform.position, _points[i].Position); if (d < distance) { distance = d; best = i; } }
            return best;
        }
        public void Interact(string id)
        {
            if (!_ready || _busy) return;
            if (phase == 2 && HandleKeyInteraction(id)) return;
            _carContext=id=="car";
            if (id == "car" && phase == 2 && _progress.CanLeaveHouse) { DepartForSchool(); return; }
            _sound?.Play(id == "box" || id == "notebook" ? "Paper" : "UI");
            var cinematicPoint = _points.Find(p => p.id == id);
            if (cinematicPoint != null && id != "car") CampaignCameraDirector.Reveal(cinematicPoint.Position);
            if (id.StartsWith("student:")) StudentInteraction(int.Parse(id.Substring(8)));
            else if (phase == 2) HouseInteraction(id); else SchoolInteraction(id);
            RestoreInventory(); Save(); Lock();
        }
        private void DepartForSchool() { Save(); StartCoroutine(SchoolTransition()); }
        private IEnumerator SchoolTransition()
        {
            yield return FuscaSequence();
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

                    if (_progress.pagesSolved) { Say("Edelzio", "Pronto, guardei tudo. Mas essa frase... Ela ainda está aqui."); break; }
                    if (!_progress.boxFound)
                    {
                        _progress.boxFound = true;
                        _progress.school.RecoverDocuments();
                        _progress.house.step = 4;
                        Cutscene("TRINTA ANOS NA MESMA CAIXA", "Entre os carrinhos estão a chave, uma fotografia, recortes de jornal e anotações de criança.\nOs documentos ficaram guardados no caderno. A anotação seguinte parece ter sido feita ontem.", View.Pages);
                    }
                    else Show(View.Pages);
                    break;
                case "car":
                    if (_progress.CanLeaveHouse) DepartForSchool();
                    else Say("Edelzio", "Antes de sair, preciso olhar a caixa e juntar essas páginas.");
                    break;
            }
        }
        private void SchoolInteraction(string id)
        {
            if(id=="renan")
            {
                _progress.renanMet=true;
                Say("Renan","Bom dia, Edelzio! Estou terminando a atividade da turma. Pode usar a mesa para organizar seu material enquanto isso. Depois me conta como foi o caminho.");
            }
            else if(id=="notebook") OpenSchoolInvestigation();
            else if(id=="mural") OpenSchoolDocument(6,View.World);
        }
        private IEnumerator RoutineAction(string id)
        {
            int routineBit = id == "wash" ? 1 : id == "food" ? 2 : id == "work" ? 4 : 8;
            if ((_progress.routine & routineBit) != 0 && !(id == "bag" && _progress.house.KeyOnCounter)) { CampaignPickupNotice.Show("Já está pronto. " + _progress.house.Objective(_progress.routine)); yield break; }
            _busy = true; Lock();
            var action = _player.GetComponent<VarginhaPlayerActionAnimation>();
            var visual = _player.GetComponent<VarginhaPlayerSpriteAnimation>();
            if (id == "bag")
            {
                var bag = GameObject.Find("Backpack_Prop");
                if (bag != null && !_player.HasBackpack)
                {
                    _sound.Play("Zip"); _sound.Play("PickupPlim"); bag.GetComponent<BackpackPickupAnimation>()?.PlayPickup(_player);
                    float time = 0; while (!_player.HasBackpack && time < 2) { time += Time.deltaTime; yield return null; }
                    if (!_player.HasBackpack) { _player.EquipBackpack(); bag.SetActive(false); }
                }
                _progress.routine |= 8;
            }
            else if (id == "food" && action != null)
            {
                bool finished = false; action.PlayDrinkCoffee(GameObject.Find("Coffee_Cup")?.transform, () => { finished = true; _player.RestoreOneHeart(); });
                float time = 0; while (!finished && time < 5) { time += Time.deltaTime; yield return null; }
                if(finished)_progress.routine |= 2;
            }
            else if (id == "work" && action != null)
            {
                bool finished = false;
                action.PlayNotebookSession(GameObject.Find("Notebook_TI")?.transform, () => { finished = true; _sound.Play("Typing"); });
                float time = 0; while (action.IsActing && time < 5) { time += Time.deltaTime; yield return null; }
                if (finished && action.isActiveAndEnabled && !action.IsActing) _progress.routine |= 4;
                else { _busy = false; VarginhaGameHUD.Instance?.CloseDialogue(); Say("Edelzio", "Vou dar a volta pra chegar no notebook."); yield break; }
            }
            else { bool finished=action==null;if(action!=null)yield return action.WashFaceRoutine(()=>{finished=true;});if(finished)_progress.routine|=1; }
            if((_progress.routine&routineBit)==0)
            {
                _busy=false;RestoreInventory();Save();CampaignPickupNotice.Show("Ação interrompida. Pode tentar novamente.");Lock();yield break;
            }
            if (id == "bag" && _progress.house.KeyOnCounter) yield return DisappearKey();
            _busy = false; VarginhaGameHUD.Instance?.CloseDialogue(); RestoreInventory(); Save();
            CampaignPickupNotice.Show(id == "bag" ? "Mochila coletada. Confira a mesa do café."
                : id == "work" ? "Notebook pronto. A tela mostrou uma árvore."
                : id == "food" ? "Café da manhã concluído. São 07:10."
                : "Rosto lavado. Preparação atualizada.");
        }
        private void StudentInteraction(int index)
        {
            if (index < 0 || index >= _students.Count) return;
            var student = _students[index]; student.Greet(); _progress.studentsTalked |= 1 << index;
            string[] first = {"Hoje estamos organizando dados por data.","Deixamos a mesa do professor livre para você.",
                "Já salvei a versão do nosso trabalho.","Vou terminar estas anotações antes do intervalo.",
                "O prazo do projeto é na próxima aula.","Precisamos conferir as fontes da pesquisa.",
                "Uma legenda clara ajuda a entender os dados.","Trouxe o material que faltava para a aula.","Daqui a pouco a turma vai fazer uma pausa."};
            Say(student.StudentName,("Estou " + student.Activity + ". ") + first[index]);
        }
        private void LateUpdate() { if (_ready) Lock(); }
        public bool OpenBackpack()
        {
            if (IsBlocked || !_player.HasBackpack) return false;
            _sound?.Play("Zip"); return VarginhaGameHUD.Instance != null && VarginhaGameHUD.Instance.OpenBackpack();
        }
        private void Say(string speaker, string line) { _speaker = speaker; _dialogue = line; _textMessage.Show(this,speaker,line); Lock(); }
        private void AdvanceDialogue() { if (_textMessage.Advance()) CloseDialogue(); }
        public void CloseDialogue() { if (_schoolTransition) return; _textMessage.Close(); _dialogue = null; Lock(); }
        private void Show(View value) { _view = value; _selected = -1; _feedback = null; Lock(); }
        private bool _cutIsSpeech;
        private void Cutscene(string title, string line, View next, string speaker = null)
        { _cutTitle = title; _cutText = line; _afterCut = next; _cutTime = 0; _cutIsSpeech = speaker != null; if (_cutIsSpeech) _textMessage.Show(this,speaker,line); Show(View.Cutscene); }
        public void FinishCutscene() { _textMessage.Close(); _cutIsSpeech = false; Show(_afterCut); }
        public void TogglePause() { _paused = !_paused; _settings = false; Time.timeScale = _paused ? 0 : 1; CampaignCinematics.Pause(_paused); _sound?.Suspend(_paused); Lock(); }
        private void Lock() { if (_player != null) _player.SetInputLocked(IsBlocked); }
        public bool SubmitPages()
        {
            if (!_progress.SubmitPages()) { _feedback = "As páginas ainda não acompanham os elementos da fotografia."; return false; }
            Save(); RestoreInventory();
            CampaignPickupNotice.Show("Chave, caderno e fragmento de mapa 1 adicionados à mochila.");
            _sound?.Play("Success");
            Cutscene("ELA AINDA ESTÁ AQUI", "Casa à esquerda. Árvore no centro. Figura à direita.\nAo alinhar as páginas, surge uma escrita recente: ELA AINDA ESTÁ AQUI.\nOs registros foram guardados no caderno.", View.World); return true;
        }
        // Retained for callers compiled against the previous chapter API; no removed answer can unlock progress.
        public bool SubmitPhotoClue(int choice)=>false;
        public bool SubmitCode()=>false;
        private string Objective()
        {
            if (phase == 2) return _progress.house.Objective(_progress.routine);
            return _progress.school.Objective;
        }
        private void OnGUI()
        {
            VarginhaGamepadUI.Begin("house-school:"+GetEntityId()+":"+_view+":"+_progress?.school.stage+":"+_schoolHints+":"+_paused+":"+(_dialogue!=null),_view!=View.World||_dialogue!=null||_paused,50);
            if (!_ready) return;
            if (VarginhaGameHUD.Instance?.IsInventoryOpen == true) return;
            ExperimentGUI.Init(); GUI.depth = -3100;
            if(_titleTime<3){CampaignChapterPreview.Draw(phase,_titleTime);return;}
            if(_view==View.Cutscene && phase!=2)CampaignChapterPreview.Background(phase,_cutTime);
            var matrix = ExperimentGUI.BeginCanvas();
            if (_cinematicShot != null) { DrawMissionCinema(); DrawMissionPause(matrix); GUI.matrix = matrix; return; }
            if (_view == View.Cutscene) DrawCutscene();
            else if (_view == View.Complete) DrawComplete();
            else
            {
                if(!IsBlocked)ExperimentGUI.Objective(CampaignSequence.Heading(phase) + (phase==2?" • 2026 • 07:00":" • 2026"), PhaseTitle, CampaignGuidance.Next(_progress,phase),CampaignHud.For(phase));
                if(!IsBlocked)CampaignGuidance.DrawMarker(_guidancePlan,_progress,phase);
                if (!IsBlocked && phase == 2) DrawHouseTutorial();
                bool hudEnabled=GUI.enabled; GUI.enabled=hudEnabled&&!_paused&&_view==View.World&&_dialogue==null;
                if (CampaignHudIcons.Button(1010, CampaignHudIcons.Icon.Notebook, "TAB", "Caderno")) Show(View.Notebook);
                bool enabled = GUI.enabled; GUI.enabled = enabled&&_player.HasBackpack;
                if (CampaignHudIcons.Button(1092, CampaignHudIcons.Icon.Backpack, "G", "Mochila")) OpenBackpack();
                GUI.enabled = enabled;
                if (CampaignHudIcons.Button(1174, CampaignHudIcons.Icon.Pause, "ESC", "Pausa")) TogglePause();
                GUI.enabled=hudEnabled;
                if (_view == View.World && _dialogue == null && VarginhaGameSettings.Current.interactionHints)
                {
                    int near=Nearest();
                    ExperimentGUI.ContextPrompt(near<0?null:"["+(_points[near].id=="car"?VarginhaInputActions.CarLabel:VarginhaInputActions.InteractLabel)+"] "+_points[near].label);
                }
                if (_view != View.World){bool before=GUI.enabled;GUI.enabled=before&&!_paused;DrawInvestigation();GUI.enabled=before;}
            }
            DrawRadioInterference();
            if (_dialogue != null && !_paused)
            {
                if (_speaker == "Renan")
                {
                    float y = CampaignMessageBox.Bounds(_textMessage).y - 40;
                    if (ExperimentGUI.Button(new Rect(195,y,270,30), "AULA")) DiscussWithRenan(0);
                    if (ExperimentGUI.Button(new Rect(505,y,270,30), "TURMA")) DiscussWithRenan(1);
                    if (ExperimentGUI.Button(new Rect(815,y,270,30), "CAMINHO")) DiscussWithRenan(2);
                }
                bool dialogueEnabled = GUI.enabled; GUI.enabled = dialogueEnabled && !_schoolTransition;
                if (CampaignMessageBox.Draw(_textMessage,_carContext)) AdvanceDialogue();
                GUI.enabled = dialogueEnabled;
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
        private void DrawRadioInterference()
        {
            if (_radioCueTime < 0 || _paused) return;
            float elapsed = Time.time - _radioCueTime;
            if (elapsed < 0 || elapsed >= .65f) return;
            float fade = 1 - Mathf.SmoothStep(0, 1, elapsed / .65f);
            // One soft pulse, with captions rendered afterwards. Reduced motion
            // keeps a quiet tint only: no white pulse, jitter or changing static.
            if (_radioReducedMotion || VarginhaGameSettings.Current.reducedMotion)
            { ExperimentGUI.Box(new Rect(0, 0, 1280, 720), new Color(0, 0, 0, .08f * fade)); return; }
            float pulse = elapsed < .07f ? elapsed / .07f : Mathf.Clamp01((.24f - elapsed) / .17f);
            ExperimentGUI.Box(new Rect(0, 0, 1280, 720), new Color(.62f, .74f, .8f, .10f * pulse));
            int tick = Mathf.FloorToInt(elapsed * 12);
            for (int i = 0; i < 5; i++)
            {
                int y = ((i * 47 + tick * 17) % 210) * 3;
                ExperimentGUI.Box(new Rect(0, y, 1280, (i % 2 + 1) * 3), new Color(.68f, .78f, .78f, .16f * fade));
            }
            for (int i = 0; i < 24; i++)
            {
                int x = ((i * 37 + tick * 23) % 420) * 3, y = ((i * 53 + tick * 11) % 240) * 3;
                ExperimentGUI.Box(new Rect(x, y, 6, 3), new Color(1, 1, 1, .18f * fade));
            }
        }
        public void DiscussWithRenan(int topic)
        {
            _sound?.Play("UI");
            Say("Renan", topic == 0 ? "Hoje a atividade é ordenar, comparar e filtrar informações. A turma está terminando os exercícios."
                : topic == 1 ? "O pessoal já dividiu as tarefas do projeto. Depois a gente confere o que cada grupo preparou."
                : "Deu tudo certo para chegar? O trânsito estava tranquilo quando passei pela entrada.");
        }
        private void DrawInvestigation()
        {
            if(_view==View.School){DrawSchoolInvestigation();return;}
            if(_view==View.Document){DrawSchoolDocument();return;}
            ExperimentGUI.Box(new Rect(0, 0, 1280, 720), new Color(0, 0, 0, .83f));
            ExperimentGUI.Panel(new Rect(105, 120, 1070, 525));
            if (_view == View.Deduction) DrawKeyDeduction();
            else if (_view == View.Pages)
            {
                ExperimentGUI.Label(new Rect(140, 143, 995, 55), "CADERNO • PÁGINAS DE 1996", true);
                ExperimentGUI.Label(new Rect(140, 204, 995, 78), "A tinta atravessa os rasgos. Una o muro e os galhos entre as páginas.\nSelecione duas páginas para trocá-las; compare o desenho, não apenas as palavras.");
                string[] cards = { "PÁGINA DA CASA", "PÁGINA DA ÁRVORE", "PÁGINA DA FIGURA" };
                for (int i = 0; i < 3; i++)
                    if (ExperimentGUI.Button(new Rect(145 + i * 335, 425, 315, 65), (_selected == i ? "[SELECIONADA]\n" : "") + cards[_progress.pages[i]]))
                    {
                        if (_selected < 0) _selected = i;
                        else { int old = _progress.pages[i]; _progress.pages[i] = _progress.pages[_selected]; _progress.pages[_selected] = old; _selected = -1; Save(); }
                    }
                DrawTornPages();
                if (ExperimentGUI.Button(new Rect(800, 550, 325, 45), "CONFERIR PÁGINAS")) SubmitPages();
            }
            else if (_view == View.Hint)
            {
                int before=_hintLevel;CampaignHints.Draw(_progress,phase,ref _hintLevel);
                if(phase>=4&&before!=_hintLevel){_progress.school.hintLevel=_hintLevel;Save();}
            }
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
            if (_cutIsSpeech)
            {
                ExperimentGUI.Label(new Rect(100,140,1080,60),_cutTitle,true);
                if(CampaignMessageBox.Draw(_textMessage) && _textMessage.Advance())FinishCutscene();
                return;
            }
            if(phase==2)
            {
                ExperimentGUI.Box(new Rect(0,0,1280,720),new Color(0,0,0,.65f));
                ExperimentGUI.Panel(new Rect(160,130,960,460));
                ExperimentGUI.Label(new Rect(190,155,900,65),_cutTitle,true);
                CampaignHouseArt.Draw(new Rect(190,245,255,255),4);
                ExperimentGUI.Caption(new Rect(475,240,605,300),_cutText,VarginhaGameSettings.Current.subtitleSize);
                if(ExperimentGUI.Button(new Rect(820,610,300,48),"CONTINUAR"))FinishCutscene();
                return;
            }
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
            ExperimentGUI.Caption(new Rect(250, 353, 780, 202), _cutText, VarginhaGameSettings.Current.subtitleSize);
            if (ExperimentGUI.Button(new Rect(890, 615, 275, 44), "CONTINUAR")) FinishCutscene();
        }
        private void DrawComplete()
        {
            ExperimentGUI.Panel(new Rect(180,180,920,320));
            ExperimentGUI.Label(new Rect(215,215,850,90),"INVESTIGAR O ACERVO DA BIBLIOTECA",true);
            ExperimentGUI.Label(new Rect(215,335,850,110),"Coleção C-96 • Acervo Municipal\nOs jornais e as anotações permanecem no caderno. A biblioteca conserva os originais para a próxima investigação.");
            if(ExperimentGUI.ContinueChapterButton())ContinueChapter();
        }
        private bool _chapterAdvancing;
        public bool ContinueChapter()
        {
            if(_progress==null||_chapterAdvancing||_paused||_view!=View.Complete||!_progress.school.completed||CampaignCinematics.IsTransitioning)return false;
            _chapterAdvancing=true;Save();CampaignStorySave.GoTo(6);return true;
        }
        public void Save()
        {
            if (_progress == null || _player == null) return;
            if(_houseExitGate!=null)_houseExitGate.enabled=!_progress.CanLeaveHouse;
            _progress.positionPhase = phase; _progress.x = _player.transform.position.x; _progress.y = _player.transform.position.y; CampaignStorySave.Write(_progress);
        }
        private void Menu() { Save(); Time.timeScale = 1; CampaignCinematics.Load("Menu_MisterioDeVarginha"); }
        private void OnApplicationPause(bool value) { if (value) Save(); }
        private void OnDestroy() { _textMessage.Close(); _radioCueTime = -1; _sound?.CinematicDucking(0); if (Active == this) Active = null; Time.timeScale = 1; }
    }
}
