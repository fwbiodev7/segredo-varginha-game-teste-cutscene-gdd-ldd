using UnityEditor;
using UnityEngine;
namespace Game.Editor.Testing
{
 public sealed class CampaignSupportArtImporter:AssetPostprocessor
 {
  void OnPreprocessTexture()
  {
   if(assetPath!="Assets/Resources/Varginha/StoryCharacters/SupportEffectsV2.png")return;
   var t=(TextureImporter)assetImporter;t.textureType=TextureImporterType.Default;t.isReadable=false;t.mipmapEnabled=false;
   t.alphaIsTransparency=true;t.filterMode=FilterMode.Point;t.wrapMode=TextureWrapMode.Clamp;
   t.npotScale=TextureImporterNPOTScale.None;t.textureCompression=TextureImporterCompression.Uncompressed;t.maxTextureSize=2048;
  }
 }
}
