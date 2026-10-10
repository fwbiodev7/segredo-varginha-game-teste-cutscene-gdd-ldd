using UnityEngine;

namespace Game.Varginha.Experiment
{
    /// <summary>The entity's return gate: opens, holds through crossing, then closes.</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class CampaignReturnSeal : MonoBehaviour
    {
        public const string Resource="Varginha/StoryEffects/ReturnSealV1";
        public const int FrameSize=128,FrameCount=24;
        public const float PixelsPerUnit=32,OpeningDuration=1.2f,ClosingDuration=1.4f,LoopFps=12;
        private static readonly Sprite[] Frames=new Sprite[FrameCount];
        private static Material _material;
        private SpriteRenderer _renderer,_entity;
        private VarginhaWorldDepth _depth;
        private int _step=-1;
        private float _age;

        public static Sprite Frame(int index)
        {
            if(index<0||index>=FrameCount)throw new System.ArgumentOutOfRangeException(nameof(index));
            var cached=Frames[index];if(cached!=null&&cached.texture!=null)return cached;
            var texture=Resources.Load<Texture2D>(Resource);
            if(texture==null||texture.width!=FrameSize*6||texture.height!=FrameSize*4)
                throw new System.InvalidOperationException("Return seal requires a 6 x 4 atlas of 128 px cells");
            cached=Sprite.Create(texture,new Rect(index%6*FrameSize,(3-index/6)*FrameSize,FrameSize,FrameSize),
                new Vector2(.5f,.25f),PixelsPerUnit,0,SpriteMeshType.FullRect);
            cached.name="ReturnSeal_"+index;Frames[index]=cached;return cached;
        }

        public void Configure(SpriteRenderer entity)
        {
            _renderer=GetComponent<SpriteRenderer>();_entity=entity;
            if(entity!=null&&entity.GetComponent<VarginhaWorldDepth>()!=null)
                _depth=VarginhaWorldDepth.Ensure(_renderer,supportingObject:entity.transform,offset:-1);
            if(_material==null)
            {
                _material=new Material(Resources.Load<Shader>("Varginha/Flash1996/CrispPixelSprite"))
                    {name="Selo de retorno — runas em pixel art",hideFlags=HideFlags.DontSave};
                _material.SetFloat("_Cutoff",.55f);
            }
            _renderer.sharedMaterial=_material;
            _renderer.color=Color.white;_renderer.enabled=false;
        }

        // Driven by the chapter controller, so pausing also pauses this presentation.
        public void Present(int step,float deltaTime,bool crossing,bool reducedMotion)
        {
            if(_renderer==null)return;
            if(step!=_step){_step=step;_age=step==2?OpeningDuration:0;}
            else _age+=Mathf.Max(0,deltaTime);
            if(_entity!=null)
            {
                _renderer.sortingLayerID=_entity.sortingLayerID;
                if(_depth!=null){_renderer.sortingOrder=0;_depth.Refresh();}
                else _renderer.sortingOrder=_entity.sortingOrder-1;
            }
            bool closing=step==3;
            _renderer.enabled=step>0&&step<4&&(!closing||_age<ClosingDuration);
            if(!_renderer.enabled)return;
            int frame;
            float opacity=1;
            if(closing)
            {
                float amount=Mathf.Clamp01(_age/ClosingDuration);
                frame=reducedMotion?6:18+Mathf.Min(5,(int)(amount*6));
                opacity=1-Mathf.SmoothStep(0,1,amount);
            }
            else if(!reducedMotion&&step==1&&_age<OpeningDuration)
                frame=Mathf.Min(5,(int)(_age/OpeningDuration*6));
            else frame=reducedMotion?6:6+(int)(Mathf.Max(0,_age-OpeningDuration)*LoopFps)%12;
            _renderer.sprite=Frame(frame);
            // Light gathers inward during crossing without hiding the creature's body.
            if(!reducedMotion&&!closing&&_age>=OpeningDuration)
                opacity=(crossing?.98f:.86f)+.04f*Mathf.Sin(_age*2.2f);
            _renderer.color=new Color(1,1,1,opacity);
        }
    }
}
