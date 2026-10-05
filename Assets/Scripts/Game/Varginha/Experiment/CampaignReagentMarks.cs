using System;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // One shared GPU atlas; markings belong to the vehicle, including its track view.
    public sealed class CampaignReagentMarks : MonoBehaviour
    {
        [Serializable] private sealed class Frame { public string name; public int x,y,width,height; }
        [Serializable] private sealed class Atlas { public int sourceWidth,sourceHeight; public Frame[] frames; }
        private static readonly Sprite[] Frames = new Sprite[3];
        private readonly SpriteRenderer[] _marks = new SpriteRenderer[3];
        private readonly float[] _revealedAt = new float[3];
        private SpriteRenderer _car;
        private bool _track;
        private bool _flipped;

        public static Sprite FrameSprite(int index)
        {
            if (Frames[index] != null && Frames[index].texture != null) return Frames[index];
            var texture = Resources.Load<Texture2D>("Varginha/StoryEffects/ReagentMarks");
            var atlas = JsonUtility.FromJson<Atlas>(Resources.Load<TextAsset>("Varginha/StoryEffects/ReagentMarksAtlas").text);
            var frame = atlas.frames[index];
            float sx = texture.width/(float)atlas.sourceWidth, sy = texture.height/(float)atlas.sourceHeight;
            Frames[index] = Sprite.Create(texture,new Rect(frame.x*sx,texture.height-(frame.y+frame.height)*sy,frame.width*sx,frame.height*sy),Vector2.one/2,100,0,SpriteMeshType.FullRect);
            Frames[index].name = "Marca_"+frame.name;
            return Frames[index];
        }

        public void Reveal(int region, bool animate = true)
        {
            if (_marks[region] != null) return;
            _car = GetComponent<SpriteRenderer>();
            var go = new GameObject("Marca_"+region); go.transform.SetParent(transform,false);
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = FrameSprite(region); renderer.sharedMaterial = CampaignFlashArt.Material;
            _marks[region] = renderer;
            _revealedAt[region] = Time.time-(animate && !VarginhaGameSettings.Current.reducedMotion ? 0 : 1);
            Place(region);
        }

        public void UseTrackView()
        {
            _track = true;
            for (int i=0;i<3;i++) if (_marks[i]!=null) Place(i);
        }
        public void FaceTrack(bool west)
        {
            if(_flipped==west)return;
            _flipped=west;
            if(_track)for(int i=0;i<3;i++)if(_marks[i]!=null)Place(i);
        }

        private void Place(int region)
        {
            var mark = _marks[region];
            float height;
            if (_track)
            {
                // Reproject onto the bonnet, door and rear engine of the side sprite.
                Vector2 position = region==0 ? new Vector2(1.35f,0) : region==1 ? new Vector2(0,-.72f) : new Vector2(-1.5f,0);
                if (_flipped) position.x=-position.x;
                mark.transform.localPosition = position/(Vector2)transform.lossyScale;
                mark.flipX = false;
                height = region==1 ? .23f : .5f;
            }
            else
            {
                var layout = CampaignIllustratedMaps.Get(10);
                Vector2 pixel = region==0 ? new Vector2(779,353) : region==1 ? new Vector2(827,455) : new Vector2(779,537);
                mark.transform.position = layout.Position(pixel.x,pixel.y);
                height = region==0 ? .62f : region==1 ? .23f : .5f;
            }
            float scale = height/mark.sprite.bounds.size.y;
            var parentScale=transform.lossyScale;
            mark.transform.localScale=new Vector3(scale/parentScale.x,scale/parentScale.y,1);
        }

        private void LateUpdate()
        {
            if (_car==null) return;
            for(int i=0;i<3;i++) if(_marks[i]!=null)
            {
                _marks[i].sortingLayerID=_car.sortingLayerID;
                _marks[i].sortingOrder=_car.sortingOrder+2;
                _marks[i].color=new Color(.88f,1,.92f,Mathf.Clamp01((Time.time-_revealedAt[i])/.45f)*.92f);
            }
        }
    }
}
