using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    [Serializable]
    public sealed class GameSettingsData
    {
        public float master = .8f, music = .55f, effects = .75f, voice = .8f;
        public int resolution = 1, display = 1, frameRate = 60, difficulty = 1, subtitleSize = 1;
        public bool vsync = true, subtitles = true, reducedMotion, interactionHints = true;
        public bool vibration = true;
        public float vibrationIntensity = .65f;
        public int inputVersion = 1;
        public void Validate()
        {
            master = Mathf.Clamp01(master); music = Mathf.Clamp01(music);
            effects = Mathf.Clamp01(effects); voice = Mathf.Clamp01(voice);
            resolution = Mathf.Clamp(resolution, 0, 3); display = Mathf.Clamp(display, 0, 2);
            difficulty = Mathf.Clamp(difficulty, 0, 2); subtitleSize = Mathf.Clamp(subtitleSize, 0, 2);
            frameRate = frameRate == 120 ? 120 : 60;
            vibrationIntensity = Mathf.Clamp01(vibrationIntensity);
        }
    }
    public static class VarginhaGameSettings
    {
        private const string Key = "Varginha.Experiment.Settings.v1";
        private static GameSettingsData _current;
        private static int _tab;
        public static GameSettingsData Current
        {
            get
            {
                if (_current != null) return _current;
                try { _current = JsonUtility.FromJson<GameSettingsData>(PlayerPrefs.GetString(Key, "")); }
                catch (ArgumentException) { _current = null; }
                _current ??= new GameSettingsData();
                if(_current.inputVersion<1){_current.vibration=true;_current.vibrationIntensity=.65f;_current.inputVersion=1;}
                _current.Validate(); return _current;
            }
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            _current = null;
            var go = new GameObject("Configurações_Áudio");
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.AddComponent<VarginhaSettingsAudio>();
            Apply(false);
        }
        public static void Save()
        {
            Current.Validate(); PlayerPrefs.SetString(Key, JsonUtility.ToJson(Current)); PlayerPrefs.Save();
            Apply(false);
        }
        public static void Apply(bool display)
        {
            var value = Current;
            AudioListener.volume = value.master;
            QualitySettings.vSyncCount = value.vsync ? 1 : 0;
            Application.targetFrameRate = value.frameRate;
            VarginhaDifficulty.Select((InvestigationDifficulty)value.difficulty);
            if (display)
            {
                int[] widths = { 960, 1280, 1600, 1920 }, heights = { 540, 720, 900, 1080 };
                var mode = value.display == 0 ? FullScreenMode.Windowed : value.display == 1
                    ? FullScreenMode.FullScreenWindow : FullScreenMode.ExclusiveFullScreen;
                Screen.SetResolution(widths[value.resolution], heights[value.resolution], mode);
            }
        }
        public static bool Draw(Action controls = null)
        {
            if(VarginhaInputActions.CancelPressed)return true;
            bool enabled=GUI.enabled;GUI.enabled=enabled&&!VarginhaGamepadBindings.Suspended;
            VarginhaGamepadUI.Begin("settings:"+SceneManager.GetActiveScene().name+":"+_tab,true,100,
                verticalRows: true, initialSelection: new Rect(155 + _tab * 245, 153, 230, 42));
            ExperimentGUI.Init(); var matrix = ExperimentGUI.BeginCanvas();
            ExperimentGUI.Panel(new Rect(120, 70, 1040, 585));
            ExperimentGUI.Label(new Rect(155, 92, 900, 45), "CONFIGURAÇÕES", true);
            string[] tabs = { "CONTROLES", "ÁUDIO", "VÍDEO", "ACESSIBILIDADE" };
            for (int i = 0; i < 4; i++)
                if (ExperimentGUI.Button(new Rect(155 + i * 245, 153, 230, 42), tabs[i])) _tab = i;
            var settings = Current;
            string old = JsonUtility.ToJson(settings);
            if (_tab == 0)
            {
                CampaignControllerSettings.DrawTabs();
                if(CampaignControllerSettings.ControllerView) CampaignControllerSettings.Draw();
                else
                {
                ExperimentGUI.Label(new Rect(175, 263, 900, 35), "WASD / setas: andar • Shift: correr • E: examinar • W: Fusca", small: true);
                if (controls != null && ExperimentGUI.Button(new Rect(175, 300, 890, 48), "REMAPEAR TECLADO E MOUSE")) controls();
                else if (controls == null) ExperimentGUI.Label(new Rect(175, 300, 890, 48), "Remapeamento completo disponível nas configurações do menu principal.", small: true);
                ExperimentGUI.Label(new Rect(175, 378, 890, 35), "DIFICULDADE DAS FASES COM COMBATE", small: true);
                string[] names = { "FÁCIL", "MÉDIO", "DIFÍCIL" };
                for (int i = 0; i < 3; i++) if (ExperimentGUI.Button(new Rect(175 + i * 297, 420, 280, 45),
                    (settings.difficulty == i ? "• " : "") + names[i])) settings.difficulty = i;
                ExperimentGUI.Label(new Rect(175, 486, 890, 38), "A infância do Ato I não possui combate, independentemente da dificuldade.", small: true);
                }
            }
            else if (_tab == 1)
            {
                settings.master = Slider(230, "VOLUME GERAL", settings.master);
                settings.music = Slider(295, "MÚSICA / AMBIÊNCIA", settings.music);
                settings.effects = Slider(360, "EFEITOS", settings.effects);
                settings.voice = Slider(425, "VOZES", settings.voice);
            }
            else if (_tab == 2)
            {
                string[] resolutions = { "960 × 540", "1280 × 720", "1600 × 900", "1920 × 1080" };
                ExperimentGUI.Label(new Rect(175, 225, 360, 36), "RESOLUÇÃO", small: true);
                if (ExperimentGUI.Button(new Rect(570, 225, 495, 42), resolutions[settings.resolution])) settings.resolution = (settings.resolution + 1) % 4;
                string[] modes = { "JANELA", "JANELA SEM BORDAS", "TELA CHEIA" };
                ExperimentGUI.Label(new Rect(175, 290, 360, 36), "MODO DE EXIBIÇÃO", small: true);
                if (ExperimentGUI.Button(new Rect(570, 290, 495, 42), modes[settings.display])) settings.display = (settings.display + 1) % 3;
                settings.vsync = Toggle(365, "SINCRONIZAÇÃO VERTICAL", settings.vsync);
                if (ExperimentGUI.Button(new Rect(175, 427, 430, 42), "LIMITE: " + settings.frameRate + " FPS")) settings.frameRate = settings.frameRate == 60 ? 120 : 60;
                if (ExperimentGUI.Button(new Rect(635, 427, 430, 42), "APLICAR EXIBIÇÃO")) Apply(true);
                ExperimentGUI.Label(new Rect(175, 490, 890, 36), "Pixel art com filtro de pontos; a câmera permanece 2D topview.", small: true);
            }
            else
            {
                settings.subtitles = Toggle(230, "LEGENDAS", settings.subtitles);
                settings.reducedMotion = Toggle(295, "REDUZIR DISTORÇÕES E CLARÃO", settings.reducedMotion);
                settings.interactionHints = Toggle(360, "INDICAÇÕES DE INTERAÇÃO", settings.interactionHints);
                string[] sizes = { "PEQUENA", "MÉDIA", "GRANDE" };
                if (ExperimentGUI.Button(new Rect(175, 417, 890, 38), "TAMANHO DAS LEGENDAS: " + sizes[settings.subtitleSize])) settings.subtitleSize = (settings.subtitleSize + 1) % 3;
                settings.vibration = Toggle(468, "VIBRAÇÃO", settings.vibration);
                settings.vibrationIntensity = Slider(522, "INTENSIDADE DA VIBRAÇÃO", settings.vibrationIntensity);
                if(!settings.vibration)VarginhaRumble.Stop();
            }
            if (old != JsonUtility.ToJson(settings)) Save();
            if (ExperimentGUI.Button(new Rect(155, 580, 370, 45), "RESTAURAR CONFIGURAÇÕES")) { _current = new GameSettingsData(); Save(); }
            bool close = ExperimentGUI.Button(new Rect(745, 580, 370, 45), "VOLTAR");
            VarginhaGamepadUI.End(); GUI.matrix = matrix; GUI.enabled=enabled; return close;
        }
        private static float Slider(float y, string label, float value)
        {
            ExperimentGUI.Label(new Rect(175, y, 390, 36), label + " " + Mathf.RoundToInt(value * 100) + "%", small: true);
            return VarginhaGamepadUI.HorizontalSlider(new Rect(590, y + 12, 475, 28), value, 0, 1);
        }
        private static bool Toggle(float y, string label, bool value)
        {
            if (ExperimentGUI.Button(new Rect(175, y, 890, 42), label + (value ? " • LIGADO" : " • DESLIGADO"))) value = !value;
            return value;
        }
    }
    public sealed class VarginhaSettingsAudio : MonoBehaviour
    {
        private readonly Dictionary<AudioSource, float> _bases = new();
        private float _next;
        private void Update()
        {
            if (Time.unscaledTime < _next) return;
            _next = Time.unscaledTime + .3f;
            foreach (var source in FindObjectsByType<AudioSource>())
            {
                // Cinematic beds own their evolving mix and apply the same music slider.
                if (source.GetComponent<CampaignAmbientBridge>() != null) continue;
                if (!_bases.TryGetValue(source, out float baseline)) { baseline = source.volume; _bases[source] = baseline; }
                var settings = VarginhaGameSettings.Current;
                string label = source.clip != null ? source.clip.name : source.name;
                bool voice = label.Contains("NewsAnchor") || label.Contains("Witness") || label.Contains("Memory");
                bool mechanical = label == "Campaign_Engine";
                source.volume = baseline * (voice ? settings.voice : source.loop && !mechanical ? settings.music : settings.effects);
            }
            var removed = new List<AudioSource>();
            foreach (var source in _bases.Keys) if (source == null) removed.Add(source);
            foreach (var source in removed) _bases.Remove(source);
        }
    }
}
