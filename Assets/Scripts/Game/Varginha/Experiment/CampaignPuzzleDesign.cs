using System;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    [Serializable]
    public sealed class CampaignPuzzleProgress
    {
        public int archiveMarks;
        public int[] archiveOrder;
        public int archiveAttempt;
        public bool EnsureArchiveOrder(bool nextAttempt=false,System.Random random=null)
        {
            bool valid=archiveOrder?.Length==6;
            if(valid)for(int i=0;i<6;i++)if(archiveOrder[i]<0||archiveOrder[i]>=6||Array.IndexOf(archiveOrder,archiveOrder[i])!=i){valid=false;break;}
            if(valid&&!nextAttempt)return false;
            var previous=archiveOrder;archiveOrder=new[]{0,1,2,3,4,5};random??=new System.Random(Guid.NewGuid().GetHashCode());
            for(int i=5;i>0;i--){int j=random.Next(i+1);(archiveOrder[i],archiveOrder[j])=(archiveOrder[j],archiveOrder[i]);}
            if(valid&&CampaignStory.Sequence(previous,archiveOrder))
                (archiveOrder[0],archiveOrder[1])=(archiveOrder[1],archiveOrder[0]);
            if(nextAttempt)archiveAttempt++;return true;
        }
        public int selected = -1;
        public int[] causes = { -1, -1, -1, -1 };
        public int[] dates = { -1, -1, -1 };
        public int[] memory = Array.Empty<int>();
        public int[] hypotheses = { -1, -1, -1 };

        public void Repair(CampaignContinuationState completed)
        {
            archiveMarks &= 63; selected = Mathf.Clamp(selected, -1, 3);
            EnsureArchiveOrder();archiveAttempt=Mathf.Max(0,archiveAttempt);
            RepairLinks(ref causes, 4); RepairLinks(ref dates, 3); RepairLinks(ref hypotheses, 3);
            if (memory == null || memory.Length > 4) memory = Array.Empty<int>();
            for (int i = 0; i < memory.Length; i++)
                if (memory[i] < 0 || memory[i] > 3 || Array.IndexOf(memory, memory[i]) != i)
                { memory = Array.Empty<int>(); break; }
            // Missing fields in an old save never reopen completed investigations.
            if (completed.solved[0]) archiveMarks = 3;
            if (completed.solved[2]) causes = new[] { 0, 1, 2, 3 };
            if (completed.solved[3]) dates = new[] { 0, 1, 2 };
            if (completed.solved[4]) memory = new[] { 0, 1, 2, 3 };
            if (completed.solved[7]) hypotheses = new[] { 1, 2, 0 };
        }
        private static void RepairLinks(ref int[] links, int count)
        {
            if (links == null || links.Length != count) { links = new int[count]; Array.Fill(links, -1); }
            for (int i = 0; i < count; i++)
                if (links[i] < -1 || links[i] >= count || links[i] >= 0 && Array.IndexOf(links, links[i]) != i) links[i] = -1;
        }
    }

    // Each chapter asks for a different investigative action; all controls use
    // the existing IMGUI/gamepad adapter and all answers remain in the story save.
    public static class CampaignPuzzleDesign
    {
        private static readonly Color Line = new(.43f, .77f, .67f);
        public static bool Redesigned(int phase) => phase == 11 || phase == 13 || phase == 14 || phase == 15 || phase == 18;
        public static bool Correct(int phase, int choice, int[] answer)
            => phase == 11 ? choice == 3 : CampaignStory.Sequence(answer, Solution(phase));
        public static int[] Solution(int phase) => phase == 18 ? new[] { 1, 2, 0 }
            : phase == 14 ? new[] { 0, 1, 2 } : new[] { 0, 1, 2, 3 };
        public static string Feedback(int phase) => phase == 11
            ? "Marque somente os detalhes apagados. Uma região que aparece igual nas duas versões não é uma alteração."
            : phase == 13 ? "Cada prova deve explicar um acontecimento diferente. Os danos antecedem a instalação da contenção."
            : phase == 14 ? "Confira a qual época pertence cada documento, não apenas a ordem em que você o encontrou."
            : phase == 15 ? "A lembrança ainda não respeita as relações dos vestígios. Compare o desenho com a falta de energia."
            : "Uma conclusão ainda não é sustentada pela prova associada. Compare intenção, ferimentos e função dos circuitos.";

        public static string Feedback(int phase, int choice, int[] answer)
        {
            if (phase == 11 && choice == 0) return "Nenhum detalhe foi marcado. Compare as duas plantas antes de registrar.";
            if (phase != 11 && (answer == null || answer.Length != Solution(phase).Length || Array.IndexOf(answer, -1) >= 0))
                return phase == 15 ? "A lembrança está incompleta. Inclua os quatro vestígios; você pode desfazer a última escolha."
                    : "Ainda há documentos sem associação. Selecione uma prova à esquerda e o acontecimento à direita; cada prova tem um único destino.";
            return Feedback(phase);
        }

        public static void DrawMap(CampaignExpansionState state, ref int selected, Action confirm, Action save)
            =>CampaignTrailPuzzle.Draw(state,ref selected,confirm,save);

        public static void Draw(CampaignStory story, int phase, Action<int,int[]> submit, Action save, CampaignSoundscape sound)
        {
            var state=story.puzzles;
            if (phase == 11) Comparison(state,submit,save);
            else if (phase == 15) Memory(state,submit,save,sound);
            else
            {
                string[] proofs, claims; int[] links; string title, instruction;
                if (phase == 13)
                {
                    title="RECONSTRUA O QUE ACONTECEU";
                    instruction="Cada documento revela uma parte da história. Selecione uma prova e relacione-a ao acontecimento que ela ajuda a explicar.";
                    proofs=new[]{"PÁGINA • mecanismo encontrado","FOTOGRAFIA • testes do grupo","SALA • danos da ruptura","INSTRUMENTOS • ligação a pessoas"};
                    claims=new[]{"1 • A DESCOBERTA", "2 • AS EXPERIÊNCIAS", "3 • A RUPTURA", "4 • A RESPOSTA: CONTENÇÃO"}; links=state.causes;
                }
                else if (phase == 14)
                {
                    title="TOMBO • ASSOCIAR DOCUMENTOS ÀS DATAS";
                    instruction="Uma época, um documento. Escolha a prova à esquerda e o ano à direita. A data sozinha não explica o que aconteceu.";
                    proofs=new[]{"Instrumentos ligados a pessoas vivas","Ligação transferida para Edelzio","Interferência e falha do vínculo"};
                    claims=new[]{"1898 • ORIGEM DO MECANISMO","1996 • ACIDENTE DE EDELZIO","2026 • INVESTIGAÇÃO ATUAL"}; links=state.dates;
                }
                else
                {
                    title="O ACORDO • SUSTENTAR TRÊS CONCLUSÕES";
                    instruction="Cada conclusão precisa de uma prova diferente. Não basta reconhecer uma frase: explique os ferimentos, o retorno e a proteção da cidade.";
                    proofs=new[]{"REGISTROS • o selo protege a ruptura","FERIMENTOS • marcas da contenção","REAGENTE • circuitos externos/internos"};
                    claims=new[]{"A contenção feriu a criatura","O circuito externo abre o retorno","O selo dura até a travessia terminar"}; links=state.hypotheses;
                }
                AssociationHeader(phase,title,instruction);
                int linked=0;for(int i=0;i<links.Length;i++)if(links[i]>=0)linked++;
                ExperimentGUI.Label(new Rect(145,213,715,24),state.selected<0?"Escolha uma prova à esquerda e um destino à direita.":"PROVA SELECIONADA • "+proofs[state.selected],small:true);
                ExperimentGUI.Label(new Rect(890,213,225,24),"VÍNCULOS "+linked+"/"+links.Length,small:true);
                // The evidence rack changes order; the first button is not always the answer.
                for (int row=0;row<proofs.Length;row++)
                {
                    int proof=(row+1)%proofs.Length;
                    Rect proofRect=new Rect(145,240+row*70,425,64);
                    if(CardButton(proofRect,"PROVA "+(proof+1),proofs[proof],"",state.selected==proof,false))state.selected=proof;
                    string assigned=links[row]<0?"— escolher prova —":"VÍNCULO • PROVA "+(links[row]+1);
                    if(CardButton(new Rect(600,240+row*70,415,64),"DESTINO "+(row+1),claims[row],assigned,links[row]>=0,true)&&state.selected>=0)
                    {
                        int previous=Array.IndexOf(links,state.selected);if(previous>=0)links[previous]=-1;
                        links[row]=state.selected;state.selected=-1;save();
                    }
                    if(UndoLinkButton(new Rect(1025,258+row*70,90,30),links[row]>=0))
                    {links[row]=-1;save();}
                }
                if(ExperimentGUI.Button(new Rect(145,530,360,32),"LIMPAR ASSOCIAÇÕES")){Array.Fill(links,-1);state.selected=-1;save();}
                if(ExperimentGUI.Button(new Rect(755,530,360,32),"CONFERIR AS PROVAS"))submit(-1,links);
            }
        }
        private static void Comparison(CampaignPuzzleProgress state, Action<int,int[]> submit, Action save)
        {
            BoardHeader("ARQUIVOS • ENCONTRAR O QUE FOI APAGADO","Compare os dois cartões de planta. Marque os detalhes que mudaram e registre somente as diferenças.");
            Blueprint(new Rect(145,340,325,178),false);Blueprint(new Rect(490,340,325,178),true);
            string[] areas={"ACESSO LATERAL","DATA DO REGISTRO","PORTA PRINCIPAL","JARDIM","ASSINATURA","ESCADA INTERNA"};
            if(state.EnsureArchiveOrder())save();
            for(int i=0;i<areas.Length;i++)
            {
                int id=state.archiveOrder[i];
                if(ExperimentGUI.Button(new Rect(840,336+i*30,275,27),((state.archiveMarks&(1<<id))!=0?"[X] ":"[ ] ")+areas[id]))
                {state.archiveMarks^=1<<id;save();}
            }
            if(ExperimentGUI.Button(new Rect(755,530,360,32),"REGISTRAR ALTERAÇÕES"))submit(state.archiveMarks,null);
        }
        private static void Blueprint(Rect r, bool altered)
        {
            ExperimentGUI.Box(r,new Color(.04f,.12f,.15f));
            ExperimentGUI.Label(new Rect(r.x+12,r.y+8,r.width-24,24),altered?"CÓPIA ALTERADA":"PLANTA PRESERVADA",small:true);
            float x=r.x+36,y=r.y+48;
            ExperimentGUI.Box(new Rect(x,y,238,3),Line);ExperimentGUI.Box(new Rect(x,y,3,92),Line);
            ExperimentGUI.Box(new Rect(x,y+92,89,3),Line);ExperimentGUI.Box(new Rect(x+145,y+92,96,3),Line);
            ExperimentGUI.Box(new Rect(x+120,y,3,62),Line);ExperimentGUI.Box(new Rect(x+235,y,3,altered?95:44),Line);
            if(!altered){ExperimentGUI.Box(new Rect(x+235,y+72,3,23),Line);ExperimentGUI.Box(new Rect(x+238,y+43,18,3),Line);}
            ExperimentGUI.Label(new Rect(x+15,y+12,190,26),"ESCADA ↑       JARDIM",small:true);
            ExperimentGUI.Label(new Rect(r.x+12,r.yMax-28,r.width-24,24),(altered?"DATA: —":"DATA: 1898")+"     Z. GOMES",small:true);
        }
        private static void Memory(CampaignPuzzleProgress state, Action<int,int[]> submit, Action save, CampaignSoundscape sound)
        {
            BoardHeader("1996 • RECONSTRUIR A LEMBRANÇA","Escolha os quatro vestígios na ordem da noite. A trilha visual abaixo mostra o que já foi lembrado.");
            string[] names={"NOTÍCIA NA TV","DESENHO","ENERGIA APAGADA","PRESENÇA NO QUINTAL"};
            string[] motifs={"Child90_TV","Child90_Desk","Lamp","Tree"};int[] display={3,0,2,1};
            for(int i=0;i<4;i++)
            {
                int clue=display[i];Rect tile=new(145+i*245,352,230,115);
                if(MemoryCard(tile,names[clue],state.memory.Length<4&&Array.IndexOf(state.memory,clue)<0)&&state.memory.Length<4&&Array.IndexOf(state.memory,clue)<0)
                {int length=state.memory.Length;Array.Resize(ref state.memory,length+1);state.memory[length]=clue;sound?.Play(clue==1?"Paper":"UI");save();}
                // Original prop sprites accompany the controls without a new raster asset.
                var sprite=CampaignInteriorArt.Contains(motifs[clue])?CampaignInteriorArt.Prop(motifs[clue]):CampaignVisualAssets.Prop(motifs[clue]);
                if(sprite!=null)Sprite(new Rect(tile.x+78,tile.y+8,74,32),sprite);
            }
            string sequence=state.memory.Length==0?"Nenhum vestígio escolhido.":string.Join(" → ",Array.ConvertAll(state.memory,i=>names[i]));
            ExperimentGUI.Label(new Rect(145,476,990,43),sequence,small:true);
            if(ExperimentGUI.Button(new Rect(145,530,360,32),"RECOMEÇAR LEMBRANÇA")){state.memory=Array.Empty<int>();save();}
            GUI.enabled=state.memory.Length>0;
            if(ExperimentGUI.Button(new Rect(515,530,230,32),"DESFAZER ÚLTIMA")){Array.Resize(ref state.memory,state.memory.Length-1);save();}
            GUI.enabled=true;
            if(ExperimentGUI.Button(new Rect(755,530,360,32),"CONFERIR LEMBRANÇA"))submit(-1,state.memory);
        }
        private static void BoardHeader(string title,string instruction)
        {
            ExperimentGUI.Panel(new Rect(135,236,1010,96));
            ExperimentGUI.Box(new Rect(145,246,5,73),ExperimentGUI.Accent);
            ExperimentGUI.Label(new Rect(165,245,950,38),title,true);
            ExperimentGUI.Label(new Rect(165,287,950,32),instruction,small:true);
        }
        private static void AssociationHeader(int phase,string title,string instruction)
        {
            // Associations need four readable rows; the caller's compact board remains
            // suitable for the other puzzles, whose controls keep their existing layout.
            ExperimentGUI.Panel(new Rect(100,80,1080,560));
            Game.UI.PixelMenuTheme.Label(new Rect(145,98,970,30),CampaignSequence.Title(phase).ToUpperInvariant(),20,ExperimentGUI.Paper);
            Game.UI.PixelMenuTheme.Label(new Rect(145,139,970,28),title,17,ExperimentGUI.Paper);
            ExperimentGUI.Label(new Rect(145,173,970,38),instruction,small:true);
        }
        private static bool UndoLinkButton(Rect rect,bool linked)
        {
            bool enabled=GUI.enabled;GUI.enabled=enabled&&linked;
            bool clicked=ExperimentGUI.Button(rect,"");
            Game.UI.PixelMenuTheme.Label(new Rect(rect.x+6,rect.y+3,rect.width-12,rect.height-6),"DESFAZER",8,ExperimentGUI.Muted,TextAnchor.MiddleCenter);
            GUI.enabled=enabled;
            return clicked;
        }
        private static bool CardButton(Rect rect,string kicker,string title,string detail,bool selected,bool destination)
        {
            bool clicked=ExperimentGUI.Button(rect,"");
            var fill=selected?new Color(.12f,.25f,.27f):destination&&detail!="— escolher prova —"?new Color(.065f,.17f,.19f):new Color(.035f,.105f,.13f);
            ExperimentGUI.Box(new Rect(rect.x+2,rect.y+2,rect.width-4,rect.height-4),fill);
            ExperimentGUI.Box(new Rect(rect.x+2,rect.y+2,4,rect.height-4),selected?ExperimentGUI.Paper:destination?ExperimentGUI.Accent:ExperimentGUI.Muted);
            Game.UI.PixelMenuTheme.Label(new Rect(rect.x+17,rect.y+4,rect.width-32,14),kicker,8,ExperimentGUI.Muted);
            ExperimentGUI.Label(new Rect(rect.x+17,rect.y+20,rect.width-32,22),title,small:true);
            if(!string.IsNullOrEmpty(detail))Game.UI.PixelMenuTheme.Label(new Rect(rect.x+17,rect.y+44,rect.width-32,16),detail,9,ExperimentGUI.Muted);
            if(selected)ExperimentGUI.Box(new Rect(rect.x+rect.width-10,rect.y+7,3,rect.height-14),ExperimentGUI.Paper);
            return clicked;
        }
        private static bool MemoryCard(Rect rect,string title,bool enabled)
        {
            bool clicked=ExperimentGUI.Button(rect,"");
            var fill=enabled?new Color(.07f,.20f,.20f):new Color(.035f,.105f,.13f);
            ExperimentGUI.Box(new Rect(rect.x+2,rect.y+2,rect.width-4,rect.height-4),fill);
            ExperimentGUI.Box(new Rect(rect.x+2,rect.y+2,4,rect.height-4),enabled?ExperimentGUI.Accent:ExperimentGUI.Muted);
            ExperimentGUI.Label(new Rect(rect.x+15,rect.y+74,rect.width-30,30),title,small:true);
            return clicked;
        }
        private static void Sprite(Rect r, Sprite sprite)
        {
            float fit=Mathf.Min(r.width/sprite.rect.width,r.height/sprite.rect.height);
            var area=new Rect(r.center-new Vector2(sprite.rect.width,sprite.rect.height)*fit/2,new Vector2(sprite.rect.width,sprite.rect.height)*fit);
            var source=sprite.rect;var texture=sprite.texture;
            GUI.DrawTextureWithTexCoords(area,texture,new Rect(source.x/texture.width,source.y/texture.height,source.width/texture.width,source.height/texture.height));
        }
    }
}
