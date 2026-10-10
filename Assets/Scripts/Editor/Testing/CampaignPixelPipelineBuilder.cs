using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Editor.Testing
{
    public static class CampaignPixelPipelineBuilder
    {
        public const string Folder = "Assets/Settings";
        [MenuItem("Varginha/Visual/Configurar URP 2D Pixel Art")]
        public static void Configure()
        {
            if (Application.isPlaying) throw new System.InvalidOperationException("Configure outside Play Mode.");
            if (!AssetDatabase.IsValidFolder(Folder)) AssetDatabase.CreateFolder("Assets", "Settings");
            var renderer = AssetDatabase.LoadAssetAtPath<Renderer2DData>(Folder + "/CampaignPixelRenderer.asset");
            if (renderer == null)
            {
                renderer = ScriptableObject.CreateInstance<Renderer2DData>();
                AssetDatabase.CreateAsset(renderer, Folder + "/CampaignPixelRenderer.asset");
            }
            var settings = new SerializedObject(renderer);
            settings.FindProperty("m_LightRenderTextureScale").floatValue = 1;
            settings.FindProperty("m_DefaultMaterialType").intValue = 1; // Existing scenery stays unlit unless explicitly opted in.
            var post = AssetDatabase.LoadAssetAtPath<PostProcessData>("Packages/com.unity.render-pipelines.universal/Runtime/Data/PostProcessData.asset");
            if (post == null) throw new System.InvalidOperationException("URP post-process resources are missing.");
            settings.FindProperty("m_PostProcessData").objectReferenceValue = post;
            settings.ApplyModifiedPropertiesWithoutUndo();
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(Folder + "/CampaignPixelPipeline.asset");
            if (pipeline == null)
            {
                pipeline = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(pipeline, Folder + "/CampaignPixelPipeline.asset");
            }
            pipeline.renderScale = 1; pipeline.msaaSampleCount = 1; pipeline.supportsHDR = false;
            GraphicsSettings.defaultRenderPipeline = pipeline;
            var previous = QualitySettings.GetQualityLevel();
            for (int i = 0; i < QualitySettings.names.Length; i++) { QualitySettings.SetQualityLevel(i, false); QualitySettings.renderPipeline = pipeline; }
            QualitySettings.SetQualityLevel(previous, false);
            EditorUtility.SetDirty(renderer); EditorUtility.SetDirty(pipeline);
            AssetDatabase.SaveAssets();
        }
    }
}
