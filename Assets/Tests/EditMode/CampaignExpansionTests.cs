using System.Collections.Generic;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CampaignExpansionTests
    {
        [Test]public void ChildhoodFurnitureKeepsItsAspectAndCollisionWithinItsBase()
        {
            var plan=CampaignMapPlan.Create(1);
            foreach(var prop in plan.furniture)
            {
                var art=System.Array.Find(CampaignIllustratedMaps.Get(1).props,p=>p.name==prop.name).art;
                Assert.That(prop.size.x/prop.size.y,Is.EqualTo(art[2]/art[3]).Within(.001f),prop.name);
                if(prop.footprint.width<=0||prop.footprint.height<=0)continue;
                var visual=new Rect(prop.position-prop.size/2,prop.size);
                Assert.That(visual.Contains(prop.footprint.min),Is.True,prop.name+" lower base");
                Assert.That(visual.Contains(prop.footprint.max),Is.True,prop.name+" upper base");
            }
        }
        [Test]public void RoamingStudentsCanReachTheYardWithoutEnteringRenansSpace()
        {
            foreach(int phase in new[]{4,5})
            {
                var plan=CampaignSchoolLife.CreateRoutePlan(phase);var path=new List<Vector2>();
                var data=CampaignIllustratedMaps.Get(phase);var seats=CampaignIllustratedMaps.SchoolSeats(phase);
                foreach(var route in new[]{(seats[10]-Vector2.up*.58f,data.Position(740,750)),(seats[11]-Vector2.up*.58f,data.Position(1080,750))})
                {
                    Assert.That(plan.Route(route.Item1,route.Item2,path),Is.True);
                    foreach(var point in path)Assert.That(CampaignSchoolLife.RenanClearArea.Contains(point),Is.False);
                }
            }
        }
        [Test]public void WalkingFramesKeepTheirHeadCenteredInEveryDirection()
        {
            var gray=ReadForInspection(Resources.Load<Texture2D>("Varginha/StoryCharacters/WalkGray"));
            try
            {
            foreach(bool equipped in new[]{false,true})for(int direction=0;direction<4;direction++)
                for(int frame=0;frame<4;frame++)
                {
                    var sprite=CampaignTeamEdelzio.Frame("EdelzioWalk",direction,frame,equipped);
                    float head=CampaignTeamEdelzio.BodyAnchorX(equipped?gray:sprite.texture,sprite.rect);
                    Assert.That((head-sprite.rect.x-sprite.pivot.x)/sprite.pixelsPerUnit,Is.EqualTo(0).Within(equipped?.045f:.001f));
                    Assert.That(sprite.bounds.min.y,Is.EqualTo(-.58f).Within(.001f));
                }
            }
            finally{Object.DestroyImmediate(gray);}
        }
        private static Texture2D ReadForInspection(Texture texture)
        {
            var rt=RenderTexture.GetTemporary(texture.width,texture.height,0,RenderTextureFormat.ARGB32);
            var active=RenderTexture.active;
            try
            {
                Graphics.Blit(texture,rt);RenderTexture.active=rt;
                var copy=new Texture2D(texture.width,texture.height,TextureFormat.RGBA32,false);
                copy.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0);copy.Apply();return copy;
            }
            finally{RenderTexture.active=active;RenderTexture.ReleaseTemporary(rt);}
        }
        [Test]public void JournalistMovesOnlyHisMouthAndClosesItAfterTheReport()
        {
            var composer=new ExperimentFrameComposer();
            try
            {
                var shot=System.Array.Find(ExperimentDefinition.Load().shots,s=>s.id=="news");
                var closed=composer.Compose(shot,0,false).GetPixels32();
                var open=composer.Compose(shot,.46f,false).GetPixels32();int changes=0;
                for(int i=0;i<closed.Length;i++)if(!closed[i].Equals(open[i]))
                {changes++;Assert.That(i%384,Is.InRange(189,198));Assert.That(i/384,Is.InRange(124,127));}
                Assert.That(changes,Is.GreaterThan(0));
                CollectionAssert.AreEqual(closed,composer.Compose(shot,shot.duration-.1f,false).GetPixels32());
                Assert.That(System.Array.Find(ExperimentDefinition.Load().shots,s=>s.id=="interference").subtitle,Is.EqualTo("Não deixe ela sair."));
            }
            finally{composer.Dispose();}
        }
        [Test]public void WallConnectionsFinishCornersWithoutBlockingOpenings()
        {
            var root=new GameObject("WallJoinTest");
            try
            {
                var walls=new[]{new Rect(-3,2,6,.6f),new Rect(-3,-2,.6f,4.6f),new Rect(0,-2,.6f,4.6f)};
                CampaignWallConnections.Build(root.transform,walls,CampaignAdultHouse.TextureTile("WallRequested",.6f),true);
                Assert.That(root.GetComponentsInChildren<SpriteRenderer>().Length,Is.EqualTo(2));
                Assert.That(root.GetComponentsInChildren<Collider2D>(),Is.Empty);
                CampaignWallConnections.Build(root.transform,walls,CampaignAdultHouse.TextureTile("WallRequested",.6f),true);
                Assert.That(root.GetComponentsInChildren<SpriteRenderer>().Length,Is.EqualTo(2));
            }
            finally{Object.DestroyImmediate(root);}
        }
        [Test]public void AllNewPosesUseDedicatedBackpackSheetsWithCompleteHeadsAndStableFeet()
        {
            var gray=ReadForInspection(Resources.Load<Texture2D>("Varginha/StoryCharacters/WalkGray"));
            try
            {
            foreach(var sheet in new[]{("EdelzioWalk",4),("EdelzioPunch",3),("EdelzioActions",6),("EdelzioInteractions",6)})
                for(int row=0;row<4;row++)for(int frame=0;frame<sheet.Item2;frame++)
                {
                    var body=CampaignTeamEdelzio.Frame(sheet.Item1,row,frame);var equipped=CampaignTeamEdelzio.Frame(sheet.Item1,row,frame,true);
                    bool walk=sheet.Item1=="EdelzioWalk";
                    Assert.That(equipped,Is.Not.Null,body.name);Assert.That(equipped.texture.name,Is.EqualTo(walk?"WalkGray":sheet.Item1+"BackpackV3"));
                    Assert.That(equipped.texture,Is.Not.EqualTo(body.texture));Assert.That(equipped.name,Does.Contain(walk?"WalkGray":"ComMochila"));
                    Assert.That(equipped.bounds.min.y,Is.EqualTo(body.bounds.min.y).Within(.001f));
                    Assert.That(equipped.rect.yMax,Is.LessThan(equipped.texture.height));Assert.That(equipped.rect.yMin,Is.GreaterThan(0));
                    Assert.That(equipped.bounds.size.y,Is.InRange(.7f,1.9f));
                    Assert.That((walk?gray:equipped.texture).GetPixel(0,0).a,Is.LessThan(.05f));
                }
            }
            finally{Object.DestroyImmediate(gray);}
        }
        [Test]public void HouseListsOnlyActualPendingTasksAndUsesRequestedTextures()
        {
            var story=new CampaignStory{routine=14};Assert.That(story.RemainingHouseTasks,Is.EqualTo("lavar o rosto na pia da cozinha"));
            story.routine=15;Assert.That(story.RemainingHouseTasks,Is.Empty);
            Assert.That(CampaignAdultHouse.TextureTile("WoodRequested",1).texture.name,Is.EqualTo("WoodRequested"));
            Assert.That(CampaignAdultHouse.TextureTile("WallRequested",.8f).texture.name,Is.EqualTo("WallRequested"));
            Assert.That(CampaignAdultHouse.Correction(0).texture.filterMode,Is.EqualTo(FilterMode.Point));
        }
        [TestCase(1)] [TestCase(3)] [TestCase(4)] [TestCase(5)] [TestCase(6)] [TestCase(7)] [TestCase(8)] [TestCase(9)] [TestCase(10)]
        public void EveryObjectiveHasFootClearanceAndAConnectedRoute(int phase)
        {
            var plan=CampaignMapPlan.Create(phase); var route=new List<Vector2>();
            Assert.That(plan.IsClear(plan.spawn),Is.True,"Spawn");
            foreach(var point in plan.points)
            {
                Assert.That(plan.IsClear(point.position,.25f),Is.True,"Fase "+phase+" / "+point.id+" footprint");
                Assert.That(plan.Route(plan.spawn,point.position,route),Is.True,"Fase "+phase+" / "+point.id+" route");
                foreach(var step in route) Assert.That(plan.IsClear(step,.25f),Is.True);
            }
        }
        [Test] public void ExpansionRequiresPhysicalEvidenceAndPreservesOptionalClues()
        {
            var state=new CampaignExpansionState { mapOrder=new[]{0,1,2},sampleOrder=new[]{0,1,2},sealOrder=new[]{0,1,2} };
            Assert.That(state.SolveMap(),Is.False); Assert.That(state.SolveAnchor(1898,1,23),Is.False); Assert.That(state.SolveSamples(),Is.False); Assert.That(state.Stabilize(),Is.False);
            state.visited=7; Assert.That(state.SolveMap(),Is.False,"A correctly placed map still needs connected directions."); state.mapDirections=new[]{1,1,0}; Assert.That(state.SolveMap(),Is.True); state.forestSigns=3; state.fabioMet=true; state.anchorClues=7;
            Assert.That(state.SolveAnchor(1898,1,23),Is.False); Assert.That(state.SolveAnchor(1996,1,23),Is.True);
            state.labClues=7; Assert.That(state.SolveSamples(),Is.False); state.evidencePresented=true; Assert.That(state.SolveSamples(),Is.True);
            Assert.That(state.Spray(0),Is.True); int remaining=state.reagentCharges; Assert.That(state.Spray(0),Is.False); Assert.That(state.reagentCharges,Is.EqualTo(remaining));
            Assert.That(state.sprayed,Is.EqualTo(7)); Assert.That(state.Complete(10),Is.True); Assert.That(state.testDriven,Is.False); state.truthClues=7;
            var restored=JsonUtility.FromJson<CampaignExpansionState>(JsonUtility.ToJson(state));restored.Repair();
            Assert.That(restored.Complete(10),Is.True); Assert.That(restored.truthClues,Is.EqualTo(7));
        }
        [Test] public void OldSavesAndMalformedOrdersAreRecovered()
        {
            var story=JsonUtility.FromJson<CampaignStory>("{\"version\":1,\"phase\":5}"); story.Repair(); Assert.That(story.expansion,Is.Not.Null); Assert.That(story.phase,Is.EqualTo(4));
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
                        Is.SameAs(CampaignOriginalFusca.Top(CampaignWorkshopVehicle.Facing.North).texture));
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
                var road=map.Find("01_Planta_Paredes_Divisoes/Arte_Integrada_0").GetComponent<SpriteRenderer>();
                Assert.That(road.sprite.texture.name,Is.EqualTo("Street"));Assert.That(road.sortingOrder,Is.LessThan(0));
                Assert.That(map.Find("01_Planta_Paredes_Divisoes/Arte_Integrada_11"),Is.Not.Null,"The illustrated street covers the complete route.");
            }
            finally{Object.DestroyImmediate(root);}
        }
        [Test]public void ChurchAndBiologyHouseHaveIndependentFloorLightingAndEquipment()
        {
            foreach(int phase in new[]{8,9})
            {
                var root=new GameObject("Atmosphere"+phase);
                try
                {
                    var map=CampaignMapConstruction.Build(root.transform,CampaignMapPlan.Create(phase));
                    Assert.That(map.GetComponent<CampaignIllustratedLighting>(),Is.Not.Null);
                    if(phase==8)
                    {
                        Assert.That(map.Find("02_Mobilia_Colisoes/Altar da âncora").GetComponent<SpriteRenderer>().sprite.texture.name,Is.EqualTo("Church"));
                        Assert.That(map.Find("02_Mobilia_Colisoes/Tapete central").GetComponents<Collider2D>(),Is.Empty);
                    }
                    else
                    {
                        Assert.That(map.Find("02_Mobilia_Colisoes/Espécime luminoso").GetComponent<SpriteRenderer>().sprite.texture.name,Is.EqualTo("Ouzana"));
                        Assert.That(map.Find("01_Planta_Paredes_Divisoes/Arte_Integrada_0").GetComponent<SpriteRenderer>().sprite.texture,Is.SameAs(Resources.Load<Texture2D>("Varginha/IllustratedMaps/Ouzana")));
                    }
                }
                finally{Object.DestroyImmediate(root);}
            }
        }
        [Test]public void RemasteredSchoolRetainsTheOriginalTwelveDeskOpenLaboratory()
        {
            var plan=CampaignMapPlan.Create(4);
            var desks=plan.furniture.FindAll(p=>p.name.StartsWith("Carteira original"));
            Assert.That(desks.Count,Is.EqualTo(12));
            var data=CampaignIllustratedMaps.Get(4);
            for(int i=0;i<12;i++)
            {
                var measured=System.Array.Find(data.props,p=>p.name==desks[i].name);
                Assert.That(desks[i].position,Is.EqualTo(data.Area(measured.art).center));
                Assert.That(desks[i].footprint,Is.EqualTo(data.Area(measured.@base)));
            }
            var route=new List<Vector2>();Assert.That(plan.Route(plan.spawn,data.Objective("notebook"),route),Is.True,"Real central gate and teacher notebook approach");
            Assert.That(plan.furniture.Find(p=>p.name=="Fusca").position,Is.EqualTo(data.Area(System.Array.Find(data.props,p=>p.name=="Fusca").art).center));
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
