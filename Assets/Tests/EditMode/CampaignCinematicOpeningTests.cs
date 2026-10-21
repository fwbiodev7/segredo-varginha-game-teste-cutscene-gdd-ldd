using System.Collections.Generic;
using System.Linq;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public sealed class CampaignCinematicOpeningTests
    {
        [Test] public void DigitalTearsHaveReferenceColoursAndStayInsideTheImage()
        {
            var bands=new List<CampaignSignalGlitch.Band>();var colours=new HashSet<Color32>();
            for(float t=0;t<.46f;t+=.04f)
            {
                CampaignSignalGlitch.Sample(t,384,216,false,bands);
                Assert.That(bands.Count,Is.GreaterThan(20));
                foreach(var b in bands)
                {
                    Assert.That(b.rect.xMin,Is.GreaterThanOrEqualTo(0));Assert.That(b.rect.yMin,Is.GreaterThanOrEqualTo(0));
                    Assert.That(b.rect.xMax,Is.LessThanOrEqualTo(384));Assert.That(b.rect.yMax,Is.LessThanOrEqualTo(216));
                    Assert.That(b.rect.width,Is.GreaterThan(b.rect.height*3));colours.Add(b.color);
                }
            }
            foreach(var c in CampaignSignalGlitch.Palette)Assert.That(colours.Contains(c),Is.True);
        }
        [Test] public void ReadableHoldsDoNotReplaceTheUnderlyingImage()
        {
            var pixels=Enumerable.Repeat(new Color32(11,29,43,255),384*216).ToArray();var before=pixels.ToArray();
            CampaignSignalGlitch.Apply(pixels,384,216,1.4f,false);
            Assert.That(pixels.SequenceEqual(before),Is.True);
            CampaignSignalGlitch.Apply(pixels,384,216,.1f,false);
            Assert.That(pixels.SequenceEqual(before),Is.False);
            Assert.That(pixels.Count(p=>p.Equals(before[0])),Is.GreaterThan(pixels.Length*.70f),"Signal damage must remain local; captions are drawn outside this image.");
        }
        [Test] public void ReducedMotionHasAStableLowAlphaSignature()
        {
            var a=new List<CampaignSignalGlitch.Band>();var b=new List<CampaignSignalGlitch.Band>();
            CampaignSignalGlitch.Sample(0,384,216,true,a);CampaignSignalGlitch.Sample(99,384,216,true,b);
            Assert.That(a.Count,Is.EqualTo(3));
            for(int i=0;i<a.Count;i++){Assert.That(a[i].rect,Is.EqualTo(b[i].rect));Assert.That(a[i].color,Is.EqualTo(b[i].color));Assert.That(a[i].color.a,Is.LessThan(64));Assert.That(a[i].displacement,Is.Zero);}
        }
        [Test] public void SignalAnimationDoesNotConsumeGameplayRandomness()
        {
            var saved=Random.state;
            try{Random.InitState(847);float expected=Random.value;Random.InitState(847);
                CampaignSignalGlitch.Sample(.2f,384,216,false,new List<CampaignSignalGlitch.Band>());
                Assert.That(Random.value,Is.EqualTo(expected));}
            finally{Random.state=saved;}
        }
        [Test] public void CabinInterferenceTravelsAcrossTheImageWithoutBlanketingIt()
        {
            var bands=new List<CampaignSignalGlitch.Band>();var regions=new HashSet<int>();
            int shortest=384,longest=0;bool translucent=false;
            for(float time=0;time<7.2f;time+=.17f)
            {
                CampaignSignalGlitch.Sample(time,384,216,false,bands);int area=0;
                foreach(var b in bands)
                {
                    regions.Add((b.rect.center.x<192?0:1)+(b.rect.center.y<108?0:2));
                    shortest=Mathf.Min(shortest,b.rect.width);longest=Mathf.Max(longest,b.rect.width);
                    translucent|=b.color.a<255;area+=b.rect.width*b.rect.height;
                }
                Assert.That(area,Is.LessThan(384*216*.4f),"The driver and scenery must retain open space.");
            }
            Assert.That(regions.Count,Is.EqualTo(4));Assert.That(longest,Is.GreaterThan(shortest*3));Assert.That(translucent,Is.True);
        }
        [Test] public void FuscaSequencePreservesGddRevealOrderAndAReadableDuration()
        {
            var titles=CampaignFuscaCinema.Shots.Select(s=>s.title).ToArray();
            CollectionAssert.AreEqual(new[]{"VARGINHA • 2026","O RÁDIO","O PAINEL","UMA VOZ CONHECIDA","SILÊNCIO","NINGUÉM TOCOU NA CHAVE","INDUSTRIAL"},titles);
            Assert.That(CampaignFuscaCinema.Shots[3].caption,Does.Contain("Não deixa ela sair."));
            Assert.That(CampaignFuscaCinema.Shots.Sum(s=>s.duration),Is.InRange(25,40));
            Assert.That(CampaignFuscaCinema.DriverFrame(.28f,true,false),Is.Not.EqualTo(CampaignFuscaCinema.DriverFrame(0,true,false)));
            Assert.That(CampaignFuscaCinema.DriverFrame(3,true,true),Is.Zero);
            Assert.That(CampaignFuscaCinema.NeedleAngle(1,true,true,false),Is.EqualTo(-140));
        }
        [Test] public void OldSavesRemainReadableAndWatchedFlagsCannotAddProgress()
        {
            var old=JsonUtility.FromJson<CampaignStory>("{\"version\":3,\"phase\":2}");old.Repair();
            Assert.That(old.cinematicsSeen,Is.Zero);old.cinematicsSeen=255;old.Repair();
            Assert.That(old.cinematicsSeen,Is.EqualTo(15));Assert.That(old.house.Complete,Is.False);Assert.That(old.CanLeaveHouse,Is.False);
            var restored=JsonUtility.FromJson<CampaignStory>(JsonUtility.ToJson(old));Assert.That(restored.cinematicsSeen,Is.EqualTo(15));
        }
        [Test] public void SchoolExitUsesTheGameplayLayoutAndAConnectedWalkToItsSpawn()
        {
            var layout=CampaignIllustratedMaps.Get(4);var plan=CampaignMapPlan.Create(4);
            Assert.That(layout.image,Is.EqualTo("School"));
            Assert.That(CampaignFuscaCinema.SchoolArrivalRoute.Count,Is.GreaterThan(1));
            for(float t=.65f;t<5.8f;t+=.06f)
                Assert.That(plan.IsClear(CampaignFuscaCinema.ArrivalFootPosition(t,false)),Is.True,"Arrival walk at "+t);
            Assert.That(Vector2.Distance(CampaignFuscaCinema.ArrivalFootPosition(5.8f,false),plan.spawn),Is.LessThan(.02f));
            Assert.That(CampaignFuscaCinema.ArrivalFootPosition(0,true),Is.EqualTo(CampaignFuscaCinema.ArrivalFootPosition(5.8f,true)));
            Assert.That(CampaignSignalGlitch.BurstStrength(.6f),Is.GreaterThan(.5f));
            Assert.That(CampaignSignalGlitch.BurstStrength(1.4f),Is.Zero);
        }
        [TestCase("FuscaCabinMorning")] [TestCase("FuscaCabinPhotoShell")] [TestCase("FuscaDashboardPhoto")] [TestCase("IgnitionCloseup")]
        public void NewCinemaAssetsHaveNativePointImportsWithoutCpuCopies(string name)
        {
            string path="Assets/Resources/Varginha/CinemaOpening/"+name+".png";
            var t=AssetDatabase.LoadAssetAtPath<Texture2D>(path);Assert.That(t,Is.Not.Null);
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            Assert.That(importer.filterMode,Is.EqualTo(FilterMode.Point));Assert.That(importer.mipmapEnabled,Is.False);
            Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));Assert.That(importer.isReadable,Is.False);
            Assert.That(importer.npotScale,Is.EqualTo(TextureImporterNPOTScale.None));Assert.That(importer.maxTextureSize,Is.GreaterThanOrEqualTo(t.width));
        }
    }
}
