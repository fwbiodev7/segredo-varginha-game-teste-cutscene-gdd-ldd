using UnityEngine;

namespace Game.Varginha.Experiment
{
    public enum CampaignTransitionStyle { Fade, Crossfade, CameraPan, Paranormal }

    [System.Serializable]
    public struct CampaignTransitionProfile
    {
        public CampaignTransitionStyle style;
        [Range(.1f, 1.5f)] public float duration;
        public bool letterbox;
        public string ambience;

        public static CampaignTransitionProfile ForScene(string scene)
        {
            string environment = scene.Contains("Fase3_") ? "Road" : scene.Contains("Fase7_") ? "Forest"
                : scene.Contains("Fase8_") || scene.Contains("Fase14_") ? "Church"
                : scene.Contains("Fase9_") ? "Lab"
                : scene.Contains("Fase4_") || scene.Contains("Fase5_") || scene.Contains("Fase11_") || scene.Contains("Fase21_") && scene.Contains("Area1") ? "School"
                : scene.Contains("Fase6_") || scene.Contains("Fase10_") || scene.Contains("Fase16_") || scene.Contains("Fase17_") || scene.Contains("Fase18_") || scene.Contains("Fase20_") || scene.Contains("Fase21_") ? "Workshop" : "House";
            var style = scene.Contains("Area") ? CampaignTransitionStyle.Crossfade
                : scene.Contains("Fase3_") || scene.Contains("Fase10_") ? CampaignTransitionStyle.CameraPan
                : scene.Contains("Fase15_") ? CampaignTransitionStyle.Paranormal : CampaignTransitionStyle.Fade;
            return new CampaignTransitionProfile { style = style, duration = style == CampaignTransitionStyle.Crossfade ? .48f : .38f,
                letterbox = style == CampaignTransitionStyle.CameraPan, ambience = scene.StartsWith("Ato") ? environment : null };
        }
    }
}
