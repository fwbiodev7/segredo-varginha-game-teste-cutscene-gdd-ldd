using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Plans are shared by construction, navigation, tests and exported floor plans.
    public sealed class CampaignMapPlan
    {
        public sealed class Surface
        {
            public string name, motif; public Rect rect; public Color color;
            public Surface(string n, string m, Rect r, Color c) { name = n; motif = m; rect = r; color = c; }
        }
        public sealed class Furnishing
        {
            public string name, motif; public Vector2 position, size; public Rect footprint;
            public Furnishing(string n, string m, Vector2 p, Vector2 s, Rect f) { name = n; motif = m; position = p; size = s; footprint = f; }
        }
        public sealed class Point
        {
            public string id, label; public Vector2 position;
            public Point(string i, string l, float x, float y) { id = i; label = l; position = new Vector2(x, y); }
        }
        public Rect bounds = new(-12, -8, 24, 16);
        public Vector2 spawn = new(0, -5);
        public string title;
        public int phase;
        public readonly List<Surface> rooms = new();
        public readonly List<Rect> walls = new();
        public readonly List<Furnishing> furniture = new();
        public readonly List<Point> points = new();
        private static readonly Color Wood = new(.39f, .29f, .23f), Stone = new(.36f, .37f, .35f), Grass = new(.12f, .24f, .19f);
        private void Room(string name, float x, float y, float w, float h, string motif = "Floor_House")
            => rooms.Add(new Surface(name, motif, new Rect(x, y, w, h), motif == "Floor_Yard" ? Grass : motif == "Floor_House" ? Wood : Stone));
        private void Wall(float x, float y, float w, float h) => walls.Add(new Rect(x, y, w, h));
        private void Enclose(float x, float y, float w, float h, float doorX)
        {
            const float t = .35f;
            Wall(x, y, t, h); Wall(x + w - t, y, t, h); Wall(x, y + h - t, w, t);
            Wall(x, y, doorX - 1 - x, t); Wall(doorX + 1, y, x + w - doorX - 1, t);
        }
        private void Prop(string n, string motif, float x, float y, float w, float h, float fw = -1, float fh = -1)
        {
            if(phase==1&&CampaignVisualAssets.Prop(motif) is Sprite art)
            {
                float scale=Mathf.Min(w/art.bounds.size.x,h/art.bounds.size.y);
                w=art.bounds.size.x*scale;h=art.bounds.size.y*scale;
            }
            fw = fw < 0 ? w * .82f : fw; fh = fh < 0 ? phase==1?Mathf.Min(.42f,h*.35f):h*.58f : fh;
            var footprint=new Rect(x-fw/2,y-h*.30f-fh/2,fw,fh);
            if(motif=="Fusca")footprint=new Rect(x-w*.31f,y-h*.38f,w*.62f,h*.76f);
            furniture.Add(new Furnishing(n, motif, new Vector2(x, y), new Vector2(w, h), footprint));
        }
        private void PointAt(string id, string label, float x, float y) => points.Add(new Point(id, label, x, y));
        public static CampaignMapPlan Create(int phase)
        {
            var p = new CampaignMapPlan { phase = phase };
            p.Room("Área de circulação", -12, -8, 24, 16, phase == 7 ? "Floor_Yard" : "SchoolFloor");
            // Outer boundaries belong to the architecture layer, never to the furniture layer.
            p.Wall(-12, -8, .35f, 16); p.Wall(11.65f, -8, .35f, 16); p.Wall(-12, 7.65f, 24, .35f); p.Wall(-12, -8, 24, .35f);
            switch (phase)
            {
                case 1: p.Childhood(); break;
                case 3: p.Road(); break;
                case 4: case 5: p.School(phase); break;
                case 6:
                    p.title = "FRAGMENTOS";
                    p.Room("Biblioteca municipal", -11, 1, 7, 6); p.Enclose(-11, 1, 7, 6, -7.5f);
                    p.Room("Arquivo da Industrial", 4, 1, 7, 6); p.Enclose(4, 1, 7, 6, 7.5f);
                    p.Room("Praça do relato de 1996", -11, -7, 22, 7, "Floor_Yard");
                    p.Room("Passeio entre os arquivos",-4,-1,8,8,"Path");
                    p.Room("Caminho da praça",-11,-4.8f,22,1.8f,"Path");
                    p.Wall(-8.5f,4.1f,.35f,2.5f);
                    p.Prop("Estante municipal", "Bookshelf", -9.7f, 5.6f, 1.6f, 2); p.Prop("Mesa de consulta", "Desk", -6.4f, 4.7f, 2.1f, 1.2f);
                    p.Prop("Arquivo escolar", "Dresser", 9.5f, 5.8f, 1.9f, 1.6f); p.Prop("Notebook de consulta", "Desk", 6.4f, 4.7f, 2.1f, 1.2f);
                    p.Prop("Banco da praça", "Pew", -7.5f, -5.7f, 3, 1.2f); p.Prop("Marco histórico", "Noticeboard", 4.5f, -5.4f, 1.6f, 1.6f);
                    p.PointAt("archive", "ARQUIVO MUNICIPAL", -6.4f, 3.5f); p.PointAt("school", "PLANTAS DA INDUSTRIAL", 6.4f, 3.5f);
                    p.PointAt("square", "RELATO DO CLARÃO", 4.5f, -3.7f); p.PointAt("map", "CRUZAR FRAGMENTOS NO NOTEBOOK", 0, -2);
                    p.PointAt("truth0", "RECORTE • TESTEMUNHO OMITIDO", -10, 3); p.PointAt("truth1", "REGISTRO • VEÍCULO SEM PLACA", -7.5f, -4.3f);
                    p.PointAt("exit", "PARTIR PARA A MATA", 9, -5); break;
                case 7:
                    p.title = "A MATA"; p.spawn = new(-9, -5);
                    p.Room("Trilha de terra", -10, -6, 3, 4, "Dirt"); p.Room("Curva da trilha",-8,-3.2f,4,1.6f,"Dirt"); p.Room("Trilha da bifurcação",-5,-3,2,3.2f,"Dirt");
                    p.Room("Clareira", -3, -1, 6, 5, "Dirt");
                    p.Room("Riacho sul", 3.5f, -7, 1.5f, 7.3f, "Water"); p.Room("Riacho norte", 3.5f, 2.1f, 1.5f, 5.5f, "Water");
                    p.Room("Passagem sobre o riacho", 3, .3f, 3, 1.8f, "Bridge");
                    p.Room("Ruína da capela", 6, 2, 5, 5, "ChurchFloor");
                    p.Enclose(6, 2, 5, 5, 8.5f);
                    // The stream has a clear bridge; trunk footprints leave the signed path open.
                    p.Wall(3.5f, -7, 1.5f, 7.3f); p.Wall(3.5f,2.1f,1.5f,5.5f);
                    for (int i = 0; i < 5; i++) { p.Prop("Árvore norte " + i, "Plant", -10 + i * 2.6f, 6.4f, 1.9f, 2.5f, .65f, .65f); }
                    for (int i = 0; i < 5; i++) { p.Prop("Árvore sul " + i,"Plant",-10+i*2.6f,-6.7f,1.9f,2.5f,.65f,.65f); }
                    p.Prop("Árvore bifurcação", "Plant", -5, 2, 2.7f, 3, .8f, .8f); p.Prop("Árvore clareira", "Plant", -.8f, -3, 2.7f, 3, .8f, .8f);
                    p.Prop("Árvore esconderijo", "Shrub", -2, 1.6f, 1.8f, 1.4f, .6f, .6f); p.Prop("Altar em ruínas", "Altar", 8.5f, 5.5f, 2, 1.5f);
                    p.PointAt("sign0", "SÍMBOLO DA ÁRVORE • PRIMEIRA ROTA", -5.8f, -2); p.PointAt("sign1", "SÍMBOLO DO RIO • CLAREIRA", 0, 1);
                    p.PointAt("sign2", "SÍMBOLO DA CAPELA • PONTE", 5.3f, 1.2f); p.PointAt("hide", "ESCONDERIJO • SEGURE-SE NO SILÊNCIO", -2, .1f);
                    p.PointAt("fabio", "PADRE FÁBIO", 8.5f, 4); p.PointAt("exit", "ENTRAR NO ESCONDERIJO", 8.5f, 3); break;
                case 8:
                    p.title = "A ÂNCORA";
                    p.Room("Arquivo de Fábio", -11, 0, 9, 7); p.Enclose(-11, 0, 9, 7, -6.5f);
                    p.Room("Subterrâneo religioso", 2, 0, 9, 7, "ChurchFloor"); p.Enclose(2, 0, 9, 7, 6.5f);
                    p.Room("Antecâmara do abrigo",-2,-6.5f,4,4.5f,"ChurchFloor"); p.Enclose(-2,-6.5f,4,4.5f,0);
                    p.Prop("Livro do Tombo", "Desk", -6.5f, 4.8f, 2.8f, 1.5f); p.Prop("Estante de registros", "Bookshelf", -9.8f, 5.5f, 1.6f, 2);
                    p.Prop("Arquivo de 1898", "Dresser", -3.5f, 5.7f, 1.6f, 1.6f); p.Prop("Altar da âncora", "Altar", 6.5f, 5.7f, 3, 1.7f);
                    p.Prop("Banco de pedra", "Pew", 9, 2.8f, 2, 1.2f);
                    p.PointAt("index", "ÍNDICE • ANO DO REGISTRO", -6.5f, 3.3f); p.PointAt("symbol", "INSCRIÇÃO • ÂNCORA", 5.3f, 4.2f);
                    p.PointAt("record", "FICHA • NÚMERO DO REGISTRO", -9.5f, 3.3f); p.PointAt("anchor", "COMPARTIMENTO DO TOMBO", 6.5f, 3.6f);
                    p.PointAt("trust", "PEDIR EXPLICAÇÃO A FÁBIO", -3, -3); p.PointAt("truth2", "DOCUMENTO DE ZÉ GOMES • 1898", -3.5f, 4);
                    p.PointAt("exit", "PROCURAR OUZANA", 9, -5); break;
                case 9:
                    p.title = "OUZANA";
                    p.Room("Laboratório de análise", -11, 0, 9, 7, "LabFloor"); p.Enclose(-11, 0, 9, 7, -6.5f);
                    p.Room("Depósito e documentação", 2, 0, 9, 7); p.Enclose(2, 0, 9, 7, 6.5f);
                    p.Room("Recepção da pesquisa",-11,-6.5f,9,4.5f,"LabFloor"); p.Enclose(-11,-6.5f,9,4.5f,-6.5f);
                    p.Prop("Bancada de amostras", "LabBench", -6.5f, 4.8f, 3.5f, 1.5f); p.Prop("Geladeira de amostras", "Fridge", -9.5f, 5.5f, 1.5f, 2);
                    p.Prop("Estante de protocolos", "Bookshelf", 9.5f, 5.5f, 1.7f, 2); p.Prop("Mesa de pesquisa", "Desk", 5, 4.8f, 2.4f, 1.5f);
                    p.PointAt("ouzana", "OUZANA • APRESENTAR EVIDÊNCIAS", -4.5f, -5.5f); p.PointAt("control", "AMOSTRA DE CONTROLE", -8, 3.3f);
                    p.PointAt("residue", "RESÍDUO ANÔMALO", -5.4f, 3.3f); p.PointAt("protocol", "PROTOCOLO DE LEITURA", 5, 3.3f);
                    p.PointAt("samples", "REALIZAR TESTE NA BANCADA", -6.5f, 2.5f); p.PointAt("tutorial", "REAGENTE • TESTE GRATUITO", 9.5f, 3.3f);
                    p.PointAt("exit", "LEVAR REAGENTE À OFICINA", 9, -5); break;
                case 10:
                    p.title = "O FUSCA MARCADO";
                    p.Room("Oficina", -11, -1, 22, 8, "LabFloor"); p.Enclose(-11, -1, 22, 8, 0);
                    p.Room("Trecho de teste", -11, -7, 22, 5, "Street");
                    p.Wall(-4.5f,-1,.35f,2); p.Wall(-4.5f,3.5f,.35f,3.5f); p.Wall(5,-1,.35f,2); p.Wall(5,3.5f,.35f,3.5f);
                    p.Prop("Bancada do estabilizador", "Desk", -7, 5.7f, 3, 1.5f); p.Prop("Peças e ferramentas", "Dresser", 8.5f, 5.7f, 2.4f, 1.8f);
                    p.Prop("Fusca", "Fusca", 0, 3.5f, 2.4f, 3.1f, 2.1f, 2.3f);
                    p.PointAt("spray0", "BORRIFAR • CAPÔ", 0, 6); p.PointAt("spray1", "BORRIFAR • PORTA", -2.3f, 3.3f);
                    p.PointAt("spray2", "BORRIFAR • MOTOR", 0, .9f); p.PointAt("seal", "CONECTAR MARCAS DO SELO", -7, 4.2f);
                    p.PointAt("refill", "RESERVA DO REAGENTE", 8.5f, 4); p.PointAt("drive", "TESTAR ESTABILIZADOR • FUSCA", 2.3f, 3.3f);
                    p.PointAt("exit", "ENCERRAR O TESTE NA PISTA", 9, -5); break;
                default: throw new System.ArgumentOutOfRangeException(nameof(phase));
            }
            p.AddDetails();p.AddIdentity();p.FitAtmosphericSprites();return p;
        }
        private void AddDetails()
        {
            void Decoration(string n,string motif,float x,float y,float w,float h)=>Prop(n,motif,x,y,w,h,0,0);
            if(phase==1)
            {
                Decoration("Tapete da sala","Rug",5,3,5,3);
                Decoration("Janela do quarto","Window",-5.5f,6.65f,2.5f,1.2f);
                Decoration("Janela da sala","Window",5,6.65f,2.5f,1.2f);
                Prop("Vaso sanitário","Toilet",6.8f,-1.8f,1.2f,1.5f);
                Prop("Box do banheiro","Shower",6.8f,-5.4f,1.8f,2);
                Prop("Pia do banheiro","Basin",3.5f,-1.8f,1.4f,1.3f);
                Decoration("Toalhas","Towels",3.5f,-3.8f,1.2f,.8f);
                Prop("Planta da sala","PottedPlant",7.5f,6,.8f,1.1f,.4f,.3f);
            }
            if(phase==3)for(int y=-5;y<139;y+=8)
            {Decoration("Telhado oeste "+y,"Roof",-9,y+2.5f,4,5);Decoration("Telhado leste "+y,"Roof",9,y+4.5f,4,5);}
            if(phase==6)
            {
                Prop("Banco do jardim","Pew",-2,-6.4f,3,1.2f);
                foreach(float x in new[]{-2.5f,2.5f})Prop("Jardineira municipal "+x,"PottedPlant",x,5.5f,1.1f,1.5f,.5f,.4f);
                Prop("Árvore da praça","Plant",-9.5f,-2.2f,2.3f,3,.6f,.6f);
                Decoration("Janela da biblioteca","Window",-7.5f,6.65f,2.5f,1);
            }
            if(phase==7)
            {
                foreach(float x in new[]{6.5f,9.5f})foreach(float y in new[]{-5.8f,-2.2f})Prop("Árvore margem "+x+" "+y,"Plant",x,y,2.2f,2.8f,.6f,.6f);
                foreach(var position in new[]{new Vector2(-8.5f,1),new Vector2(-7,4),new Vector2(2,6.5f)})Prop("Arbusto "+position,"Shrub",position.x,position.y,1.3f,1,.5f,.4f);
            }
            if(phase==8)
            {
                Decoration("Tapete da antecâmara","Rug",0,-4.5f,2.4f,1.6f);
                Prop("Registros religiosos","Bookshelf",-8,2.2f,1.6f,2);
                Decoration("Janela da cripta","Window",6.5f,6.65f,2,1);
            }
            if(phase==9)
            {
                Prop("Sofá da recepção","Sofa",-8.5f,-3.6f,2.5f,1.4f);
                Prop("Mesa da recepção","CoffeeTable",-8.5f,-5.1f,2,.8f);
                Prop("Planta da recepção","PottedPlant",-9.9f,-3.3f,1.1f,1.4f,.4f,.4f);
                Prop("Bancada auxiliar","LabBench",-3.4f,1.5f,1.2f,2);
                Decoration("Janela do laboratório","Window",-6.5f,6.65f,2.5f,1);
            }
            if(phase==10)
            {
                furniture.Find(f=>f.name=="Bancada do estabilizador").motif="Workbench";
                furniture.Find(f=>f.name=="Peças e ferramentas").motif="ToolRack";
                Decoration("Janela da oficina","Window",-7,6.65f,2.5f,1);
            }
        }
        private void FitAtmosphericSprites()
        {
            foreach(var prop in furniture)
            {
                var sprite=CampaignAtmosphereAssets.Prop(prop.motif);if(sprite==null)continue;
                float aspect=sprite.rect.width/sprite.rect.height;
                var fit=prop.size.x/prop.size.y>aspect?new Vector2(prop.size.y*aspect,prop.size.y):new Vector2(prop.size.x,prop.size.x/aspect);
                prop.position+=Vector2.up*(fit.y-prop.size.y)/2;prop.size=fit;
                if(prop.footprint.width>0){var center=prop.footprint.center;var size=new Vector2(Mathf.Min(prop.footprint.width,fit.x*.82f),Mathf.Min(prop.footprint.height,fit.y*.58f));prop.footprint=new Rect(center-size/2,size);}
            }
        }
        private void AddIdentity()
        {
            void Decor(string n,string motif,float x,float y,float w,float h)=>Prop(n,motif,x,y,w,h,0,0);
            void Replace(string name,string motif){var item=furniture.Find(p=>p.name==name);if(item!=null)item.motif=motif;}
            void Resize(string name,string motif,Vector2 position,Vector2 size)
            {
                var item=furniture.Find(p=>p.name==name);if(item==null)return;
                item.motif=motif;item.position=position;item.size=size;
                item.footprint=new Rect(position.x-size.x*.41f,position.y-size.y*.59f,size.x*.82f,size.y*.58f);
            }
            if(phase==1)
            {
                Resize("Cama infantil","ChildhoodToys",new Vector2(-6.3f,4.5f),new Vector2(2.8f,3.2f));
                furniture.RemoveAll(p=>p.name=="Brinquedos");Replace("Mesa da cozinha","DiningTable");
                Decor("Samambaias do quintal","ForestCluster",24,-6,2.5f,1.4f);
                Decor("Pedras junto à cerca","ForestCluster",12,5.7f,2.4f,1.3f);
                Prop("Árvore da sombra","AncientTree",24.3f,4.8f,3.4f,4.1f,.7f,.6f);
                Prop("Árvore do fundo","Tree",23.5f,-4,2.6f,3.2f,.6f,.5f);
            }
            if(phase==3)
            {
                furniture.RemoveAll(p=>p.motif=="Roof");
                for(int y=-5,i=0;y<139;y+=8,i++)
                {
                    Decor("Fachada oeste "+i,i%4==0?"CityBakeryEast":i%4==2?"CityMarketEast":"CityHouseEast",-9,y+2.5f,4.4f,5.1f);
                    Decor("Fachada leste "+i,i==2?"WaterTower":i%3==0?"CityTownhouseWest":"CityHouseWest",9,y+2.5f,4.4f,5.1f);
                    Room("Travessa urbana "+i,-12,y-2.5f,24,1.5f,"Street");
                    Room("Faixa central "+i,-.07f,y,.14f,2,"RoadMarking");
                    for(int stripe=0;stripe<4;stripe++){Room("Travessia oeste "+i+" "+stripe,-5.7f,y-2.25f+stripe*.3f,1.3f,.12f,"RoadMarking");Room("Travessia leste "+i+" "+stripe,4.4f,y-2.25f+stripe*.3f,1.3f,.12f,"RoadMarking");}
                    Decor("Placa de Varginha "+i,"CitySigns",i%2==0?-6.2f:6.2f,y-1,1,1.6f);
                    Prop("Poste urbano "+i,"Lamp",-6.2f,y+3.4f,.55f,2,.18f,.2f);
                    if(i%3==0)Decor("Ponto de ônibus "+i,"BusStop",6.2f,y+1.8f,2.4f,1.4f);
                }
            }
            if(phase==4||phase==5)
            {
                Replace("Mural de investigação","TechnicalBoard");
                Decor("Identificação do campus","CitySigns",10.4f,-8.3f,1,1.8f);
                Decor("Jardim da entrada oeste","ForestCluster",-10,-8.1f,2,1.1f);
                Decor("Jardim da entrada leste","ForestCluster",10,-8.1f,2,1.1f);
            }
            if(phase==6)
            {
                Replace("Estante municipal","ArchiveShelf");Replace("Arquivo escolar","SealedArchive");
                var plaza=furniture.Find(p=>p.name=="Banco da praça");plaza.motif="CityPlaza";plaza.size=new Vector2(4,3.3f);plaza.position+=Vector2.up*1.05f;
                Replace("Marco histórico","TechnicalBoard");
                Decor("Paisagismo da praça","ForestCluster",-4.5f,-2.9f,2.8f,1.3f);
                Decor("História da cidade","CitySigns",10,-2,1.2f,2);
            }
            if(phase==7)
            {
                foreach(var tree in furniture)if(tree.name.StartsWith("Árvore")&&tree.motif=="Plant")tree.motif="AncientTree";
                for(int i=0;i<6;i++)Decor("Vegetação rasteira "+i,"ForestCluster",-10+i*3,-5.4f,2.4f,1.3f);
                Decor("Arco da capela em ruínas","ChapelArch",8.5f,2.7f,3.2f,3.1f);
                Decor("Sepultura esquecida","Gravestone",9.5f,5.2f,1.1f,1.3f);
                Replace("Altar em ruínas","HolyBasin");
            }
            if(phase==8)
            {
                foreach(var room in rooms)room.motif="ChurchFloor";
                Room("Capela de oração e vigília",-11,-6.5f,8,5,"ChurchFloor");Enclose(-11,-6.5f,8,5,-6.5f);
                Room("Cripta dos antigos guardiões",3,-6.5f,8,5,"ChurchFloor");Enclose(3,-6.5f,8,5,8);
                points.Find(p=>p.id=="trust").position=new Vector2(-5,-5.5f);
                points.Find(p=>p.id=="exit").position=new Vector2(10,-7);
                Replace("Livro do Tombo","Lectern");Replace("Estante de registros","SealedArchive");
                var lectern=furniture.Find(p=>p.name=="Livro do Tombo");lectern.size=new Vector2(2.8f,2.1f);lectern.position+=Vector2.up*.3f;
                Replace("Registros religiosos","ArchiveShelf");Replace("Arquivo de 1898","SealedArchive");
                Replace("Janela da cripta","GothicWindow");
                Prop("Pilar do arquivo","StonePillar",-10.2f,4,1,2.2f,.45f,.4f);
                Prop("Pilar da cripta","StonePillar",10.2f,4,1,2.2f,.45f,.4f);
                Prop("Sarcófago da contenção","Sarcophagus",3.9f,2.3f,2.1f,1.4f,1.6f,.65f);
                Prop("Pia de água benta","HolyBasin",-1,-4.4f,.7f,1.1f,.45f,.3f);
                Prop("Banco da nave norte","Pew",0,2.5f,2.4f,1.0f,1.6f,.4f);
                Prop("Banco da nave sul","Pew",0,.7f,2.4f,1.0f,1.6f,.4f);
                Prop("Banco de leitura da cripta","Pew",4,4.2f,2.4f,1,1.6f,.4f);
                Prop("Banco de vigília","Pew",7.7f,1.8f,2.5f,1,1.8f,.4f);
                Prop("Ambão da capela","Lectern",-9,-3,1.4f,1.8f,.7f,.35f);
                Prop("Banco de oração","Pew",-8,-4.8f,2.3f,.9f,1.7f,.35f);
                Prop("Água benta da capela","HolyBasin",-4.4f,-3,.65f,1.1f,.4f,.3f);
                Prop("Sarcófago oeste","Sarcophagus",5,-3.2f,2.1f,1.3f,1.4f,.55f);
                Prop("Sarcófago leste","Sarcophagus",8.8f,-3.2f,2.1f,1.3f,1.4f,.55f);
                Prop("Sepultura dos guardiões","Gravestone",5,-5.1f,1.3f,1.4f,.5f,.35f);
                Prop("Relicário selado","SealedArchive",9.7f,-5.0f,.85f,1.5f,.5f,.35f);
                Decor("Vitral da capela","GothicWindow",-6.5f,-1.8f,1.1f,1.6f);
                foreach(float x in new[]{-10.4f,-3.7f,3.7f,10.4f})Decor("Tocha das câmaras "+x,"Torch",x,-2.1f,.5f,1.1f);
                Decor("Vitral da nave","GothicWindow",0,6.1f,1.5f,2.2f);
                Decor("Vitral do arquivo","GothicWindow",-6.5f,6.05f,1.25f,1.6f);
                foreach(var t in new[]{new Vector2(-10.5f,6.1f),new Vector2(-2.5f,6.1f),new Vector2(2.5f,6.1f),new Vector2(10.5f,6.1f),new Vector2(-1.5f,-2.7f),new Vector2(1.5f,-2.7f)})Decor("Tocha medieval "+t,"Torch",t.x,t.y,.55f,1.2f);
                Decor("Restos da contenção","Gravestone",7.2f,-5.1f,1.2f,1.2f);
            }
            if(phase==9)
            {
                Room("Cultivo e conservação biológica",2,-6.5f,9,4.5f,"LabFloor");Enclose(2,-6.5f,9,4.5f,6.5f);
                furniture.RemoveAll(p=>p.name=="Janela do laboratório");
                foreach(var room in rooms)if(room.motif=="Floor_House"||room.motif=="SchoolFloor")room.motif="LabFloor";
                Replace("Bancada de amostras","MicroscopeBench");Replace("Geladeira de amostras","ScientificFreezer");
                var bench=furniture.Find(p=>p.name=="Bancada de amostras");bench.size=new Vector2(3.5f,2.2f);bench.position+=Vector2.up*.35f;
                Resize("Estante de protocolos","Terrarium",new Vector2(9.5f,5.5f),new Vector2(1.8f,2.2f));
                Resize("Mesa de pesquisa","ScienceCabinet",new Vector2(5,5.5f),new Vector2(1.8f,2.2f));
                Resize("Bancada auxiliar","DecontaminationSink",new Vector2(-3.4f,1.7f),new Vector2(1.6f,1.3f));
                Prop("Tanque biológico","SpecimenTank",-9.7f,2.7f,1.1f,2,.65f,.6f);
                Prop("Carrinho de coleta","SampleCart",-3.3f,5.6f,1.3f,1.6f,.65f,.5f);
                Prop("Microscopia auxiliar","MicroscopeBench",9,1.8f,2.7f,1.7f,1.8f,.5f);
                Prop("Amostra viva da pesquisa","SpecimenTank",0,4.4f,1.3f,2.3f,.7f,.5f);
                Prop("Carrinho de recepção","SampleCart",-3.7f,-3.2f,1,1.4f,.5f,.4f);
                Prop("Bancada de recepção científica","MicroscopeBench",-5.6f,-3.2f,2.1f,1.5f,1.5f,.4f);
                Prop("Preparação de lâminas","SampleCart",-7.6f,1.8f,1.1f,1.6f,.6f,.4f);
                Prop("Amostras da documentação","SampleCart",5,1.8f,1.1f,1.6f,.6f,.4f);
                Prop("Terrário de cultivo","Terrarium",4,-3.2f,1.7f,2.3f,1.2f,.5f);
                Prop("Lavagem do cultivo","DecontaminationSink",6.5f,-3.2f,2,1.4f,1.5f,.4f);
                Prop("Amostra controlada","SpecimenTank",9.6f,-3.1f,1.1f,2,.65f,.5f);
                Prop("Arquivo do cultivo","ScienceCabinet",3.5f,-5.2f,1,1.6f,.65f,.4f);
                Decor("Protocolos de cultivo","DNABoard",7,-2.2f,2.2f,1.1f);
                Decor("Estudos de DNA","DNABoard",-6.5f,6.1f,2.4f,1.25f);
                Decor("Painel de células","DNABoard",6.5f,6.2f,1.6f,.9f);
                Decor("Referências da recepção","DNABoard",-6.5f,-2.6f,2,1.1f);
            }
            if(phase==10)
            {
                Replace("Bancada do estabilizador","MechanicBench");
                foreach(var room in rooms)if(room.name=="Oficina")room.motif="GarageFloor";
                for(int i=0;i<6;i++)Room("Faixa da pista de teste "+i,-9+i*3,-4.6f,1.5f,.08f,"RoadMarking");
                Decor("Biologia aplicada ao selo","SampleCart",8.5f,2.1f,1.4f,1.5f);
                Decor("Placa da oficina","CitySigns",-9.5f,-2.7f,1.2f,1.9f);
            }
        }
        private void School(int number)
        {
            title=number==4?"INDUSTRIAL • ENTRE AULAS E PISTAS":"INDUSTRIAL • O CÓDIGO DAS 23:23";
            bounds=new Rect(-12.5f,-14.3f,25,20.4f);spawn=new Vector2(.75f,-8.4f);
            rooms.Clear();walls.Clear();
            Room("Jardim do campus",-12.5f,-14.3f,25,20.4f,"Floor_Yard");
            Room("Pista e estacionamento",-12.5f,-14.3f,25,5.4f,"Street");
            Room("Calçada da Industrial",-8.05f,-8.1f,17.1f,2.4f,"Path");
            Room("Acesso ao portão",-.5f,-9.8f,2.5f,4.1f,"Path");
            Room("Laboratório de Sistemas original",-7.7f,-5.7f,16.4f,11.4f,"SchoolFloor");
            Wall(-8.05f,-5.7f,.7f,11.4f);Wall(8.35f,-5.7f,.7f,11.4f);Wall(-8.05f,5.35f,17.1f,.7f);
            Wall(-8.05f,-6.05f,7.55f,.7f);Wall(2,-6.05f,7.05f,.7f);
            Wall(-12.5f,-14.3f,.3f,20.4f);Wall(12.2f,-14.3f,.3f,20.4f);Wall(-12.5f,-14.3f,25,.3f);
            float[] columns={-5.85f,-3.05f,3.75f,6.55f};
            void OriginalProp(string n,string motif,float x,float y,float w,float h,float cw,float ch,float offset)
                => furniture.Add(new Furnishing(n,motif,new Vector2(x,y),new Vector2(w,h),new Rect(x-cw/2,y+offset-ch/2,cw,ch)));
            for(int row=0;row<3;row++)for(int column=0;column<4;column++)
            {
                float x=columns[column],y=3.15f-row*2.4f;bool computer=column!=1;
                OriginalProp("Carteira original "+(row*4+column),computer?"OriginalComputerDesk":"OriginalDesk",x,y+(computer?.16f:0),1.72f,computer?1.35f:.95f,1.55f,.4f,-.15f-(computer?.16f:0));
                OriginalProp("Cadeira azul "+(row*4+column),"OriginalChair",x,y-.7f,.76f,.9f,.44f,.32f,-.1f);
            }
            OriginalProp("Mesa do professor","OriginalTeacherDesk",.8f,4.64f,1.85f,.65f,1.65f,.3f,-.12f);
            OriginalProp("Arquivo escolar","OriginalShelf",-6.65f,-4.67f,.7f,1.3f,.62f,.45f,-.35f);
            Prop("Mural de investigação","Noticeboard",8,-4.55f,.5f,.9f,0,0);
            for(int i=0;i<3;i++)Prop("Janela original "+i,"OriginalWindow",-5.25f+i*3.15f,5.2f,2.7f,.92f,0,0);
            Prop("Lousa original","OriginalWhiteboard",6.55f,5.18f,2.15f,.82f,0,0);
            Prop("Projetor original","OriginalProjector",3.25f,5.2f,.75f,.42f,0,0);
            Prop("Fusca","Fusca",-5.5f,-10.4f,2.4f,3.1f);
            foreach(float x in new[]{-10.7f,-7.25f,-3.75f,3.5f,7,10.5f})Room("Divisão de vaga "+x,x,-12.5f,.08f,3.45f,"RoadMarking");
            Room("Fundo das vagas oeste",-10.7f,-12.5f,6.95f,.08f,"RoadMarking");
            Room("Fundo das vagas leste",3.5f,-12.5f,7,.08f,"RoadMarking");
            for(int i=0;i<5;i++)Room("Travessia "+i,-.3f,-10.9f+i*.38f,2.1f,.18f,"RoadMarking");
            Prop("Banco do jardim","Pew",5.7f,-7.1f,2.2f,.75f);
            foreach(float x in new[]{-10,10.3f})Prop("Jardineira "+x,"Shrub",x,-6.7f,1.6f,1.3f,.7f,.45f);
            PointAt("lesson","MESA DO PROFESSOR",.8f,3.8f);PointAt("archive","ARQUIVO DE PLANTAS",-6.65f,-3.7f);
            PointAt("notebook","NOTEBOOK DE INVESTIGAÇÃO",-3.05f,1.7f);PointAt("mural","LEGENDA DO LEVANTAMENTO",7.7f,-3.7f);
            PointAt("research","CORRESPONDÊNCIA DE OUZANA",2.2f,4.35f);PointAt("renan","RENAN",3,-9);PointAt("car","FUSCA",-3.5f,-10.4f);
        }
        private void Road()
        {
            title="NÃO DEIXE ELA SAIR • TRAVESSIA URBANA";bounds=new Rect(-12,-8,24,150);spawn=new Vector2(-2,0);rooms.Clear();walls.Clear();
            Room("Asfalto",-5,-8,10,150,"Street");Room("Calçada esquerda",-7,-8,2,150,"Path");Room("Calçada direita",5,-8,2,150,"Path");
            Room("Jardins urbanos",-12,-8,5,150,"Floor_Yard");Room("Jardins leste",7,-8,5,150,"Floor_Yard");
            for(int y=-5;y<139;y+=8){Wall(-11,y,4,5);Wall(7,y,4,5);}
            PointAt("start","PARTIDA",-2,0);PointAt("breakdown","PANE",-2,55);PointAt("arrival","INDUSTRIAL",-2,120);
        }
        private void Childhood()
        {
            title = "CASA DE EDELZIO • 1996"; bounds = new Rect(-9, -8, 36, 16); spawn = new Vector2(6.15f, 2.8f);
            rooms.Clear(); walls.Clear();
            Room("Circulação da casa", -9, -7, 18, 14);
            Room("Quarto infantil", -9, 1, 8, 6); Room("Cozinha", -9, -7, 8, 6, "SchoolFloor");
            Room("Corredor", -1, -7, 2, 14); Room("Sala", 1, 1, 8, 6); Room("Banheiro e serviço", 1, -7, 8, 6, "SchoolFloor");
            Room("Quintal", 9, -8, 18, 16, "Floor_Yard"); Room("Passagem do quintal", 9, -1, 7, 2, "SchoolFloor");
            Wall(-9, -7, .35f, 14); Wall(-9, 6.65f, 18, .35f); Wall(-9, -7, 18, .35f);
            Wall(8.65f, 1.2f, .35f, 5.8f); Wall(8.65f, -7, .35f, 5.8f);
            Wall(-1.2f, 3.8f, .35f, 3.2f); Wall(-1.2f, -7, .35f, 7.8f); Wall(-9, .8f, 6, .35f);
            Wall(1, 3.8f, .35f, 3.2f); Wall(1, -7, .35f, 7.8f); Wall(3, .8f, 6, .35f);
            Wall(26.65f, -8, .35f, 16); Wall(9, 7.65f, 18, .35f); Wall(9, -8, 18, .35f);
            Prop("Cama infantil", "Bed", -6.3f, 4.5f, 2.4f, 2.8f); Prop("Brinquedos", "Nightstand", -7.6f, 2.5f, 1, 1);
            Prop("Mesa de desenho", "Desk", -4.55f, 5.8f, 2, 1); Prop("Fogão", "Stove", -7.4f, -4.8f, 1.4f, 1.8f);
            Prop("Mesa da cozinha", "CoffeeTable", -4.5f, -3.5f, 2.3f, 1.5f); Prop("Geladeira", "Fridge", -7.5f, -1.4f, 1.3f, 1.8f);
            Prop("TV CRT", "TV", 4.6f, 5.8f, 1.8f, 1.5f); Prop("Sofá", "SofaFacingTV", 4.6f, 2.9f, 2.4f, 1.5f);
            Prop("Mesa do jornal", "CoffeeTable", 4.6f, 4, 1.6f, .9f); Prop("Lavatório", "Kitchen", 4.7f, -5.5f, 2, 1.5f);
            Prop("Árvore do clarão", "Tree", 18, 5, 3, 3, .8f, .8f);
            PointAt("tv", "TV", 4.6f, 4.5f); PointAt("paper", "JORNAL", 6.2f, 4); PointAt("drawing", "DESENHO", -4.55f, 4.5f);
            PointAt("toys", "BRINQUEDOS", -7.6f, 1.5f); PointAt("yard", "QUINTAL", 17, 0);
        }
        public bool IsClear(Vector2 point, float radius = .28f)
        {
            if (point.x - radius < bounds.xMin || point.x + radius > bounds.xMax || point.y - radius < bounds.yMin || point.y + radius > bounds.yMax) return false;
            foreach (var wall in walls) if (Touches(wall, point, radius)) return false;
            foreach (var prop in furniture) if (prop.footprint.width > 0 && prop.footprint.height > 0 && Touches(prop.footprint, point, radius)) return false;
            return true;
        }
        private static bool Touches(Rect rect, Vector2 point, float radius)
        {
            var nearest = new Vector2(Mathf.Clamp(point.x, rect.xMin, rect.xMax), Mathf.Clamp(point.y, rect.yMin, rect.yMax));
            return (point - nearest).sqrMagnitude < radius * radius;
        }
        public bool Route(Vector2 from, Vector2 to, List<Vector2> path)
        {
            path.Clear(); const float step = .5f;
            int width = Mathf.RoundToInt(bounds.width / step), height = Mathf.RoundToInt(bounds.height / step);
            int Index(Vector2 v) => Mathf.Clamp(Mathf.RoundToInt((v.y - bounds.yMin) / step), 0, height - 1) * width + Mathf.Clamp(Mathf.RoundToInt((v.x - bounds.xMin) / step), 0, width - 1);
            Vector2 Position(int i) => new(bounds.xMin + i % width * step, bounds.yMin + i / width * step);
            int start = Index(from), end = Index(to); var previous = new int[width * height]; System.Array.Fill(previous, -1);
            var queue = new Queue<int>(); queue.Enqueue(start); previous[start] = start;
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            while (queue.Count > 0 && previous[end] < 0)
            {
                int current = queue.Dequeue();
                foreach (var direction in directions)
                {
                    int x = current % width + direction.x, y = current / width + direction.y;
                    if (x < 0 || x >= width || y < 0 || y >= height) continue;
                    int next = y * width + x; if (previous[next] >= 0 || !IsClear(Position(next), .32f)) continue;
                    previous[next] = current; queue.Enqueue(next);
                }
            }
            if (previous[end] < 0) return false;
            for (int i = end; i != start; i = previous[i]) path.Add(Position(i));
            path.Reverse(); return true;
        }
    }
}
