using UnityEngine;

namespace Game.Varginha.Experiment
{
    // A single authored burst, followed by knockback, head contact and stillness.
    // Gameplay and the existing child walking sprites stay separate from this action.
    public sealed class CampaignFlashEncounter:MonoBehaviour
    {
        private const float Launch=.35f,Flight=.78f;
        private static readonly float[] PoseCues={0,.35f,.5f,.67f,1f,1.13f,1.28f,1.46f};
        private EdelzioTopDownController _actor;
        private Rigidbody2D _body;
        private SpriteRenderer _child,_blast;
        private Vector2 _start,_end;
        private float _time;
        private bool _landed;
        private bool _burstPlayed;
        private CampaignSoundscape _sound;
        public bool HasHitHead=>_landed;
        public bool IsLying=>_time>=1.46f;
        public float Elapsed=>_time;
        public SpriteRenderer Burst=>_blast;
        public void Begin(EdelzioTopDownController actor,SpriteRenderer child,CampaignSoundscape sound)
        {
            _actor=actor;_child=child;_sound=sound;
            _body=actor.GetComponent<Rigidbody2D>();_start=_body.position;
            var plan=CampaignMapPlan.Create(1);
            _end=_start+Vector2.left*2.2f;
            // Keep the landing in the yard, even if the player approaches at an angle.
            for(int i=0;i<22&&!plan.IsClear(_end,.22f);i++)_end=Vector2.MoveTowards(_end,_start,.1f);
            _actor.SetInputLocked(true);_actor.IsScriptedMotion=true;_body.linearVelocity=Vector2.zero;
            _child.flipX=false;_child.sharedMaterial=CampaignFlashArt.Material;
            _child.sprite=CampaignFlashArt.Fall[0];
            var go=new GameObject("Explosão_Alienígena_PixelArt");go.transform.SetParent(transform,false);
            go.transform.position=new Vector3(17.2f,_start.y+.45f,0);
            _blast=go.AddComponent<SpriteRenderer>();_blast.sharedMaterial=CampaignFlashArt.Material;
            _blast.sprite=CampaignFlashArt.Explosion[0];_blast.sortingOrder=26000;
        }
        private void Update()
        {
            if(_actor==null||Time.timeScale<=0)return;
            _time+=Time.deltaTime;
            if(!_burstPlayed&&_time>=Launch){_burstPlayed=true;_sound?.Play("AlienBurst");}
            int pose=0;for(int i=1;i<PoseCues.Length;i++)if(_time>=PoseCues[i])pose=i;
            _child.sprite=CampaignFlashArt.Fall[pose];
            // Hold the small core briefly; expand once, then disperse the ring.
            int frame=Mathf.Clamp(Mathf.FloorToInt((_time-.12f)*10),0,11);
            _blast.sprite=CampaignFlashArt.Explosion[frame];
            _blast.enabled=_time<1.8f;
            _blast.color=new Color(1,1,1,_time<1.2f?1:Mathf.Clamp01((1.8f-_time)/.6f));
            if(!_landed&&_time>=Launch+Flight)
            {
                _landed=true;_sound?.Play("Impact");
                _body.linearVelocity=Vector2.zero;
            }
        }
        private void FixedUpdate()
        {
            if(_body==null||Time.timeScale<=0)return;
            float t=Mathf.Clamp01((_time-Launch)/Flight);
            float travel=1-Mathf.Pow(1-t,2);
            var position=Vector2.Lerp(_start,_end,travel)+Vector2.up*(Mathf.Sin(t*Mathf.PI)*.32f);
            _body.MovePosition(position);
        }
        private void OnDestroy()
        {
            if(_actor!=null)_actor.IsScriptedMotion=false;
            if(_body!=null)_body.linearVelocity=Vector2.zero;
        }
    }
}
