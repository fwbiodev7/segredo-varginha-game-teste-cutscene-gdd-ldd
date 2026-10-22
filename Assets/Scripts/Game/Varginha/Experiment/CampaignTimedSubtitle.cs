using UnityEngine;

namespace Game.Varginha.Experiment
{
    /// <summary>Uses the cinematic's paused scene clock; IMGUI never advances the clock itself.</summary>
    public sealed class CampaignTimedSubtitle
    {
        private readonly CampaignMessagePresentation _message = new();
        private string _text;
        private int _size = -1;
        private float _started;
        public void Draw(string text, float elapsed, float duration = 0)
        {
            if (string.IsNullOrEmpty(text) || !VarginhaGameSettings.Current.subtitles) return;
            if (_text != text || _size != VarginhaGameSettings.Current.subtitleSize)
            {
                _text = text; _size = VarginhaGameSettings.Current.subtitleSize; _started = elapsed;
                _message.Show(null,"",text);
            }
            if (elapsed < _started) _started = 0;
            _message.SeekCaption(elapsed-_started,duration);
            var area = new Rect(230,611,820,66);
            ExperimentGUI.Box(area,new Color(.025f,.045f,.065f,.78f));
            var style = CampaignMessagePresentation.TextStyle(_message.FontSize);style.richText=true;
            GUI.Label(new Rect(250,623,780,44),_message.RenderText,style);
        }
    }
}
