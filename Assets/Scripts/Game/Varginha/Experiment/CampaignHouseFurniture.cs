using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    /// <summary>Rear-facing variants replace only the approved furniture crops; colliders stay authored.</summary>
    public static class CampaignHouseFurniture
    {
        [Serializable] private sealed class Atlas { public Entry[] frames; }
        [Serializable] private sealed class Entry { public string name; public int[] rect; }
        private static readonly Dictionary<string,Sprite> Sprites=new();
        public static Sprite Frame(string name,string atlasName="HouseFurnitureBacks")
        {
            string key=atlasName+":"+name;
            if(Sprites.TryGetValue(key,out var sprite)&&sprite!=null)return sprite;
            var texture=Resources.Load<Texture2D>("Varginha/HouseFurniture/"+atlasName);
            var json=Resources.Load<TextAsset>("Varginha/HouseFurniture/"+atlasName);
            if(texture==null||json==null)return null;
            foreach(var f in JsonUtility.FromJson<Atlas>(json.text).frames)
                if(f.name==name)
                {
                    sprite=Sprite.Create(texture,new Rect(f.rect[0],texture.height-f.rect[1]-f.rect[3],f.rect[2],f.rect[3]),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);
                    sprite.name="HouseFurniture_"+name;Sprites[key]=sprite;return sprite;
                }
            return null;
        }
        public static void Apply(Transform map,CampaignIllustratedMaps.Layout data)
        {
            Replace(map,data,"Fridge_Kitchen","FridgeBack",new[]{850f,631,69,120});
            Replace(map,data,"Stove_Kitchen","StoveBack",new[]{489f,674,72,76});
            Replace(map,data,"Kitchen_Cabinet","SinkBack",new[]{564f,685,84,67});
            Replace(map,data,"Bookshelf_Office","BookshelfBack",new[]{32f,670,198,81});
            Replace(map,data,"Dresser_Bedroom","DresserBack",new[]{27f,286,94,105});
            Replace(map,data,"Desk_Office","DeskEmpty",new[]{28f,432,92,73});
            Replace(map,data,"CoffeeTable_Kitchen","CoffeeTableEmpty",new[]{649f,502,114,84});
            var lamp=Item(map,data,"Lampada_da_mesa","DeskLamp",new[]{32f,418,27,38});
            VarginhaWorldDepth.Ensure(lamp,supportingObject:map.Find("02_Mobilia_Colisoes/Desk_Office"),offset:3);
            var emptyCup=Item(map,data,"Xicara_vazia_escritorio","CoffeeEmpty",new[]{105f,441,14,16});
            VarginhaWorldDepth.Ensure(emptyCup,supportingObject:map.Find("02_Mobilia_Colisoes/Desk_Office"),offset:4);
        }
        private static void Replace(Transform map,CampaignIllustratedMaps.Layout data,string objectName,string frame,float[] target)
        {
            var sprite=Frame(frame);var node=map.Find("02_Mobilia_Colisoes/"+objectName);if(sprite==null||node==null)return;
            var original=node.GetComponent<SpriteRenderer>();original.enabled=false;
            // Visual dimensions belong to a child; the authored collider's transform is immutable.
            var visual=new GameObject("Vista_"+frame).transform;visual.SetParent(node,false);
            var renderer=visual.gameObject.AddComponent<SpriteRenderer>();renderer.sprite=sprite;
            Rect area=data.Area(target);visual.position=area.center;
            visual.localScale=new Vector3(area.width/sprite.bounds.size.x/node.lossyScale.x,area.height/sprite.bounds.size.y/node.lossyScale.y,1);
            VarginhaWorldDepth.Ensure(renderer,supportingObject:node,offset:0);
        }
        public static SpriteRenderer Item(Transform parent,CampaignIllustratedMaps.Layout data,string name,string frame,float[] target)
        {
            var sprite=Frame(frame,"HouseRoutineItems");
            var go=new GameObject(name);go.transform.SetParent(parent,false);var renderer=go.AddComponent<SpriteRenderer>();renderer.sprite=sprite;
            Rect area=data.Area(target);go.transform.position=area.center;
            go.transform.localScale=new Vector3(area.width/sprite.bounds.size.x,area.height/sprite.bounds.size.y,1);
            return renderer;
        }
    }
}
