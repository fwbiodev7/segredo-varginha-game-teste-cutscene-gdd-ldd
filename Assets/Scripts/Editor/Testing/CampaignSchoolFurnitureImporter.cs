using UnityEditor;
using UnityEngine;

namespace Game.Editor.Testing
{
    public sealed class CampaignSchoolFurnitureImporter:AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if(assetPath!="Assets/Resources/Varginha/StoryCharacters/RenanDeskProps.png")return;
            var t=(TextureImporter)assetImporter;t.textureType=TextureImporterType.Default;
            t.filterMode=FilterMode.Point;t.mipmapEnabled=false;t.isReadable=false;
            t.alphaIsTransparency=true;t.wrapMode=TextureWrapMode.Clamp;
            t.npotScale=TextureImporterNPOTScale.None;t.textureCompression=TextureImporterCompression.Uncompressed;
            t.maxTextureSize=4096;
        }
    }
}
