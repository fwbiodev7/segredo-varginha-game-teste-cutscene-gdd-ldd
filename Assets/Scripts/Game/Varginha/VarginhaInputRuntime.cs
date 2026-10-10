using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Game.Varginha.Experiment;

namespace Game.Varginha
{
    public static class VarginhaInputActions
    {
        private static InputActionAsset _asset;
        private static InputActionMap _gameplay, _ui;
        private static bool _building;
        public static bool UsingGamepad { get; private set; }
        public const float Deadzone = .20f;
        public static InputActionAsset Asset { get { Ensure(); return _asset; } }
        public static Vector2 Move => Gameplay("Move").ReadValue<Vector2>();
        public static Vector2 Aim => Gameplay("Aim").ReadValue<Vector2>();
        public static bool CarPressed => Gameplay("CarInteract").WasPressedThisFrame();
        public static bool JournalPressed => Gameplay("Journal").WasPressedThisFrame();
        public static bool InventoryPressed => Gameplay("Inventory").WasPressedThisFrame();
        public static bool FlashlightPressed => Gameplay("Flashlight").WasPressedThisFrame();
        public static bool PausePressed => Gameplay("Pause").WasPressedThisFrame();
        public static bool CancelPressed => UI("Cancel").WasPressedThisFrame();
        public static string InteractLabel => UsingGamepad ? VarginhaGamepadBindings.Label("Interact") : VarginhaInputBindings.DisplayName(VarginhaInputAction.Interact, false);
        public static string CarLabel => UsingGamepad ? VarginhaGamepadBindings.Label("Interact") : "W";
        public static string CancelLabel => UsingGamepad ? "B" : "ESC";
        public static string JournalLabel => UsingGamepad ? VarginhaGamepadBindings.Label("Journal") : "TAB";
        public static string InventoryLabel => UsingGamepad ? VarginhaGamepadBindings.Label("Inventory") : "G";
        public static string PauseLabel => UsingGamepad ? "START" : "ESC";
        public static string ActionLabel(VarginhaInputAction action)=>UsingGamepad?PadLabel(action):VarginhaInputBindings.DisplayName(action,false);
        public static string MoveLabel=>UsingGamepad?"ANALÓGICO / D-PAD":string.Join(" / ",new[]{
            ActionLabel(VarginhaInputAction.MoveUp),ActionLabel(VarginhaInputAction.MoveLeft),
            ActionLabel(VarginhaInputAction.MoveDown),ActionLabel(VarginhaInputAction.MoveRight)});
        public static string NavigationLabel=>UsingGamepad?"ANALÓGICO / D-PAD":"SETAS";
        public static string SubmitLabel=>UsingGamepad?"A":"ENTER";
        public static string Prompt(string text)
        {
            if(string.IsNullOrEmpty(text))return text;
            text=text.Replace("E / A",InteractLabel).Replace("W / A",CarLabel).Replace("[E]","["+InteractLabel+"]");
            if(!UsingGamepad)return text;
            string result=text.Replace("WASD / SETAS","ANALÓGICO / D-PAD").Replace("WASD","ANALÓGICO / D-PAD")
                .Replace("[W]","["+CarLabel+"]").Replace("[G]","["+InventoryLabel+"]").Replace("[V]","["+VarginhaGamepadBindings.Label("Flashlight")+"]").Replace("[ESPAÇO]","["+InteractLabel+"]")
                .Replace(" E •"," "+InteractLabel+" •").Replace("E: sair",InteractLabel+": sair");
            return System.Text.RegularExpressions.Regex.Replace(result,@"\b(TAB|ESC)\b",m=>m.Value=="TAB"?JournalLabel:"START");
        }
        public static InputAction Gameplay(string name) { Ensure(); return _gameplay.FindAction(name, true); }
        public static InputAction UI(string name) { Ensure(); return _ui.FindAction(name, true); }
        public static InputAction Button(VarginhaInputAction action) => Gameplay(action.ToString());
        public static string PadLabel(VarginhaInputAction action) => action switch
        {
            VarginhaInputAction.MoveUp => "ANALÓGICO / D-PAD ↑", VarginhaInputAction.MoveDown => "ANALÓGICO / D-PAD ↓",
            VarginhaInputAction.MoveLeft => "ANALÓGICO / D-PAD ←", VarginhaInputAction.MoveRight => "ANALÓGICO / D-PAD →",
            _ => VarginhaGamepadBindings.Label(action.ToString())
        };
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            Dispose(); UsingGamepad = false;
            InputSystem.onDeviceChange -= DeviceChanged; InputSystem.onDeviceChange += DeviceChanged;
        }
        public static void Rebuild() { if (_asset != null && !_building) { Dispose(); Ensure(); } }
        public static void Shutdown() { VarginhaGamepadBindings.Reset(); VarginhaRumble.Stop(); Dispose(); }
        private static void Dispose()
        {
            if (_asset == null) return;
            _asset.Disable(); UnityEngine.Object.DestroyImmediate(_asset); _asset = null;
        }
        private static void Ensure()
        {
            if (_asset != null || _building) return;
            _building = true;
            InputSystem.onDeviceChange -= DeviceChanged; InputSystem.onDeviceChange += DeviceChanged;
            _asset = ScriptableObject.CreateInstance<InputActionAsset>(); _asset.name = "VarginhaRuntimeInputs";
            _gameplay = new InputActionMap("Gameplay"); _ui = new InputActionMap("UI");
            _asset.AddActionMap(_gameplay); _asset.AddActionMap(_ui);
            InputSystem.settings.defaultDeadzoneMin = Deadzone; InputSystem.settings.defaultDeadzoneMax = .95f;
            string[] pad = { "leftStick/up", "leftStick/down", "leftStick/left", "leftStick/right", "leftTrigger", "buttonSouth", "buttonWest", "leftShoulder", "buttonEast", "leftStickPress", "rightShoulder" };
            foreach (VarginhaInputAction command in Enum.GetValues(typeof(VarginhaInputAction)))
            {
                var action = Add(_gameplay, command.ToString(), "<Gamepad>/" + pad[(int)command]);
                Key key = VarginhaInputBindings.GetKeyboard(command);
                if (key != Key.None) action.AddBinding("<Keyboard>/" + key.ToString());
                int mouse = VarginhaInputBindings.GetMouseButton(command);
                if (mouse >= 0) action.AddBinding("<Mouse>/" + new[] { "leftButton", "rightButton", "middleButton" }[mouse]);
                if (command == VarginhaInputAction.Interact && key == Key.E)
                { action.AddBinding("<Keyboard>/space"); action.AddBinding("<Keyboard>/enter"); }
                if ((int)command < 4) action.AddBinding("<Gamepad>/dpad/" + new[] { "up", "down", "left", "right" }[(int)command]);
            }
            var move = AddVector(_gameplay, "Move"); move.AddBinding("<Gamepad>/leftStick"); move.AddBinding("<Gamepad>/dpad");
            var keys = move.AddCompositeBinding("2DVector");
            keys.With("Up", KeyboardPath(VarginhaInputAction.MoveUp)).With("Down", KeyboardPath(VarginhaInputAction.MoveDown))
                .With("Left", KeyboardPath(VarginhaInputAction.MoveLeft)).With("Right", KeyboardPath(VarginhaInputAction.MoveRight));
            move.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow").With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
            AddVector(_gameplay, "Aim").AddBinding("<Gamepad>/rightStick");
            Add(_gameplay, "CarInteract", "<Keyboard>/w", "<Gamepad>/buttonSouth");
            Add(_gameplay, "Journal", "<Keyboard>/tab", "<Gamepad>/select");
            Add(_gameplay, "Inventory", "<Keyboard>/g", "<Gamepad>/buttonNorth");
            Add(_gameplay, "Flashlight", "<Keyboard>/v", "<Gamepad>/rightTrigger");
            Add(_gameplay, "Pause", "<Keyboard>/escape", "<Gamepad>/start");
            var navigation = AddVector(_ui, "Navigate"); navigation.AddBinding("<Gamepad>/leftStick"); navigation.AddBinding("<Gamepad>/dpad");
            navigation.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow").With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
            Add(_ui, "Submit", "<Gamepad>/buttonSouth", "<Keyboard>/enter");
            Add(_ui, "Cancel", "<Gamepad>/buttonEast", "<Keyboard>/escape");
            Add(_ui, "KeyboardActivity", "<Keyboard>/anyKey"); Add(_ui, "MouseActivity", "<Mouse>/leftButton", "<Mouse>/rightButton", "<Mouse>/middleButton");
            AddVector(_ui, "PointerActivity").AddBinding("<Mouse>/delta");
            VarginhaGamepadBindings.Apply(_asset);
            _building = false; _asset.Enable();
        }
        private static string KeyboardPath(VarginhaInputAction action)
        { int mouse=VarginhaInputBindings.GetMouseButton(action);return mouse>=0?"<Mouse>/"+new[]{"leftButton","rightButton","middleButton"}[mouse]:"<Keyboard>/"+VarginhaInputBindings.GetKeyboard(action); }
        private static InputAction Add(InputActionMap map, string name, params string[] bindings)
        { var action = map.AddAction(name, InputActionType.Button); foreach (var binding in bindings) action.AddBinding(binding); action.performed += Track; return action; }
        private static InputAction AddVector(InputActionMap map, string name)
        { var action = map.AddAction(name, InputActionType.Value, expectedControlLayout: "Vector2"); action.performed += Track; return action; }
        private static void Track(InputAction.CallbackContext context)
        {
            if (context.action.expectedControlType == "Vector2" && context.ReadValue<Vector2>().sqrMagnitude < .0001f) return;
            UsingGamepad = context.control.device is Gamepad;
        }
        private static void DeviceChanged(InputDevice device, InputDeviceChange change)
        {
            if (device is Gamepad && (change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected || change == InputDeviceChange.Disabled))
            { VarginhaRumble.Stop(); UsingGamepad = false; }
        }
    }

    public static class VarginhaRumble
    {
        private static Gamepad _pad;
        private static float _until;
        public static void Play(float low, float high, float seconds)
        {
            if (!VarginhaGameSettings.Current.vibration || !VarginhaInputActions.UsingGamepad || Gamepad.current == null) return;
            Stop(); _pad = Gamepad.current; _until = Time.unscaledTime + Mathf.Clamp(seconds, .02f, 2f);
            float gain = VarginhaGameSettings.Current.vibrationIntensity;
            Set(Mathf.Clamp01(low) * gain, Mathf.Clamp01(high) * gain);
        }
        public static void Tick() { if (_pad != null && (Time.unscaledTime >= _until || !VarginhaGameSettings.Current.vibration)) Stop(); }
        public static void Stop() { Set(0, 0); _pad = null; _until = 0; }
        private static void Set(float low, float high)
        {
            if (_pad == null || !_pad.added) return;
            try { _pad.SetMotorSpeeds(low, high); }
            catch (NotSupportedException) { _pad = null; }
            catch (InvalidOperationException) { _pad = null; }
        }
    }

    [DefaultExecutionOrder(-30000)]
    public sealed class VarginhaInputRuntime : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<VarginhaInputRuntime>() != null) return;
            var runner = new GameObject("Entrada_teclado_mouse_controle"); DontDestroyOnLoad(runner); runner.AddComponent<VarginhaInputRuntime>();
        }
        private void OnEnable() => SceneManager.sceneLoaded += SceneChanged;
        private void Update() { _ = VarginhaInputActions.Asset; VarginhaGamepadBindings.Tick(); VarginhaGamepadUI.Tick(); VarginhaRumble.Tick(); }
        private void SceneChanged(Scene scene, LoadSceneMode mode) { VarginhaGamepadBindings.Reset(); VarginhaGamepadUI.Reset(); VarginhaRumble.Stop(); }
        private void OnApplicationFocus(bool focus) { if (!focus) { VarginhaGamepadBindings.Reset(); VarginhaRumble.Stop(); } }
        private void OnApplicationQuit() => VarginhaRumble.Stop();
        private void OnApplicationPause(bool pause) { if (pause) VarginhaRumble.Stop(); }
        private void OnDisable() { SceneManager.sceneLoaded -= SceneChanged; VarginhaGamepadBindings.Reset(); VarginhaRumble.Stop(); VarginhaGamepadUI.Reset(); }
    }
}
