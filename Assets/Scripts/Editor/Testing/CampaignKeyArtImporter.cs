using UnityEditor;
using UnityEngine;

namespace Game.Editor.Testing
{
    public sealed class CampaignKeyArtImporter:AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if(!assetPath.StartsWith("Assets/Resources/Varginha/KeyInvestigation/"))return;
            var importer=(TextureImporter)assetImporter;
            importer.textureType=TextureImporterType.Default;importer.filterMode=FilterMode.Point;
            importer.textureCompression=TextureImporterCompression.Uncompressed;importer.mipmapEnabled=false;
            importer.npotScale=TextureImporterNPOTScale.None;importer.alphaSource=TextureImporterAlphaSource.FromInput;
        }
    }
}
