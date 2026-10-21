using UnityEngine;

namespace Game.Varginha.Experiment
{
    // New daylight is confined to the glass. Original frames/furniture/walls stay intact.
    public static class CampaignAdultMorning
    {
        private static readonly float[] MorningAmbient={.98f,.97f,.91f};
        private static Material _daylight;
        public static readonly Rect[] GlassPixels={
            new(264,37,38,18),new(310,37,41,18),new(359,37,43,18),
            new(264,61,38,15),new(310,61,41,15),new(359,61,43,15),
            new(593,37,38,18),new(639,37,41,18),new(688,37,43,18),
            new(593,61,38,15),new(639,61,41,15),new(688,61,43,15),
            new(930,218,20,26),new(930,253,20,28),new(930,544,20,25),new(930,581,20,26),
            new(3,551,16,35),new(3,598,16,48)
        };
        public static void Lighting(CampaignIllustratedMaps.Layout data)
        {
            if(data?.phase!=2||ReferenceEquals(data.ambient,MorningAmbient))return;
            data.ambient=MorningAmbient;
            if(data.lights!=null)foreach(var light in data.lights)
                if(light.color?.Length>=3&&light.color[2]>light.color[0])light.color=new[]{1f,.97f,.79f};
        }
        public static void Build(Transform map,CampaignIllustratedMaps.Layout data)
        {
            if(data.phase!=2||map.Find("Manha_07h")!=null)return;
            var texture=Resources.Load<Texture2D>("Varginha/CinemaOpening/MorningTown");if(texture==null)return;
            var shader=Resources.Load<Shader>("Varginha/CinemaOpening/IllustratedMorning");
            if(shader!=null)
            {
                if(_daylight==null)_daylight=new Material(shader){name="Luz da manhã 07h",hideFlags=HideFlags.DontSave};
                foreach(var painted in map.GetComponentsInChildren<SpriteRenderer>())painted.sharedMaterial=_daylight;
            }
            var root=new GameObject("Manha_07h").transform;root.SetParent(map,false);
            var lifetime=root.gameObject.AddComponent<CampaignMorningGlassLifetime>();
            foreach(var rect in GlassPixels)
            {
                Rect window=rect.y<100?(rect.x<500?new Rect(264,37,139,39):new Rect(593,37,139,39)):
                    rect.x<50?new Rect(3,551,16,95):rect.y<400?new Rect(930,218,20,63):new Rect(930,544,20,63);
                float height=texture.height*.2f,width=height*window.width/window.height;
                // Adjacent panes crop ONE continuous view rather than repeating tiny panoramas.
                var crop=new Rect(texture.width*.48f-width/2+(rect.x-window.x)/window.width*width,
                    texture.height*.7f+(window.yMax-rect.yMax)/window.height*height,
                    width*rect.width/window.width,height*rect.height/window.height);
                var sprite=Sprite.Create(texture,crop,Vector2.one*.5f,100,0,SpriteMeshType.FullRect);
                lifetime.sprites.Add(sprite);
                var go=new GameObject("Vidro_diurno_"+rect.x+"_"+rect.y);go.transform.SetParent(root,false);
                var area=data.Area(new[]{rect.x,rect.y,rect.width,rect.height});go.transform.position=area.center;
                var sr=go.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.sortingOrder=23000;
                go.transform.localScale=new Vector3(area.width/sprite.bounds.size.x,area.height/sprite.bounds.size.y,1);
            }
        }
    }
    public sealed class CampaignMorningGlassLifetime:MonoBehaviour
    {
        public readonly System.Collections.Generic.List<Sprite> sprites=new();
        private void OnDestroy(){foreach(var sprite in sprites)if(sprite!=null)Destroy(sprite);}
    }
}
