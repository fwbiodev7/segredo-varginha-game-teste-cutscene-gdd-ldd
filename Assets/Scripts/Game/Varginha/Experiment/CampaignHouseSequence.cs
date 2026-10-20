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
        private ExperimentFrameComposer _missionComposer;

        private void BuildKeyInvestigation()
        {
            var layout = CampaignIllustratedMaps.Get(2);
            Vector2 Position(float x, float y, Vector2 fallback) => layout != null ? layout.Position(x,y) + Vector2.up*.58f : fallback;
            // These are walkable approach points, independent from the tabletop artwork.
            AddEvidencePoint("counter", "BALCÃO • CHAVE DO FUSCA", Position(620,620,new Vector2(3,-3)));
            AddEvidencePoint("exitclue", "PORTA • FECHADURA", Position(947,424,new Vector2(8,0)));
            AddEvidencePoint("paperclue", "PAPEL PRESO JUNTO AO QUARTO", Position(407,325,new Vector2(-1,1)));
            foreach(var objective in _guidancePlan.points)
            {var contextual=_points.Find(p=>p.id==objective.id);if(contextual!=null)objective.label=contextual.label;}
            _keyRest = layout != null ? (Vector3)layout.Position(590,698) : new Vector3(3,-4);
            var key = new GameObject("Chave_Fusca_Balcao"); key.transform.SetParent(transform);
            key.transform.position = _keyRest;
            _counterKey = key.AddComponent<SpriteRenderer>();
            _counterKey.sprite = CampaignHouseArt.Frame(0);
            key.transform.localScale = Vector3.one * .52f; _counterKey.sortingOrder = 30;
            var support=GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/Kitchen_Cabinet")??GameObject.Find("Kitchen_Cabinet");
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
                    if ((_progress.routine & 7) != 7) Say("Edelzio",h.Objective(_progress.routine));
                    else StartCoroutine(PlaceKey());
                }
                else if (h.KeyOnCounter) Say("Edelzio","Deixei a chave aqui. Falta buscar a mochila no quarto.");
                else if (!h.Complete) ObserveKeyClue(0,"O balcão está vazio. O copo continua no mesmo lugar; não há chave no chão.");
                else Say("Edelzio","A chave está guardada comigo. Não vou deixá-la aqui outra vez.");
                return true;
            }
            if (id == "exitclue" || id == "paperclue")
            {
                if (!h.Missing) { Say("Edelzio",h.Objective(_progress.routine)); return true; }
                ObserveKeyClue(id == "exitclue" ? 1 : 2,id == "exitclue"
                    ? "O trinco continua fechado por dentro. Não abri esta porta desde que acordei."
                    : "Um pedacinho de papel envelhecido ficou preso junto ao quarto. Há um traço azul, igual ao das minhas anotações de infância.");
                return true;
            }
            if (id == "bag" && h.step == 0) { Say("Edelzio",h.Objective(_progress.routine)); return true; }
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
            _busy = true; Lock(); BeginMissionShot("world","ANTES DE SAIR","Vou deixar a chave no balcão. Depois pego a mochila.");
            CampaignCameraDirector.Reveal(_keyRest,2.8f);
            var action = _player.GetComponent<VarginhaPlayerActionAnimation>();
            _counterKey.enabled = true;
            _counterKey.sprite=CampaignHouseArt.Frame(1);
            Vector3 hand = _player.transform.position + Vector3.up*.1f;
            _counterKey.transform.position=hand;
            if (action != null) StartCoroutine(action.ReachRoutine(.9f));
            for (float t=0;t<.45f;t+=Time.deltaTime)
            { _counterKey.transform.position=Vector3.Lerp(hand,_keyRest,Mathf.SmoothStep(0,1,t/.45f)); yield return null; }
            _counterKey.transform.position = _keyRest; _counterKey.sprite=CampaignHouseArt.Frame(0); _sound.PlayAt("Key",_keyRest);
            _progress.house.PlaceKey(_progress.routine); Save();
            yield return new WaitForSeconds(1.1f);
            EndMissionShot(); _busy = false; Lock();
        }
        private IEnumerator DisappearKey()
        {
            // Commit while the player is in the bedroom: reloading cannot duplicate the key.
            _progress.house.TakeBag(); _counterKey.enabled = false; Save();
            transform.Find("Vestigio_Papel_Quarto")?.gameObject.SetActive(true);
            BeginMissionShot("world","UM RUÍDO FORA DO QUARTO","[Um toque metálico na cozinha. Depois, papel raspando.]");
            _sound.PlayAt("Key",_keyRest); yield return new WaitForSeconds(.6f);
            CampaignCameraDirector.Reveal(_keyRest,2.8f);
            _sound.PlayAt("Scratching",_points.Find(p=>p.id=="box").Position);
            yield return new WaitForSeconds(1.8f);
            EndMissionShot();
        }
        private IEnumerator OpenKeyBox()
        {
            _busy=true;Lock();
            BeginMissionShot("world","A CAIXA DE 1996","[Os arranhões cessam quando Edelzio se abaixa.]");
            var point=_points.Find(p=>p.id=="box").Position;CampaignCameraDirector.Reveal(point,2.5f);
            var action=_player.GetComponent<VarginhaPlayerActionAnimation>();
            if(action!=null)yield return action.CrouchRoutine(.9f);
            _sound.PlayAt("Paper",point);
            _progress.boxFound=true;_progress.house.step=4;Save();RestoreInventory();
            yield return new WaitForSeconds(.6f);EndMissionShot();_busy=false;
            Cutscene("EU SEI QUE VI ALGUMA COISA", "A chave está dentro da caixa que estava fechada. Ao lado dela, o caderno de 1996.\nNa primeira página: Eu sei que vi alguma coisa.\nO desenho foi rasgado. Há tinta recente sobre os pedaços.",View.Pages);
        }
        private void BeginMissionShot(string shot,string title,string caption,int panel=0)
        { _cinematicShot=shot; _cinemaTitle=title; _cinemaCaption=caption; _cinemaPanel=panel; _cinemaClock=Time.time; }
        private void EndMissionShot() { _cinematicShot=null; _sound.CinematicDucking(0); }
        private IEnumerator FuscaSequence()
        {
            _schoolTransition = _busy = true; _dialogue=null; Lock();
            BeginMissionShot("world","O CAMINHO DA INDUSTRIAL","Chave no bolso. Mochila pronta. Ainda é possível chegar à aula.");
            var car = GameObject.Find("Mapa_Campanha/02_Mobilia_Colisoes/Fusca");
            if (car == null) car=GameObject.Find("Fusca");
            if (car != null && car.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                CampaignCameraDirector.Reveal(car.transform.position,4);
                var door=car.GetComponent<FuscaDoorMotion>()??car.AddComponent<FuscaDoorMotion>();
                var body=_player.GetComponent<Rigidbody2D>(); if(body!=null){body.linearVelocity=Vector2.zero;body.simulated=false;}
                var start=_player.transform.position;
                for(float t=0;t<.7f;t+=Time.deltaTime)
                {door.SetOpenAmount(t/.7f);_player.transform.position=Vector3.Lerp(start,door.EntryPosition,Mathf.SmoothStep(0,1,t/.7f));yield return null;}
                _player.SetCarriedItemsVisible(false);
                foreach(var visual in _player.GetComponentsInChildren<Renderer>())visual.enabled=false;
                for(float t=0;t<.4f;t+=Time.deltaTime){door.SetOpenAmount(1-t/.4f);yield return null;}
                door.SetOpenAmount(0); _sound.Play("Starter");
                var departure=car.GetComponent<FuscaDepartureAnimation>()??car.AddComponent<FuscaDepartureAnimation>();
                departure.Configure(new[]{renderer.sprite}); departure.Depart(null,1.3f);
                yield return new WaitForSeconds(1.5f);
            }
            _sound.BeginRoad();
            BeginMissionShot("road","VARGINHA • 2026","[Motor e trânsito.] O caminho de todos os dias.");
            yield return new WaitForSeconds(2.2f);
            BeginMissionShot("interior","O RÁDIO","O sinal se perde. Mesmo desligado, o rádio continua chiando.",2);
            _radioReducedMotion=VarginhaGameSettings.Current.reducedMotion; _sound.PlayRadioInterference(_radioReducedMotion);
            yield return new WaitForSeconds(2.5f);
            BeginMissionShot("interior","O PAINEL","O velocímetro dispara. Os faróis apagam. O Fusca está parando.",1);
            _sound.Engine(false); _sound.Play("Starter"); yield return new WaitForSeconds(2.5f);
            BeginMissionShot("interior","UMA VOZ CONHECIDA","[Voz infantil no rádio] “Não deixa ela sair.”",2);
            _sound.PlayRadioInterference(_radioReducedMotion);
            // A real recorded line may be supplied without replacing it with synthetic speech.
            var voice=Resources.Load<AudioClip>("Varginha/Audio/EdelzioCrianca_NaoDeixaElaSair");
            _sound.Voice(voice);
            yield return new WaitForSeconds(3);
            BeginMissionShot("interior","SILÊNCIO","A ignição está quente. Nenhum som. Edelzio solta a chave.",3);
            _sound.CinematicDucking(1); yield return new WaitForSeconds(2.5f);
            BeginMissionShot("road","NINGUÉM TOCOU NA CHAVE","[O motor volta sozinho.] Preciso conversar com alguém na escola.");
            _sound.CinematicDucking(0); _sound.Play("Starter"); _sound.Engine(true,.4f); yield return new WaitForSeconds(2.2f);
            _sound.Engine(false);
            BeginMissionShot("arrival","INDUSTRIAL","A aula vai começar. Aquela voz era a minha... quando criança. Preciso entender o que aconteceu.");
            yield return new WaitForSeconds(2);
            EndMissionShot();
        }
        private void DrawMissionCinema()
        {
            bool reduced=VarginhaGameSettings.Current.reducedMotion;
            float t=Time.time-_cinemaClock;
            if(_cinematicShot!="world")
            {
                float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f);
                float extraX=(Screen.width/scale-1280)/2,extraY=(Screen.height/scale-720)/2;
                if(extraX>0)
                {ExperimentGUI.Box(new Rect(-extraX-1,-extraY,extraX+1,720+2*extraY),Color.black);ExperimentGUI.Box(new Rect(1280,-extraY,extraX+1,720+2*extraY),Color.black);}
                if(extraY>0)
                {ExperimentGUI.Box(new Rect(0,-extraY-1,1280,extraY+1),Color.black);ExperimentGUI.Box(new Rect(0,720,1280,extraY+1),Color.black);}
            }
            if(_cinematicShot=="interior")
            {
                _missionComposer??=new ExperimentFrameComposer();
                _missionComposer.Compose(new ExperimentShot{asset="FuscaBreakdown",panel=_cinemaPanel,mode=_cinemaPanel==2?"interference":"still"},t,reduced);
                GUI.DrawTexture(new Rect(0,0,1280,720),_missionComposer.Frame);
            }
            else if(_cinematicShot=="road") DrawRoadShot(t,reduced);
            else if(_cinematicShot=="arrival")
            {
                ExperimentGUI.Box(new Rect(0,0,1280,720),new Color(.03f,.05f,.07f));
                var facade=Resources.Load<Texture2D>(VarginhaIndustrialSchoolFacade.ResourcePath);
                if(facade!=null)GUI.DrawTexture(new Rect(60,90,1160,440),facade,ScaleMode.ScaleToFit);
                DrawOriginalCar(new Rect(840,430,240,125),CampaignOriginalFusca.Side);
            }
            ExperimentGUI.Box(new Rect(0,0,1280,86),Color.black);
            ExperimentGUI.Box(new Rect(0,550,1280,170),Color.black);
            ExperimentGUI.Label(new Rect(55,24,1090,46),_cinemaTitle,true);
            ExperimentGUI.Caption(new Rect(65,575,1150,115),_cinemaCaption,VarginhaGameSettings.Current.subtitleSize);
            if(_cinemaTitle=="A CAIXA DE 1996")
            {ExperimentGUI.Panel(new Rect(460,145,360,360));CampaignHouseArt.Draw(new Rect(490,165,300,300),t<.6f?3:4);}
            if(t<.22f)ExperimentGUI.Box(new Rect(0,0,1280,720),new Color(0,0,0,1-t/.22f));
            if(!_paused&&CampaignHudIcons.Button(1174,CampaignHudIcons.Icon.Pause,"ESC","Pausa"))TogglePause();
        }
        private void DrawRoadShot(float t,bool reduced)
        {
            ExperimentGUI.Box(new Rect(0,0,1280,720),new Color(.10f,.16f,.18f));
            ExperimentGUI.Box(new Rect(420,0,440,720),new Color(.18f,.19f,.22f));
            ExperimentGUI.Box(new Rect(438,0,7,720),new Color(.75f,.75f,.60f));
            ExperimentGUI.Box(new Rect(835,0,7,720),new Color(.75f,.75f,.60f));
            int shift=reduced?0:Mathf.FloorToInt(t*64)%140;
            for(int y=-140;y<720;y+=140)ExperimentGUI.Box(new Rect(637,y+shift,6,70),new Color(.85f,.73f,.34f));
            DrawOriginalCar(new Rect(666,305,108,202),CampaignOriginalFusca.Top(CampaignWorkshopVehicle.Facing.North));
        }
        private static void DrawOriginalCar(Rect rect,Sprite sprite)
        { var uv=sprite.rect;GUI.DrawTextureWithTexCoords(rect,sprite.texture,new Rect(uv.x/sprite.texture.width,uv.y/sprite.texture.height,uv.width/sprite.texture.width,uv.height/sprite.texture.height)); }
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
            ExperimentGUI.Panel(new Rect(145,605,990,58));ExperimentGUI.Label(new Rect(163,615,954,42),line,small:true);
            if(VarginhaInputActions.InventoryPressed)_progress.house.tutorial|=4;
        }
        private void DrawKeyDeduction()
        {
            ExperimentGUI.Label(new Rect(140,143,995,55),"CHAVE DESAPARECIDA • CRUZAR EVIDÊNCIAS",true);
            ExperimentGUI.Label(new Rect(145,215,975,130),"BALCÃO: vazio; copo intacto.\nSAÍDA: trinco fechado por dentro.\nQUARTO: papel envelhecido com traço azul junto à passagem.");
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
