using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Upper bodies occupy the gap between tabletop and backrest. All source pixels stay authored.
    [DefaultExecutionOrder(11000)]
    public sealed class CampaignSeatingLayers : MonoBehaviour
    {
        private static readonly Dictionary<string,Sprite> StudentPoses = new();
        private SpriteRenderer _actor, _chair, _back;
        private VarginhaPlayerSpriteAnimation _action;
        private bool _always;
        private bool _chairBehind;
        private string _student;
        public bool IsOfficeSeat => _chair!=null&&_chair.name=="Office_Chair";
        public Collider2D SeatCollider => _chair!=null?_chair.GetComponent<Collider2D>():null;
        public static Sprite StudentPose(string name)
        {
            if (StudentPoses.TryGetValue(name,out var pose) && pose != null) return pose;
            var original=VarginhaStudentSprites.Frame(name,3,0); if(original==null)return null;
            var rect=original.rect; float trim=rect.height*.28f;
            rect.y+=trim;rect.height-=trim;
            // Crop the legs hidden by the chair and lift the original torso into the sitting position.
            pose=Sprite.Create(original.texture,rect,new Vector2(original.pivot.x/rect.width,
                (original.pivot.y-trim-original.pixelsPerUnit*.86f)/rect.height),original.pixelsPerUnit,0,SpriteMeshType.FullRect);
            pose.name="StudentSeated_"+name; StudentPoses[name]=pose;return pose;
        }
        public static void Attach(SpriteRenderer actor,SpriteRenderer chair,bool always=false,float lower=.59f,float upper=1,float margin=.09f)
        {
            if(actor==null||chair==null)return;
            var layer=actor.GetComponent<CampaignSeatingLayers>()??actor.gameObject.AddComponent<CampaignSeatingLayers>();
            layer._actor=actor;layer._chair=chair;layer._always=always;layer._action=actor.GetComponent<VarginhaPlayerSpriteAnimation>();
            if(always)
            {
                layer._student=actor.GetComponent<CampaignSchoolLife>()?.StudentName
                    ?? (actor.name.StartsWith("Refem_")?actor.name.Substring(6).Replace("_"," "):actor.name);
                layer.AlignStudent();
            }
            if(layer._back!=null)return;
            var s=chair.sprite;var rect=s.rect;
            float inset=rect.width*margin,trim=rect.height*lower,height=rect.height*(upper-lower);
            rect.x+=inset;rect.width-=inset*2;rect.y+=trim;rect.height=height;
            var sprite=Sprite.Create(s.texture,rect,new Vector2((s.pivot.x-inset)/rect.width,(s.pivot.y-trim)/rect.height),s.pixelsPerUnit,0,SpriteMeshType.FullRect);
            var go=new GameObject("Encosto_"+actor.name);go.transform.SetParent(chair.transform.parent,false);
            layer._back=go.AddComponent<SpriteRenderer>();layer._back.sprite=sprite;layer._back.enabled=false;
            VarginhaWorldDepth.Ensure(layer._back,supportingObject:actor.transform,offset:3);
        }
        private void LateUpdate()
        {
            if(_back==null||_chair==null)return;
            if(_always)AlignStudent();
            _back.enabled=_actor.enabled&&(_always||_action?.IsSeated==true&&_action.ActionFacingDirection.y>0);
            bool behind=IsOfficeSeat&&_back.enabled;
            if(behind!=_chairBehind)
            {
                _chairBehind=behind;
                VarginhaWorldDepth.Ensure(_chair,ground:_chair.GetComponent<Collider2D>(),supportingObject:behind?_actor.transform:null,offset:behind?-3:0);
            }
            _back.transform.SetPositionAndRotation(_chair.transform.position,_chair.transform.rotation);
            _back.transform.localScale=_chair.transform.lossyScale;
        }
        private void AlignStudent()
        {
            var pose=StudentPose(_student);if(pose==null||_actor==null||_chair==null)return;
            _actor.sprite=pose;
            // Anchor the cropped torso to the seat itself, not an arbitrary point
            // below the chair. The same calculation serves the school and epilogue.
            var seat=_chair.bounds;
            Vector2 position=new(seat.center.x,seat.center.y+seat.size.y*.05f-pose.bounds.min.y*_actor.transform.lossyScale.y);
            _actor.transform.position=new Vector3(position.x,position.y,_actor.transform.position.z);
            var body=_actor.GetComponent<Rigidbody2D>();if(body!=null){body.position=position;body.linearVelocity=Vector2.zero;}
            var feet=_actor.GetComponent<CircleCollider2D>();
            if(feet!=null)
            {
                float ground=SeatCollider!=null?SeatCollider.bounds.min.y-feet.radius-.025f:seat.min.y+.08f;
                feet.offset=new Vector2(0,(ground-position.y)/Mathf.Max(.01f,_actor.transform.lossyScale.y));
            }
            var life=_actor.GetComponent<CampaignSchoolLife>();if(life!=null)life.Home=position;
            VarginhaWorldDepth.Ensure(_actor,ground:feet,supportingObject:_chair.transform,offset:1);
        }
        private void OnDestroy(){if(_back!=null){Destroy(_back.sprite);Destroy(_back.gameObject);}}
    }
}
