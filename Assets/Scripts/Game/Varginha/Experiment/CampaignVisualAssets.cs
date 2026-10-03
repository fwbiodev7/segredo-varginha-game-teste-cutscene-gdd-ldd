using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignVisualAssets
    {
        private static readonly Dictionary<string,Sprite> Cache=new();
        private static readonly string[] Details={"Toilet","Shower","Basin","Towels","Rug","PottedPlant","Window","Roof"};
        private static readonly Rect[] DetailRects={new(130,100,182,302),new(525,60,327,352),new(993,137,223,238),new(1377,140,345,251),new(44,506,416,287),new(560,495,304,299),new(943,494,319,298),new(1389,428,325,417)};
        private static readonly string[] Furniture={"Desk","Chair","Sofa","CoffeeTable","Bookshelf","Nightstand","Dresser","Kitchen","Bed","SchoolDesk","Pew","Altar","TV","Stove","Fridge","Lamp","Noticeboard","Door","Tree","Shrub","LabBench","Fusca","Workbench","ToolRack"};
        // Measured source rectangles: the generated sheet is not an exact equal-cell grid.
        private static readonly Rect[] FurnitureRects={
            new(44,79,222,174),new(316,71,154,186),new(522,66,238,187),new(788,114,238,139),new(1071,47,196,210),new(1337,98,142,151),
            new(56,288,189,196),new(289,285,204,196),new(536,285,207,198),new(770,285,248,203),new(1043,308,250,176),new(1319,307,185,177),
            new(57,528,194,213),new(311,542,154,200),new(561,515,139,226),new(843,488,69,250),new(1039,554,216,154),new(1337,526,138,202),
            new(27,749,242,248),new(296,830,191,146),new(522,789,241,191),new(797,767,190,213),new(1020,784,270,196),new(1318,751,186,234)};
        private static Sprite Cell(string sheet,int index,int columns,int rows,float ppu)
        {
            string key=sheet+index+":"+ppu;
            if(Cache.TryGetValue(key,out var sprite)&&sprite!=null)return sprite;
            var image=Resources.Load<Texture2D>("Varginha/WorldArtV2/"+sheet);if(image==null)return null;
            int w=image.width/columns,h=image.height/rows;
            int padding=sheet=="Architecture"?3:0;
            Rect rect=new(index%columns*w+padding,(rows-1-index/columns)*h+padding,w-padding*2,h-padding*2);
            if(sheet=="Furniture") { var measured=FurnitureRects[index];rect=new Rect(measured.x,image.height-measured.yMax,measured.width,measured.height); }
            if(sheet=="Details") { var measured=DetailRects[index];rect=new Rect(measured.x,image.height-measured.yMax,measured.width,measured.height); }
            sprite=Sprite.Create(image,rect,Vector2.one/2,ppu-padding*2,0,SpriteMeshType.FullRect);
            sprite.name="WorldArtV2_"+sheet+"_"+index;Cache[key]=sprite;return sprite;
        }
        public static Sprite Floor(string motif)
        {
            if(motif=="GarageFloor")
            {
                if(Cache.TryGetValue(motif,out var concrete)&&concrete!=null)return concrete;
                var source=Resources.Load<Texture2D>("Varginha/WorldArtV2/Architecture");if(source==null)return null;
                int unit=source.width/4;concrete=Sprite.Create(source,new Rect(unit+3,unit/3,unit-6,unit*2/3-3),Vector2.one/2,unit-6,0,SpriteMeshType.FullRect);Cache[motif]=concrete;return concrete;
            }
            if(motif=="RoadMarking")
            {
                if(Cache.TryGetValue(motif,out var line)&&line!=null)return line;
                var paint=new Texture2D(2,2,TextureFormat.RGBA32,false){filterMode=FilterMode.Point};
                var color=new Color(.88f,.86f,.71f);paint.SetPixels(new[]{color,color,color,color});paint.Apply();
                line=Sprite.Create(paint,new Rect(0,0,2,2),Vector2.one/2,2,0,SpriteMeshType.FullRect);Cache[motif]=line;return line;
            }
            int index=motif=="Floor_House"?0:motif=="SchoolFloor"?1:motif=="ChurchFloor"?2:motif=="Floor_Yard"?3
                :motif=="Dirt"?4:motif.StartsWith("Street")?5:motif=="Water"?6:motif=="LabFloor"?7:motif.StartsWith("Driveway")||motif=="Path"?14:motif=="Bridge"?15:1;
            var image=Resources.Load<Texture2D>("Varginha/WorldArtV2/Architecture");return image==null?null:Cell("Architecture",index,4,4,image.width/4);
        }
        public static Sprite Wall(int phase)
        {
            int index=phase==1?8:phase==4||phase==5?9:phase==6?8:phase==7?11:phase==8?10:phase==9?12:13;
            var image=Resources.Load<Texture2D>("Varginha/WorldArtV2/Architecture");return image==null?null:Cell("Architecture",index,4,4,image.width/4);
        }
        public static Sprite Prop(string motif)
        {
            var atmospheric=CampaignAtmosphereAssets.Prop(motif);if(atmospheric!=null)return atmospheric;
            if(motif.StartsWith("Original"))return OriginalSchool(motif);
            int detail=System.Array.IndexOf(Details,motif);
            if(detail>=0)return Cell("Details",detail,4,2,256);
            int index=System.Array.IndexOf(Furniture,motif);if(index<0)return null;
            var image=Resources.Load<Texture2D>("Varginha/WorldArtV2/Furniture");return image==null?null:Cell("Furniture",index,6,4,image.width/6);
        }
        private static Sprite OriginalSchool(string motif)
        {
            if(Cache.TryGetValue(motif,out var sprite)&&sprite!=null)return sprite;
            bool chair=motif=="OriginalChair";
            var source=Resources.Load<Texture2D>("Varginha/WorldArtV2/"+(chair?"OriginalSchoolChairRear":"OriginalSchoolComputerLab"));
            if(source==null)return null;
            Rect top=motif switch {
                "OriginalDesk"=>new Rect(10,195,435,240),"OriginalComputerDesk"=>new Rect(480,95,425,340),
                "OriginalWhiteboard"=>new Rect(1305,120,459,260),"OriginalWindow"=>new Rect(5,525,555,275),
                "OriginalShelf"=>new Rect(605,480,225,365),"OriginalProjector"=>new Rect(995,525,210,190),
                "OriginalTeacherDesk"=>new Rect(1275,610,489,230),_=>new Rect(0,0,source.width,source.height)};
            sprite=Sprite.Create(source,new Rect(top.x,source.height-top.yMax,top.width,top.height),Vector2.one/2,32,0,SpriteMeshType.FullRect);
            sprite.name="WorldArtV2_"+motif;Cache[motif]=sprite;return sprite;
        }
        public static void Clear() => Cache.Clear();
    }
}
