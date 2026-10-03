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
        private enum View { World, Pages, Building, Code, Notebook, Cutscene, Complete }
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
        private string PhaseTitle => phase == 2 ? "A CHAVE E A CAIXA" : phase == 4 ? "ENTRE AULAS E PISTAS" : "O CÓDIGO DAS 23:23";
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
            if (_progress.positionPhase == phase && (phase==2 || CampaignMapPlan.Create(phase).IsClear(new Vector2(_progress.x,_progress.y-.58f)))) _player.transform.position = new Vector3(_progress.x, _progress.y);
            else _player.transform.position = phase == 2 ? new Vector3(-5, 2) : (Vector3)(CampaignMapPlan.Create(phase).spawn+Vector2.up*.58f);
            RestoreInventory();
            _sound = gameObject.AddComponent<CampaignSoundscape>(); _sound.Configure(phase == 2 ? "House" : "School", _player);
            GameManager.Instance?.StartGame(); _ready = true; Lock(); Save();
        }
        private void RestoreInventory()
        {
            _player.HasBackpack = phase > 2 || (_progress.routine & 8) != 0;
            _player.HasFuscaKey = _player.HasResearchNotebook = phase > 2 || _progress.pagesSolved;
            _player.HasDecodedData = phase > 2 || (_progress.routine & 4) != 0;
        }
        private void BuildHousePoints()
        {
            _points.Add(new Point("wash", "HIGIENE • LAVATÓRIO", CampaignAdultHouse.WashApproach));
            _points.Add(new Point("food", "CAFÉ DA MANHÃ", new(4.6f, -2.05f)));
            _points.Add(new Point("work", "PREPARAR NOTEBOOK • CADEIRA", new(-5, -2.3f)));
            _points.Add(new Point("bag", "MOCHILA", new(-3.35f, 2.65f)));
            _points.Add(new Point("box", "CAIXA SOB A CAMA", new(-6.3f, 3.15f)));
            _points.Add(new Point("car", "FUSCA • IR À INDUSTRIAL", new(20, 0)));
            foreach (var exit in FindObjectsByType<FuscaLevelExit>(FindObjectsInactive.Include, FindObjectsSortMode.None)) exit.enabled = false;
            var bag = GameObject.Find("Backpack_Prop"); if (bag != null && (_progress.routine & 8) != 0) bag.SetActive(false);
        }
        private void BuildSchoolPoints()
        {
            var school = GameObject.Find("Escola_3_Sistema_Ambiente");
            if (school != null) CampaignMapConstruction.PreserveSchoolFacade(school.transform);
            foreach (var hostage in FindObjectsByType<VarginhaStudentHostage>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                // These are ordinary classes in Act II; retain each student's existing sprite.
                hostage.enabled = false;
                foreach (Transform child in hostage.transform) if (child.name.Contains("Cage") || child.name.Contains("Grade") || child.name.Contains("Jaula")) child.gameObject.SetActive(false);
            }
            foreach (var sr in FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (sr.name.Contains("HostageCage") || sr.name.Contains("Jaula") || sr.name.Contains("Cage")) sr.gameObject.SetActive(false);
            Vector2[] seats = { new(-5.85f,2.33f),new(3.75f,2.33f),new(6.55f,2.33f),new(-5.85f,-.07f),new(3.75f,-.07f),new(6.55f,-.07f),new(-5.85f,-2.47f),new(3.75f,-2.47f),new(6.55f,-2.47f) };
            string[] activities = { "conferindo arquivos", "organizando o projeto", "comparando imagens", "anotando os resultados", "fazendo a pesquisa", "revisando documentos", "verificando a legenda", "levando material à aula", "conversando no intervalo" };
            for (int i = 0; i < VarginhaPhase2Controller.StudentNames.Length; i++)
            {
                string name = VarginhaPhase2Controller.StudentNames[i]; var actor = GameObject.Find("Refem_" + name.Replace(" ", "_"));
                if (actor == null) continue;
                var life = actor.AddComponent<CampaignSchoolLife>();
                life.Configure(name, activities[i], seats[i], i == 7 ? new Vector2(.75f,-7.1f) : new Vector2(7.8f,-7.3f), i >= 7);
                _students.Add(life); _points.Add(new Point("student:" + i, name.ToUpperInvariant() + " • " + activities[i], seats[i], actor.transform));
            }
            var renan = new GameObject("Renan_Industrial_Campanha"); renan.transform.SetParent(transform); renan.transform.position = new Vector3(3,-7.7f);
            var renderer = renan.AddComponent<SpriteRenderer>(); renderer.sprite = VarginhaExperimentArt.Body(1); renderer.sortingOrder = 6;
            var renanFeet=renan.AddComponent<CircleCollider2D>();renanFeet.radius=.24f;renanFeet.offset=Vector2.down*.58f;
            VarginhaWorldDepth.Ensure(renderer,ground:renanFeet);
            _points.Add(new Point("renan", "RENAN", new(2.8f, -7.8f)));
            _points.Add(new Point("lesson", "EXPEDIENTE • MESA DO PROFESSOR", new(.75f, 4.75f)));
            _points.Add(new Point("archive", "ARQUIVO • PLANTAS E REGISTROS", new(-6.5f, -4.65f)));
            _points.Add(new Point("notebook", "NOTEBOOK • INVESTIGAÇÃO", new(-2.8f, 3f)));
            _points.Add(new Point("mural", "MURAL • LEGENDA DO LEVANTAMENTO", new(7.95f, -4.1f)));
            _points.Add(new Point("research", "PASTA • CORRESPONDÊNCIA DE PESQUISA", new(3.3f, 5.2f)));
            _points.Add(new Point("car", "FUSCA • PREPARAR SAÍDA", new(-5.5f, -10.4f)));
            var notebook = new GameObject("Notebook_Campanha_Industrial"); notebook.transform.SetParent(transform); notebook.transform.position = new Vector3(-3.05f,3.35f);
            var screen = notebook.AddComponent<SpriteRenderer>(); screen.sprite = VarginhaPixelArtSprites.Create("Notebook_Inventory", Color.gray); screen.sortingOrder = 5; notebook.transform.localScale = new Vector3(.72f,.52f,1);
            var folder = new GameObject("Pasta_Ouzana_Campanha"); folder.transform.SetParent(transform); folder.transform.position = new Vector3(1.25f,4.8f);
            var paper = folder.AddComponent<SpriteRenderer>(); paper.sprite = VarginhaPixelArtSprites.Create("Doc_Ouzana", new Color(.8f,.73f,.5f)); paper.sortingOrder = 8; folder.transform.localScale = new Vector3(.6f,.6f,1);
            var plan=CampaignMapPlan.Create(phase);
            foreach(var point in _points)
                foreach(var target in plan.points) if(target.id==point.id) point.position=target.position+new Vector2(0,.58f);
        }
        private void Update()
        {
            if (!_ready) return;
            if (VarginhaGameHUD.Instance?.IsInventoryOpen == true) return;
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
            {
                if (_dialogue != null) CloseDialogue();
                else if (_view != View.World && _view != View.Cutscene && _view != View.Complete) Show(View.World);
                else TogglePause();
            }
            if (_paused) return;
            if (_titleTime < 3) { _titleTime += Time.unscaledDeltaTime; Lock(); return; }
            if (_view == View.Cutscene)
            { _cutTime += Time.unscaledDeltaTime; if (_cutTime >= 7) FinishCutscene(); return; }
            if (IsBlocked) return;
            if (Keyboard.current?.gKey.wasPressedThisFrame == true) { OpenBackpack(); return; }
            if (Keyboard.current?.tabKey.wasPressedThisFrame == true) Show(View.Notebook);
            if (VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.Interact))
            { int nearby = Nearest(); if (nearby >= 0) Interact(_points[nearby].id); }
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
            if (id == "car" && phase == 2 && _progress.CanLeaveHouse) { CampaignStorySave.GoTo(3); return; }
            if (id == "car" && phase == 4 && _progress.buildingSolved) { CampaignStorySave.GoTo(5); return; }
            _sound?.Play(id == "box" || id == "archive" || id == "mural" ? "Paper" : "UI");
            if (id.StartsWith("student:")) StudentInteraction(int.Parse(id.Substring(8)));
            else if (phase == 2) HouseInteraction(id); else SchoolInteraction(id);
            RestoreInventory(); Save(); Lock();
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
                    if (_progress.routine != 15) { Say("Edelzio", "Antes de investigar a caixa, falta: "+_progress.RemainingHouseTasks+"."); break; }
                    if (_progress.pagesSolved) { Say("Edelzio", "Guardei a chave, o caderno e o primeiro fragmento de mapa. A mensagem continua ali: ELA AINDA ESTÁ AQUI."); break; }
                    if (!_progress.boxFound)
                    {
                        _progress.boxFound = true;
                        Cutscene("TRINTA ANOS NA MESMA CAIXA", "Entre os carrinhos estão a chave, uma fotografia e páginas do caderno.\nA letra infantil para no meio. A anotação seguinte parece ter sido feita ontem.", View.Pages);
                    }
                    else Show(View.Pages);
                    break;
                case "car":
                    if (_progress.CanLeaveHouse) CampaignStorySave.GoTo(3);
                    else Say("Edelzio", "Falta a chave. Termine a rotina e investigue a caixa sob a cama antes de sair.");
                    break;
            }
        }
        private void SchoolInteraction(string id)
        {
            switch (id)
            {
                case "renan":
                    _progress.renanMet = true;
                    if (phase == 5 && _progress.codeSolved)
                    {
                        _progress.renanConfirmed = true;
                        Cutscene("23:23 • ANTIGA DIOCESE", "Renan: As marcas indicam 23:23. O arquivo confirma: antiga diocese.\nEdelzio pega a mochila e parte.", View.Complete);
                    }
                    else Say("Renan", phase == 4
                        ? "Bom dia, Edelzio. Comece a aula e procure as plantas no armário. Compare com a foto do caderno."
                        : "Leia as marcas por posição na foto. Use a legenda do mural e teste no notebook.");
                    break;
                case "lesson":
                    _progress.lesson = true;
                    Say("Edelzio", "Presença conferida. A turma compara os arquivos; vou investigar os registros antigos."); break;
                case "students": Say("Alunos", "Professor, o projetor piscou sem ninguém tocar. Parecia uma pessoa atrás do prédio... depois a foto voltou ao normal."); break;
                case "archive":
                    if (!_progress.renanMet || !_progress.lesson) { Say("Edelzio", "Vou falar com Renan e iniciar a aula antes de mexer no arquivo."); break; }
                    _progress.archive = true;
                    Say("ARQUIVO • REGISTRO DE LEVANTAMENTO", "Três plantas antigas: depósito, anexo da diocese e escola.\nA foto do caderno mostra um portão central, três janelas estreitas à direita e uma torre recuada à esquerda. A referência marginal diz DIO — levantamento de 1996."); break;
                case "notebook":
                    if (phase == 4)
                    {
                        if (_progress.CanSolveBuilding) Show(View.Building);
                        else Say("Edelzio", "Preciso conversar com Renan, iniciar o expediente e buscar a planta no arquivo.");
                    }
                    else if (!_progress.symbolsFound)
                    {
                        _progress.symbolsFound = true;
                        Say("FOTO • AMPLIAÇÃO", "Quatro marcas têm números de posição:\nIII: círculo • I: círculo • IV: triângulo • II: triângulo.\nAs letras D I O se repetem no carimbo da planta.");
                    }
                    else if (_progress.CanDecode) Show(View.Code);
                    else Say("Edelzio", "Já anotei as posições da fotografia. Falta a legenda dos símbolos no mural.");
                    break;
                case "mural":
                    _progress.legendFound = true;
                    Say("MURAL • LEGENDA", "No levantamento: CÍRCULO = DUAS janelas; TRIÂNGULO = TRÊS águas do telhado.\nA ordem dos dados segue I, II, III, IV. Um separador foi colocado depois do segundo símbolo."); break;
                case "research":
                    _progress.ouzanaNote = true;
                    Say("OUZANA • CORRESPONDÊNCIA", "Há uma carta de Ouzana sobre amostras da região rural: 'Algumas marcas só aparecem quando o material reage à luz.'\nRenan guardou a pasta para você. O encontro com ela acontecerá depois que a investigação levar vocês à mata."); break;
                case "car":
                    if (phase == 4 && _progress.buildingSolved) CampaignStorySave.GoTo(5);
                    else if (phase == 5 && _progress.renanConfirmed) Show(View.Complete);
                    else Say("Edelzio", phase == 4 ? "Ainda preciso identificar a construção no notebook." : "Preciso decodificar a mensagem e confirmar a descoberta com Renan.");
                    break;
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
            else { if (action != null) yield return action.ReachRoutine(.5f); _progress.routine |= 1; }
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
                "Separei os recortes por data. As duas cópias da foto têm detalhes diferentes.",
                "Renan disse que o armário guarda plantas que nunca foram digitalizadas.",
                "Há marcas nos cantos das fotos. Compare a posição de cada uma.",
                "Confira portão, janelas e torre. Um portão sozinho não identifica o prédio.",
                "Escola e diocese estão no mesmo levantamento. Há um mapa no verso da planta.",
                "A sombra aparece só numa cópia. Na foto antiga, o portão estava vazio.",
                "Veja a legenda no mural: círculos contam janelas; triângulos, águas do telhado.",
                "O projetor piscou com o cabo conectado. Renan também viu.",
                "A carta de Ouzana trata da pesquisa rural. Está na pasta perto do projetor."
            };
            Say(student.StudentName, phase == 5 && _progress.codeSolved
                ? "23:23... Guarde a foto original. Precisamos comparar se ela mudar de novo."
                : (_progress.lesson ? "Estou " + student.Activity + ". " : "Bom dia, professor! ") + first[index]);
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
        public void TogglePause() { _paused = !_paused; _settings = false; Time.timeScale = _paused ? 0 : 1; _sound?.Suspend(_paused); Lock(); }
        private void Lock() { if (_player != null) _player.SetInputLocked(IsBlocked); }
        public bool SubmitPages()
        {
            if (!_progress.SubmitPages()) { _feedback = "As páginas ainda não acompanham os elementos da fotografia."; return false; }
            Save(); RestoreInventory();
            _sound?.Play("Success");
            Cutscene("ELA AINDA ESTÁ AQUI", "Casa à esquerda. Árvore no centro. Figura à direita.\nAo alinhar as páginas, surge uma escrita recente: ELA AINDA ESTÁ AQUI.\nChave, caderno e FRAGMENTO DE MAPA 1 adicionados à mochila.", View.World); return true;
        }
        public bool SubmitBuilding(int choice)
        {
            if (!_progress.SubmitBuilding(choice)) { _feedback = "Confira o portão, as janelas e a posição da torre. Todos devem coincidir."; return false; }
            Save();
            _sound?.Play("Success");
            Cutscene("UMA IMAGEM QUE NÃO ESTAVA ALI", "A planta B corresponde ao anexo da antiga diocese. No verso, outro trecho do mapa.\nFRAGMENTO DE MAPA 2 recolhido. Por um instante, a fotografia no notebook mostra uma silhueta junto ao portão.", View.Complete); return true;
        }
        public bool SubmitCode()
        {
            if (!_progress.SubmitCode()) { _feedback = "A leitura não fecha. Releia as posições I–IV e a legenda do levantamento."; return false; }
            Save(); RestoreInventory();
            _sound?.Play("Success");
            Cutscene("O PADRÃO SE REORGANIZA", "I → círculo → 2     II → triângulo → 3\nIII → círculo → 2     IV → triângulo → 3\n\n23:23 • DIOCESE ANTIGA\nVolte a Renan para confirmar a mensagem.", View.World); return true;
        }
        private string Objective()
        {
            if (phase == 2) return _progress.routine != 15 ? "Falta: "+_progress.RemainingHouseTasks+"."
                : !_progress.pagesSolved ? "A chave sumiu. Investigue a caixa sob a cama e organize as páginas." : "Leve as pistas ao trabalho. Vá ao Fusca no quintal à direita.";
            if (phase == 4) return !_progress.renanMet ? "Encontre Renan na entrada da Industrial."
                : !_progress.lesson ? "Inicie o expediente na mesa do professor." : !_progress.archive ? "Examine as plantas no armário do arquivo."
                : "Compare a fotografia com as plantas no notebook da sala.";
            return !_progress.symbolsFound ? "Amplie a fotografia no notebook." : !_progress.legendFound ? "Procure a legenda no mural à direita."
                : !_progress.codeSolved ? "Organize os símbolos e teste a associação no notebook." : "Confirme a mensagem das 23:23 com Renan na entrada.";
        }
        private void OnGUI()
        {
            if (!_ready) return;
            if (VarginhaGameHUD.Instance?.IsInventoryOpen == true) return;
            ExperimentGUI.Init(); GUI.depth = -3100;
            bool full = _view == View.Cutscene || _view == View.Complete || _titleTime < 3;
            if (full) ExperimentGUI.Box(new Rect(0, 0, Screen.width, Screen.height), Color.black);
            var matrix = ExperimentGUI.BeginCanvas();
            if (_titleTime < 3)
            {
                ExperimentGUI.Label(new Rect(250, 220, 870, 65), "ATO II — O CHAMADO", true);
                ExperimentGUI.Label(new Rect(250, 315, 900, 80), "FASE " + phase + " • " + PhaseTitle + "\nVarginha, 2026.");
            }
            else if (_view == View.Cutscene) DrawCutscene();
            else if (_view == View.Complete) DrawComplete();
            else
            {
                ExperimentGUI.Panel(new Rect(24, 18, 955, 102));
                ExperimentGUI.Label(new Rect(45, 29, 920, 40), "FASE " + phase + " • " + PhaseTitle, true);
                ExperimentGUI.Label(new Rect(45, 72, 920, 42), Objective(), small: true);
                if (ExperimentGUI.Button(new Rect(1080, 22, 175, 43), "PAUSAR")) TogglePause();
                if (ExperimentGUI.Button(new Rect(1080, 75, 175, 43), "CADERNO")) Show(View.Notebook);
                if (_player.HasBackpack && ExperimentGUI.Button(new Rect(1080,128,175,43),"MOCHILA")) OpenBackpack();
                if (_view == View.World && _dialogue == null && VarginhaGameSettings.Current.interactionHints)
                {
                    int near = Nearest(); ExperimentGUI.Panel(new Rect(230, 654, 820, 44));
                    ExperimentGUI.Label(new Rect(250, 664, 780, 30), near < 0 ? "WASD: ANDAR • E: EXAMINAR • TAB: CADERNO • G: MOCHILA" : "[E] " + _points[near].label, small: true);
                }
                if (_view != View.World) DrawInvestigation();
            }
            if (_dialogue != null)
            {
                ExperimentGUI.Panel(new Rect(125, 405, 1030, 255));
                bool portrait = _speaker == "Renan" || _students.Exists(student => student.StudentName == _speaker);
                if (_speaker == "Renan") ExperimentGUI.Character(new Rect(145, 430, 125, 135), 1);
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
                    if (ExperimentGUI.Button(new Rect(385,607,210,38), "ARQUIVOS")) DiscussWithRenan(1);
                    if (ExperimentGUI.Button(new Rect(615,607,210,38), "OUZANA")) DiscussWithRenan(2);
                }
                if (ExperimentGUI.Button(new Rect(870, 607, 250, 38), "CONTINUAR")) CloseDialogue();
            }
            if (_paused)
            {
                if (_settings) { GUI.matrix = matrix; if (VarginhaGameSettings.Draw()) _settings = false; return; }
                ExperimentGUI.Box(new Rect(0, 0, 1280, 720), new Color(0, 0, 0, .85f));
                ExperimentGUI.Panel(new Rect(385, 190, 510, 355));
                ExperimentGUI.Label(new Rect(415, 217, 450, 45), "PAUSADO", true);
                if (ExperimentGUI.Button(new Rect(415, 290, 450, 45), "CONTINUAR")) TogglePause();
                if (ExperimentGUI.Button(new Rect(415, 355, 450, 45), "CONFIGURAÇÕES")) _settings = true;
                if (ExperimentGUI.Button(new Rect(415, 420, 450, 45), "SALVAR E VOLTAR AO MENU")) Menu();
            }
            GUI.matrix = matrix;
        }
        public void DiscussWithRenan(int topic)
        {
            _sound?.Play("UI");
            Say("Renan", topic == 0 ? "A foto é antiga, mas a tinta sobre as marcas parece recente. Compare a posição do portão, das janelas e da torre com as plantas. Depois procure a ordem dos símbolos."
                : topic == 1 ? "Guardei os levantamentos no armário. Os alunos estão organizando versões digitais, mas algumas plantas só existem em papel. Cada documento pode responder uma parte da investigação."
                : "Ouzana enviou material da região rural. A pasta está perto do projetor. Ela observou marcas que reagem à luz; vamos guardar essa informação até termos motivo para investigar a mata.");
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
            else if (_view == View.Building)
            {
                ExperimentGUI.Label(new Rect(140, 143, 995, 55), "FOTO × PLANTA • QUAL CONSTRUÇÃO?", true);
                ExperimentGUI.Label(new Rect(140, 203, 995, 92), "Foto: portão CENTRAL • TRÊS janelas estreitas à DIREITA • torre RECUADA à ESQUERDA.\nQual planta coincide com os três marcos? Consulte o caderno se precisar.");
                string[] choices = { "PLANTA A • DEPÓSITO\nPortão lateral\n3 janelas à direita\nSem torre", "PLANTA B • ANEXO DA DIOCESE\nPortão central\n3 janelas à direita\nTorre recuada à esquerda", "PLANTA C • ANTIGA ESCOLA\nPortão central\n2 janelas à esquerda\nTorre na frente" };
                for (int i = 0; i < 3; i++) if (ExperimentGUI.Button(new Rect(145 + i * 335, 315, 315, 188), choices[i])) SubmitBuilding(i);
            }
            else if (_view == View.Code)
            {
                ExperimentGUI.Label(new Rect(140, 143, 995, 55), "FOTOGRAFIA • MENSAGEM OCULTA", true);
                ExperimentGUI.Label(new Rect(140, 203, 995, 90), "Dados encontrados: III ○ • I ○ • IV △ • II △. Ordene por posição I–IV.\nLegenda: círculo = duas janelas; triângulo = três águas do telhado. Clique para alterar.");
                for (int i = 0; i < 4; i++)
                    if (ExperimentGUI.Button(new Rect(160 + i * 245, 308, 220, 77), "POSIÇÃO " + (i + 1) + "\n" + (_progress.symbols[i] == 0 ? "CÍRCULO ○" : "TRIÂNGULO △"))) { _progress.symbols[i] = 1 - _progress.symbols[i]; Save(); }
                if (ExperimentGUI.Button(new Rect(165, 422, 360, 58), "CÍRCULO = " + _progress.circle)) { _progress.circle = _progress.circle % 4 + 1; Save(); }
                if (ExperimentGUI.Button(new Rect(555, 422, 360, 58), "TRIÂNGULO = " + _progress.triangle)) { _progress.triangle = _progress.triangle % 4 + 1; Save(); }
                if (ExperimentGUI.Button(new Rect(800, 550, 325, 45), "DECODIFICAR")) SubmitCode();
            }
            else DrawNotebook();
            if (!string.IsNullOrEmpty(_feedback)) ExperimentGUI.Label(new Rect(145, 506, 955, 42), _feedback, small: true);
            if (ExperimentGUI.Button(new Rect(145, 578, 270, 50), "VOLTAR")) { Save(); Show(View.World); }
        }
        private void DrawNotebook()
        {
            ExperimentGUI.Label(new Rect(140, 143, 995, 55), "CADERNO • EVIDÊNCIAS", true);
            var entries = new List<string>();
            if (_progress.boxFound) entries.Add("Caixa de 1996: chave, fotografia e páginas com desenhos.");
            if (_progress.pagesSolved) entries.Add("Mensagem recente: ELA AINDA ESTÁ AQUI. Fragmento de mapa 1 recolhido.");
            if (_progress.arrival) entries.Add("Pane no Fusca: rádio, ignição e painel contradizem o que vi.");
            if (_progress.archive) entries.Add("Foto: portão central, 3 janelas à direita, torre recuada à esquerda. Carimbo DIO.");
            if (_progress.buildingSolved) entries.Add("Planta B: anexo da diocese antiga. Fragmento de mapa 2 recolhido.");
            if (_progress.symbolsFound) entries.Add("Foto ampliada: III ○ • I ○ • IV △ • II △. Ordenar por I, II, III, IV.");
            if (_progress.legendFound) entries.Add("Legenda: círculo = 2 janelas; triângulo = 3 águas do telhado.");
            if (_progress.codeSolved) entries.Add("Mensagem decodificada: 23:23 • DIOCESE ANTIGA.");
            if (_progress.ouzanaNote) entries.Add("Carta de Ouzana: marcas nas amostras da região rural reagem à luz. Guardada para investigar no futuro.");
            _journalScroll = GUI.BeginScrollView(new Rect(140, 206, 675, 335), _journalScroll, new Rect(0, 0, 650, Mathf.Max(335, entries.Count * 94)));
            ExperimentGUI.Label(new Rect(0, 0, 650, Mathf.Max(335, entries.Count * 94)), entries.Count == 0 ? "Nenhuma pista recolhida. Observe os objetos e converse com as pessoas." : string.Join("\n\n", entries), small: true);
            GUI.EndScrollView();
            ExperimentGUI.Label(new Rect(860, 207, 265, 42), "MAPA • " + _progress.MapFragments + "/3", true);
            for (int i = 0; i < _progress.MapFragments; i++) GUI.DrawTextureWithTexCoords(new Rect(860 + i * 130, 270, 125, 115), VarginhaExperimentArt.Map(), VarginhaExperimentArt.MapUV(i == 0 ? 2 : 0));
            ExperimentGUI.Label(new Rect(860, 420, 255, 90), "O terceiro fragmento será investigado na diocese, no próximo ato.", small: true);
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
            if (phase == 5 && _progress.codeSolved) ExperimentGUI.Label(new Rect(480, 207, 500, 55), "23 : 23", true);
            ExperimentGUI.Caption(new Rect(250, 353, 780, 202), _cutText, VarginhaGameSettings.Current.subtitleSize);
            if (ExperimentGUI.Button(new Rect(890, 615, 275, 44), "CONTINUAR")) FinishCutscene();
        }
        private void DrawComplete()
        {
            ExperimentGUI.Label(new Rect(220, 165, 895, 55), "FASE " + phase + " CONCLUÍDA", true);
            ExperimentGUI.Label(new Rect(220, 270, 870, 155), phase == 4
                ? "O segundo fragmento está no caderno. A fotografia ainda contém marcas que não deveriam estar ali.\n\nPróxima fase: O Código das 23:23."
                : "Renan confirmou a mensagem. Cruze os registros municipais, o relato urbano e os arquivos da Industrial.\n\nAto II concluído. Próxima fase: Fragmentos.");
            if (phase == 4 && ExperimentGUI.Button(new Rect(370, 490, 540, 55), "CONTINUAR PARA A FASE 5")) CampaignStorySave.GoTo(5);
            if (phase == 5 && ExperimentGUI.Button(new Rect(370, 490, 540, 55), "CONTINUAR PARA A FASE 6 • FRAGMENTOS")) CampaignStorySave.GoTo(6);
            if (ExperimentGUI.Button(new Rect(370, 570, 540, 45), "SALVAR E VOLTAR AO MENU")) Menu();
        }
        public void Save()
        {
            if (_progress == null || _player == null) return;
            _progress.positionPhase = phase; _progress.x = _player.transform.position.x; _progress.y = _player.transform.position.y; CampaignStorySave.Write(_progress);
        }
        private void Menu() { Save(); Time.timeScale = 1; SceneManager.LoadScene("Menu_MisterioDeVarginha"); }
        private void OnApplicationPause(bool value) { if (value) Save(); }
        private void OnDestroy() { if (Active == this) Active = null; Time.timeScale = 1; }
    }
}
