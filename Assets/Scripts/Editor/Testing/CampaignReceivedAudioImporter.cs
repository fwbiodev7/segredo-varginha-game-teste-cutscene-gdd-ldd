using UnityEditor;
using UnityEngine;

namespace Game.Editor.Testing
{
    public sealed class CampaignReceivedAudioImporter:AssetPostprocessor
    {
        private void OnPreprocessAudio()
        {
            if(!assetPath.StartsWith("Assets/Resources/Varginha/Audio/Received20261021/"))return;
            var importer=(AudioImporter)assetImporter;
            string name=System.IO.Path.GetFileNameWithoutExtension(assetPath);
            bool bed=name.EndsWith("Ambience")||name=="Engine"||name=="FabioShield";
            var settings=importer.defaultSampleSettings;
            settings.loadType=bed?AudioClipLoadType.Streaming:AudioClipLoadType.DecompressOnLoad;
            settings.compressionFormat=bed?AudioCompressionFormat.Vorbis:AudioCompressionFormat.PCM;
            settings.quality=.8f;settings.sampleRateSetting=AudioSampleRateSetting.PreserveSampleRate;
            settings.preloadAudioData=!bed;
            importer.defaultSampleSettings=settings;importer.forceToMono=!bed;
            importer.loadInBackground=bed;
        }
    }
}
