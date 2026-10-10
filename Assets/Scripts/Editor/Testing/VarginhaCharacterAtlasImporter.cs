using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.Testing
{
    // Keep authored frame coordinates intact even when older builders reimport art.
    public sealed class VarginhaCharacterAtlasImporter : AssetPostprocessor
    {
        const string ManifestPath = "Assets/Resources/Varginha/CharacterFrameGeometry.json";
        [Serializable] sealed class Sheet { public string path; public int width, height; }
        [Serializable] sealed class Manifest { public Sheet[] sheets; }
        static DateTime _modified;
        static Dictionary<string, Sheet> _sheets;
        public override int GetPostprocessOrder() => 1000;

        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Resources/Varginha/") || !File.Exists(ManifestPath)) return;
            var modified = File.GetLastWriteTimeUtc(ManifestPath);
            if (_sheets == null || _modified != modified)
            {
                _sheets = new Dictionary<string, Sheet>(); _modified = modified;
                foreach (var sheet in JsonUtility.FromJson<Manifest>(File.ReadAllText(ManifestPath)).sheets)
                    _sheets[sheet.path] = sheet;
            }
            if (!_sheets.TryGetValue(assetPath, out var source)) return;
            var importer = (TextureImporter)assetImporter;
            int size = Mathf.Clamp(Mathf.NextPowerOfTwo(Mathf.Max(source.width,source.height)),32,8192);
            importer.maxTextureSize = size;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.wrapMode = TextureWrapMode.Clamp;
            foreach (var platform in new[] { "Standalone", "Android", "iPhone", "WebGL" })
            {
                var settings = importer.GetPlatformTextureSettings(platform);
                if (!settings.overridden) continue;
                settings.maxTextureSize = size;
                settings.textureCompression = TextureImporterCompression.Uncompressed;
                settings.format = TextureImporterFormat.RGBA32;
                importer.SetPlatformTextureSettings(settings);
            }
        }
    }
}
