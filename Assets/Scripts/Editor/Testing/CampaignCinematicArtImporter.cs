using UnityEditor;
using UnityEngine;

namespace Game.Editor.Testing
{
    public sealed class CampaignCinematicArtImporter:AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if(!assetPath.StartsWith("Assets/Resources/Varginha/CinemaOpening/")&&!assetPath.StartsWith("Assets/Resources/Varginha/TrailPuzzle/"))return;
            var t=(TextureImporter)assetImporter;
            t.textureType=TextureImporterType.Default;t.filterMode=FilterMode.Point;t.wrapMode=TextureWrapMode.Clamp;
            t.mipmapEnabled=false;t.isReadable=false;t.alphaIsTransparency=true;
            t.npotScale=TextureImporterNPOTScale.None;t.textureCompression=TextureImporterCompression.Uncompressed;
            t.maxTextureSize=assetPath.EndsWith("IgnitionCloseup.png")||assetPath.EndsWith("DashboardPhoto.png")||assetPath.EndsWith("DashboardCartoon.png")||assetPath.EndsWith("Terrain.png")?4096:2048;
        }
    }
}
