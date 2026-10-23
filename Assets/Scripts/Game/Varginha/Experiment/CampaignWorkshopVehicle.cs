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
        private bool _parking,_track;
        [SerializeField] private float forwardSpeed=2.65f,reverseSpeed=1.7f,acceleration=3.5f,coastDeceleration=8,brakeDeceleration=12;
        [SerializeField] private float maximumSteerAngle=35,steerResponse=3.5f,steerReturn=4,wheelbase=1.6f,lateralGrip=18;
        private float _speed,_steer,_yaw;
        public float Speed=>_speed;
        public float Steering=>_steer;
        public float Heading=>_yaw;
        public Facing Direction { get; private set; }
        public Vector2 BayPosition { get; private set; }
        public bool CanPark => _parking && Mathf.Abs(Mathf.DeltaAngle(_yaw,0))<12 && Vector2.Distance(_body.position,BayPosition)<.35f && _velocity.magnitude<.16f;

        public static Sprite FrameSprite(Facing facing)
        {
            return CampaignOriginalFusca.Top(facing);
        }

        public void Configure(CampaignExpansionState state)
        {
            _renderer=GetComponent<SpriteRenderer>();_box=GetComponent<BoxCollider2D>();
            _body=GetComponent<Rigidbody2D>();if(_body==null)_body=gameObject.AddComponent<Rigidbody2D>();
            _body.gravityScale=0;_body.constraints=RigidbodyConstraints2D.None;
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
                if(state.workshopHasYaw){_yaw=state.workshopYaw;_body.rotation=_yaw;UpdateDirection();}
                _body.position=state.workshopHasCarPosition?new Vector2(state.workshopCarX,state.workshopCarY):layout.Position(779,816);
                Physics2D.SyncTransforms();
                // A saved pose can become occupied after a layout change; use the clear approach.
                if(IsOccupied(_box.size,_yaw)){_body.position=layout.Position(779,816);SetFacing(Facing.North);}
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
        private bool IsOccupied(Vector2 size,float angle)
        {
            int count=Physics2D.OverlapBox(_body.position,size,angle,_filter,_contacts);
            for(int i=0;i<count;i++)if(_contacts[i]!=null&&_contacts[i].attachedRigidbody!=_body&&!_contacts[i].transform.IsChildOf(transform))return true;
            return false;
        }
        private static Vector2 Size(Facing direction)=>direction==Facing.North||direction==Facing.South?new Vector2(1.85f,3.45f):new Vector2(3.45f,1.85f);
        public void SetFacing(Facing facing)
        {
            Direction=facing;_renderer.sprite=FrameSprite(_parking?Facing.North:facing);_renderer.flipX=false;
            _yaw=facing==Facing.East?-90:facing==Facing.West?90:facing==Facing.South?180:0;
            _box.size=Size(_parking?Facing.North:facing);_box.offset=Vector2.zero;
            _body.rotation=_parking?_yaw:0;
            _lights.localRotation=Quaternion.Euler(0,0,_parking?0:_yaw);
            GetComponent<CampaignReagentMarks>()?.FaceTrack(facing==Facing.West);
        }
        public bool TickParking()
        {
            var keyboard=Keyboard.current;
            Vector2 direction=new Vector2((keyboard?.dKey.isPressed==true||keyboard?.rightArrowKey.isPressed==true?1:0)-(keyboard?.aKey.isPressed==true||keyboard?.leftArrowKey.isPressed==true?1:0),
                (keyboard?.wKey.isPressed==true||keyboard?.upArrowKey.isPressed==true?1:0)-(keyboard?.sKey.isPressed==true||keyboard?.downArrowKey.isPressed==true?1:0));
            if(VarginhaInputActions.Move.sqrMagnitude>0)direction=VarginhaInputActions.Move;
            SetControls(direction);
            return CanPark&&VarginhaInputActions.CarPressed;
        }
        public void SetControls(Vector2 input)=>_input=Vector2.ClampMagnitude(input,1);
        public void StopInput() { _input=Vector2.zero;_velocity=Vector2.zero;_speed=_steer=0; }
        private void UpdateDirection()
        {
            float angle=Mathf.Repeat(_yaw+45,360);
            Direction=angle<90?Facing.North:angle<180?Facing.West:angle<270?Facing.South:Facing.East;
        }
        private void FixedUpdate()
        {
            if(_track){TickTrack();return;}
            if(!_parking)return;
            float dt=Time.fixedDeltaTime,throttle=_input.y;
            _speed=CampaignVehiclePhysics.Speed(_speed,throttle,forwardSpeed,reverseSpeed,acceleration,coastDeceleration,brakeDeceleration,dt);
            _steer=Mathf.MoveTowards(_steer,_input.x,Mathf.Max(.1f,Mathf.Abs(_input.x)<.01f?steerReturn:steerResponse)*dt);
            float nextYaw=_yaw-_speed/Mathf.Max(.5f,wheelbase)*Mathf.Tan(_steer*Mathf.Clamp(maximumSteerAngle,1,55)*Mathf.Deg2Rad)*Mathf.Rad2Deg*dt;
            if(!IsOccupied(_box.size,nextYaw)){_yaw=nextYaw;_body.MoveRotation(_yaw);UpdateDirection();}
            Vector2 forward=new(-Mathf.Sin(_yaw*Mathf.Deg2Rad),Mathf.Cos(_yaw*Mathf.Deg2Rad));
            _velocity=Vector2.Lerp(_velocity,forward*_speed,1-Mathf.Exp(-Mathf.Max(.1f,lateralGrip)*dt));
            Vector2 delta=_velocity*Time.fixedDeltaTime;if(delta.sqrMagnitude<.000001f)return;
            float distance=CampaignVehiclePhysics.ClearDistance(_body,_box,_yaw,delta,_hits);
            if(distance<delta.magnitude){_velocity=Vector2.zero;_speed=0;}
            _body.MovePosition(_body.position+delta.normalized*distance);
        }
        public void ParkAtBay()
        {
            _parking=false;StopInput();SetFacing(Facing.North);_body.position=BayPosition;transform.position=BayPosition;
            _body.bodyType=RigidbodyType2D.Static;_bay.gameObject.SetActive(false);CampaignFuscaLighting.SetEnabled(_lights,false);Physics2D.SyncTransforms();
        }
        public void BeginTrack()
        {
            _parking=false;_track=true;StopInput();_body.bodyType=RigidbodyType2D.Kinematic;
            SetFacing(Facing.East);CampaignFuscaLighting.SetEnabled(_lights,true);
            GetComponent<CampaignReagentMarks>()?.UseTrackView();
        }
        public void SetPosition(Vector2 position) { _body.position=position;transform.position=position; }
        public void SetTrackControls(float input)=>_input=new Vector2(Mathf.Clamp(input,-1,1),0);
        private void TickTrack()
        {
            // This optional test is a straight lane. Direction still has inertia and braking.
            _speed=CampaignVehiclePhysics.Speed(_speed,_input.x,4,4,acceleration,coastDeceleration,brakeDeceleration,Time.fixedDeltaTime);
            if(Mathf.Abs(_speed)>.02f&&Direction!=(_speed<0?Facing.West:Facing.East))SetFacing(_speed<0?Facing.West:Facing.East);
            Vector2 delta=Vector2.right*_speed*Time.fixedDeltaTime;
            delta.x=Mathf.Clamp(_body.position.x+delta.x,-8,8)-_body.position.x;
            float distance=CampaignVehiclePhysics.ClearDistance(_body,_box,0,delta,_hits);
            if(distance<delta.magnitude)_speed=0;
            _velocity=Vector2.right*_speed;
            if(delta.sqrMagnitude>.000001f)_body.MovePosition(_body.position+delta.normalized*distance);
        }
        public void SaveParking(CampaignExpansionState state)
        {
            if(!_parking)return;
            state.workshopHasCarPosition=true;state.workshopCarX=_body.position.x;state.workshopCarY=_body.position.y;state.workshopHeading=(int)Direction;
            state.workshopHasYaw=true;state.workshopYaw=_yaw;
        }
        public void FinishTrack() { _track=false;StopInput();_body.bodyType=RigidbodyType2D.Static;CampaignFuscaLighting.SetEnabled(_lights,false);Physics2D.SyncTransforms(); }
        public void BeginDeparture() { _track=false;StopInput();SetFacing(Facing.East);_body.simulated=false;CampaignFuscaLighting.SetEnabled(_lights,true); }
    }
}
