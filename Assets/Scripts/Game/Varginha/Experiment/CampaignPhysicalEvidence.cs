using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // One catalog binds physical evidence, its authored support and the existing stable interaction ID.
    public sealed class CampaignPhysicalEvidence:MonoBehaviour
    {
        [Serializable]public sealed class Catalog{public Art[] sprites;public Entry[] entries;}
        [Serializable]public sealed class Art{public string id,resource;public float[] rectUnity,pivot;}
        [Serializable]public sealed class Support{public string name;public int depthOffset;}
        [Serializable]public sealed class Entry{public int phase,area,layoutId;public string id,spriteId,mode;public float[] visualPixel,worldSize,suggestedInteractionPixel;public Support support;}
        sealed class Item{public string id;public SpriteRenderer sprite;public Collider2D collider;public Vector2 approach;public Color original;}
        static Catalog _catalog;
        static readonly Dictionary<string,Sprite> Sprites=new();
        static Material _crisp;
        readonly List<Item> _items=new();
        int _phase;
        static Catalog Data=>_catalog??=JsonUtility.FromJson<Catalog>(Resources.Load<TextAsset>("Varginha/Revision20261028/InvestigationEvidenceCatalogV1").text);
        public static void ApplyPoints(CampaignMapPlan plan,CampaignIllustratedMaps.Layout layout)
        {
            foreach(var e in Data.entries)
                if(e.layoutId==plan.phase&&e.suggestedInteractionPixel?.Length==2)
                {var point=plan.points.Find(p=>p.id==e.id);if(point!=null)point.position=layout.Position(e.suggestedInteractionPixel);}
        }
        public static void Build(Transform map,CampaignMapPlan plan,CampaignIllustratedMaps.Layout layout)
        {
            if(plan.phase!=6&&plan.phase!=7&&plan.phase!=8&&plan.phase!=12&&plan.phase!=112&&plan.phase!=13)return;
            var owner=new GameObject("Evidencias_fisicas");owner.transform.SetParent(map,false);
            var binder=owner.AddComponent<CampaignPhysicalEvidence>();binder._phase=plan.phase;
            foreach(var e in Data.entries)
            {
                if(e.layoutId!=plan.phase||e.mode!="overlay")continue; // Already painted book/globe/notice stay canonical.
                var art=Array.Find(Data.sprites,a=>a.id==e.spriteId);
                if(!Sprites.TryGetValue(art.id,out var sprite))
                {
                    var tex=Resources.Load<Texture2D>(art.resource);var r=art.rectUnity;
                    sprite=Sprite.Create(tex,new Rect(r[0],r[1],r[2],r[3]),new Vector2(art.pivot[0],art.pivot[1]),100,0,SpriteMeshType.FullRect);Sprites[art.id]=sprite;
                }
                Transform support=map.Find("02_Mobilia_Colisoes/"+e.support.name);
                binder.Add(e.id,sprite,layout.Position(e.visualPixel),new Vector2(e.worldSize[0],e.worldSize[1]),plan.points.Find(p=>p.id==e.id).position,support,e.support.depthOffset);
            }
            if(plan.phase==7)
            {
                var tex=Resources.Load<Texture2D>("Varginha/Revision20261028/ForestEvidenceV1");
                for(int i=0;i<3;i++)
                {
                    string id="sign"+i,key="forest-"+i;
                    if(!Sprites.TryGetValue(key,out var sprite)){sprite=Sprite.Create(tex,new Rect(i*724,0,724,724),Vector2.one/2,384,0,SpriteMeshType.FullRect);Sprites[key]=sprite;}
                    var point=plan.points.Find(p=>p.id==id).position;
                    binder.Add(id,sprite,point,Vector2.one*(i==1?1.25f:1.05f),point,null,1);
                }
            }
        }
        void Add(string id,Sprite sprite,Vector2 position,Vector2 size,Vector2 approach,Transform support,int depth)
        {
            var go=new GameObject("Evidencia_"+id);go.transform.SetParent(transform,false);go.transform.position=position;
            float fit=Mathf.Min(size.x/sprite.bounds.size.x,size.y/sprite.bounds.size.y);go.transform.localScale=Vector3.one*fit;
            var renderer=go.AddComponent<SpriteRenderer>();renderer.sprite=sprite;
            _crisp??=new Material(Resources.Load<Shader>("Varginha/Flash1996/CrispPixelSprite"));renderer.sharedMaterial=_crisp;
            var trigger=go.AddComponent<BoxCollider2D>();trigger.isTrigger=true;trigger.size=sprite.bounds.size;
            if(support!=null)
            {
                var contact=new GameObject("Apoio_"+id).transform;contact.SetParent(transform,false);
                var floor=support.GetComponent<Collider2D>();contact.position=floor!=null?new Vector2(floor.bounds.center.x,floor.bounds.min.y):support.position;
                VarginhaWorldDepth.Ensure(renderer,supportingObject:contact,offset:depth);
            }
            else VarginhaWorldDepth.Ensure(renderer,supportingObject:go.transform,offset:depth);
            _items.Add(new Item{id=id,sprite=renderer,collider=trigger,approach=approach,original=renderer.color});
        }
        void LateUpdate()
        {
            var expansion=CampaignExpansionController.Active;var continuation=CampaignContinuationController.Active;
            var story=_phase<11?expansion?.Progress:continuation?.Progress;if(story==null)return;
            var actor=_phase<11?EdelzioTopDownController.Instance:continuation.Actor;if(actor==null)return;
            Vector2 feet=(Vector2)actor.transform.position-Vector2.up*.58f;
            foreach(var item in _items)
            {
                bool visible=!(_phase==112&&item.id=="key"&&story.continuation.serviceKey);
                item.sprite.enabled=visible;item.collider.enabled=visible;
                bool available=visible&&!actor.IsInputLocked&&Vector2.Distance(feet,item.approach)<1.45f;
                // Only actual evidence receives this small proximity lift, never surrounding scenery.
                item.sprite.color=available?new Color(1,.96f,.82f):item.original;
            }
        }
    }
}
