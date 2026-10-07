using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Varginha.Experiment
{
    public sealed class CampaignWorkshopVehicle : MonoBehaviour
    {
        public enum Facing { North,South,East,West }
        private static Sprite _line;
        private readonly RaycastHit2D[] _hits=new RaycastHit2D[16];
        private readonly Collider2D[] _contacts=new Collider2D[16];
        private ContactFilter2D _filter;
        private SpriteRenderer _renderer;
        private BoxCollider2D _box;
        private Rigidbody2D _body;
        private Transform _lights,_bay;
        private Vector2 _input,_velocity;
        private bool _parking;
        public Facing Direction { get; private set; }
        public Vector2 BayPosition { get; private set; }
        public bool CanPark => _parking && Direction==Facing.North && Vector2.Distance(transform.position,BayPosition)<.35f && _velocity.magnitude<.16f;

        public static Sprite FrameSprite(Facing facing)
        {
            return CampaignOriginalFusca.Top(facing);
        }

        public void Configure(CampaignExpansionState state)
        {
            _renderer=GetComponent<SpriteRenderer>();_box=GetComponent<BoxCollider2D>();
            _body=GetComponent<Rigidbody2D>();if(_body==null)_body=gameObject.AddComponent<Rigidbody2D>();
            _body.gravityScale=0;_body.constraints=RigidbodyConstraints2D.FreezeRotation;
            _body.interpolation=RigidbodyInterpolation2D.Interpolate;
            _filter=new ContactFilter2D { useTriggers=false };_filter.SetLayerMask(~0);
            transform.localScale=Vector3.one;transform.rotation=Quaternion.identity;
            // Retain the map's shared actor material and its ambient/lamp property block.
            var layout=CampaignIllustratedMaps.Get(10);BayPosition=layout.Position(779,448);
            _lights=new GameObject("Luzes_do_Fusca").transform;_lights.SetParent(transform,false);
            CampaignFuscaLighting.Add(_lights);
            // Lamp centers measured in the original-model overhead view (50x95).
            foreach(Transform beam in _lights)
            {
                beam.localPosition=new Vector3(Mathf.Sign(beam.localPosition.x)*.69f,1.62f,0);
                beam.localScale=new Vector3(.7f,3.5f,1);
                var light=beam.GetComponent<CampaignDynamicLight>();light.reach=3.5f;light.halfAngle=12f;
            }
            _bay=new GameObject("Vaga_de_analise").transform;_bay.SetParent(transform.parent,false);_bay.position=BayPosition;
            DrawBay();
            if(state.workshopParked) ParkAtBay();
            else
            {
                _parking=true;_body.bodyType=RigidbodyType2D.Kinematic;
                SetFacing((Facing)state.workshopHeading);
                _body.position=state.workshopHasCarPosition?new Vector2(state.workshopCarX,state.workshopCarY):layout.Position(779,816);
                Physics2D.SyncTransforms();
                // A saved pose can become occupied after a layout change; use the clear approach.
                if(IsOccupied(_box.size))_body.position=layout.Position(779,816);
            }
        }

        private void DrawBay()
        {
            if(_line==null)_line=Sprite.Create(Texture2D.whiteTexture,new Rect(0,0,1,1),Vector2.one/2,1);
            foreach(float x in new[]{-1.22f,1.22f})foreach(float y in new[]{-2.12f,2.12f})
            {
                Line(new Vector2(x,y),new Vector2(.08f,.38f));
                Line(new Vector2(x-Mathf.Sign(x)*.15f,y),new Vector2(.38f,.08f));
            }
        }
        private void Line(Vector2 offset,Vector2 size)
        {
            var go=new GameObject("Canto_da_vaga");go.transform.SetParent(_bay,false);go.transform.localPosition=offset;go.transform.localScale=new Vector3(size.x,size.y,1);
            var render=go.AddComponent<SpriteRenderer>();render.sprite=_line;render.color=new Color(.9f,.8f,.4f,.65f);render.sortingOrder=1;
        }
        private bool IsOccupied(Vector2 size)
        {
            int count=Physics2D.OverlapBox(_body.position,size,0,_filter,_contacts);
            for(int i=0;i<count;i++)if(_contacts[i]!=null&&_contacts[i].attachedRigidbody!=_body&&!_contacts[i].transform.IsChildOf(transform))return true;
            return false;
        }
        private static Vector2 Size(Facing direction)=>direction==Facing.North||direction==Facing.South?new Vector2(1.85f,3.45f):new Vector2(3.45f,1.85f);
        public void SetFacing(Facing facing)
        {
            Direction=facing;_renderer.sprite=FrameSprite(facing);_renderer.flipX=false;
            _box.size=Size(facing);_box.offset=Vector2.zero;
            _lights.localRotation=Quaternion.Euler(0,0,facing==Facing.East?-90:facing==Facing.West?90:facing==Facing.South?180:0);
            GetComponent<CampaignReagentMarks>()?.FaceTrack(facing==Facing.West);
        }
        public bool TickParking()
        {
            var keyboard=Keyboard.current;
            Vector2 direction=new Vector2((keyboard?.dKey.isPressed==true||keyboard?.rightArrowKey.isPressed==true?1:0)-(keyboard?.aKey.isPressed==true||keyboard?.leftArrowKey.isPressed==true?1:0),
                (keyboard?.wKey.isPressed==true||keyboard?.upArrowKey.isPressed==true?1:0)-(keyboard?.sKey.isPressed==true||keyboard?.downArrowKey.isPressed==true?1:0));
            if(VarginhaInputActions.Move.sqrMagnitude>0)direction=VarginhaInputActions.Move;
            // Cardinal movement matches the four authored views and keeps garage turns readable.
            if(Mathf.Abs(direction.y)>0)direction.x=0;
            if(direction!=Vector2.zero)
            {
                var next=direction.y>0?Facing.North:direction.y<0?Facing.South:direction.x>0?Facing.East:Facing.West;
                if(next!=Direction)
                {
                    if(!IsOccupied(Size(next)))SetFacing(next);else direction=Vector2.zero;
                }
            }
            _input=direction;
            return CanPark&&VarginhaInputActions.CarPressed;
        }
        public void StopInput() { _input=Vector2.zero;_velocity=Vector2.zero; }
        private void FixedUpdate()
        {
            if(!_parking)return;
            _velocity=Vector2.MoveTowards(_velocity,_input*2.65f,Time.fixedDeltaTime*7);
            Vector2 delta=_velocity*Time.fixedDeltaTime;if(delta.sqrMagnitude<.000001f)return;
            int count=_body.Cast(delta.normalized,_filter,_hits,delta.magnitude+.035f);
            float distance=delta.magnitude;
            for(int i=0;i<count;i++)if(_hits[i].collider!=null)distance=Mathf.Min(distance,Mathf.Max(0,_hits[i].distance-.035f));
            if(distance<delta.magnitude)_velocity=Vector2.zero;
            _body.MovePosition(_body.position+delta.normalized*distance);
        }
        public void ParkAtBay()
        {
            _parking=false;StopInput();SetFacing(Facing.North);_body.position=BayPosition;transform.position=BayPosition;
            _body.bodyType=RigidbodyType2D.Static;_bay.gameObject.SetActive(false);CampaignFuscaLighting.SetEnabled(_lights,false);Physics2D.SyncTransforms();
        }
        public void BeginTrack()
        {
            _parking=false;StopInput();_body.bodyType=RigidbodyType2D.Kinematic;
            SetFacing(Facing.East);CampaignFuscaLighting.SetEnabled(_lights,true);
            GetComponent<CampaignReagentMarks>()?.UseTrackView();
        }
        public void SetPosition(Vector2 position) { _body.position=position;transform.position=position; }
        public void SaveParking(CampaignExpansionState state)
        {
            if(!_parking)return;
            state.workshopHasCarPosition=true;state.workshopCarX=_body.position.x;state.workshopCarY=_body.position.y;state.workshopHeading=(int)Direction;
        }
        public void FinishTrack() { _body.bodyType=RigidbodyType2D.Static;CampaignFuscaLighting.SetEnabled(_lights,false);Physics2D.SyncTransforms(); }
        public void BeginDeparture() { StopInput();SetFacing(Facing.East);_body.simulated=false;CampaignFuscaLighting.SetEnabled(_lights,true); }
    }
}
