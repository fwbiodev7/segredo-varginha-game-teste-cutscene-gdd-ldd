using System.IO;
using UnityEngine;

namespace Game.Editor.Testing
{
    public static class CampaignDirectionalReferences
    {
        public static void Export()
        {
            string folder=System.Environment.GetEnvironmentVariable("VARGINHA_CINEMA_QA");Directory.CreateDirectory(folder+"/references");
            foreach(string role in new[]{"Rusher","Skirmisher","Sentinel"})
            {
                var sprite=Game.Varginha.VarginhaPixelArtSprites.Create("ET_Subordinate_"+role,new Color(.68f,.36f,.18f));
                File.WriteAllBytes(folder+"/references/ET_"+role+".png",sprite.texture.EncodeToPNG());
            }
        }
    }
}
