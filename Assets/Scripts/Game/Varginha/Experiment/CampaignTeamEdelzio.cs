using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // New transparent animation sheets use the supplied clean portrait as their design reference.
    [DefaultExecutionOrder(500)]
    public sealed class CampaignTeamEdelzio : MonoBehaviour
    {
        private static readonly Dictionary<string,Sprite> Frames=new();
        private static readonly Dictionary<Texture2D,Rect[]> Outlines=new();
        private EdelzioTopDownController _actor;
        private VarginhaPlayerSpriteAnimation _actions;
        private SpriteRenderer _renderer;
        private bool _wasPunching;
        private float _punchStarted;
        private string _lastInteraction;
        private float _interactionStarted;
        private void Start()
        {
            _actor=GetComponent<EdelzioTopDownController>();_actions=GetComponent<VarginhaPlayerSpriteAnimation>();_renderer=GetComponent<SpriteRenderer>();
            if(_actions!=null)_actions.enabled=false;
        }
        private void LateUpdate()
        {
            if(_actor==null||_renderer==null)return;
            bool acting=_actions!=null&&_actions.HasActionPose;
            Vector2 face=acting?_actions.ActionFacingDirection:_actor.FacingDirection;
            int direction=Mathf.Abs(face.y)>=Mathf.Abs(face.x)?face.y>0?3:0:face.x<0?1:2;
            bool punching=_actions!=null&&_actions.IsPunching;
            if(punching&&!_wasPunching)_punchStarted=Time.time;
            _wasPunching=punching;
            string interaction=acting?_actions.CurrentActionPose:null;
            if(interaction!=_lastInteraction){_interactionStarted=Time.time;_lastInteraction=interaction;}
            Sprite pose;
            if(punching)pose=Frame("EdelzioPunch",direction,Mathf.Clamp((int)((Time.time-_punchStarted)*12),0,2));
            else if(_actions!=null&&_actions.IsSeated)
                pose=Frame("EdelzioActions",direction,_actions.CurrentActionPose=="Edelzio_UseNotebook"?(int)(Time.time*4)%3:_actions.ActionFrame);
            else if(_actions!=null&&_actions.IsDrinking)pose=Frame("EdelzioActions",direction,3+_actions.ActionFrame);
            else if(acting&&(_actions.CurrentActionPose=="Edelzio_Crouch"||_actions.CurrentActionPose=="Edelzio_Reach"))
                pose=Frame("EdelzioInteractions",direction,(_actions.CurrentActionPose=="Edelzio_Reach"?3:0)+Mathf.Clamp((int)((Time.time-_interactionStarted)*8),0,2));
            else
            {
                int frame=_actor.IsMoving&&!_actor.IsInputLocked?(int)(Time.time*7)%4:1;
                pose=direction==0&&!_actor.IsMoving?Idle:Frame("EdelzioWalk",direction,frame);
            }
            if(pose!=null){_renderer.sprite=pose;_renderer.color=Color.white;_renderer.flipX=false;}
            foreach(Transform part in transform)if(part.name.Contains("Beard")||part.name.Contains("Barba"))part.gameObject.SetActive(false);
        }
        public static Sprite Idle=>Frame("EdelzioIdle",0,0);
        public static Sprite Frame(string name,int row,int column)
        {
            bool idle=name=="EdelzioIdle";
            int columns=idle?1:name=="EdelzioWalk"?4:name=="EdelzioPunch"?3:6;
            if(row<0||row>=(idle?1:4)||column<0||column>=columns)return null;
            string key=name+row+":"+column;
            if(Frames.TryGetValue(key,out var cached)&&cached!=null)return cached;
            var texture=Resources.Load<Texture2D>("Varginha/TeamArt/"+name+(idle?"":"V2"));
            if(texture==null)return null;
            var rectangles=FindOutlines(texture,columns,idle?1:4);
            var rect=rectangles[row*columns+column];if(rect.width<=0||rect.height<=0)return null;
            int standingColumn=idle?0:name=="EdelzioActions"||name=="EdelzioInteractions"?4:name=="EdelzioWalk"?1:0;
            var standing=rectangles[row*columns+standingColumn];
            float ppu=Mathf.Max(1,standing.height)/1.82f;
            // Outline rectangles retain the complete head even when artwork crosses a nominal grid edge.
            float cellCenter=(column+.5f)*(texture.width/(float)columns);
            float pivotX=Mathf.Clamp01((cellCenter-rect.x)/rect.width);
            var sprite=Sprite.Create(texture,rect,new Vector2(pivotX,ppu*.58f/rect.height),ppu,0,SpriteMeshType.FullRect);
            sprite.name="Team_"+key;Frames[key]=sprite;return sprite;
        }
        private static Rect[] FindOutlines(Texture2D texture,int columns,int rows)
        {
            if(Outlines.TryGetValue(texture,out var cached))return cached;
            var pixels=texture.GetPixels32();var visited=new bool[pixels.Length];var queue=new int[pixels.Length];
            var counts=new int[columns*rows];var rectangles=new Rect[counts.Length];
            int width=texture.width,height=texture.height;
            for(int seed=0;seed<pixels.Length;seed++)
            {
                if(visited[seed]||pixels[seed].a<100)continue;
                int head=0,tail=1;queue[0]=seed;visited[seed]=true;
                int left=width,right=0,bottom=height,top=0;
                while(head<tail)
                {
                    int i=queue[head++],x=i%width,y=i/width;
                    left=Mathf.Min(left,x);right=Mathf.Max(right,x);bottom=Mathf.Min(bottom,y);top=Mathf.Max(top,y);
                    void Add(int n){if(!visited[n]&&pixels[n].a>=100){visited[n]=true;queue[tail++]=n;}}
                    if(x>0)Add(i-1);if(x+1<width)Add(i+1);if(y>0)Add(i-width);if(y+1<height)Add(i+width);
                }
                int column=Mathf.Clamp((left+right)*columns/(2*width),0,columns-1);
                int row=rows-1-Mathf.Clamp((bottom+top)*rows/(2*height),0,rows-1);
                int slot=row*columns+column;
                if(tail>counts[slot]){counts[slot]=tail;rectangles[slot]=new Rect(left,bottom,right-left+1,top-bottom+1);}
            }
            Outlines[texture]=rectangles;return rectangles;
        }
    }
}
