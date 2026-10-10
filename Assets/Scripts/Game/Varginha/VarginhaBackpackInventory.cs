using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Game.Varginha
{
    /// <summary>
    /// Pause-owned backpack inventory unified with PixelHUDFrame / Pause Menu styling.
    /// </summary>
    public sealed class VarginhaBackpackInventory : MonoBehaviour
    {
        public bool IsOpen { get; private set; }
        private GameObject _canvasObject;
        private GameObject _ownedEventSystem;
        private TMP_FontAsset _font;
        private VarginhaStudentAllySquad _squad;
        private EdelzioTopDownController _player;
        public bool ShowingStudents { get; private set; }
        public bool ShowingSupport {get;private set;}
        private int _inspected;
        private float _resumeScale;
        private int _openedFrame;
        private EventSystem _navigationOwner;
        private bool _navigationEvents;

        private readonly UnityEngine.UI.Image[] _cells = new UnityEngine.UI.Image[9];
        private readonly TMP_Text[] _states = new TMP_Text[9];
        private readonly TMP_Text[] _cellNames = new TMP_Text[9];
        private readonly UnityEngine.UI.Image[] _cellPortraits = new UnityEngine.UI.Image[9];
        private UnityEngine.UI.Button _itemsTab, _studentsTab;
        private UnityEngine.UI.Button _supportTab;
        private TMP_Text _supportTabLabel;
        private TMP_Text _itemsTabLabel, _studentsTabLabel;
        private TMP_Text _equipCaption;

        private static readonly string[] ItemNames = { "Mochila", "Chave do Fusca", "Caderno de 1996", "Notebook", "Documento de 1898", "Lanterna" };
        private static readonly string[] ItemArt = { "Backpack_Inventory", "Inventory_Key", "Inventory_Journal", "Notebook_Inventory", "Doc_Inventory", "Inventory_Flashlight" };
        private static readonly string[] ItemDescriptions = {
            "Guarda itens e organiza a equipe.",
            "Abre o Fusca.",
            "Anotações de 1996. Necessário para viajar.",
            "Usado na decodificação de dados.",
            "Pista histórica importante.",
            "Equipe na mochila e use V / RT para ligar."
        };

        private TMP_Text _name, _description, _availability, _equipped, _items;
        private UnityEngine.UI.Image _portrait;
        private UnityEngine.UI.Button _equip;

        // Unified PixelHUDFrame color palette matching VarginhaGameHUD
        private static readonly Color PanelColor = new(.042f, .056f, .068f, .98f);
        private static readonly Color PanelBorder = new(.28f, .40f, .44f, .98f);
        private static readonly Color PaperColor = new(.92f, .91f, .82f);
        private static readonly Color AccentCyan = new(.64f, .84f, .86f);
        private static readonly Color MutedCyan = new(.48f, .59f, .61f);
        private static readonly Color FocusGold = new(.96f, .78f, .34f);

        // Cached UI 9-slice textures and sprites
        private static Sprite _modalFrameSprite;
        private static Sprite _subPanelSprite;
        private static Sprite _slotNormalSprite;
        private static Sprite _slotSelectedSprite;
        private static Sprite _buttonNormalSprite;
        private static Sprite _buttonActiveSprite;
        private static Sprite _separatorSprite;

        public bool Open(EdelzioTopDownController player)
        {
            if (IsOpen || player == null || !player.HasBackpack || player.IsInputLocked
                || player.CurrentSanity <= 0 || Time.timeScale <= 0
                || VarginhaTravelCinematic.IsTravelling
                || VarginhaGameHUD.Instance?.IsDialogueOpen == true
                || VarginhaGameHUD.Instance?.IsVictoryOpen == true) return false;

            _squad = Experiment.CampaignFinalAllies.Active?.Squad??Object.FindAnyObjectByType<VarginhaStudentAllySquad>();
            _player = player;
            ShowingStudents = false;
            ShowingSupport = false;
            _inspected = 0;
            player.GetComponent<VarginhaPlayerAttack>()?.EndHitstopForModal();
            _resumeScale = Time.timeScale;

            if (_canvasObject == null) Build();
            IsOpen = true;
            _openedFrame = Time.frameCount;
            Time.timeScale = 0;
            _canvasObject.SetActive(true);
            if (_ownedEventSystem != null) _ownedEventSystem.SetActive(true);
            var events=EventSystem.current;
            if(events!=null)
            {
                _navigationOwner=events;_navigationEvents=events.sendNavigationEvents;events.sendNavigationEvents=false;
                var input=events.GetComponent<InputSystemUIInputModule>();
                if(input==null)input=events.gameObject.AddComponent<InputSystemUIInputModule>();
                if(input.actionsAsset==null)input.AssignDefaultActions();
                input.enabled=true;
            }

            string physical = "";
            string[] names = { "Mochila", "Chave do Fusca", "Caderno de 1996", "Notebook", "Documento de 1898" };
            for (int i = 1; i < 5; i++) if (player.HasInventoryItem(i)) physical += "\n• " + names[i];
            _items.text = physical.Length == 0 ? "Nenhum item pendente." : physical.TrimStart('\n');
            Refresh();
            return true;
        }

        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            if (_canvasObject != null) _canvasObject.SetActive(false);
            if (_ownedEventSystem != null) _ownedEventSystem.SetActive(false);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            if(_navigationOwner!=null)_navigationOwner.sendNavigationEvents=_navigationEvents;
            _navigationOwner=null;
            if (Time.timeScale == 0 && Game.Managers.GameManager.Instance?.IsPaused != true)
                Time.timeScale = _resumeScale;
        }

        private void OnDisable() => Close();
        private void OnDestroy() => Close();

        private float _navigationRepeat; private Vector2 _lastNavigation;
        private bool _lastDevice;
        private void Update()
        {
            if (!IsOpen || Time.frameCount == _openedFrame) return;
            var keyboard = Keyboard.current;var pad=Gamepad.current;

            if (VarginhaInputActions.CancelPressed || VarginhaInputActions.InventoryPressed)
            {
                VarginhaGameHUD.Instance?.CloseBackpack();
                return;
            }

            if (keyboard?.tabKey.wasPressedThisFrame==true||pad?.rightShoulder.wasPressedThisFrame==true)
            {
                if(ShowingStudents&&Experiment.CampaignFinalAllies.Active!=null)ShowSupport();
                else ShowTab(!ShowingStudents&&!ShowingSupport);
                return;
            }

            if(_lastDevice!=VarginhaInputActions.UsingGamepad){_lastDevice=VarginhaInputActions.UsingGamepad;Refresh();}
            int next = _inspected;
            int count=ShowingSupport?3:ShowingStudents?9:6;
            if (keyboard?.leftArrowKey.wasPressedThisFrame==true||pad?.dpad.left.wasPressedThisFrame==true) next = (_inspected + count-1) % count;
            if (keyboard?.rightArrowKey.wasPressedThisFrame==true||pad?.dpad.right.wasPressedThisFrame==true) next = (_inspected + 1) % count;
            if (keyboard?.upArrowKey.wasPressedThisFrame==true||pad?.dpad.up.wasPressedThisFrame==true) next = (_inspected + count-3) % count;
            if (keyboard?.downArrowKey.wasPressedThisFrame==true||pad?.dpad.down.wasPressedThisFrame==true) next = (_inspected + 3) % count;
            Vector2 raw=VarginhaInputActions.UI("Navigate").ReadValue<Vector2>();
            Vector2 nav=raw.sqrMagnitude<.16f?Vector2.zero:Mathf.Abs(raw.x)>Mathf.Abs(raw.y)?new Vector2(Mathf.Sign(raw.x),0):new Vector2(0,Mathf.Sign(raw.y));
            if(nav.sqrMagnitude<.16f)_lastNavigation=Vector2.zero;
            else if(_lastNavigation!=nav||Time.unscaledTime>=_navigationRepeat)
            {
                _navigationRepeat=Time.unscaledTime+(_lastNavigation==Vector2.zero?.32f:.12f);_lastNavigation=nav;
                int step=Mathf.Abs(nav.x)>Mathf.Abs(nav.y)?(nav.x>0?1:-1):(nav.y>0?-3:3);next=(_inspected+count+step)%count;
            }
            if (next != _inspected) { _inspected = next; Refresh(); }

            if (VarginhaInputActions.UI("Submit").WasPressedThisFrame()) Equip();
        }

        private void Equip()
        {
            if(ShowingSupport){if(Experiment.CampaignFinalAllies.Active?.SelectSupport(_inspected)==true){Refresh();Experiment.CampaignContinuationController.Active?.SaveTeamChoice();}return;}
            if (ShowingStudents && _squad != null && _squad.SelectStudent(_inspected))
            {
                Refresh();
                Experiment.CampaignContinuationController.Active?.SaveTeamChoice();
                return;
            }

            if (!ShowingStudents && _inspected == 5 && _player != null && _player.HasFlashlight)
            {
                _player.IsFlashlightEquippedInHotbar = !_player.IsFlashlightEquippedInHotbar;
                Refresh();
            }
        }

        public void ShowTab(bool students)
        {
            ShowingSupport=false;
            ShowingStudents = students;
            _inspected = students ? Mathf.Max(0, _squad != null ? _squad.SelectedStudentIndex : 0) : 0;
            Refresh();
        }
        public void ShowSupport()
        {
            if(Experiment.CampaignFinalAllies.Active==null)return;
            ShowingStudents=false;ShowingSupport=true;
            _inspected=Mathf.Max(0,Experiment.CampaignFinalAllies.Active.SelectedSupport);Refresh();
        }

        private void Refresh()
        {
            EnsureTextures();
            if(EventSystem.current!=null&&_cells[_inspected]!=null)EventSystem.current.SetSelectedGameObject(_cells[_inspected].gameObject);
            bool finalCombat=Experiment.CampaignFinalAllies.Active!=null;
            _supportTab.gameObject.SetActive(finalCombat);
            SetTabWidth(_itemsTab,_itemsTabLabel,finalCombat?150:200);
            SetTabWidth(_studentsTab,_studentsTabLabel,finalCombat?130:200);
            _studentsTab.image.rectTransform.anchoredPosition=new Vector2(finalCombat?766:814,-24);
            _supportTab.image.sprite=ShowingSupport?_buttonActiveSprite:_buttonNormalSprite;
            _supportTabLabel.color=ShowingSupport?AccentCyan:MutedCyan;
            _itemsTab.image.sprite=!ShowingStudents&&!ShowingSupport?_buttonActiveSprite:_buttonNormalSprite;
            _studentsTab.image.sprite=ShowingStudents?_buttonActiveSprite:_buttonNormalSprite;
            _itemsTabLabel.color=!ShowingStudents&&!ShowingSupport?AccentCyan:MutedCyan;
            _studentsTabLabel.color=ShowingStudents?AccentCyan:MutedCyan;
            if(ShowingSupport){RefreshSupport();return;}

            // Tab styling: active tab gets glowing cyan border & teal background
            if (_itemsTab != null)
            {
                _itemsTab.image.sprite = ShowingStudents ? _buttonNormalSprite : _buttonActiveSprite;
                _itemsTab.image.color = Color.white;
                if (_itemsTabLabel != null) _itemsTabLabel.color = ShowingStudents ? MutedCyan : AccentCyan;
            }
            if (_studentsTab != null)
            {
                _studentsTab.image.sprite = ShowingStudents ? _buttonActiveSprite : _buttonNormalSprite;
                _studentsTab.image.color = Color.white;
                if (_studentsTabLabel != null) _studentsTabLabel.color = ShowingStudents ? AccentCyan : MutedCyan;
            }

            _equipped.text = _squad?.SelectedStudent != null && _squad.SelectedStudent.IsActive
                ? $"<color=#{ColorUtility.ToHtmlStringRGB(FocusGold)}>EQUIPADO</color>\n<b>{_squad.SelectedStudent.StudentName}</b>\n\n<size=12><color=#{ColorUtility.ToHtmlStringRGB(AccentCyan)}>{VarginhaInputBindings.DisplayName(VarginhaInputAction.AllyCommand)}</color> para atacar</size>"
                : $"<color=#{ColorUtility.ToHtmlStringRGB(MutedCyan)}>NENHUM ALUNO EQUIPADO</color>\n<size=12>Selecione na aba Alunos</size>";
            if(finalCombat)_equipped.text=FinalTeamSummary();
            if (Experiment.VarginhaCampaignStage.Active != null)
            {
                _equipped.text = "<b>INVESTIGAÇÃO</b>\n\n" + Experiment.VarginhaCampaignStage.Active.Progress.MapFragments + "/3 fragmentos de mapa\n\n<TAB> Caderno: documentos e hipóteses.";
                if (ShowingStudents) { RefreshCampaignStudents(); return; }
            }

            if (!ShowingStudents)
            {
                for (int i = 0; i < 9; i++)
                {
                    _cells[i].gameObject.SetActive(i<ItemNames.Length);
                    bool owned = i < ItemNames.Length && _player != null && _player.HasInventoryItem(i);
                    bool selected = i == _inspected;
                    _cells[i].sprite = selected ? _slotSelectedSprite : _slotNormalSprite;
                    _cells[i].color = Color.white;
                    _cellPortraits[i].enabled = owned;
                    if (owned) _cellPortraits[i].sprite = VarginhaPixelArtSprites.Create(ItemArt[i], Color.gray);
                    _cellNames[i].text = owned ? ItemNames[i] : "";
                    _cellNames[i].color = PaperColor;
                    _states[i].text = owned
                        ? (i == 0 ? "EQUIPAMENTO"
                          : i == 5 ? (_player.IsFlashlightEquippedInHotbar ? "NA HOTBAR" : "NA MOCHILA")
                          : "GUARDADO")
                        : "VAZIO";
                    _states[i].color = owned
                        ? (i == 0 ? FocusGold
                          : i == 5 ? (_player.IsFlashlightEquippedInHotbar ? FocusGold : AccentCyan)
                          : AccentCyan)
                        : MutedCyan;
                }

                bool selectedOwned = _inspected < ItemNames.Length && _player != null && _player.HasInventoryItem(_inspected);
                _name.text = selectedOwned ? ItemNames[_inspected] : "Espaço livre";
                _name.color = selectedOwned ? AccentCyan : MutedCyan;
                _portrait.enabled = selectedOwned;
                if (selectedOwned) _portrait.sprite = VarginhaPixelArtSprites.Create(ItemArt[_inspected], Color.gray);
                _description.text = selectedOwned ? ItemDescriptions[_inspected] : "Nenhum objeto guardado neste espaço.";
                _description.color = PaperColor;
                if (_inspected == 5 && selectedOwned)
                {
                    bool inHotbar = _player.IsFlashlightEquippedInHotbar;
                    _availability.text = inHotbar
                        ? "Lanterna equipada na Hotbar! Selecione-a na barra rápida (tecla [6] ou clique) e ligue/desligue com [ESPAÇO], [ENTER] ou [V]."
                        : "Pressione [ENTER] ou clique abaixo para EQUIPAR a lanterna na Hotbar (barra rápida de itens).";
                    _availability.color = inHotbar ? FocusGold : AccentCyan;
                    _equip.interactable = true;
                    _equip.image.sprite = _buttonActiveSprite;
                    _equipCaption.text = inHotbar ? "DESEQUIPAR DA HOTBAR" : "EQUIPAR NA HOTBAR";
                    _equipCaption.color = PaperColor;
                }
                else
                {
                    _availability.text = "Objetos são usados ao interagir com o cenário. As pistas obtidas ficam salvas permanentemente no caderno.";
                    _availability.color = MutedCyan;
                    _equip.interactable = false;
                    _equip.image.sprite = _buttonNormalSprite;
                    _equipCaption.text = "USO NO CENÁRIO";
                    _equipCaption.color = MutedCyan;
                }
                return;
            }

            _portrait.enabled = true;
            _equipCaption.text = "EQUIPAR ALUNO";

            for (int i = 0; i < 9; i++)
            {
                _cells[i].gameObject.SetActive(true);
                _cellPortraits[i].enabled = true;
                _cellPortraits[i].sprite = VarginhaStudentAllySquad.Portrait(i);
                _cellNames[i].text = VarginhaPhase2Controller.StudentNames[i];
                _cellNames[i].color = PaperColor;

                var ally = _squad != null && i < _squad.Allies.Count ? _squad.Allies[i] : null;
                bool available = ally != null && ally.IsActive;
                bool equipped = available && _squad.SelectedStudentIndices.Contains(i);
                bool selected = i == _inspected;

                _cells[i].sprite = selected ? _slotSelectedSprite : _slotNormalSprite;
                _cells[i].color = Color.white;

                _states[i].text = !available ? "BLOQUEADO" : equipped ? "EQUIPADO" : (ally.ManualCooldownRemaining > 0
                    ? $"{ally.ManualCooldownRemaining:0.0}s" : "PRONTO");
                _states[i].color = equipped ? FocusGold : (available ? AccentCyan : MutedCyan);
            }

            string studentName = VarginhaPhase2Controller.StudentNames[_inspected];
            var selectedAlly = _squad != null && _inspected < _squad.Allies.Count ? _squad.Allies[_inspected] : null;
            bool unlocked = selectedAlly != null && selectedAlly.IsActive;

            _name.text = studentName;
            _name.color = unlocked ? AccentCyan : MutedCyan;
            _portrait.sprite = VarginhaStudentAllySquad.Portrait(_inspected);
            _description.text = VarginhaStudentAlly.DescribeAttack(studentName);
            _description.color = PaperColor;

            _availability.text = finalCombat?"Até 3 alunos e 1 apoio.\nA / Enter equipa ou remove. Uma quarta escolha substitui a última vaga.\nRecargas são individuais.":unlocked
                ? "Recarga individual: 5 segundos.\n\nTrocar de aluno não reinicia recargas. O comando de aliado chama o aluno equipado."
                : "Este aliado ainda não foi resgatado. Encontre a turma na escola; os especiais ficam ativos na igreja.";
            _availability.color = unlocked ? PaperColor : MutedCyan;

            _equip.interactable = unlocked;
            if(finalCombat)_equipCaption.text=_squad.SelectedStudentIndices.Contains(_inspected)?"REMOVER DA EQUIPE":_squad.SelectedStudentIndices.Count==3?"SUBSTITUIR ÚLTIMA VAGA":"EQUIPAR ALUNO";
            _equip.image.sprite = unlocked ? _buttonActiveSprite : _buttonNormalSprite;
            _equipCaption.color = unlocked ? PaperColor : MutedCyan;
        }
        private void RefreshCampaignStudents()
        {
            var progress = Experiment.VarginhaCampaignStage.Active.Progress;
            var actors = Object.FindObjectsByType<Experiment.CampaignSchoolLife>();
            for (int i = 0; i < 9; i++)
            {
                _cells[i].gameObject.SetActive(true);
                bool known = (progress.studentsTalked & 1 << i) != 0;
                _cells[i].sprite = i == _inspected ? _slotSelectedSprite : _slotNormalSprite;
                _cellPortraits[i].enabled = true; _cellPortraits[i].sprite = VarginhaStudentAllySquad.Portrait(i);
                _cellNames[i].text = VarginhaPhase2Controller.StudentNames[i]; _states[i].text = known ? "CONVERSA REGISTRADA" : "NA INDUSTRIAL";
                _states[i].color = known ? AccentCyan : MutedCyan;
            }
            string name = VarginhaPhase2Controller.StudentNames[_inspected];
            _name.text = name; _portrait.enabled = true; _portrait.sprite = VarginhaStudentAllySquad.Portrait(_inspected);
            var actor = System.Array.Find(actors, candidate => candidate.StudentName == name);
            _description.text = actor != null ? "Na escola: " + actor.Activity + "." : "Aluno da Industrial. Converse com ele durante o expediente.";
            _availability.text = (progress.studentsTalked & 1 << _inspected) != 0 ? "A conversa foi registrada. As falas acompanham o andamento da investigação." : "Aproxime-se e pressione E para conversar. Cada aluno trabalha em uma atividade e possui observações próprias.";
            _equip.interactable = false; _equipCaption.text = "CONVERSAR NA ESCOLA"; _equip.image.sprite = _buttonNormalSprite;
        }
        private void RefreshSupport()
        {
            var allies=Experiment.CampaignFinalAllies.Active;
            for(int i=0;i<9;i++)
            {
                bool available=i<3;_cells[i].gameObject.SetActive(available);_cells[i].sprite=i==_inspected?_slotSelectedSprite:_slotNormalSprite;
                _cellPortraits[i].enabled=available;
                if(available)_cellPortraits[i].sprite=Experiment.CampaignFinalAllies.Portrait(i);
                _cellNames[i].text=available?Experiment.CampaignFinalAllies.Names[i]:"";
                _states[i].text=available?(allies.SelectedSupport==i?"EQUIPADO":allies.Cooldown(i)>0?$"{allies.Cooldown(i):0.0}s":"PRONTO"):"VAZIO";
            }
            bool selected=_inspected<3;_portrait.enabled=selected;
            if(selected)_portrait.sprite=Experiment.CampaignFinalAllies.Portrait(_inspected);
            _name.text=selected?Experiment.CampaignFinalAllies.Names[_inspected]:"Espaço livre";
            _description.text=selected?Experiment.CampaignFinalAllies.Descriptions[_inspected]:"Selecione um dos três aliados de apoio.";
            _availability.text="Trocar de aliado preserva a recarga de cada um. Use "+VarginhaInputBindings.DisplayName(VarginhaInputAction.SupportCommand)+" depois de fechar a mochila.";
            _equip.interactable=selected;_equipCaption.text=allies.SelectedSupport==_inspected?"REMOVER APOIO":"EQUIPAR APOIO";
            _equipped.text=FinalTeamSummary();
        }
        private string FinalTeamSummary()
        {
            var allies=Experiment.CampaignFinalAllies.Active;string text="<size=11><color=#a3c9cc>ALUNOS "+_squad.SelectedStudentIndices.Count+"/3</color>";
            foreach(int i in _squad.SelectedStudentIndices)text+="\n"+_squad.Allies[i].StudentName;
            text+="\n<color=#a3c9cc>APOIO "+(allies.SelectedSupport>=0?"1/1":"0/1")+"</color>";
            if(allies.SelectedSupport>=0)text+="\n"+Experiment.CampaignFinalAllies.Names[allies.SelectedSupport];
            return text+"</size>";
        }

        private void Build()
        {
            EnsureTextures();
            _font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (_font == null) throw new System.InvalidOperationException("A mochila requer TMP Essential Resources (LiberationSans SDF).");

            _canvasObject = new GameObject("Mochila_Inventario", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            _canvasObject.transform.SetParent(transform, false);

            var canvas = _canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            var scaler = _canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1120, 650);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            // Dim backdrop overlay matching Pause screen
            var backdrop = Panel(_canvasObject.transform, "Escurecer", 0, 0, 1120, 650, new Color(.005f, .01f, .015f, .78f));
            backdrop.rectTransform.anchorMin = Vector2.zero;
            backdrop.rectTransform.anchorMax = Vector2.one;
            backdrop.rectTransform.offsetMin = Vector2.zero;
            backdrop.rectTransform.offsetMax = Vector2.zero;
            backdrop.raycastTarget = true;

            // Main Modal Window framed with PixelHUDFrame metal style
            var page = new GameObject("PaginaModal", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image)).GetComponent<RectTransform>();
            page.SetParent(_canvasObject.transform, false);
            page.sizeDelta = new Vector2(1070, 610);
            page.anchorMin = page.anchorMax = new Vector2(.5f, .5f);
            var pageImg = page.GetComponent<UnityEngine.UI.Image>();
            pageImg.sprite = _modalFrameSprite;
            pageImg.type = UnityEngine.UI.Image.Type.Sliced;
            pageImg.color = Color.white;
            pageImg.raycastTarget = true;

            // Header matching PauseHeader style
            var title = Label(page, "MOCHILA", 36, 18, 500, 32, 24);
            title.fontStyle = FontStyles.Bold;
            title.color = PaperColor;
            Label(page, "INVESTIGAÇÃO • INVENTÁRIO E EQUIPE DE ALUNOS", 38, 48, 500, 20, 11).color = AccentCyan;

            // Tabs matching Pause button styling
            _itemsTab = CreateTabButton(page, "ITENS FÍSICOS", 600, 24, 200, 36, () => ShowTab(false), out _itemsTabLabel);
            _studentsTab = CreateTabButton(page, "ALUNOS", 814, 24, 200, 36, () => ShowTab(true), out _studentsTabLabel);
            _supportTab=CreateTabButton(page,"APOIO",910,24,130,36,ShowSupport,out _supportTabLabel);
            _supportTab.gameObject.SetActive(false);

            // Top decorative divider line
            Separator(page, 32, 68, 1006, 2);

            // ================= LEFT COLUMN =================
            var leftPanel = Panel(page, "PainelEsquerdo", 32, 78, 230, 474, Color.white);
            leftPanel.sprite = _subPanelSprite;
            leftPanel.type = UnityEngine.UI.Image.Type.Sliced;

            Label(leftPanel.transform, "EQUIPAMENTO", 12, 12, 206, 20, 13, TextAlignmentOptions.Center).color = AccentCyan;
            var bagBox = Panel(leftPanel.transform, "MochilaBox", 61, 38, 108, 108, Color.white);
            bagBox.sprite = _slotNormalSprite;
            bagBox.type = UnityEngine.UI.Image.Type.Sliced;

            var bagIcon = Panel(bagBox.transform, "MochilaIcon", 10, 10, 88, 88, Color.white);
            bagIcon.sprite = VarginhaPixelArtSprites.Create("Backpack_Inventory", Color.gray);
            bagIcon.preserveAspect = true;

            _equipped = Label(leftPanel.transform, "", 12, 150, 206, 108, 14, TextAlignmentOptions.Center);

            Separator(leftPanel.transform, 16, 266, 198, 2);

            Label(leftPanel.transform, "ITENS FÍSICOS", 16, 278, 198, 18, 12).color = AccentCyan;
            _items = Label(leftPanel.transform, "", 16, 300, 198, 110, 13);
            _items.color = PaperColor;

            var footNote = Label(leftPanel.transform, "Itens usados saem.\nPistas permanecem.", 12, 424, 206, 36, 12, TextAlignmentOptions.Center);
            footNote.color = FocusGold;

            // ================= CENTER COLUMN (3x3 Grid) =================
            var centerPanel = Panel(page, "PainelCentro", 274, 78, 496, 474, Color.white);
            centerPanel.sprite = _subPanelSprite;
            centerPanel.type = UnityEngine.UI.Image.Type.Sliced;

            for (int i = 0; i < 9; i++)
            {
                int index = i;
                int col = i % 3;
                int row = i / 3;
                float x = 14 + col * 158;
                float y = 14 + row * 148;

                _cells[i] = Panel(centerPanel.transform, "Slot_" + i, x, y, 152, 142, Color.white);
                _cells[i].sprite = _slotNormalSprite;
                _cells[i].type = UnityEngine.UI.Image.Type.Sliced;
                _cells[i].raycastTarget = true;

                var button = _cells[i].gameObject.AddComponent<UnityEngine.UI.Button>();
                button.targetGraphic = _cells[i];
                Game.UI.PixelButtonHoverUGUI.Attach(button);
                button.navigation = new Navigation { mode = Navigation.Mode.None };
                button.onClick.AddListener(() => { if(ShowingSupport&&index>=3||!ShowingStudents&&!ShowingSupport&&index>=6)return;_inspected = index; Refresh(); });

                // Frame for portrait
                var portBox = Panel(_cells[i].transform, "PortBox_" + i, 44, 8, 64, 64, new Color(0, 0, 0, .45f));
                var portrait = Panel(portBox.transform, "Retrato_" + i, 2, 2, 60, 60, Color.white);
                _cellPortraits[i] = portrait;
                portrait.sprite = VarginhaStudentAllySquad.Portrait(i);
                portrait.preserveAspect = true;

                _cellNames[i] = Label(_cells[i].transform, "", 4, 78, 144, 34, 13, TextAlignmentOptions.Center);
                _cellNames[i].color = PaperColor;

                _states[i] = Label(_cells[i].transform, "", 4, 114, 144, 20, 11, TextAlignmentOptions.Center);
            }

            // ================= RIGHT COLUMN (Inspection) =================
            var rightPanel = Panel(page, "PainelDireito", 782, 78, 256, 474, Color.white);
            rightPanel.sprite = _subPanelSprite;
            rightPanel.type = UnityEngine.UI.Image.Type.Sliced;

            _name = Label(rightPanel.transform, "", 12, 16, 232, 40, 20, TextAlignmentOptions.Center);
            _name.fontStyle = FontStyles.Bold;

            var inspectPortBox = Panel(rightPanel.transform, "InspectPortBox", 76, 62, 104, 104, Color.white);
            inspectPortBox.sprite = _slotNormalSprite;
            inspectPortBox.type = UnityEngine.UI.Image.Type.Sliced;

            _portrait = Panel(inspectPortBox.transform, "Retrato_Inspecionado", 4, 4, 96, 96, Color.white);
            _portrait.preserveAspect = true;

            _description = Label(rightPanel.transform, "", 16, 176, 224, 110, 14, TextAlignmentOptions.Center);

            Separator(rightPanel.transform, 16, 294, 224, 2);

            _availability = Label(rightPanel.transform, "", 16, 304, 224, 104, 12);

            _equip = Button(rightPanel.transform, "EQUIPAR ALUNO", 16, 420, 224, 40, Equip, out _equipCaption);

            // ================= FOOTER =================
            Separator(page, 32, 560, 1006, 2);

            Label(page, "SETAS / D-PAD: navegar    TAB / RB: abas    ENTER / A: equipar    ESC / B: voltar",
                38, 570, 780, 26, 13);

            TMP_Text backLabel;
            Button(page, "VOLTAR", 894, 566, 144, 34, () => VarginhaGameHUD.Instance?.CloseBackpack(), out backLabel);

            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                _ownedEventSystem = new GameObject("Mochila_EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
                _ownedEventSystem.transform.SetParent(transform, false);
                _ownedEventSystem.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            }
        }
        private static void SetTabWidth(UnityEngine.UI.Button tab,TMP_Text label,float width)
        {
            tab.image.rectTransform.sizeDelta=new Vector2(width,36);
            label.rectTransform.sizeDelta=new Vector2(width,36);
        }

        private static void EnsureTextures()
        {
            if (_modalFrameSprite != null && _modalFrameSprite.texture != null) return;

            _modalFrameSprite = CreateFramedSprite(64, 64, PanelColor, PanelBorder, true, new Vector4(8, 8, 8, 8));
            _subPanelSprite = CreateFramedSprite(32, 32, new Color(.028f, .036f, .044f, .96f), new Color(.18f, .26f, .30f, .85f), false, new Vector4(6, 6, 6, 6));
            _slotNormalSprite = CreateFramedSprite(32, 32, new Color(.042f, .056f, .068f, .96f), new Color(.22f, .32f, .36f, .85f), false, new Vector4(4, 4, 4, 4));
            _slotSelectedSprite = CreateFramedSprite(32, 32, new Color(.09f, .14f, .17f, .98f), AccentCyan, false, new Vector4(4, 4, 4, 4), true);
            _buttonNormalSprite = CreateFramedSprite(32, 32, PanelColor, PanelBorder, false, new Vector4(4, 4, 4, 4));
            _buttonActiveSprite = CreateButtonActiveSprite(32, 32, new Color(.10f, .15f, .17f, .98f), AccentCyan, new Vector4(4, 4, 4, 4));
            _separatorSprite = CreateSeparatorSprite();
        }

        private static Sprite CreateFramedSprite(int w, int h, Color bg, Color border, bool rivets, Vector4 slice, bool glow = false)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = $"HUD_Frame_{w}x{h}"
            };
            var pixels = new Color32[w * h];

            Color32 cBg = bg;
            Color32 cBorder = border;
            Color32 cShadow = new Color32(2, 3, 5, 200);
            Color32 cEdge = new Color32(6, 9, 12, 250);
            Color32 cHi = Color.Lerp(border, Color.white, .28f);
            Color32 cRivet = Color.Lerp(border, Color.white, .5f);

            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                int val = (x * 17 + y * 29 + x * y * 3) % 23;
                int grain = val < 3 ? 10 : (val > 18 ? -12 : 0);
                byte r = (byte)Mathf.Clamp(cBg.r + grain, 0, 255);
                byte g = (byte)Mathf.Clamp(cBg.g + grain, 0, 255);
                byte b = (byte)Mathf.Clamp(cBg.b + grain, 0, 255);
                pixels[y * w + x] = new Color32(r, g, b, cBg.a);
            }

            // Outer edge & border
            for (int x = 0; x < w; x++)
            {
                pixels[x] = cShadow;
                pixels[(h - 1) * w + x] = cShadow;
                if (h > 2) { pixels[w + x] = cEdge; pixels[(h - 2) * w + x] = cEdge; }
                if (h > 4) { pixels[2 * w + x] = cBorder; pixels[(h - 3) * w + x] = cBorder; }
            }
            for (int y = 0; y < h; y++)
            {
                pixels[y * w] = cShadow;
                pixels[y * w + w - 1] = cShadow;
                if (w > 2) { pixels[y * w + 1] = cEdge; pixels[y * w + w - 2] = cEdge; }
                if (w > 4) { pixels[y * w + 2] = cBorder; pixels[y * w + w - 3] = cBorder; }
            }

            // Top-left highlight bevel
            for (int x = 3; x < w - 3; x++) pixels[(h - 4) * w + x] = cHi;
            for (int y = 3; y < h - 3; y++) pixels[y * w + 3] = cHi;

            if (rivets && w >= 32 && h >= 32)
            {
                int[] rx = { 6, w - 8 };
                int[] ry = { 6, h - 8 };
                foreach (int ox in rx) foreach (int oy in ry)
                {
                    pixels[oy * w + ox] = cRivet;
                    pixels[oy * w + ox + 1] = cRivet;
                    pixels[(oy + 1) * w + ox] = cRivet;
                    pixels[(oy + 1) * w + ox + 1] = cRivet;
                }
            }

            tex.SetPixels32(pixels);
            tex.Apply(false, true);
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(.5f, .5f), 100f, 0, SpriteMeshType.FullRect, slice);
        }

        private static Sprite CreateButtonActiveSprite(int w, int h, Color bg, Color border, Vector4 slice)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "HUD_BtnActive"
            };
            var pixels = new Color32[w * h];
            Color32 cBg = bg;
            Color32 cBorder = border;

            for (int i = 0; i < pixels.Length; i++) pixels[i] = cBg;

            // Border
            for (int x = 0; x < w; x++)
            {
                pixels[x] = cBorder;
                pixels[(h - 1) * w + x] = cBorder;
            }
            for (int y = 0; y < h; y++)
            {
                pixels[y * w] = cBorder;
                pixels[y * w + w - 1] = cBorder;
            }

            // Vertical 2px accent bar on left edge (matching PauseButton)
            for (int y = 2; y < h - 2; y++)
            {
                pixels[y * w + 2] = cBorder;
                pixels[y * w + 3] = cBorder;
            }

            tex.SetPixels32(pixels);
            tex.Apply(false, true);
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(.5f, .5f), 100f, 0, SpriteMeshType.FullRect, slice);
        }

        private static Sprite CreateSeparatorSprite()
        {
            var tex = new Texture2D(16, 2, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "HUD_Sep"
            };
            var pixels = new Color32[32];
            Color32 top = PanelBorder;
            Color32 bot = new Color32(10, 14, 18, 200);
            for (int i = 0; i < 16; i++) { pixels[16 + i] = top; pixels[i] = bot; }
            tex.SetPixels32(pixels);
            tex.Apply(false, true);
            return Sprite.Create(tex, new Rect(0, 0, 16, 2), new Vector2(.5f, .5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(2, 0, 2, 0));
        }

        private static RectTransform Place(GameObject go, Transform parent, float x, float y, float w, float h)
        {
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(w, h);
            return rect;
        }

        private static UnityEngine.UI.Image Panel(Transform parent, string name, float x, float y, float w, float h, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
            Place(go, parent, x, y, w, h);
            var graphic = go.GetComponent<UnityEngine.UI.Image>();
            graphic.color = color;
            graphic.raycastTarget = false;
            return graphic;
        }

        private static void Separator(Transform parent, float x, float y, float w, float h)
        {
            var sep = Panel(parent, "Separador", x, y, w, h, Color.white);
            sep.sprite = _separatorSprite;
            sep.type = UnityEngine.UI.Image.Type.Sliced;
        }

        private TMP_Text Label(Transform parent, string value, float x, float y, float w, float h, int size,
            TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft)
        {
            var go = new GameObject("Texto", typeof(RectTransform), typeof(CanvasRenderer));
            Place(go, parent, x, y, w, h);
            var label = go.AddComponent<TextMeshProUGUI>();
            label.font = _font;
            label.fontSize = size;
            label.enableAutoSizing = false;
            label.color = PaperColor;
            label.raycastTarget = false;
            label.alignment = alignment;
            label.text = value;
            return label;
        }

        private UnityEngine.UI.Button CreateTabButton(Transform parent, string caption, float x, float y, float w, float h,
            UnityEngine.Events.UnityAction action, out TMP_Text label)
        {
            var graphic = Panel(parent, "Tab_" + caption, x, y, w, h, Color.white);
            graphic.sprite = _buttonNormalSprite;
            graphic.type = UnityEngine.UI.Image.Type.Sliced;
            graphic.raycastTarget = true;

            var button = graphic.gameObject.AddComponent<UnityEngine.UI.Button>();
            button.targetGraphic = graphic;
            Game.UI.PixelButtonHoverUGUI.Attach(button);
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            button.onClick.AddListener(action);

            label = Label(graphic.transform, caption, 0, 0, w, h, 14, TextAlignmentOptions.Center);
            label.fontStyle = FontStyles.Bold;
            label.color = MutedCyan;
            return button;
        }

        private UnityEngine.UI.Button Button(Transform parent, string caption, float x, float y, float w, float h,
            UnityEngine.Events.UnityAction action, out TMP_Text label)
        {
            var graphic = Panel(parent, "Botao_" + caption, x, y, w, h, Color.white);
            graphic.sprite = _buttonNormalSprite;
            graphic.type = UnityEngine.UI.Image.Type.Sliced;
            graphic.raycastTarget = true;

            var button = graphic.gameObject.AddComponent<UnityEngine.UI.Button>();
            button.targetGraphic = graphic;
            Game.UI.PixelButtonHoverUGUI.Attach(button);
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            button.onClick.AddListener(action);

            label = Label(graphic.transform, caption, 0, 0, w, h, 14, TextAlignmentOptions.Center);
            label.fontStyle = FontStyles.Bold;
            label.color = PaperColor;
            return button;
        }
    }
}
