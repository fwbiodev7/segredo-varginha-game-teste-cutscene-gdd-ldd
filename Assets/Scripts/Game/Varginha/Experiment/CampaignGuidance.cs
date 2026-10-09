using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignGuidance
    {
        private static readonly Dictionary<int,CampaignContinuationDefinition> Definitions=new();
        public static string Destination(int phase) => phase switch {
            2=>"INDUSTRIAL", 4 or 5=>"BIBLIOTECA", 6=>"MATA", 7=>"ARQUIVO DE FÁBIO",
            8=>"CASA DE OUZANA", 9=>"OFICINA", 10=>"INDUSTRIAL", 11=>"CASARÃO",
            12=>"PORÃO", 13=>"FÁBIO", 14=>"LEMBRANÇA DE 1996", 15 or 16=>"COMPLEXO",
            18 or 19=>"CÂMARA DA MANIFESTAÇÃO", 20=>"PASSAGEM DE RETORNO", 21=>"INDUSTRIAL", _=>"PRÓXIMA ÁREA" };
        public static string Next(CampaignStory story,int phase,int area=0)
        {
            if(story==null)return "Observe o cenário e abra o caderno para consultar as pistas.";
            var e=story.expansion;var c=story.continuation;
            switch(phase)
            {
                case 1:return "Examine a TV. Quando a energia falhar, siga a luz pela porta do quintal.";
                case 2:return !story.boxFound?"Primeiro, examine a caixa sob a cama. Use E / A junto do objeto."
                    :!story.pagesSolved?"Compare as páginas com a fotografia. Escolha duas páginas para trocá-las."
                    :"A chave e o caderno estão guardados. Vá ao Fusca e use W / A para seguir à Industrial.";
                case 4:case 5:return !story.renanMet?"Converse com Renan junto ao quadro branco."
                    :!story.buildingSolved?"Abra o notebook do professor e procure a diferença na fotografia."
                    :!story.codeSolved?"Reabra o notebook: compare os dois grupos na margem para descobrir o horário."
                    :!story.renanConfirmed?"Mostre o horário descoberto a Renan."
                    :"Investigação concluída. Converse com Renan para seguir à biblioteca.";
                case 6:return (e.visited&1)==0?"Examine o livro aberto: ele guarda o terceiro fragmento e a direção da trilha."
                    :(e.visited&2)==0?"Agora examine o globo para orientar a travessia do rio."
                    :(e.visited&4)==0?"Consulte o painel para localizar a entrada da capela."
                    :!e.mapSolved?"Na mesa central, posicione os marcos e gire suas setas para conectar a trilha."
                    :"Mapa concluído. Saia pela entrada sul e siga para a mata.";
                case 7:return e.forestSigns<3?"Siga a marca de "+new[]{"ÁRVORE","RIO","CAPELA"}[e.forestSigns]+". Use o esconderijo se a presença se aproximar."
                    :!e.fabioMet?"O caminho está registrado. Converse com Fábio na capela."
                    :"Fábio indicou o arquivo. Use a saída da capela para continuar.";
                case 8:return e.anchorFound?"O registro de Edelzio está no caderno. Use a saída para procurar Ouzana."
                    :"Consulte índice, inscrição e ficha. Combine ano, símbolo e registro no Livro do Tombo.";
                case 9:return e.reagentUnlocked?"Reagente recebido. A porta no canto inferior direito leva à oficina."
                    :"Apresente a Ouzana o registro obtido no Tombo; compare as leituras com ela.";
                case 10:return !e.workshopParked?"Conduza até a vaga, voltado para cima. Pare e use W / A para sair do Fusca."
                    :!e.stabilized?e.reagentCharges==0?"Reponha o reagente na bancada de reserva e volte à lataria."
                        :"Aplique o reagente na lataria. Uma aplicação revela as três marcas."
                    :"As marcas estão registradas. Use a saída da oficina para partir.";
                case 11:return c.solved[0]?"Planta recuperada. Use a saída para seguir ao casarão."
                    :(c.clues[0]&128)==0?"Converse com Renan: ele preservou as versões do documento."
                    :!c.HasAll(11,3)?"No notebook, examine impressão, arquivo original e cópia alterada."
                    :"Compare as plantas e marque somente os dois detalhes que foram apagados.";
                case 12:return (c.clues[1]&1)==0?"Examine a planta preservada para localizar o acesso de serviço."
                    :!c.serviceRevealed?"Compare a parede de serviço com a planta e revele seu contorno com reagente."
                    :!c.serviceKey?area==0?"Entre pela porta principal. A chave está no escritório, à direita do hall."
                        :"Cruze a porta do escritório à direita do hall e procure a chave junto às gavetas."
                    :area==0?"A passagem foi revelada e a chave está guardada. Abra o acesso lateral ao porão."
                        :"Vá pelo escritório ao corredor de serviço e use o acesso ao porão.";
                case 13:case 14:case 15:case 18:
                    bool done=phase==18?c.AgreementComplete:c.solved[phase-11];
                    if(done)return "Investigação concluída. A saída leva a "+Destination(phase)+".";
                    if(phase==14&&(c.clues[3]&128)==0)return "Apresente os registros a Fábio antes de consultar a página ocultada.";
                    var definition=Definition(phase);
                    if(!c.HasAll(phase,definition.required))
                        for(int i=0;i<definition.required;i++)if((c.clues[phase-11]&(1<<i))==0)
                            return "Próxima prova: "+definition.labels[i]+". Examine o objeto e consulte o caderno.";
                    return phase==13?"Na mesa, associe os vestígios às causas e consequências das experiências."
                        :phase==14?"No Tombo, associe cada documento à sua época."
                        :phase==15?"Reconstrua a lembrança acionando os vestígios na sequência vivida."
                        :"Associe cada conclusão sobre a criatura a uma prova diferente.";
                case 20:return !c.manifestationDispelled?c.chambersPrepared!=7?"Prepare os três circuitos: árvore, rio e capela."
                        :"Dissipe a manifestação. Saia dos avisos de ataque e use seus aliados."
                    :!c.finalCalibrated?"No mecanismo central, ajuste as três leituras mantendo o selo ativo."
                    :"Retorno calibrado. Use a saída para alcançar a passagem segura.";
                case 21:
                    if(area==1)return (c.clues[10]&1)==0?"Converse com Renan para receber as cópias preservadas."
                        :(c.clues[10]&2)==0?"Abra o caderno no notebook e registre o fim da investigação."
                        :"Volte ao Fusca para encerrar a história.";
                    return new[]{"Abra a passagem no mecanismo; mantenha o selo ativo.","Acione aguardar a travessia e espere a criatura passar.",
                        "A criatura atravessou. Agora encerre a ligação no mecanismo.","Aguarde o fechamento da ruptura.","O acordo terminou. Use a saída para voltar à Industrial."}[Mathf.Clamp(c.finalStep,0,4)];
                default:return "Consulte o caderno e examine os pontos de investigação.";
            }
        }
        private static CampaignContinuationDefinition Definition(int phase)
        {if(!Definitions.TryGetValue(phase,out var value))Definitions[phase]=value=CampaignContinuationDefinition.Get(phase);return value;}
        public static string Target(CampaignStory s,int phase,int area=0)
        {
            var e=s.expansion;var c=s.continuation;
            switch(phase)
            {
                case 2:return s.pagesSolved?"car":"box";
                case 4:case 5:return !s.renanMet||s.codeSolved?"renan":"notebook";
                case 6:return (e.visited&1)==0?"archive":(e.visited&2)==0?"school":(e.visited&4)==0?"square":e.mapSolved?"exit":"map";
                case 7:return e.forestSigns<3?"sign"+e.forestSigns:e.fabioMet?"exit":"fabio";
                case 8:return e.anchorFound?"exit":"anchor";
                case 9:return e.reagentUnlocked?"exit":"ouzana";
                case 10:return !e.workshopParked?null:e.stabilized?"exit":e.reagentCharges==0?"refill":"spray2";
                case 11:return c.solved[0]?"exit":(c.clues[0]&128)==0?"renan":"notebook";
                case 12:return (c.clues[1]&1)==0?"plan":!c.serviceRevealed?"reveal":!c.serviceKey?area==0?"entrance":"key":"service";
                case 13:case 14:case 15:case 18:
                    if(phase==18?c.AgreementComplete:c.solved[phase-11])return "exit";
                    if(phase==14&&(c.clues[3]&128)==0)return "fabio";
                    var d=Definition(phase);
                    for(int i=0;i<d.required;i++)if((c.clues[phase-11]&(1<<i))==0)return d.ids[i];
                    return phase==14?"timeline":"puzzle";
                case 20:return !c.manifestationDispelled?null:c.finalCalibrated?"exit":"procedure";
                case 21:return area==1?(c.clues[10]&1)==0?"renan":(c.clues[10]&2)==0?"notebook":"fusca":c.finalStep==4?"exit":"procedure";
                default:return null;
            }
        }
        public static void DrawMarker(CampaignMapPlan plan,CampaignStory story,int phase,int area=0,string waypoint=null)
        {
            if(!VarginhaGameSettings.Current.interactionHints||Camera.main==null)return;
            string id=waypoint??Target(story,phase,area);if(id==null)return;
            var point=plan.points.Find(p=>p.id==id);if(point==null)return;
            var screen=Camera.main.WorldToScreenPoint(point.position+Vector2.up*.4f);
            float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f);
            var canvas=new Vector2((screen.x-(Screen.width-1280*scale)/2)/scale,
                (Screen.height-screen.y-(Screen.height-720*scale)/2)/scale);
            bool outside=canvas.x<120||canvas.x>1160||canvas.y<172||canvas.y>575;
            string location=id=="exit"?"SAÍDA • "+Destination(phase):id=="service"?"ACESSO AO PORÃO"
                :point.label.Split('•')[0].Trim();
            string direction=!outside?"PRÓXIMO":Mathf.Abs(canvas.x-640)>Mathf.Abs(canvas.y-360)
                ?canvas.x<640?"À ESQUERDA":"À DIREITA":canvas.y<360?"ACIMA":"ABAIXO";
            float x=Mathf.Clamp(canvas.x-125,28,1002),y=Mathf.Clamp(canvas.y-38,172,565);
            var rect=new Rect(x,y,250,30);ExperimentGUI.Box(rect,new Color(.02f,.055f,.065f,.94f));
            ExperimentGUI.Box(new Rect(x,y,3,30),new Color(.43f,.77f,.67f));
            ExperimentGUI.Label(new Rect(x+10,y+4,230,24),direction+" • "+location,small:true);
        }
        public static string RoomName(CampaignMapPlan plan,Vector2 feet)
        {
            // Smaller zones (such as the service corridor) take precedence over
            // their containing room; painted walls are never treated as floor.
            CampaignMapPlan.Surface found=null;
            foreach(var room in plan.rooms)
                if(room.name!="Cenário ilustrado"&&room.rect.Contains(feet)
                    &&(found==null||room.rect.width*room.rect.height<found.rect.width*found.rect.height))found=room;
            return found?.name;
        }
        public static void DrawRoom(CampaignMapPlan plan,Vector2 feet)
        {
            if(plan.phase!=112&&plan.phase!=13)return;
            var name=RoomName(plan,feet);if(name==null)return;
            ExperimentGUI.Box(new Rect(28,574,360,30),new Color(.02f,.055f,.065f,.9f));
            ExperimentGUI.Label(new Rect(40,578,336,24),(plan.phase==112?"TÉRREO • ":"PORÃO • ")+name.ToUpperInvariant(),small:true);
        }
    }
}
