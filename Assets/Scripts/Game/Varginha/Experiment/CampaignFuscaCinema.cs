using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // A layered scene, not a replacement face/portrait. Original Driving frames retain
    // their registration and one uniform pixel scale throughout the steering cycle.
    public static class CampaignFuscaCinema
    {
        public const string MorningPath="Varginha/CinemaOpening/MorningTown";
        public const string FramePath="Varginha/CinemaOpening/FuscaCabinCartoon";
        private static Texture _morning,_frame,_dashboard,_ignition,_column;
        public static readonly Vector2 SteeringHub=new(540,462);
        public static readonly Rect DashboardBounds=new(390,300,940,313);
        // Measured wheel hubs in the untouched 1254px Driving source atlas, top-left origin.
        private static readonly Vector2[] WheelSource={new(273,812),new(559,810),new(853,812),new(1146,810)};
        private static readonly List<Vector2> ArrivalRoute=new();
        private static CampaignIllustratedMaps.Layout _schoolLayout;
        private static float _arrivalLength;
        public readonly struct Shot
        {
            public readonly string view,title,caption;public readonly int panel;public readonly float duration;
            public Shot(string view,string title,string caption,int panel,float duration)
            {this.view=view;this.title=title;this.caption=caption;this.panel=panel;this.duration=duration;}
        }
        // GDD Ato II: everyday drive -> electrical failure -> childhood phrase ->
        // hot ignition and silence -> spontaneous restart -> school. No extra revelation.
        public static readonly Shot[] Shots={
            new("road","VARGINHA • 2026","[Motor e trânsito.] O caminho de todos os dias.",0,4.2f),
            new("interior","O RÁDIO","O sinal se perde. Mesmo desligado, o rádio continua chiando.",2,3.2f),
            new("interior","O PAINEL","O velocímetro dispara. Os faróis apagam. O Fusca está parando.",1,3.4f),
            new("interior","UMA VOZ CONHECIDA","[Voz infantil no rádio] “Não deixa ela sair.”",2,3.4f),
            new("interior","SILÊNCIO","A ignição está quente. Nenhum som. Edelzio solta a chave.",3,4.3f),
            new("road","NINGUÉM TOCOU NA CHAVE","[O motor volta sozinho.] Preciso conversar com alguém na escola.",0,3f),
            new("arrival","INDUSTRIAL","A aula vai começar. Aquela voz era a minha... quando criança. Preciso entender o que aconteceu.",0,5.8f)
        };
        public static int DriverFrame(float time,bool moving,bool reduced)=>!moving||reduced?0:Mathf.FloorToInt(Mathf.Max(0,time)*4)%4;
        public static float NeedleAngle(float time,bool anomaly,bool stopped,bool reduced)
            =>stopped?-140:anomaly?(reduced?85:-140+Mathf.Max(0,time)*470):-105;

        public static void DrawCabin(float time,string title,int panel,bool reduced)
        {
            _morning=CampaignCinemaTextures.Resolve("MorningTown");_frame=CampaignCinemaTextures.Resolve("FuscaCabinCartoon");
            _dashboard=CampaignCinemaTextures.Resolve("FuscaDashboardCartoon");
            bool silence=title=="SILÊNCIO",voice=title=="UMA VOZ CONHECIDA",fault=panel==1;
            bool moving=!silence&&!voice&&(!fault||time<1.2f);
            float shift=moving&&!reduced?Mathf.Floor(time*18)%1280:0;
            if(_morning!=null)
            {
                // A clean outdoor-only window region: cabin borders never slide through the glass.
                var uv=new Rect(.1f,0,.8f,.75f);
                GUI.DrawTextureWithTexCoords(new Rect(-shift,0,1280,720),_morning,uv);
                GUI.DrawTextureWithTexCoords(new Rect(1280-shift,0,1280,720),_morning,uv);
            }
            if(_frame!=null)GUI.DrawTexture(new Rect(0,0,1280,720),_frame);
            // Registered layers: scenery -> cabin -> dashboard/column -> driver -> front trim.
            // The gauge is directly behind the existing steering wheel. The column
            // physically connects its hub to the under-dash bracket.
            DrawDashboard(false);
            // Needles, gauge ticks and radio belong to the dashboard. Drawing
            // them after the driver painted amber pixels over the wheel rim.
            DrawInstruments(time,fault,silence||voice,reduced);
            _column=Resources.Load<Texture2D>("Varginha/HouseFurniture/FuscaSteeringColumn");
            if(_column!=null)GUI.DrawTexture(new Rect(SteeringHub.x-5,SteeringHub.y-8,72,27),_column);
            if(panel==2)CampaignSignalGlitch.DrawEcho(_frame,new Rect(0,0,1280,720),time,reduced);
            int driverFrame=DriverFrame(time,moving,reduced);
            var actor=CampaignStorySprites.Frame("Driving",2,driverFrame);
            DrawSprite(DriverBounds(actor,driverFrame),actor);
            DrawDashboard(true);
            if(silence)
            {
                // GDD: ignition becomes hot, hand is withdrawn; no injury, repair or culprit is invented.
                float darkness=Mathf.Clamp01(time/1.1f)*.82f;
                ExperimentGUI.Box(new Rect(0,0,1280,720),new Color(.015f,.025f,.035f,darkness));
                DrawIgnition(time,reduced);
            }
            if(panel==2||fault)
            {
                CampaignSignalGlitch.DrawCabin(new Rect(0,86,1280,464),_frame,time,reduced,voice||fault);
            }
        }
        public static Rect DriverBounds(Sprite sprite,int frame)
        {
            if(sprite==null)return Rect.zero;
            var r=sprite.rect;var point=WheelSource[Mathf.Clamp(frame,0,3)];
            float sx=sprite.texture.width/1254f,sy=sprite.texture.height/1254f;
            var local=new Vector2(point.x*sx-r.x,point.y*sy-(sprite.texture.height-r.yMax));
            const float scale=2.05f;
            return new Rect(Mathf.Round(SteeringHub.x-local.x*scale),Mathf.Round(SteeringHub.y-local.y*scale),r.width*scale,r.height*scale);
        }
        private static void DrawDashboard(bool foreground)
        {
            if(_dashboard==null)return;
            // Split the same artwork at a fixed seam; no duplicate full overlay.
            const float split=.77f;var r=DashboardBounds;
            if(foreground)GUI.DrawTextureWithTexCoords(new Rect(r.x,r.y+r.height*split,r.width,r.height*(1-split)),_dashboard,new Rect(0,0,1,1-split));
            else GUI.DrawTextureWithTexCoords(new Rect(r.x,r.y,r.width,r.height*split),_dashboard,new Rect(0,1-split,1,split));
        }
        private static void DrawIgnition(float time,bool reduced)
        {
            // A registered three-pose closeup keeps the key inserted in the same
            // cylinder. Only the bare hand releases; the short-sleeve identity is
            // preserved and the engine restarting later is still unexplained.
            _ignition=Resources.Load<Texture2D>("Varginha/CinemaOpening/IgnitionCloseup");
            ExperimentGUI.Panel(new Rect(762,138,380,380));
            int pose=time<.85f?0:time<1.5f?1:2;
            if(_ignition!=null)GUI.DrawTextureWithTexCoords(new Rect(767,143,370,370),_ignition,new Rect(pose/3f,0,1f/3,1));
        }
        private static void DrawActor(Sprite sprite,Vector2 sole,float scale)
        {
            if(sprite==null)return;
            var r=sprite.rect;
            var dst=new Rect(Mathf.Round(sole.x-sprite.pivot.x*scale),Mathf.Round(sole.y-r.height*scale),r.width*scale,r.height*scale);
            GUI.DrawTextureWithTexCoords(dst,sprite.texture,new Rect(r.x/sprite.texture.width,r.y/sprite.texture.height,r.width/sprite.texture.width,r.height/sprite.texture.height));
        }
        private static void DrawInstruments(float time,bool fault,bool stopped,bool reduced)
        {
            Vector2 PanelPoint(float x,float y)=>new(DashboardBounds.x+x*DashboardBounds.width,DashboardBounds.y+y*DashboardBounds.height);
            var centre=PanelPoint(141f/780,109f/260);Color amber=stopped?new Color(.16f,.14f,.10f):new Color(.95f,.65f,.28f);
            for(int i=0;i<11;i++)
            {
                float a=(-140+i*28)*Mathf.Deg2Rad;
                ExperimentGUI.Box(new Rect(Mathf.Round(centre.x+Mathf.Cos(a)*23),Mathf.Round(centre.y+Mathf.Sin(a)*37),3,4),amber);
            }
            float angle=NeedleAngle(time,fault,stopped,reduced)*Mathf.Deg2Rad;
            var end=centre+new Vector2(Mathf.Cos(angle)*20,Mathf.Sin(angle)*33);
            PixelLine(centre,end,amber,3);
            ExperimentGUI.Box(new Rect(centre.x-4,centre.y-4,8,8),new Color(.16f,.17f,.18f));
            var radioPoint=PanelPoint(285f/780,127f/260);var radio=new Rect(radioPoint.x,radioPoint.y,70,19);
            ExperimentGUI.Box(radio,new Color(.015f,.035f,.045f));
            for(int i=0;i<7;i++)ExperimentGUI.Box(new Rect(radio.x+5+i*9,radio.y+5,2,8),stopped?new Color(.06f,.09f,.09f):new Color(.12f,.65f,.66f));
            if(!stopped)ExperimentGUI.Box(new Rect(radio.x+7+Mathf.Round(time*8)%52,radio.y+2,3,15),new Color(1,.46f,.17f));
        }
        private static void PixelLine(Vector2 from,Vector2 to,Color color,int size)
        {
            int steps=Mathf.CeilToInt(Vector2.Distance(from,to));
            for(int i=0;i<=steps;i++){var p=Vector2.Lerp(from,to,(float)i/Mathf.Max(1,steps));ExperimentGUI.Box(new Rect(Mathf.Round(p.x),Mathf.Round(p.y),size,size),color);}
        }
        public static void DrawArrival(float time,bool reduced)
        {
            EnsureArrivalRoute();if(_schoolLayout==null)return;
            // Exactly the gameplay map and source-pixel coordinates, including
            // the parking bay, pavement and gate. No substitute facade artwork.
            float scale=1120f/_schoolLayout.width;
            var map=new Rect(80,86-490*scale,1120,_schoolLayout.height*scale);
            var school=Resources.Load<Texture2D>("Varginha/IllustratedMaps/"+_schoolLayout.image);
            if(school!=null)GUI.DrawTexture(map,school);
            Vector2 Image(Vector2 world)=>new((world.x-_schoolLayout.Bounds.xMin)/_schoolLayout.Scale,
                (_schoolLayout.Bounds.yMax-world.y)/_schoolLayout.Scale);
            Vector2 ScreenPoint(Vector2 pixel)=>new(Mathf.Round(map.x+pixel.x*scale),Mathf.Round(map.y+pixel.y*scale));
            var prop=System.Array.Find(_schoolLayout.props,p=>p.name=="Fusca");if(prop==null)return;
            var carRect=new Rect(map.x+prop.art[0]*scale,map.y+prop.art[1]*scale,prop.art[2]*scale,prop.art[3]*scale);
            if(reduced||time>.3f)
            {
                var point=ArrivalFootPosition(time,reduced);var next=ArrivalFootPosition(time+.08f,reduced);
                var delta=next-point;int row=Mathf.Abs(delta.y)>Mathf.Abs(delta.x)?(delta.y>0?3:0):(delta.x<0?1:2);
                bool walking=!reduced&&time<5.5f;
                var actor=CampaignStorySprites.Frame("WalkGray",row,walking?Mathf.FloorToInt(time*7)%4:0);
                if(actor!=null)DrawActor(actor,ScreenPoint(Image(point)),CampaignTeamEdelzio.StandingHeight/_schoolLayout.Scale*scale/actor.rect.height);
            }
            // Foreground car naturally occludes the first step out of its door.
            var sprite=CampaignOriginalFusca.Top(CampaignWorkshopVehicle.Facing.North);
            float fit=Mathf.Min(carRect.width/sprite.rect.width,carRect.height/sprite.rect.height);
            DrawSprite(new Rect(carRect.center.x-sprite.rect.width*fit/2,carRect.center.y-sprite.rect.height*fit/2,sprite.rect.width*fit,sprite.rect.height*fit),sprite);
        }
        public static IReadOnlyList<Vector2> SchoolArrivalRoute {get{EnsureArrivalRoute();return ArrivalRoute;}}
        private static void EnsureArrivalRoute()
        {
            if(_schoolLayout!=null&&ArrivalRoute.Count>0)return;
            _schoolLayout=CampaignIllustratedMaps.Get(4);if(_schoolLayout==null)return;
            var car=System.Array.Find(_schoolLayout.props,p=>p.name=="Fusca");if(car==null)return;
            var start=_schoolLayout.Position(car.art[0]-28,car.art[1]+car.art[3]*.57f);
            var plan=CampaignMapPlan.Create(4);ArrivalRoute.Clear();
            if(!plan.Route(start,plan.spawn,ArrivalRoute))return;
            // Route uses a quarter-metre navigation grid. Restore exact authored
            // endpoints so the scene handoff cannot introduce a visible jump.
            ArrivalRoute.Insert(0,start);ArrivalRoute.Add(plan.spawn);
            _arrivalLength=0;for(int i=1;i<ArrivalRoute.Count;i++)_arrivalLength+=Vector2.Distance(ArrivalRoute[i-1],ArrivalRoute[i]);
        }
        public static Vector2 ArrivalFootPosition(float time,bool reduced)
        {
            EnsureArrivalRoute();if(ArrivalRoute.Count==0)return _schoolLayout!=null?_schoolLayout.Position(_schoolLayout.spawn):Vector2.zero;
            if(!reduced&&time<.65f)
            {
                var car=System.Array.Find(_schoolLayout.props,p=>p.name=="Fusca");
                var inside=_schoolLayout.Position(car.art[0]+car.art[2]*.38f,car.art[1]+car.art[3]*.57f);
                return Vector2.Lerp(inside,ArrivalRoute[0],Mathf.SmoothStep(0,1,Mathf.Clamp01(time/.65f)));
            }
            float distance=(reduced?1:Mathf.Clamp01((time-.65f)/4.85f))*_arrivalLength;
            for(int i=1;i<ArrivalRoute.Count;i++)
            {float length=Vector2.Distance(ArrivalRoute[i-1],ArrivalRoute[i]);if(distance<=length)return Vector2.Lerp(ArrivalRoute[i-1],ArrivalRoute[i],length>0?distance/length:1);distance-=length;}
            return ArrivalRoute[^1];
        }
        private static void DrawSprite(Rect rect,Sprite sprite)
        {if(sprite==null)return;var r=sprite.rect;GUI.DrawTextureWithTexCoords(rect,sprite.texture,new Rect(r.x/sprite.texture.width,r.y/sprite.texture.height,r.width/sprite.texture.width,r.height/sprite.texture.height));}
    }
}
