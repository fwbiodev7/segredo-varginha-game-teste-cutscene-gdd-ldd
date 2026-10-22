using System.Collections;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public sealed partial class VarginhaCampaignStage
    {
        private SpriteRenderer _counterKey;
        private Vector3 _keyRest;
        private string _cinematicShot, _cinemaTitle, _cinemaCaption;
        private int _cinemaPanel;
        private float _cinemaClock;
        private readonly CampaignTimedSubtitle _cinemaSubtitle = new();
        private int _cinemaEvent;
        private bool _cinemaSkipAllowed,_cinemaSkipRequested;
        private float _cinemaEventClock;
        public bool CanSkipMissionCinema=>_cinemaEvent!=0&&_cinemaSkipAllowed&&!_paused&&Time.time-_cinemaEventClock>.6f;
        public bool TrySkipMissionCinema(){if(!CanSkipMissionCinema)return false;_cinemaSkipRequested=true;return true;}
        private void BeginMissionEvent(int flag)
        {_cinemaEvent=flag;_cinemaSkipAllowed=(_progress.cinematicsSeen&flag)!=0;_cinemaSkipRequested=false;_cinemaEventClock=Time.time;}
        private void CompleteMissionEvent()
        {_progress.cinematicsSeen|=_cinemaEvent;Save();_cinemaEvent=0;_cinemaSkipAllowed=_cinemaSkipRequested=false;}
        private IEnumerator MissionWait(float seconds)
        {for(float t=0;t<seconds&&!_cinemaSkipRequested;t+=Time.deltaTime)yield return null;}

        private void BuildKeyInvestigation()
        {
            var layout = CampaignIllustratedMaps.Get(2);
            Vector2 Position(float x, float y, Vector2 fallback) => layout != null ? layout.Position(x,y) + Vector2.up*.58f : fallback;
            // These are walkable approach points, independent from the tabletop artwork.
            AddEvidencePoint("counter", "MESA DO CAFÉ • CHAVE DO FUSCA", Position(749,600,new Vector2(4.6f,-2.05f)));
            AddEvidencePoint("exitclue", "PORTA • FECHADURA", Position(947,424,new Vector2(8,0)));
            AddEvidencePoint("paperclue", "PAPEL PRESO JUNTO AO QUARTO", Position(407,325,new Vector2(-1,1)));
            foreach(var objective in _guidancePlan.points)
            {var contextual=_points.Find(p=>p.id==objective.id);if(contextual!=null)objective.label=contextual.label;}
            _keyRest = layout != null ? (Vector3)layout.Position(733,531) : new Vector3(3,-4);
            var key = new GameObject("Chave_Fusca_Balcao"); key.transform.SetParent(transform);
            key.transform.position = _keyRest;
            _counterKey = key.AddComponent<SpriteRenderer>();
            _counterKey.sprite = CampaignHouseArt.Frame(0);
            key.transform.localScale = Vector3.one * .9f; _counterKey.sortingOrder = 30;
            var support=GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/CoffeeTable_Kitchen")??GameObject.Find("CoffeeTable_Kitchen");
            VarginhaWorldDepth.Ensure(_counterKey,supportingObject:support!=null?support.transform:null,offset:2);
            _counterKey.enabled = _progress.house.KeyOnCounter;
            var paper = new GameObject("Vestigio_Papel_Quarto"); paper.transform.SetParent(transform);
            paper.transform.position = (Vector3)(Position(407,325,new Vector2(-1,1)) - Vector2.up*.5f);
            var scrap=paper.AddComponent<SpriteRenderer>();
            scrap.sprite=CampaignHouseArt.Frame(2);
            paper.transform.localScale=Vector3.one*.42f;scrap.sortingOrder=4;
            VarginhaWorldDepth.Ensure(scrap);
            paper.SetActive(_progress.house.Missing);
        }
        private void AddEvidencePoint(string id, string label, Vector2 position)
        {
            _points.Add(new Point(id,label,position));
            _guidancePlan.points.Add(new CampaignMapPlan.Point(id,label,position.x,position.y-.58f));
        }
        private bool PointAvailable(string id) => phase != 2 || id switch
        {
            "counter" => _progress.house.step >= 1 || (_progress.routine & 7) == 7,
            "exitclue" or "paperclue" => _progress.house.Missing && !_progress.house.Complete,
            _ => true
        };
        private bool HandleKeyInteraction(string id)
        {
            var h = _progress.house;
            if (id == "counter")
            {
                if (h.step == 0)
                {
                    if ((_progress.routine & 7) != 7) CampaignPickupNotice.Show(h.Objective(_progress.routine));
                    else StartCoroutine(PlaceKey());
                }
                else if (h.KeyOnCounter) Say("Edelzio","Deixei a chave aqui. Falta buscar a mochila no quarto.");
                else if (!h.Complete) ObserveKeyClue(0,"A chave não está na mesa do café. Não há nada no chão ao redor.");
                else Say("Edelzio","A chave está guardada comigo. Não vou deixá-la aqui outra vez.");
                return true;
            }
            if (id == "exitclue" || id == "paperclue")
            {
                if (!h.Missing) { CampaignPickupNotice.Show(h.Objective(_progress.routine)); return true; }
                ObserveKeyClue(id == "exitclue" ? 1 : 2,id == "exitclue"
                    ? "O trinco continua fechado por dentro. Não abri esta porta desde que acordei."
                    : "Um pedacinho de papel envelhecido ficou preso junto ao quarto. Há um traço azul, igual ao das minhas anotações de infância.");
                return true;
            }
            if (id == "bag" && h.step == 0) { CampaignPickupNotice.Show(h.Objective(_progress.routine)); return true; }
            if (id == "box" && h.CanOpenBox && !_progress.boxFound) { StartCoroutine(OpenKeyBox()); return true; }
            if (id == "box" && !h.CanOpenBox)
            {
                if (h.Missing && h.clues == 7) Show(View.Deduction);
                else Say("Edelzio",h.Missing ? "A caixa continua fechada. Antes de mexer, preciso entender o que mudou na casa." : "Minha caixa de brinquedos de 1996. Faz anos que não abro. Preciso preparar a aula.");
                return true;
            }
            return false;
        }
        private void ObserveKeyClue(int clue,string line)
        {
            if (!_progress.house.Observe(clue)) return;
            _sound.PlayAt(clue == 2 ? "Paper" : "Key",_player.transform.position);
            Save();
            if (_progress.house.clues == 7 && !_progress.house.deduced) Show(View.Deduction);
            else Say("Edelzio",line + " A observação ficou registrada no caderno.");
        }
        public bool SubmitKeyConclusion(int choice)
        {
            if (!_progress.house.Deduce(choice)) { _feedback = "Essa hipótese não explica a saída fechada e o papel junto ao quarto."; return false; }
            Save(); Show(View.World);
            _sound.PlayAt("Scratching",_points.Find(p=>p.id=="box").Position);
            Say("Edelzio","A porta não foi aberta. O papel parece vir da minha caixa... Agora escuto arranhões debaixo da cama.");
            return true;
        }
        private IEnumerator PlaceKey()
        {
            _busy = true; Lock();BeginMissionEvent(1); BeginMissionShot("world","ANTES DE SAIR","Vou deixar a chave na mesa do café. Depois pego a mochila.");
            CampaignCameraDirector.Reveal(_keyRest,3.8f);
            var action = _player.GetComponent<VarginhaPlayerActionAnimation>();
            _counterKey.enabled = true;
            _counterKey.sprite=CampaignHouseArt.Frame(1);
            Vector3 hand = _player.transform.position + Vector3.up*.1f;
            _counterKey.transform.position=hand;
            if (action != null) StartCoroutine(action.ReachRoutine(1.35f));
            yield return MissionWait(.28f);
            for (float t=0;t<.75f&&!_cinemaSkipRequested;t+=Time.deltaTime)
            {float p=Mathf.SmoothStep(0,1,t/.75f);_counterKey.transform.position=Vector3.Lerp(hand,_keyRest,p)+Vector3.up*Mathf.Sin(p*Mathf.PI)*.13f; yield return null; }
            _counterKey.transform.position = _keyRest; _counterKey.sprite=CampaignHouseArt.Frame(0); _sound.PlayAt("Key",_keyRest);
            _progress.house.PlaceKey(_progress.routine); Save();
            yield return MissionWait(1.65f);
            while(action!=null&&action.IsActing)yield return null;
            CompleteMissionEvent();EndMissionShot(); _busy = false; Lock();
        }
        private IEnumerator DisappearKey()
        {
            // Commit while the player is in the bedroom: reloading cannot duplicate the key.
            BeginMissionEvent(2);_progress.house.TakeBag(); _counterKey.enabled = false; Save();
            transform.Find("Vestigio_Papel_Quarto")?.gameObject.SetActive(true);
            BeginMissionShot("world","UM RUÍDO FORA DO QUARTO","[Um toque metálico na cozinha. Depois, papel raspando.]");
            _sound.PlayAt("Key",_keyRest); yield return MissionWait(.6f);
            CampaignCameraDirector.Reveal(_keyRest,2.8f);
            _sound.PlayAt("Scratching",_points.Find(p=>p.id=="box").Position);
            yield return MissionWait(2.5f);
            CompleteMissionEvent();EndMissionShot();
        }
        private IEnumerator OpenKeyBox()
        {
            _busy=true;Lock();BeginMissionEvent(4);
            BeginMissionShot("world","A CAIXA DE 1996","[Os arranhões cessam quando Edelzio se abaixa.]");
            var point=_points.Find(p=>p.id=="box").Position;CampaignCameraDirector.Reveal(point,2.5f);
            var action=_player.GetComponent<VarginhaPlayerActionAnimation>();
            if(action!=null)yield return action.CrouchRoutine(.9f);
            _sound.PlayAt("Paper",point);
            _progress.boxFound=true;_progress.house.step=4;Save();RestoreInventory();
            yield return MissionWait(1.6f);CompleteMissionEvent();EndMissionShot();_busy=false;
            Cutscene("EU SEI QUE VI ALGUMA COISA", "A chave está dentro da caixa que estava fechada. Ao lado dela, o caderno de 1996.\nNa primeira página: Eu sei que vi alguma coisa.\nO desenho foi rasgado. Há tinta recente sobre os pedaços.",View.Pages);
        }
        private void BeginMissionShot(string shot,string title,string caption,int panel=0)
        { _cinematicShot=shot; _cinemaTitle=title; _cinemaCaption=caption; _cinemaPanel=panel; _cinemaClock=Time.time; }
        private void EndMissionShot() { _cinematicShot=null; _sound.CinematicDucking(0); }
        private IEnumerator FuscaSequence()
        {
            _schoolTransition=_busy=true;_textMessage.Close();_dialogue=null;Lock();BeginMissionEvent(8);
            var visuals=_player.GetComponentsInChildren<Renderer>(true);
            var visibility=new bool[visuals.Length];for(int i=0;i<visuals.Length;i++)visibility[i]=visuals[i].enabled;
            var body=_player.GetComponent<Rigidbody2D>();bool simulated=body!=null&&body.simulated;
            bool complete=false;FuscaDoorMotion door=null;
            try
            {
                BeginMissionShot("world","O CAMINHO DA INDUSTRIAL","Chave no bolso. Mochila pronta. Ainda é possível chegar à aula.");
                var car=GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/Fusca")??GameObject.Find("Fusca");
                if(car!=null&&car.TryGetComponent<SpriteRenderer>(out var renderer))
                {
                    CampaignCameraDirector.Reveal(car.transform.position,4);
                    door=car.GetComponent<FuscaDoorMotion>()??car.AddComponent<FuscaDoorMotion>();
                    if(body!=null){body.linearVelocity=Vector2.zero;body.simulated=false;}
                    Vector3 start=_player.transform.position;_sound.Play("DoorOpen");
                    for(float t=0;t<.7f&&!_cinemaSkipRequested;t+=Time.deltaTime)
                    {door.SetOpenAmount(t/.7f);_player.transform.position=Vector3.Lerp(start,door.EntryPosition,Mathf.SmoothStep(0,1,t/.7f));yield return null;}
                    _player.SetCarriedItemsVisible(false);
                    foreach(var visual in visuals)visual.enabled=false;
                    for(float t=0;t<.4f&&!_cinemaSkipRequested;t+=Time.deltaTime){door.SetOpenAmount(1-t/.4f);yield return null;}
                    door.SetOpenAmount(0);_sound.Play("Starter");
                    var departure=car.GetComponent<FuscaDepartureAnimation>()??car.AddComponent<FuscaDepartureAnimation>();
                    departure.Configure(new[]{renderer.sprite});departure.Depart(null,1.3f);
                    yield return MissionWait(1.5f);
                }
                for(int i=0;i<CampaignFuscaCinema.Shots.Length&&!_cinemaSkipRequested;i++)
                {
                    var shot=CampaignFuscaCinema.Shots[i];
                    BeginMissionShot(shot.view,shot.title,shot.caption,shot.panel);
                    switch(i)
                    {
                        case 0:_sound.BeginRoad();_sound.Engine(true,.4f);break;
                        case 1:_sound.PlayRadioInterference(VarginhaGameSettings.Current.reducedMotion);break;
                        case 2:_sound.Engine(false);_sound.Play("CarFail");break;
                        case 3:
                            _sound.PlayRadioInterference(VarginhaGameSettings.Current.reducedMotion);
                            _sound.Voice(Resources.Load<AudioClip>("Varginha/Audio/Voices/"+CampaignMessagePresentation.IdFor("Edelzio criança","Não deixa ela sair."))??Resources.Load<AudioClip>("Varginha/Audio/EdelzioCrianca_NaoDeixaElaSair"));break;
                        case 4:_sound.CinematicDucking(1);break;
                        case 5:_sound.CinematicDucking(0);_sound.Play("Starter");_sound.Engine(true,.4f);break;
                        case 6:_sound.Engine(false);_sound.Play("DoorOpen");break;
                    }
                    yield return MissionWait(shot.duration);
                }
                bool skipped=_cinemaSkipRequested;
                complete=true;CompleteMissionEvent();
                // Keep the arrival framing under the scene fade, including when a seen scene is skipped.
                if(skipped){BeginMissionShot("arrival","INDUSTRIAL",CampaignFuscaCinema.Shots[6].caption);_cinemaClock-=CampaignFuscaCinema.Shots[6].duration;}
            }
            finally
            {
                if(door!=null)door.SetOpenAmount(0);
                if(body!=null){body.simulated=simulated;body.linearVelocity=Vector2.zero;}
                for(int i=0;i<visuals.Length;i++)if(visuals[i]!=null)visuals[i].enabled=visibility[i];
                if(_player!=null){_player.SetCarriedItemsVisible(true);RestoreInventory();}
                _sound?.Engine(false);_sound?.CinematicDucking(0);
                if(!complete){EndMissionShot();_cinemaEvent=0;_busy=_schoolTransition=false;Lock();}
            }
        }
        private void DrawMissionCinema()
        {
            bool reduced=VarginhaGameSettings.Current.reducedMotion;
            float t=Time.time-_cinemaClock;
            if(_cinematicShot!="world")
            {
                // Arrival uses an inset school illustration. Clear its entire
                // viewport first so the live house cannot show through its margins.
                ExperimentGUI.Box(new Rect(0,0,1280,720),Color.black);
                float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f);
                float extraX=(Screen.width/scale-1280)/2,extraY=(Screen.height/scale-720)/2;
                if(extraX>0)
                {ExperimentGUI.Box(new Rect(-extraX-1,-extraY,extraX+1,720+2*extraY),Color.black);ExperimentGUI.Box(new Rect(1280,-extraY,extraX+1,720+2*extraY),Color.black);}
                if(extraY>0)
                {ExperimentGUI.Box(new Rect(0,-extraY-1,1280,extraY+1),Color.black);ExperimentGUI.Box(new Rect(0,720,1280,extraY+1),Color.black);}
            }
            if(_cinematicShot=="interior"||_cinematicShot=="road")
                CampaignFuscaCinema.DrawCabin(t,_cinemaTitle,_cinematicShot=="road"?0:_cinemaPanel,reduced);
            else if(_cinematicShot=="arrival")
            {
                CampaignFuscaCinema.DrawArrival(t,reduced);
            }
            ExperimentGUI.Box(new Rect(0,0,1280,86),Color.black);
            ExperimentGUI.Box(new Rect(0,550,1280,170),Color.black);
            ExperimentGUI.Label(new Rect(55,24,1090,46),_cinemaTitle,true);
            if(CanSkipMissionCinema&&GUI.Button(new Rect(850,25,300,38),"PULAR • ["+VarginhaInputActions.InteractLabel+"]"))TrySkipMissionCinema();
            _cinemaSubtitle.Draw(_cinemaCaption,t);
            if(_cinemaTitle=="A CAIXA DE 1996")
            {ExperimentGUI.Panel(new Rect(460,145,360,360));CampaignHouseArt.Draw(new Rect(490,165,300,300),t<.6f?3:4);}
            if(t<.22f)ExperimentGUI.Box(new Rect(0,0,1280,720),new Color(0,0,0,1-t/.22f));
            if(!_paused&&CampaignHudIcons.Button(1174,CampaignHudIcons.Icon.Pause,"ESC","Pausa"))TogglePause();
        }
        private void DrawMissionPause(Matrix4x4 matrix)
        {
            if(!_paused)return;
            if(_settings){GUI.matrix=matrix;if(VarginhaGameSettings.Draw())_settings=false;return;}
            int action=ExperimentGUI.PausePanel("INVESTIGAÇÃO • PAUSA");
            if(action==1)TogglePause();if(action==2)_settings=true;if(action==3)Menu();
        }
        private void DrawHouseTutorial()
        {
            string line;
            if (_progress.house.step==0 && (_progress.house.tutorial & 1)==0)
            { line="Movimento: "+(VarginhaInputActions.UsingGamepad?"ANALÓGICO / D-PAD":VarginhaInputBindings.GetKeyboard(VarginhaInputAction.MoveUp)+" / "+VarginhaInputBindings.GetKeyboard(VarginhaInputAction.MoveLeft)+" / "+VarginhaInputBindings.GetKeyboard(VarginhaInputAction.MoveDown)+" / "+VarginhaInputBindings.GetKeyboard(VarginhaInputAction.MoveRight))+" • aproxime-se da pia."; if(VarginhaInputActions.Move.sqrMagnitude>.1f)_progress.house.tutorial|=1; }
            else if (_progress.house.step==0 && (_progress.routine & 7)!=7) line="["+VarginhaInputActions.InteractLabel+"] interagir • ["+CampaignHudIcons.KeyLabel(CampaignHudIcons.Icon.Notebook)+"] consultar objetivos e pistas • F1 dicas graduais";
            else if ((_progress.routine & 8)!=0 && (_progress.house.tutorial&4)==0) line="["+CampaignHudIcons.KeyLabel(CampaignHudIcons.Icon.Backpack)+"] abrir mochila • consulte seus itens antes de partir";
            else return;
            if((_progress.house.tutorial&1)==0)line="Movimente-se e aproxime-se da pia.";
            else if((_progress.routine&7)!=7)line="["+VarginhaInputActions.InteractLabel+"] interagir   ["+CampaignHudIcons.KeyLabel(CampaignHudIcons.Icon.Notebook)+"] pistas   F1 dicas";
            else line="["+CampaignHudIcons.KeyLabel(CampaignHudIcons.Icon.Backpack)+"] abrir mochila";
            var style=CampaignMessagePresentation.TextStyle(10);float width=Mathf.Clamp(line.Length*10+32,240,560);
            float height=style.CalcHeight(new GUIContent(line),width-24)+18;
            var area=new Rect(640-width/2,690-height,width,height);ExperimentGUI.Panel(area);
            GUI.Label(new Rect(area.x+12,area.y+9,width-24,height-18),line,style);
            if(VarginhaInputActions.InventoryPressed)_progress.house.tutorial|=4;
        }
        private void DrawKeyDeduction()
        {
            ExperimentGUI.Label(new Rect(140,143,995,55),"CHAVE DESAPARECIDA • CRUZAR EVIDÊNCIAS",true);
            ExperimentGUI.Label(new Rect(145,215,975,130),"MESA DO CAFÉ: chave ausente; nada no chão.\nSAÍDA: trinco fechado por dentro.\nQUARTO: papel envelhecido com traço azul junto à passagem.");
            string[] choices={"Alguém entrou pela porta e levou a chave.","O papel liga o desaparecimento aos objetos antigos do quarto.","A chave foi esquecida no carro antes de entrar em casa."};
            for(int i=0;i<3;i++)if(ExperimentGUI.Button(new Rect(145,355+i*48,975,42),choices[i]))SubmitKeyConclusion(i);
        }
        private void DrawTornPages()
        {
            for(int slot=0;slot<3;slot++)
            {
                int card=_progress.pages[slot];float x=160+slot*335;
                CampaignHouseArt.Draw(new Rect(x+70,280,144,144),5+card);
            }
        }
    }
}
