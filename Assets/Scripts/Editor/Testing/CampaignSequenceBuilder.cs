using System.IO;
using System.Linq;
using Game.Varginha.Experiment;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.Testing
{
    public static class CampaignSequenceBuilder
    {
        [MenuItem("Varginha/Campanha/Aplicar sequência de 15 fases")]
        public static void Apply()
        {
            var scenes = EditorBuildSettings.scenes;
            foreach (var scene in scenes)
            {
                string name = Path.GetFileNameWithoutExtension(scene.path);
                if (new[] { 3, 5, 16, 19 }.Any(id => name == CampaignStorySave.Scene(id))) scene.enabled = false;
                else if (CampaignSequence.Entries.Concat(new[] { 10, 13 }).Any(id => name == CampaignStorySave.Scene(id))
                    || name == CampaignContinuationDefinition.SceneName(12, 1)
                    || name == CampaignContinuationDefinition.SceneName(17, 1)
                    || name == CampaignContinuationDefinition.SceneName(17, 2)
                    || name == CampaignContinuationDefinition.SceneName(21, 1)) scene.enabled = true;
            }
            EditorBuildSettings.scenes = scenes;
            AssetDatabase.SaveAssets();
            Debug.Log("CAMPAIGN_SEQUENCE_READY=15");
        }
    }
}
