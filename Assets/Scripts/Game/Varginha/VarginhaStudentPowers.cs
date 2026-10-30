using System.Collections;
using Game.Player;
using Game.UI;
using UnityEngine;
using Game.Varginha.Experiment;
namespace Game.Varginha
{
    public sealed class VarginhaSleepStatus : MonoBehaviour
    {
        public float Remaining { get; private set; }
        public bool IsSleeping => Remaining > 0 && GetComponent<VarginhaCombatTarget>()?.IsDead != true;
        public static bool Apply(VarginhaCombatTarget target, float seconds=3)
        {
            if(target==null||target.IsDead||target.Kind==VarginhaCombatTarget.EnemyKind.AncestralEntity)return false;
            var boss=target.GetComponent<CampaignManifestationCombat>();
            if(boss!=null&&!boss.IsMinor)return false;
            var status=target.GetComponent<VarginhaSleepStatus>()??target.gameObject.AddComponent<VarginhaSleepStatus>();
            status.Remaining=Mathf.Max(status.Remaining,Mathf.Clamp(seconds,.1f,4));
            target.GetComponent<VarginhaCombatEnemy>()?.ApplyAllyControl(Vector2.zero,0,0);
            boss?.Stagger(seconds);return true;
        }
        void Update(){if(Time.timeScale>0&&CampaignContinuationController.Active?.Modal!=true)Remaining=Mathf.Max(0,Remaining-Time.deltaTime);}
        void OnDisable(){Remaining=0;}
        void OnGUI()
        {
            if(!IsSleeping||Camera.main==null||VarginhaGameHUD.Instance?.IsInventoryOpen==true)return;
            var p=Camera.main.WorldToScreenPoint(transform.position+Vector3.up*1.1f);if(p.z<=0)return;
            for(int i=0;i<3;i++)PixelMenuTheme.Label(new Rect(p.x-35+i*22,Screen.height-p.y-20-Mathf.Repeat(Time.time*18+i*10,35),45,25),i==1?"zZ":"Z",12,new Color(.65f,.8f,1));
        }
    }
    public sealed class VarginhaArcaneShield : MonoBehaviour
    {
        public float Remaining {get;private set;}
        public Transform Caster {get;private set;}
        public bool Active=>Remaining>0&&isActiveAndEnabled&&GetComponent<HealthSystem>()?.IsDead!=true;
        public Vector2 Centre=>Caster!=null?(Vector2)Caster.position:(Vector2)transform.position;
        SpriteRenderer visual;float pulse;
        public void Begin(Transform caster,float seconds=3)
        {
            Caster=caster;Remaining=Mathf.Clamp(seconds,.1f,3);pulse=0;
            if(visual==null){var go=new GameObject("Barreira_dourada_Giovana");go.transform.SetParent(transform);visual=go.AddComponent<SpriteRenderer>();visual.sortingOrder=29999;}
            visual.sprite=CampaignHeroArt.Effect("Giovana",0,0);visual.enabled=true;
        }
        public bool Covers(Vector2 point)=>Active&&Vector2.Distance(Centre,point)<=2.5f;
        public bool TryBlock(){if(!Covers(transform.position))return false;pulse=.25f;return true;}
        void Update()
        {
            if(Time.timeScale>0&&CampaignContinuationController.Active?.Modal!=true){Remaining=Mathf.Max(0,Remaining-Time.deltaTime);pulse=Mathf.Max(0,pulse-Time.deltaTime);}
            if(visual==null)return;visual.enabled=Active;if(!Active)return;
            visual.transform.position=Centre;visual.transform.localScale=Vector3.one*(1.8f+pulse);
            visual.color=new Color(1,.84f,.36f,Mathf.Clamp01(Remaining*3)*(.65f+pulse));
        }
        void OnDisable(){Remaining=0;if(visual!=null)visual.enabled=false;}
    }
    public static class VarginhaStudentPowers
    {
        public static void Protect(VarginhaStudentAlly caster,Transform leader)
        {
            if(leader==null)return;
            (leader.GetComponent<VarginhaArcaneShield>()??leader.gameObject.AddComponent<VarginhaArcaneShield>()).Begin(caster.transform);
            foreach(var target in Object.FindObjectsByType<VarginhaCombatTarget>())
            {
                if(!caster.CanReachTarget(target)||Vector2.Distance(target.transform.position,leader.position)>2.5f)continue;
                target.GetComponent<CampaignManifestationCombat>()?.Repel(leader.position,1.5f);
                target.GetComponent<VarginhaCombatEnemy>()?.ApplyAllyControl(((Vector2)target.transform.position-(Vector2)leader.position).normalized,.5f,4);
            }
        }
        public static IEnumerator Books(VarginhaStudentAlly caster,VarginhaCombatTarget target)
        {
            for(int i=0;i<3;i++)
            {
                if(!caster.CanReachTarget(target))yield break;
                var go=new GameObject("Livro_de_estudo_Bianca");var book=go.AddComponent<VarginhaBookProjectile>();book.Begin(caster,target,8);
                yield return new WaitForSeconds(.18f);
            }
        }
    }
    public sealed class VarginhaBookProjectile:MonoBehaviour
    {
        VarginhaStudentAlly caster;VarginhaCombatTarget target;SpriteRenderer visual;Vector2 start,end;float age,damage;
        public void Begin(VarginhaStudentAlly c,VarginhaCombatTarget t,float value)
        {caster=c;target=t;damage=value;start=c.transform.position;end=t.transform.position;transform.position=start;visual=gameObject.AddComponent<SpriteRenderer>();visual.sprite=CampaignHeroArt.Effect("Bianca",0,0);visual.sortingOrder=30000;transform.localScale=Vector3.one*.45f;}
        void Update()
        {
            if(caster==null||target==null||!caster.isActiveAndEnabled||!caster.IsActive){Destroy(gameObject);return;}
            if(Time.timeScale<=0||CampaignContinuationController.Active?.Modal==true)return;
            if(!caster.CanCommand){Destroy(gameObject);return;}
            age+=Time.deltaTime;float u=Mathf.Clamp01(age/.48f);var next=Vector2.Lerp(start,end,u)+Vector2.up*Mathf.Sin(u*Mathf.PI)*.6f;
            foreach(var h in Physics2D.LinecastAll(transform.position,next))
            {
                if(h.collider.isTrigger||h.collider.GetComponentInParent<VarginhaStudentAlly>()!=null||h.collider.GetComponentInParent<EdelzioTopDownController>()!=null)continue;
                var enemy=h.collider.GetComponentInParent<VarginhaCombatTarget>();
                if(enemy!=null){if(caster.CanReachTarget(enemy))enemy.ReceiveHit(damage,(end-start).normalized,.03f);Destroy(gameObject);return;}
                Destroy(gameObject);return;
            }
            transform.position=next;transform.rotation=Quaternion.Euler(0,0,u*540);
            if(u>=1){if(caster.CanReachTarget(target)&&Vector2.Distance(target.transform.position,end)<1)target.ReceiveHit(damage,(end-start).normalized,.03f);Destroy(gameObject);}
        }
    }
}
