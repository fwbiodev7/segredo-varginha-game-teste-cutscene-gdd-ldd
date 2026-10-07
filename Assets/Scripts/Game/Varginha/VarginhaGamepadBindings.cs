using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Game.Varginha
{
    // Gameplay bindings share the existing Input Actions. Menu recovery stays A/B/Start.
    public static class VarginhaGamepadBindings
    {
        public static readonly string[] Commands = { "Interact", "Attack", "Dodge", "Inventory", "Flashlight", "Run", "AllyCommand", "SupportCommand", "Jump", "Journal" };
        static readonly string[] Defaults = { "buttonSouth", "buttonWest", "buttonEast", "buttonNorth", "rightTrigger", "leftTrigger", "leftShoulder", "rightShoulder", "leftStickPress", "select" };
        public static readonly string[] Buttons = { "buttonSouth", "buttonEast", "buttonWest", "buttonNorth", "leftShoulder", "rightShoulder", "leftTrigger", "rightTrigger", "leftStickPress", "rightStickPress", "select" };
        const string Prefix = "Varginha.Controls.Gamepad.";
        static string _capturing;
        static InputActionAsset _disabled;
        static float _deadline;
        static bool _released;
        public static string Capturing => _capturing;
        public static bool Suspended => _disabled != null;
        public static string Message { get; private set; } = "Selecione uma ação e pressione o botão desejado.";

        public static string Control(string command)
        {
            int index = Array.IndexOf(Commands, command);
            if(index < 0) return command == "Pause" ? "start" : "buttonSouth";
            string saved = PlayerPrefs.GetString(Prefix + command, Defaults[index]);
            return Array.IndexOf(Buttons, saved) >= 0 ? saved : Defaults[index];
        }
        public static string Label(string command) => ButtonLabel(Control(command));
        public static string ButtonLabel(string control) => control switch
        {
            "buttonSouth" => "A", "buttonEast" => "B", "buttonWest" => "X", "buttonNorth" => "Y",
            "leftShoulder" => "LB", "rightShoulder" => "RB", "leftTrigger" => "LT", "rightTrigger" => "RT",
            "leftStickPress" => "L3", "rightStickPress" => "R3", "select" => "BACK", "start" => "START", _ => control
        };
        public static string ActionName(string command) => command switch
        {
            "Interact" => "Interagir / Fusca", "Attack" => "Atacar / combo", "Dodge" => "Esquivar", "Inventory" => "Mochila",
            "Flashlight" => "Lanterna", "Run" => "Correr", "AllyCommand" => "Comandar aluno", "SupportCommand" => "Comandar professor",
            "Jump" => "Pular", "Journal" => "Caderno / pistas", _ => command
        };
        public static void Set(string command, string control)
        {
            if(Array.IndexOf(Commands, command) < 0 || Array.IndexOf(Buttons, control) < 0) throw new ArgumentException("Botão ou ação inválida.");
            string previous = Control(command);
            foreach(var other in Commands)
                if(other != command && Control(other) == control) PlayerPrefs.SetString(Prefix + other, previous);
            PlayerPrefs.SetString(Prefix + command, control); PlayerPrefs.Save();
            Apply(VarginhaInputActions.Asset);
        }
        public static void RestoreDefaults()
        {
            foreach(var command in Commands) PlayerPrefs.DeleteKey(Prefix + command);
            PlayerPrefs.Save(); Apply(VarginhaInputActions.Asset);
            Message = "Mapeamento original do controle restaurado.";
        }
        public static void Apply(InputActionAsset asset)
        {
            var gameplay = asset.FindActionMap("Gameplay", true);
            foreach(var command in Commands) ApplyAction(gameplay.FindAction(command, true), Control(command));
            ApplyAction(gameplay.FindAction("CarInteract", true), Control("Interact"));
        }
        static void ApplyAction(InputAction action, string control)
        {
            for(int i=0;i<action.bindings.Count;i++)
                if(action.bindings[i].path.StartsWith("<Gamepad>/", StringComparison.Ordinal))
                    action.ApplyBindingOverride(i, "<Gamepad>/" + control);
        }
        public static void BeginCapture(string command)
        {
            if(Array.IndexOf(Commands, command)<0) return;
            if(Gamepad.current == null) { Message = "Conecte um controle para remapear seus botões."; return; }
            if(Suspended) return;
            _disabled = VarginhaInputActions.Asset; _disabled.Disable(); VarginhaRumble.Stop();
            _capturing = command; _released = false; _deadline = Time.unscaledTime + 15;
            Message = "Solte os botões; depois pressione o novo botão. START / ESC cancela.";
        }
        static bool AnyButtonHeld(Gamepad pad)
        {
            if(pad.startButton.isPressed) return true;
            foreach(var control in Buttons) if(pad.TryGetChildControl<ButtonControl>(control)?.isPressed == true) return true;
            return false;
        }
        public static void Tick()
        {
            if(!Suspended) return;
            var pad = Gamepad.current;
            if(pad == null) { Reset(); Message = "Controle desconectado. O remapeamento foi cancelado."; return; }
            if(_capturing == null)
            {
                if(!AnyButtonHeld(pad)) Reset(true);
                return;
            }
            if(pad.startButton.isPressed || Keyboard.current?.escapeKey.isPressed == true || Time.unscaledTime >= _deadline)
            { CancelCapture(); return; }
            if(!_released) { _released = !AnyButtonHeld(pad); return; }
            foreach(var control in Buttons)
                if(pad.TryGetChildControl<ButtonControl>(control)?.isPressed == true)
                {
                    string command = _capturing; Set(command, control); _capturing = null;
                    Message = ActionName(command) + " → " + ButtonLabel(control) + ". Salvo.";
                    return;
                }
        }
        public static void CancelCapture() { _capturing = null; Message = "Remapeamento cancelado. Os botões anteriores foram mantidos."; }
        public static void Reset(bool keepMessage=false)
        {
            if(_disabled != null) _disabled.Enable();
            _disabled = null; _capturing = null; _released = false;
            if(!keepMessage) Message="Selecione uma ação e pressione o botão desejado. Botões repetidos trocam de ação.";
        }
    }
}
