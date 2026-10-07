using Game.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Varginha.Experiment
{
    internal static class CampaignControllerSettings
    {
        static Texture2D _controller;
        static int _view = -1;
        static readonly string[] DiagramOrder={"Run","AllyCommand","Jump","Journal","SupportCommand","Interact","Inventory","Attack","Dodge","Flashlight"};
        public static bool ControllerView => _view < 0 ? VarginhaInputActions.UsingGamepad : _view == 1;
        public static void DrawTabs()
        {
            if(ExperimentGUI.Button(new Rect(175, 215, 430, 38), (ControllerView ? "" : "• ") + "TECLADO / MOUSE")) _view=0;
            if(ExperimentGUI.Button(new Rect(635, 215, 430, 38), (ControllerView ? "• " : "") + "CONTROLE")) _view=1;
        }
        public static void Draw()
        {
            _controller ??= Resources.Load<Texture2D>("Varginha/Interface/ControllerDiagram");
            var image = new Rect(449, 282, 382, 222);
            if(_controller != null) GUI.DrawTexture(image, _controller, ScaleMode.ScaleToFit, true);
            PixelMenuTheme.Label(new Rect(430, 263, 420, 18), Gamepad.current == null ? "CONTROLE DESCONECTADO" : "CONTROLE CONECTADO", 8, ExperimentGUI.Muted, TextAnchor.MiddleCenter);
            bool enabled = GUI.enabled; GUI.enabled = !VarginhaGamepadBindings.Suspended;
            for(int i=0;i<DiagramOrder.Length;i++)
            {
                string command=DiagramOrder[i];
                bool right=i>=5; int row=i%5;
                var rect = new Rect(right?850:175, 279+row*41, 255, 36);
                bool selected=VarginhaGamepadUI.Selected(rect) || rect.Contains(Event.current.mousePosition);
                string control=VarginhaGamepadBindings.Control(command);
                var accent=selected?ExperimentGUI.Paper:new Color(.23f,.34f,.35f);
                Vector2 anchor=image.position+Vector2.Scale(Anchor(control),image.size);
                float edge=right?rect.x:rect.xMax, elbow=right?838:442;
                Line(anchor, new Vector2(elbow, anchor.y), accent);
                Line(new Vector2(elbow, anchor.y), new Vector2(elbow, rect.center.y), accent);
                Line(new Vector2(elbow, rect.center.y), new Vector2(edge, rect.center.y), accent);
                if(selected || VarginhaGamepadBindings.Capturing==command)
                { ExperimentGUI.Box(new Rect(anchor.x-3,anchor.y-3,6,6),ExperimentGUI.Paper); }
                if(PixelMenuTheme.Button(rect, "["+VarginhaGamepadBindings.Label(command)+"] "+VarginhaGamepadBindings.ActionName(command),size:8))
                    VarginhaGamepadBindings.BeginCapture(command);
            }
            PixelMenuTheme.Label(new Rect(420, 480, 440, 18), "ANALÓGICO / D-PAD: MOVER", 8, ExperimentGUI.Muted, TextAnchor.MiddleCenter);
            PixelMenuTheme.Label(new Rect(420, 500, 440, 18), "DIREITO: MIRAR • START: PAUSAR", 8, ExperimentGUI.Muted, TextAnchor.MiddleCenter);
            PixelMenuTheme.Label(new Rect(175, 530, 930, 18), VarginhaGamepadBindings.Capturing != null
                ? "PRESSIONE O NOVO BOTÃO • START / ESC CANCELA"
                : VarginhaGamepadBindings.Message, 8, ExperimentGUI.Paper);
            if(PixelMenuTheme.Button(new Rect(175, 552, 370, 24), "RESTAURAR BOTÕES DO CONTROLE",size:9)) VarginhaGamepadBindings.RestoreDefaults();
            PixelMenuTheme.Label(new Rect(565, 554, 550, 30), "MENUS: A CONFIRMA • B VOLTA (FIXOS)", 8, ExperimentGUI.Muted, TextAnchor.MiddleCenter);
            GUI.enabled=enabled;
        }
        static Vector2 Anchor(string control) => control switch
        {
            "buttonSouth"=>new(.70f,.49f), "buttonEast"=>new(.80f,.41f), "buttonWest"=>new(.62f,.40f), "buttonNorth"=>new(.70f,.31f),
            "leftShoulder"=>new(.29f,.18f), "rightShoulder"=>new(.71f,.18f), "leftTrigger"=>new(.29f,.10f), "rightTrigger"=>new(.71f,.10f),
            "leftStickPress"=>new(.30f,.39f), "rightStickPress"=>new(.62f,.63f), "select"=>new(.455f,.43f), _=>new(.55f,.43f)
        };
        static void Line(Vector2 a, Vector2 b, Color color)
        {
            if(Mathf.Abs(a.x-b.x)<.01f) ExperimentGUI.Box(new Rect(a.x, Mathf.Min(a.y,b.y), 1, Mathf.Max(1,Mathf.Abs(a.y-b.y))),color);
            else ExperimentGUI.Box(new Rect(Mathf.Min(a.x,b.x), a.y, Mathf.Max(1,Mathf.Abs(a.x-b.x)), 1),color);
        }
    }
}
