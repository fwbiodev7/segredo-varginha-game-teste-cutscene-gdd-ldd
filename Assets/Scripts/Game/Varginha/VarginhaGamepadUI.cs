using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha
{
    // Navigation adapter for the existing IMGUI menus. Mouse rendering and clicks
    // stay native; the controller selects the same buttons, sliders and scroll rows.
    public static class VarginhaGamepadUI
    {
        private sealed class Node
        { public int id; public Rect rect, local; public bool slider, car; public int scroll; }
        private sealed class Context
        { public string key; public int priority, frame, selected; public bool carOnly, verticalRows; public readonly List<Node> nodes = new(); public Node selection; }
        private static readonly Dictionary<string, Context> Contexts = new();
        private static Context _current, _active, _pressed;
        private static int _pressedId, _pressedFrame = -1, _usedFrame = -1, _sliderUsed = -1, _sliderId;
        private static float _repeatAt, _sliderDelta;
        private static Vector2 _lastNav;
        private static int _scroll;
        private static bool _showSelection;
        public static bool HasSelection => _active?.selection != null;
        public static string ActiveContext => _active?.key;
        public static string CurrentContext => _current?.key;
        public static bool PointerActive { get; private set; }=true;
        public static void Reset() { Contexts.Clear(); _current = _active = _pressed = null; _pressedFrame = -1; _lastNav = Vector2.zero; _sliderDelta = 0; _sliderUsed = _usedFrame = -1; _repeatAt = 0; _showSelection = false; PointerActive=true; Game.UI.PixelButtonHover.Reset(); }
        public static void Begin(string key, bool interactive = true, int priority = 0, bool carOnly = false, bool verticalRows = false, Rect? initialSelection = null)
        {
            _scroll = 0;
            if (!interactive) { _current = null; return; }
            if (!Contexts.TryGetValue(key, out _current)) Contexts[key] = _current = new Context { key = key, selected = initialSelection?.GetHashCode() ?? 0 };
            _current.frame = Time.frameCount; _current.priority = priority;
            _current.carOnly=carOnly;
            _current.verticalRows=verticalRows;
            if (Event.current?.type == EventType.Repaint) _current.nodes.Clear();
        }
        public static void End() { _current = null; _scroll = 0; }
        public static void Tick()
        {
            var mouse=UnityEngine.InputSystem.Mouse.current;
            if(mouse?.delta.ReadValue().sqrMagnitude>.25f||mouse?.leftButton.wasPressedThisFrame==true)PointerActive=true;
            else if(VarginhaInputActions.UI("Navigate").ReadValue<Vector2>().sqrMagnitude>.16f
                ||VarginhaInputActions.UI("Submit").WasPressedThisFrame())PointerActive=false;
            var previous = _active;
            _active = null;
            if(VarginhaGamepadBindings.Suspended) return;
            foreach (var context in Contexts.Values)
            {
                if(context.nodes.Count==0)continue;
                if(!context.nodes.Exists(n=>n.id==context.selected)){context.selection=context.nodes[0];context.selected=context.selection.id;}
                if (context.frame >= Time.frameCount - 1 && context.selection != null && (_active == null || context.priority > _active.priority)) _active = context;
            }
            if (_active == null) return;
            if (_active != previous) { _sliderDelta = 0; _lastNav = Vector2.zero; _repeatAt = 0; }
            var value = VarginhaInputActions.UI("Navigate").ReadValue<Vector2>();
            Vector2 direction = Mathf.Abs(value.x) > Mathf.Abs(value.y) ? new Vector2(Mathf.Sign(value.x), 0) : new Vector2(0, -Mathf.Sign(value.y));
            if (value.sqrMagnitude < .16f) { _lastNav = Vector2.zero; }
            else if (_lastNav != direction || Time.unscaledTime >= _repeatAt)
            {
                _showSelection = true;
                _repeatAt = Time.unscaledTime + (_lastNav == direction ? .12f : .32f); _lastNav = direction;
                if (_active.selection.slider && direction.x != 0) { _sliderDelta = direction.x * .05f; _sliderId = _active.selected; }
                else Navigate(direction);
            }
            bool submit = _active.selection.car ? VarginhaInputActions.CarPressed : VarginhaInputActions.UI("Submit").WasPressedThisFrame();
            if (submit) { _pressed = _active; _pressedId = _active.selected; _pressedFrame = Time.frameCount; }
        }
        private static void Navigate(Vector2 direction)
        {
            _sliderDelta = 0;
            Node best = null; float score = float.PositiveInfinity;
            foreach (var node in _active.nodes)
            {
                if (node.id == _active.selected) continue;
                Vector2 delta = node.rect.center - _active.selection.rect.center;
                float along = Vector2.Dot(delta, direction); if (along <= 1) continue;
                float across = Mathf.Abs(delta.x * direction.y - delta.y * direction.x);
                // Settings follow rows, so a nearby slider wins over a distant footer.
                float cost = _active.verticalRows && direction.y != 0 ? along + across * .001f : along + across * 3;
                if (cost < score) { score = cost; best = node; }
            }
            if (best == null)
            {
                // Wrap to the opposite edge, favouring the same row/column.
                foreach (var node in _active.nodes)
                {
                    if (node.id == _active.selected) continue;
                    Vector2 delta = node.rect.center - _active.selection.rect.center;
                    float cost = Vector2.Dot(delta, direction) + Mathf.Abs(delta.x * direction.y - delta.y * direction.x) * 3;
                    if (cost < score) { score = cost; best = node; }
                }
            }
            if (best != null) { _active.selected = best.id; _active.selection = best; }
        }
        private static Node Register(Rect rect, bool slider = false, bool car = false)
        {
            if (_current == null || !GUI.enabled) return null;
            int id = rect.GetHashCode();
            var screen = GUIUtility.GUIToScreenPoint(rect.position); var corner = GUIUtility.GUIToScreenPoint(rect.max);
            var node = new Node { id = id, local = rect, rect = new Rect(screen, corner - screen), slider = slider, car = car || _current.carOnly, scroll = _scroll };
            int index=_current.nodes.FindIndex(n=>n.id==id);
            if(index<0)_current.nodes.Add(node);
            else
            {
                // Focus queries re-register the same rectangle: retain its special binding.
                node.car|=_current.nodes[index].car;node.slider|=_current.nodes[index].slider;
                _current.nodes[index]=node;
            }
            if (_current.selected == 0 || _current.selected == id)
            { _current.selected = id; _current.selection = node; }
            GUI.SetNextControlName(_current.key + id);
            return node;
        }
        public static bool Selected(Rect rect, bool includeKeyboard = false)
        {
            var node = Register(rect);
            return node != null && _current.selected == node.id && (VarginhaInputActions.UsingGamepad || includeKeyboard && _showSelection);
        }
        public static bool Button(Rect rect, string text, GUIStyle style = null) => Button(rect, new GUIContent(text), style);
        public static bool Button(Rect rect, GUIContent text, GUIStyle style = null, bool highlight = true)
        {
            var node = Register(rect);
            float hover=highlight?Game.UI.PixelButtonHover.Amount(rect,Game.UI.PixelButtonHover.Focused(rect)):0;
            var drawingStyle=highlight?Game.UI.PixelButtonHover.SmoothStyle(style,hover):style??GUI.skin.button;
            bool clicked=false;
            if(node?.car==true)GUI.Box(rect,new GUIContent("["+VarginhaInputActions.CarLabel+"] "+text.text),drawingStyle);
            else clicked = GUI.Button(rect, text, drawingStyle);
            if(highlight)Game.UI.PixelButtonHover.Decorate(rect,hover);
            if (clicked && node != null) { _current.selected = node.id; _current.selection = node; _showSelection = false; }
            Focus(node); return clicked || Pressed(node);
        }
        public static bool CarButton(Rect rect, string text, GUIStyle style = null)
        {
            var node = Register(rect, car: true);
            float hover=Game.UI.PixelButtonHover.Amount(rect,Game.UI.PixelButtonHover.Focused(rect));
            GUI.Box(rect, "[" + VarginhaInputActions.CarLabel + "] " + text, Game.UI.PixelButtonHover.SmoothStyle(style,hover));
            Game.UI.PixelButtonHover.Decorate(rect,hover);
            return Pressed(node);
        }
        private static bool Pressed(Node node)
        {
            if (node == null || _pressed != _current || _pressedFrame != Time.frameCount || _usedFrame == Time.frameCount || node.id != _pressedId) return false;
            _usedFrame = Time.frameCount; return true;
        }
        private static void Focus(Node node)
        { if (node != null && _current.selected == node.id && VarginhaInputActions.UsingGamepad) GUI.FocusControl(_current.key + node.id); }
        private static void Highlight(Rect rect, Node node)
        {
            if (node == null || _current.selected != node.id || (!VarginhaInputActions.UsingGamepad && !_showSelection && !node.car)) return;
            var before = GUI.color; GUI.color = new Color(.76f, .81f, .59f);
            GUI.DrawTexture(new Rect(rect.x - 2, rect.y - 2, rect.width + 4, 2), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x - 2, rect.yMax, rect.width + 4, 2), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x - 2, rect.y, 2, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax, rect.y, 2, rect.height), Texture2D.whiteTexture); GUI.color = before;
        }
        public static float HorizontalSlider(Rect rect, float value, float min, float max)
        {
            var node = Register(rect, slider: true); Highlight(rect, node);
            if (node != null && Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            { _current.selected = node.id; _current.selection = node; _showSelection = false; }
            if (node != null && _current == _active && _current.selected == node.id && _sliderId == node.id && _sliderDelta != 0 && _sliderUsed != Time.frameCount)
            { value = Mathf.Clamp(value + _sliderDelta * (max - min), min, max); _sliderDelta = 0; _sliderUsed = Time.frameCount; }
            // Navigation owns keyboard increments too; avoid a second native IMGUI step.
            if (node != null && _current == _active && _current.selected == node.id && Event.current.type == EventType.KeyDown &&
                (Event.current.keyCode == KeyCode.LeftArrow || Event.current.keyCode == KeyCode.RightArrow)) Event.current.Use();
            float result = GUI.HorizontalSlider(rect, value, min, max); return result;
        }
        public static Vector2 BeginScrollView(Rect viewport, Vector2 position, Rect content)
        {
            _scroll = viewport.GetHashCode();
            var selected = _current?.selection;
            if (selected != null && selected.scroll == _scroll)
            {
                if (selected.local.yMin < position.y) position.y = selected.local.yMin;
                else if (selected.local.yMax > position.y + viewport.height) position.y = selected.local.yMax - viewport.height;
                position.y = Mathf.Clamp(position.y, 0, Mathf.Max(0, content.height - viewport.height));
            }
            return GUI.BeginScrollView(viewport, position, content);
        }
        public static void EndScrollView() { GUI.EndScrollView(); _scroll = 0; }
    }
}
