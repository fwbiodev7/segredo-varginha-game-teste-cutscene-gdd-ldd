using UnityEditor;
using UnityEngine;
namespace Game.Editor.Testing
{
    public sealed class CampaignHouseFurnitureImporter:AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if(!assetPath.StartsWith("Assets/Resources/Varginha/HouseFurniture/")||!assetPath.EndsWith(".png"))return;
            var importer=(TextureImporter)assetImporter;importer.textureType=TextureImporterType.Default;
            importer.isReadable=false;importer.mipmapEnabled=false;importer.filterMode=FilterMode.Point;
            importer.alphaIsTransparency=true;importer.wrapMode=TextureWrapMode.Clamp;importer.npotScale=TextureImporterNPOTScale.None;
            importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=2048;
        }
    }
}
