using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Game.Varginha.Experiment;

namespace Game.Editor.Testing
{
    // Runs explicitly inside the user's Editor; never edits scene or importer YAML.
    public static class CampaignCasaraoIntegration
    {
        public static void Import()
        {
            if(Application.isPlaying)throw new InvalidOperationException("Stop Play mode before importing mansion artwork.");
            const string folder="Assets/Resources/Varginha/IllustratedMaps/";
            foreach(string name in new[]{"CasaraoExteriorBase","CasaraoInteriorBase","CasaraoEsconderijoBase","CasaraoProps",
                "Child1996Divisoes","AdultEmptyDivisoes","IndustrialLibraryDivisoes","OuzanaDivisoes",
                "WorkshopEmptyDivisoes","Continuation16Divisoes","Continuation18Divisoes"})
            {
                string path=folder+name+".png";
                AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
                CampaignIllustratedMapBuilder.ConfigureTexture(path);
                if(name=="Continuation18Divisoes")
                {
                    // Preserve the panoramic source's 2155 pixels without importer scaling.
                    var panoramic=(TextureImporter)AssetImporter.GetAtPath(path);
                    panoramic.maxTextureSize=4096;
                    panoramic.SaveAndReimport();
                }
                if(name!="CasaraoProps")continue;
                var importer=(TextureImporter)AssetImporter.GetAtPath(path);
                importer.alphaSource=TextureImporterAlphaSource.FromInput;
                importer.alphaIsTransparency=true;
                importer.SaveAndReimport();
            }
            AssetDatabase.ImportAsset(folder+"Layouts.json",ImportAssetOptions.ForceSynchronousImport);
            CampaignIllustratedMaps.Reload();
            Directory.CreateDirectory("Preview/Casarao20261009");
            File.WriteAllText("Preview/Casarao20261009/Imported.txt",DateTime.UtcNow.ToString("O"));
        }
    }
}
