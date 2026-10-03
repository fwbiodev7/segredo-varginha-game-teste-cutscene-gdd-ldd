using System.Collections.Generic;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CampaignExpansionTests
    {
        [TestCase(1)] [TestCase(3)] [TestCase(4)] [TestCase(5)] [TestCase(6)] [TestCase(7)] [TestCase(8)] [TestCase(9)] [TestCase(10)]
        public void EveryObjectiveHasFootClearanceAndAConnectedRoute(int phase)
        {
            var plan=CampaignMapPlan.Create(phase); var route=new List<Vector2>();
            Assert.That(plan.IsClear(plan.spawn),Is.True,"Spawn");
            foreach(var point in plan.points)
            {
                Assert.That(plan.IsClear(point.position),Is.True,"Fase "+phase+" / "+point.id+" footprint");
                Assert.That(plan.Route(plan.spawn,point.position,route),Is.True,"Fase "+phase+" / "+point.id+" route");
                foreach(var step in route) Assert.That(plan.IsClear(step,.32f),Is.True);
            }
        }
        [Test] public void ExpansionRequiresPhysicalEvidenceAndPreservesOptionalClues()
        {
            var state=new CampaignExpansionState { mapOrder=new[]{0,1,2},sampleOrder=new[]{0,1,2},sealOrder=new[]{0,1,2} };
            Assert.That(state.SolveMap(),Is.False); Assert.That(state.SolveAnchor(1996,1,23),Is.False); Assert.That(state.SolveSamples(),Is.False); Assert.That(state.Stabilize(),Is.False);
            state.visited=7; Assert.That(state.SolveMap(),Is.True); state.forestSigns=3; state.fabioMet=true; state.anchorClues=7;
            Assert.That(state.SolveAnchor(1898,1,23),Is.False); Assert.That(state.SolveAnchor(1996,1,23),Is.True);
            state.labClues=7; Assert.That(state.SolveSamples(),Is.False); state.evidencePresented=true; Assert.That(state.SolveSamples(),Is.True);
            Assert.That(state.Spray(0),Is.True); int remaining=state.reagentCharges; Assert.That(state.Spray(0),Is.False); Assert.That(state.reagentCharges,Is.EqualTo(remaining));
            Assert.That(state.Spray(1)&&state.Spray(2)&&state.Stabilize(),Is.True); Assert.That(state.Complete(10),Is.False); state.testDriven=true; state.truthClues=7;
            var restored=JsonUtility.FromJson<CampaignExpansionState>(JsonUtility.ToJson(state));restored.Repair();
            Assert.That(restored.Complete(10),Is.True); Assert.That(restored.truthClues,Is.EqualTo(7));
        }
        [Test] public void OldSavesAndMalformedOrdersAreRecovered()
        {
            var story=JsonUtility.FromJson<CampaignStory>("{\"version\":1,\"phase\":5}"); story.Repair(); Assert.That(story.expansion,Is.Not.Null); Assert.That(story.phase,Is.EqualTo(5));
            var state=new CampaignExpansionState { mapOrder=new[]{9,9,9},sampleOrder=null,anchorFound=true,reagentUnlocked=true,stabilized=true,testDriven=true,reagentCharges=-1 };
            state.Repair(); Assert.That(state.Complete(10),Is.False); Assert.That(state.reagentCharges,Is.Zero); Assert.That(state.mapOrder,Is.EqualTo(new[]{2,0,1}));
        }
        [Test] public void ArchitectureAndFurnitureHaveSeparateIdempotentLayers()
        {
            var root=new GameObject("TestMap");
            try
            {
                var plan=CampaignMapPlan.Create(6); var map=CampaignMapConstruction.Build(root.transform,plan,false);
                Assert.That(map.Find("02_Mobilia_Colisoes"),Is.Null);
                Assert.That(map.GetComponentsInChildren<Collider2D>().Length,Is.EqualTo(plan.walls.Count));
                CampaignMapConstruction.Furnish(map,plan); CampaignMapConstruction.Furnish(map,plan);
                Assert.That(map.GetComponentsInChildren<Collider2D>().Length,Is.EqualTo(plan.walls.Count+plan.furniture.FindAll(p=>p.footprint.width>0&&p.footprint.height>0).Count));
                Assert.That(CampaignMapConstruction.Build(root.transform,plan),Is.EqualTo(map));
            }
            finally { Object.DestroyImmediate(root); }
        }
        [TestCase(1)] [TestCase(4)] [TestCase(7)] [TestCase(9)] [TestCase(10)]
        public void WorldCollidersMatchPlansIncludingRotatedWalls(int phase)
        {
            var root=new GameObject("ColliderGeometry");
            try
            {
                var plan=CampaignMapPlan.Create(phase);var map=CampaignMapConstruction.Build(root.transform,plan);
                Physics2D.SyncTransforms();
                for(int i=0;i<plan.walls.Count;i++)
                    AssertBounds(map.Find("01_Planta_Paredes_Divisoes/Parede_"+i).GetComponent<Collider2D>().bounds,plan.walls[i]);
                foreach(var prop in plan.furniture)
                    if(prop.footprint.width>0&&prop.footprint.height>0)AssertBounds(map.Find("02_Mobilia_Colisoes/"+prop.name).GetComponent<Collider2D>().bounds,prop.footprint);
                    else Assert.That(map.Find("02_Mobilia_Colisoes/"+prop.name).GetComponent<Collider2D>(),Is.Null);
                if(phase==4||phase==10)
                    Assert.That(map.Find("02_Mobilia_Colisoes/Fusca").GetComponent<SpriteRenderer>().sprite.texture,
                        Is.EqualTo(Resources.Load<Texture2D>("Varginha/Experiment/FuscaTopView")));
            }
            finally{Object.DestroyImmediate(root);}
        }
        private static void AssertBounds(Bounds actual,Rect expected)
        {
            Assert.That(actual.center.x,Is.EqualTo(expected.center.x).Within(.002f));
            Assert.That(actual.center.y,Is.EqualTo(expected.center.y).Within(.002f));
            Assert.That(actual.size.x,Is.EqualTo(expected.width).Within(.002f));
            Assert.That(actual.size.y,Is.EqualTo(expected.height).Within(.002f));
        }
        [Test]public void GeneratedCharacterFramesHaveCompleteOutlinesAndStableFootAnchors()
        {
            foreach(string sheet in new[]{"EdelzioWalk","EdelzioPunch","EdelzioActions","EdelzioInteractions"})
            {
                int columns=sheet=="EdelzioWalk"?4:sheet=="EdelzioPunch"?3:6;
                for(int row=0;row<4;row++)for(int column=0;column<columns;column++)
                {
                    var sprite=CampaignTeamEdelzio.Frame(sheet,row,column);
                    Assert.That(sprite,Is.Not.Null,sheet+row+":"+column);
                    Assert.That(sprite.texture.name,Does.EndWith("V2"));
                    Assert.That(sprite.rect.width,Is.GreaterThan(20));Assert.That(sprite.rect.height,Is.GreaterThan(50));
                    Assert.That(sprite.rect.yMin,Is.GreaterThan(0));Assert.That(sprite.rect.yMax,Is.LessThan(sprite.texture.height));
                    Assert.That(sprite.bounds.min.y,Is.EqualTo(-.58f).Within(.002f));
                    Assert.That(sprite.bounds.size.y,Is.InRange(.7f,2.1f));
                }
            }
        }
        [Test]public void AtmosphericLandmarksUseTheirDedicatedCompleteTransparentSheets()
        {
            foreach(var entry in new[]{("GothicWindow","Church"),("Lectern","Church"),("Sarcophagus","Church"),("MicroscopeBench","Biology"),("SpecimenTank","Biology"),("DNABoard","Biology"),("CityHouse","City"),("CityBakery","City"),("AncientTree","Identity"),("ChapelArch","Identity"),("BusStop","BusStopVarginha")})
            {
                var sprite=CampaignAtmosphereAssets.Prop(entry.Item1);Assert.That(sprite,Is.Not.Null,entry.Item1);
                Assert.That(sprite.texture.name,Is.EqualTo(entry.Item2));
                Assert.That(sprite.texture.GetPixel(0,0).a,Is.LessThan(.05f));
                Assert.That(sprite.rect.width,Is.GreaterThan(50));Assert.That(sprite.rect.height,Is.GreaterThan(50));
                Assert.That(sprite.rect.xMin,Is.GreaterThanOrEqualTo(0));Assert.That(sprite.rect.xMax,Is.LessThanOrEqualTo(sprite.texture.width));
            }
        }
        [Test]public void LongUrbanMapFloorLayersRemainBelowBuildingsAndFurniture()
        {
            var root=new GameObject("CityLayers");
            try
            {
                var map=CampaignMapConstruction.Build(root.transform,CampaignMapPlan.Create(3));
                foreach(var renderer in map.Find("01_Planta_Paredes_Divisoes").GetComponentsInChildren<SpriteRenderer>())
                    if(renderer.name=="Piso")Assert.That(renderer.sortingOrder,Is.LessThan(0));
                Assert.That(map.Find("02_Mobilia_Colisoes/Ponto de ônibus 0").GetComponent<SpriteRenderer>().sprite.texture.name,Is.EqualTo("BusStopVarginha"));
            }
            finally{Object.DestroyImmediate(root);}
        }
        [Test]public void CryptAndBiologyLabHaveIndependentFloorLightingAndEquipment()
        {
            foreach(int phase in new[]{8,9})
            {
                var root=new GameObject("Atmosphere"+phase);
                try
                {
                    var map=CampaignMapConstruction.Build(root.transform,CampaignMapPlan.Create(phase));
                    Assert.That(map.GetComponentInChildren<Game.Varginha.VarginhaSoftLighting>().SourceCount,Is.GreaterThan(0));
                    if(phase==8)Assert.That(map.Find("02_Mobilia_Colisoes/Sarcófago da contenção"),Is.Not.Null);
                    else Assert.That(map.Find("02_Mobilia_Colisoes/Tanque biológico"),Is.Not.Null);
                }
                finally{Object.DestroyImmediate(root);}
            }
        }
        [Test]public void RemasteredSchoolRetainsTheOriginalTwelveDeskOpenLaboratory()
        {
            var plan=CampaignMapPlan.Create(4);
            var desks=plan.furniture.FindAll(p=>p.name.StartsWith("Carteira original"));
            Assert.That(desks.Count,Is.EqualTo(12));
            float[] columns={-5.85f,-3.05f,3.75f,6.55f};
            for(int i=0;i<12;i++)
            {
                Assert.That(desks[i].position.x,Is.EqualTo(columns[i%4]));
                Assert.That(desks[i].footprint.center.y,Is.EqualTo(3-i/4*2.4f).Within(.001f));
                Assert.That(CampaignVisualAssets.Prop(desks[i].motif).texture.name,Is.EqualTo("OriginalSchoolComputerLab"));
            }
            var route=new List<Vector2>();Assert.That(plan.Route(new Vector2(.75f,-8.4f),new Vector2(.8f,3.8f),route),Is.True,"Original central gate and aisle");
            Assert.That(plan.furniture.Find(p=>p.name=="Fusca").position,Is.EqualTo(new Vector2(-5.5f,-10.4f)));
        }
        [Test] public void SchoolFrontagePreservesWholeArtworkGateAndAspectRatio()
        {
            var root=new GameObject("FacadeGeometry");
            try
            {
                var facade=CampaignMapConstruction.PreserveSchoolFacade(root.transform);
                var renderer=facade.GetComponent<SpriteRenderer>();var source=Resources.Load<Texture2D>(Game.Varginha.VarginhaIndustrialSchoolFacade.ResourcePath);
                Assert.That(renderer.sprite.texture,Is.EqualTo(source));
                Assert.That(renderer.sprite.rect.size,Is.EqualTo(new Vector2(source.width,source.height)));
                Assert.That(renderer.transform.localScale,Is.EqualTo(Vector3.one));
                Assert.That(renderer.sprite.pivot.x,Is.EqualTo(300).Within(.01));
                facade.ShowInterior(true);Assert.That(renderer.enabled,Is.False);
                facade.ShowInterior(false);Assert.That(renderer.enabled,Is.True);
            }
            finally{Object.DestroyImmediate(root);}
        }
    }
}
