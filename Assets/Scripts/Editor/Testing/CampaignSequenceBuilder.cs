using System.IO;
using System.Linq;
using Game.Varginha.Experiment;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.Testing
{
    public static class CampaignSequenceBuilder
    {
        [MenuItem("Varginha/Campanha/Aplicar sequência de 14 fases")]
        public static void Apply()
        {
            // Remove the retired chapter through the Editor API, including its metadata.
            var retired = new[] { CampaignContinuationDefinition.SceneName(17), CampaignContinuationDefinition.SceneName(17,1), CampaignContinuationDefinition.SceneName(17,2) };
            var scenes = EditorBuildSettings.scenes.Where(s => !retired.Contains(Path.GetFileNameWithoutExtension(s.path))).ToArray();
            foreach(var name in retired) AssetDatabase.DeleteAsset("Assets/Scenes/"+name+".unity");
            AssetDatabase.DeleteAsset("Assets/Resources/Varginha/IllustratedMaps/Continuation17.png");
            foreach (var scene in scenes)
            {
                string name = Path.GetFileNameWithoutExtension(scene.path);
                if (new[] { 3, 5, 16, 19 }.Any(id => name == CampaignStorySave.Scene(id))) scene.enabled = false;
                else if (CampaignSequence.Entries.Concat(new[] { 10, 13 }).Any(id => name == CampaignStorySave.Scene(id))
                    || name == CampaignContinuationDefinition.SceneName(12, 1)
                    || name == CampaignContinuationDefinition.SceneName(21, 1)) scene.enabled = true;
            }
            EditorBuildSettings.scenes = scenes;
            AssetDatabase.SaveAssets();
            Debug.Log("CAMPAIGN_SEQUENCE_READY="+CampaignSequence.Count);
        }
    }
}
