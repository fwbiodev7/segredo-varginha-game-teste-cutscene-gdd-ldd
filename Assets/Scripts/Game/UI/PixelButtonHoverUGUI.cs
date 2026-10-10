using Game.Varginha;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Game.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class PixelButtonHoverUGUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        private Button _button;
        private PixelHoverGraphic _overlay;
        private readonly PixelButtonHoverState _motion=new();
        private bool _pointer, _selected;
        private TMP_Text[] _labels;
        private Color[] _baseColors, _writtenColors;
        public float Amount=>_motion.Amount;

        public static void Attach(Button button)
        { if(!button.GetComponent<PixelButtonHoverUGUI>())button.gameObject.AddComponent<PixelButtonHoverUGUI>(); }

        private void Awake()
        {
            _button=GetComponent<Button>();
            var colors=_button.colors;
            colors.highlightedColor=colors.selectedColor=colors.normalColor;
            colors.fadeDuration=PixelButtonHoverState.Duration;
            _button.colors=colors;
            var child=new GameObject("HoverSuave",typeof(RectTransform),typeof(CanvasRenderer),typeof(PixelHoverGraphic));
            child.transform.SetParent(transform,false);
            child.transform.SetAsFirstSibling();
            var rect=(RectTransform)child.transform;
            rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;
            rect.offsetMin=rect.offsetMax=Vector2.zero;
            _overlay=child.GetComponent<PixelHoverGraphic>();
            _overlay.raycastTarget=false;
        }

        private void Update()
        {
            bool enabled=_button.IsActive()&&_button.IsInteractable();
            _motion.Advance(VarginhaGamepadUI.PointerActive?_pointer:_selected,Time.unscaledDeltaTime,enabled);
            _overlay.SetAmount(_motion.Amount);
            if(_labels==null)
            {
                _labels=GetComponentsInChildren<TMP_Text>(true);
                _baseColors=new Color[_labels.Length];_writtenColors=new Color[_labels.Length];
                for(int i=0;i<_labels.Length;i++)_baseColors[i]=_writtenColors[i]=_labels[i].color;
            }
            for(int i=0;i<_labels.Length;i++)
            {
                if(!_labels[i])continue;
                // Inventory refreshes status colors; adopt them before applying the tint.
                if(_labels[i].color!=_writtenColors[i])_baseColors[i]=_labels[i].color;
                _writtenColors[i]=Color.Lerp(_baseColors[i],new Color(1,1,1,_baseColors[i].a),_motion.Amount*PixelButtonHover.TextStrength);
                _labels[i].color=_writtenColors[i];
            }
        }
        private void OnDisable()
        {
            _pointer=_selected=false;_motion.Advance(false,0,false);
            if(_overlay)_overlay.SetAmount(0);
            if(_labels!=null)for(int i=0;i<_labels.Length;i++)
                if(_labels[i]&&_labels[i].color==_writtenColors[i])_labels[i].color=_writtenColors[i]=_baseColors[i];
        }
        public void OnPointerEnter(PointerEventData data)=>_pointer=true;
        public void OnPointerExit(PointerEventData data)=>_pointer=false;
        public void OnSelect(BaseEventData data)=>_selected=true;
        public void OnDeselect(BaseEventData data)=>_selected=false;
    }

    // Square geometry keeps the existing sliced sprites and pixel edges untouched.
    public sealed class PixelHoverGraphic : MaskableGraphic
    {
        private float _amount;
        public void SetAmount(float value)
        {if(Mathf.Approximately(value,_amount))return;_amount=value;SetVerticesDirty();}
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();if(_amount<=0)return;
            var rect=GetPixelAdjustedRect();
            Color tint=PixelButtonHover.Accent;tint.a=_amount*PixelButtonHover.FillAlpha;
            Quad(mesh,new Rect(rect.x+1,rect.y+1,rect.width-2,rect.height-2),tint);
            tint.a=_amount*PixelButtonHover.BorderAlpha;
            Quad(mesh,new Rect(rect.x,rect.y,rect.width,1),tint);
            Quad(mesh,new Rect(rect.x,rect.yMax-1,rect.width,1),tint);
            Quad(mesh,new Rect(rect.x,rect.y+1,1,rect.height-2),tint);
            Quad(mesh,new Rect(rect.xMax-1,rect.y+1,1,rect.height-2),tint);
        }
        private static void Quad(VertexHelper mesh,Rect rect,Color color)
        {
            int start=mesh.currentVertCount;
            mesh.AddVert(new Vector3(rect.xMin,rect.yMin),color,Vector2.zero);
            mesh.AddVert(new Vector3(rect.xMin,rect.yMax),color,Vector2.zero);
            mesh.AddVert(new Vector3(rect.xMax,rect.yMax),color,Vector2.zero);
            mesh.AddVert(new Vector3(rect.xMax,rect.yMin),color,Vector2.zero);
            mesh.AddTriangle(start,start+1,start+2);mesh.AddTriangle(start,start+2,start+3);
        }
    }
}
