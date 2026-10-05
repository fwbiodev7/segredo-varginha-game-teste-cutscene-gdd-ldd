using System.IO;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CampaignManifestationArtTests
    {
        [TestCase("StoryCharacters/ManifestationBossCombatV2", 8, 4, 160, 192)]
        [TestCase("StoryCharacters/ManifestationChildCombatV2", 8, 4, 96, 128)]
        [TestCase("StoryCharacters/ManifestationBossFlowV1", 6, 4, 160, 192)]
        [TestCase("StoryCharacters/ManifestationChildFlowV1", 6, 4, 96, 128)]
        [TestCase("StoryEffects/BossManifestationCinematicV1", 6, 2, 128, 160)]
        [TestCase("StoryEffects/BossRiftCinematicV1", 6, 2, 160, 160)]
        public void EveryPoseHasTransparentGuttersAndSharpRuntimeImport(string asset, int columns, int rows, int width, int height)
        {
            string path = "Assets/Resources/Varginha/" + asset + ".png";
            var runtime = Resources.Load<Texture2D>("Varginha/" + asset);
            Assert.That(runtime, Is.Not.Null, path);
            Assert.That(runtime.width, Is.EqualTo(columns * width));
            Assert.That(runtime.height, Is.EqualTo(rows * height));
            Assert.That(runtime.filterMode, Is.EqualTo(FilterMode.Point));
            Assert.That(runtime.mipmapCount, Is.EqualTo(1));
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            Assert.That(importer.textureCompression, Is.EqualTo(TextureImporterCompression.Uncompressed));
            Assert.That(importer.alphaIsTransparency, Is.True);

            var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            try
            {
                Assert.That(ImageConversion.LoadImage(source, File.ReadAllBytes(path)), Is.True);
                var pixels = source.GetPixels32();
                for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                {
                    int visible = 0;
                    for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        byte alpha = pixels[(row * height + y) * source.width + column * width + x].a;
                        if (alpha > 128) visible++;
                        if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                            Assert.That(alpha, Is.LessThanOrEqualTo(8), asset + " clipped edge at " + column + "/" + row);
                    }
                    // The closing rift deliberately ends in only a few fading sparks.
                    Assert.That(visible, Is.GreaterThan(width * height * (asset.Contains("Rift") ? .001f : .01f)), asset + " empty pose at " + column + "/" + row);
                    Assert.That(visible, Is.LessThan(width * height * .75f), asset + " opaque rectangular matte at " + column + "/" + row);
                }
            }
            finally { Object.DestroyImmediate(source); }
        }
    }
}
