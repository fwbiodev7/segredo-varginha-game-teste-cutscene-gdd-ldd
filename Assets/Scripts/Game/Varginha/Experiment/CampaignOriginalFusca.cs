using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignOriginalFusca
    {
        const string Root="Varginha/StoryEffects/FuscaOriginal";
        static readonly Dictionary<string,Sprite> Sprites=new();
        public static Sprite Side
        {
            get
            {
                if(Sprites.TryGetValue("Side",out var sprite)&&sprite!=null) return sprite;
                var texture=Resources.Load<Texture2D>(Root);
                // Measured first frame in the user's original 300x100 file. Pixels are untouched.
                sprite=Sprite.Create(texture,new Rect(1,35,99,53),Vector2.one/2,99/3.9f,0,SpriteMeshType.FullRect);
                sprite.name="Fusca_Original_Lateral";Sprites["Side"]=sprite;return sprite;
            }
        }
        public static Sprite Front => View("Front",2.05f);
        public static Sprite Top(CampaignWorkshopVehicle.Facing direction,float length=3.9f) => View("Top"+direction,length);
        static Sprite View(string name,float size)
        {
            string key=name+":"+size;
            if(Sprites.TryGetValue(key,out var sprite)&&sprite!=null) return sprite;
            var texture=Resources.Load<Texture2D>(Root+name);
            sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.one/2,Mathf.Max(texture.width,texture.height)/size,0,SpriteMeshType.FullRect);
            sprite.name="Fusca_Original_"+name;Sprites[key]=sprite;return sprite;
        }
        public static void Fit(SpriteRenderer renderer, Vector2 size)
        {
            renderer.drawMode=SpriteDrawMode.Simple;
            float scale=Mathf.Min(size.x/renderer.sprite.bounds.size.x,size.y/renderer.sprite.bounds.size.y);
            renderer.transform.localScale=Vector3.one*scale;
        }
    }
}
