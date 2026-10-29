using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;

namespace Game.Tests.EditMode
{
    public sealed class CampaignToroRoutineTests
    {
        [TestCase("wash")][TestCase("food")][TestCase("work")]
        public void CloseRoutineAnchorsRemainClearAndConnected(string id)
        {
            var plan=CampaignMapPlan.Create(2);var target=plan.points.Find(p=>p.id==id).position;
            Assert.That(plan.IsClear(target,.28f),Is.True,id);
            Assert.That(plan.Route(plan.spawn,target,new List<Vector2>()),Is.True,id);
            Assert.That((Vector2)CampaignAdultHouse.RoutineApproach(id)-Vector2.up*.58f,Is.EqualTo(target));
            Assert.That(CampaignAdultHouse.RoutineInteractionRange,Is.LessThan(.75f));
        }
        [Test]public void ToroHasDedicatedNativeArtAndDoesNotReplaceTheFusca()
        {
            var toro=CampaignPassingToro.Side;Assert.That(toro,Is.Not.Null);Assert.That(toro.texture,Is.Not.SameAs(CampaignOriginalFusca.Side.texture));
            Assert.That(toro.texture.name,Is.EqualTo("FiatToroSide"));Assert.That(toro.texture.filterMode,Is.EqualTo(FilterMode.Point));
            Assert.That(toro.texture.mipmapCount,Is.EqualTo(1));Assert.That(toro.rect,Is.EqualTo(new Rect(2,11,123,52)));
            Assert.That(toro.texture.width,Is.EqualTo(128));Assert.That(toro.texture.height,Is.EqualTo(72));
            Assert.That(CampaignOriginalFusca.Side.rect,Is.EqualTo(new Rect(1,35,99,53)));
        }
        [Test]public void ToroWheelsCycleAndReducedMotionKeepsTheBodyAndWheelsStill()
        {
            var frames=new HashSet<int>();
            for(int i=0;i<40;i++)
            {
                float time=CampaignSchoolMusicCar.StartTime+i/36f;frames.Add(CampaignPassingToro.WheelFrameAt(time,false));
                Assert.That(Mathf.Abs(CampaignPassingToro.BodyOffsetAt(time,false)),Is.LessThanOrEqualTo(1));
                Assert.That(CampaignPassingToro.WheelFrameAt(time,true),Is.Zero);Assert.That(CampaignPassingToro.BodyOffsetAt(time,true),Is.Zero);
            }
            Assert.That(frames.Count,Is.EqualTo(8));
        }
        [TestCase("FuscaCabin")][TestCase("FuscaDashboard")]
        public void ClassicCabinKeepsNativeGeometryAndEveryWindowAlphaValue(string name)
        {
            string source="Assets/Resources/Varginha/CinemaOpening/"+name+"Cartoon.png";
            string path="Assets/Resources/Varginha/Revision20261029/"+name+"Classic.png";
            var before=new Texture2D(2,2);var after=new Texture2D(2,2);
            try
            {
                before.LoadImage(File.ReadAllBytes(source));after.LoadImage(File.ReadAllBytes(path));
                Assert.That(after.width,Is.EqualTo(before.width));Assert.That(after.height,Is.EqualTo(before.height));
                var a=before.GetPixels32();var b=after.GetPixels32();Assert.That(b.Select(p=>p.a),Is.EqualTo(a.Select(p=>p.a)),"Every window cutout keeps its original registration.");
                Assert.That(a.Zip(b,(x,y)=>!x.Equals(y)).Count(changed=>changed),Is.GreaterThan(10000),"The new material must actually replace the old paint/wood.");
                var importer=(TextureImporter)AssetImporter.GetAtPath(path);
                Assert.That(importer.filterMode,Is.EqualTo(FilterMode.Point));Assert.That(importer.mipmapEnabled,Is.False);
                Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));Assert.That(importer.isReadable,Is.False);
                Assert.That(importer.npotScale,Is.EqualTo(TextureImporterNPOTScale.None));Assert.That(importer.maxTextureSize,Is.GreaterThanOrEqualTo(after.width));
            }
            finally{Object.DestroyImmediate(before);Object.DestroyImmediate(after);}
        }
    }
}
