using UnityEngine;

namespace Game.Varginha.Experiment
{
    [DefaultExecutionOrder(12000)]
    public sealed class CampaignForestPresence:MonoBehaviour
    {
        SpriteRenderer _renderer;MaterialPropertyBlock _block;
        void Awake(){_renderer=GetComponent<SpriteRenderer>();_block=new MaterialPropertyBlock();}
        void LateUpdate()
        {
            if(_renderer==null)return;
            // Preserve the scene's directional light, with a restrained floor
            // for the silhouette and ember eyes. The creature never disappears.
            _renderer.GetPropertyBlock(_block);_block.SetFloat("_ForestPresence",1);_renderer.SetPropertyBlock(_block);
            float breath=VarginhaGameSettings.Current.reducedMotion?0:Mathf.Sin(Time.time*1.7f)*.025f;
            _renderer.color=new Color(.86f,.93f,.86f,.94f+breath);
        }
    }
}
