using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Furniture uses feet; solid walls also stop the torso before it enters a painted pillar.
    [DefaultExecutionOrder(-100)]
    public sealed class CampaignWallBody:MonoBehaviour
    {
        public const int WallLayer=30;
        private EdelzioTopDownController _actor;
        private CapsuleCollider2D _body;
        public static bool SolidWalls(int phase)=>phase==1||phase==2;
        public static bool Guards(int phase,Rect wall,Rect bounds)=>SolidWalls(phase)&&wall.height>wall.width*1.3f&&wall.xMin>bounds.xMin+.15f&&wall.xMax<bounds.xMax-.15f;
        public static float Height(bool child)=>child?1.15f:CampaignTeamEdelzio.CurrentStandingHeight+.08f;
        public static void Ensure(EdelzioTopDownController actor,bool child)
        {
            var component=actor.GetComponent<CampaignWallBody>()??actor.gameObject.AddComponent<CampaignWallBody>();
            component._actor=actor;
            component._body ??=actor.gameObject.AddComponent<CapsuleCollider2D>();
            var feet=actor.GetComponent<CircleCollider2D>();
            float scaleX=Mathf.Max(.01f,Mathf.Abs(actor.transform.lossyScale.x)),scaleY=Mathf.Max(.01f,Mathf.Abs(actor.transform.lossyScale.y));
            float height=Height(child)/scaleY;
            component._body.direction=CapsuleDirection2D.Vertical;
            component._body.size=new Vector2((child?.44f:.46f)/scaleX,height);
            component._body.offset=feet.offset+Vector2.up*(height/2+.03f/scaleY);
            component._body.includeLayers=1<<WallLayer;component._body.excludeLayers=~(1<<WallLayer);
            component._body.sharedMaterial=feet.sharedMaterial;
        }
        private void FixedUpdate(){if(_body!=null)_body.enabled=!_actor.IsScriptedMotion;}
    }
}
