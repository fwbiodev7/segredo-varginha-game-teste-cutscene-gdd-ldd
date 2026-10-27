using UnityEngine;

namespace Game.Varginha.Experiment
{
    [DefaultExecutionOrder(12000)]
    public sealed class CampaignForestPresence:MonoBehaviour
    {
        public const string Atlas="ForestPresenceSix20261027";
        int _view,_frame=-1;
        bool _flip;
        SpriteRenderer _renderer;MaterialPropertyBlock _block;
        void Awake(){_renderer=GetComponent<SpriteRenderer>();_block=new MaterialPropertyBlock();}
        public void Present(Vector2 facing,int frame)
        {
            if(_renderer==null)return;
            int view=_view;
            if(facing.sqrMagnitude>.000001f)
            {
                var direction=facing.normalized;
                if(direction.y>.12f)view=1;else if(direction.y<-.12f)view=0;
                if(Mathf.Abs(direction.x)>.12f)_flip=direction.x>0;
            }
            frame=((frame%3)+3)%3;
            if(view!=_view||frame!=_frame)_renderer.sprite=CampaignStorySprites.Frame(Atlas,view,frame);
            _view=view;_frame=frame;_renderer.flipX=_flip;
        }
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
