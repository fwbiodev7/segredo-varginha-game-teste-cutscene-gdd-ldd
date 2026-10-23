using UnityEngine;

namespace Game.Varginha.Experiment
{
    // All playable Fusca routes use the same separating-contact rule.
    public static class CampaignVehiclePhysics
    {
        public static float Speed(float current,float throttle,float forward,float reverse,float acceleration,float coast,float brake,float dt)
        {
            float target=throttle*(throttle>=0?forward:reverse);
            float rate=Mathf.Abs(throttle)<.01f?coast:current*throttle<-.01f?brake:acceleration;
            return Mathf.MoveTowards(current,target,Mathf.Max(.1f,rate)*dt);
        }
        public static float ClearDistance(Rigidbody2D body,BoxCollider2D box,float yaw,Vector2 delta,RaycastHit2D[] hits)
        {
            if(delta.sqrMagnitude<.000001f)return 0;
            var filter=new ContactFilter2D{useTriggers=false};filter.SetLayerMask(~0);
            int count=Physics2D.BoxCast(body.position,box.size,yaw,delta.normalized,filter,hits,delta.magnitude+.035f);
            float distance=delta.magnitude;
            for(int i=0;i<count;i++)
            {
                var hit=hits[i];if(hit.collider==null||hit.collider.attachedRigidbody==body)continue;
                if(hit.distance<=.001f)
                {
                    var contact=box.Distance(hit.collider);
                    if(contact.distance<=.002f&&Vector2.Dot(contact.normal,delta)<=0)continue;
                }
                if(Vector2.Dot(hit.normal,delta)>=0)continue;
                distance=Mathf.Min(distance,Mathf.Max(0,hit.distance-.035f));
            }
            return distance;
        }
    }
}
