using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Each theme has its own production sheet; opaque outlines determine the slices.
    public static class CampaignAtmosphereAssets
    {
        private static readonly string[][] Names={
            new[]{"GothicWindow","Lectern","StonePillar","Sarcophagus","Torch","HolyBasin","SealedArchive","Gravestone"},
            new[]{"MicroscopeBench","SpecimenTank","SampleCart","ScientificFreezer","DNABoard","Terrarium","DecontaminationSink","ScienceCabinet"},
            new[]{"CityHouse","CityTownhouse","CityBakery","CityMarket","BusStop","WaterTower","CityPlaza","CitySigns"},
            new[]{"ChildhoodToys","DiningTable","ArchiveShelf","TechnicalBoard","AncientTree","ChapelArch","MechanicBench","ForestCluster"},
            new[]{"CityHouseEast","CityTownhouseEast","CityBakeryEast","CityMarketEast","CityHouseWest","CityTownhouseWest","CityBakeryWest","CityMarketWest"}};
        private static readonly string[] Sheets={"Church","Biology","City","Identity","CityDirectional"};
        private static readonly Dictionary<string,Sprite> Sprites=new();
        private static readonly Dictionary<Texture2D,Rect[]> Bounds=new();
        public static Sprite Prop(string motif)
        {
            if(Sprites.TryGetValue(motif,out var sprite)&&sprite!=null)return sprite;
            if(motif=="BusStop")
            {
                var shelter=Resources.Load<Texture2D>("Varginha/WorldArtV3/BusStopVarginha");
                if(shelter==null)return null;
                var pixels=shelter.GetPixels32();int left=shelter.width,right=0,bottom=shelter.height,top=0;
                for(int i=0;i<pixels.Length;i++)if(pixels[i].a>=100){int x=i%shelter.width,y=i/shelter.width;left=Mathf.Min(left,x);right=Mathf.Max(right,x);bottom=Mathf.Min(bottom,y);top=Mathf.Max(top,y);}
                sprite=Sprite.Create(shelter,new Rect(left,bottom,right-left+1,top-bottom+1),Vector2.one/2,64,0,SpriteMeshType.FullRect);
                sprite.name="WorldArtV3_BusStop_Varginha_Real";Sprites[motif]=sprite;return sprite;
            }
            for(int sheet=0;sheet<Sheets.Length;sheet++)
            {
                int index=System.Array.IndexOf(Names[sheet],motif);if(index<0)continue;
                var texture=Resources.Load<Texture2D>("Varginha/WorldArtV3/"+Sheets[sheet]);if(texture==null)return null;
                var rect=Outlines(texture)[index];if(rect.width<=0)return null;
                sprite=Sprite.Create(texture,rect,Vector2.one/2,64,0,SpriteMeshType.FullRect);
                sprite.name="WorldArtV3_"+motif;Sprites[motif]=sprite;return sprite;
            }
            return null;
        }
        private static Rect[] Outlines(Texture2D texture)
        {
            if(Bounds.TryGetValue(texture,out var cached))return cached;
            var pixels=texture.GetPixels32();var seen=new bool[pixels.Length];var queue=new int[pixels.Length];var rects=new Rect[8];
            int width=texture.width,height=texture.height;
            for(int seed=0;seed<pixels.Length;seed++)
            {
                if(seen[seed]||pixels[seed].a<100)continue;
                int head=0,tail=1;queue[0]=seed;seen[seed]=true;
                int left=width,right=0,bottom=height,top=0;
                while(head<tail)
                {
                    int p=queue[head++],x=p%width,y=p/width;
                    left=Mathf.Min(left,x);right=Mathf.Max(right,x);bottom=Mathf.Min(bottom,y);top=Mathf.Max(top,y);
                    void Add(int next){if(!seen[next]&&pixels[next].a>=100){seen[next]=true;queue[tail++]=next;}}
                    if(x>0)Add(p-1);if(x+1<width)Add(p+1);if(y>0)Add(p-width);if(y+1<height)Add(p+width);
                }
                if(tail<50)continue;
                int slot=(1-Mathf.Clamp((bottom+top)/(height),0,1))*4+Mathf.Clamp((left+right)*2/width,0,3);
                var rect=new Rect(left,bottom,right-left+1,top-bottom+1);
                if(rects[slot].width>0){var previous=rects[slot];rect=Rect.MinMaxRect(Mathf.Min(previous.xMin,rect.xMin),Mathf.Min(previous.yMin,rect.yMin),Mathf.Max(previous.xMax,rect.xMax),Mathf.Max(previous.yMax,rect.yMax));}
                rects[slot]=rect;
            }
            Bounds[texture]=rects;return rects;
        }
    }
}
