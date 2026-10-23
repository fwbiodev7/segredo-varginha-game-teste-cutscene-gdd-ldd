using System.Linq;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public sealed class CampaignRecordedAudioTests
    {
        [System.Serializable]sealed class ArtFrame{public int[] rect;public string id;}
        [System.Serializable]sealed class ArtAtlas{public ArtFrame[] frames;}
        [Test] public void RuntimeSupportFramesMatchTheExportedArtSlicing()
        {
            string path=System.IO.Path.Combine(Application.dataPath,"../output/audio-integration-2026-10-21/art/export/atlas.json");
            var atlas=JsonUtility.FromJson<ArtAtlas>(System.IO.File.ReadAllText(path));Assert.That(atlas.frames.Length,Is.EqualTo(24));
            var method=typeof(CampaignFinalAllies).GetMethod("Effect",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic);
            for(int i=0;i<24;i++)
            {
                var sprite=(Sprite)method.Invoke(null,new object[]{i/8,i%8});var r=atlas.frames[i].rect;
                Assert.That(sprite.rect,Is.EqualTo(new Rect(r[0],sprite.texture.height-r[1]-r[3],r[2],r[3])),atlas.frames[i].id);
                Assert.That(sprite.pivot,Is.EqualTo(new Vector2(160,160)));Assert.That(sprite.pixelsPerUnit,Is.EqualTo(96));
            }
            Assert.That(Resources.Load<Shader>("Varginha/StoryCharacters/SupportShieldClean"),Is.Not.Null);
        }
        [Test] public void EveryCatalogEntryResolvesToAnImportedClip()
        {
            Assert.That(CampaignRecordedAudio.Definitions.Count,Is.GreaterThanOrEqualTo(45));
            Assert.That(CampaignRecordedAudio.Definitions.Select(e=>e.path).Distinct().Count(),Is.GreaterThanOrEqualTo(41));
            foreach(var e in CampaignRecordedAudio.Definitions)
            {
                var clip=CampaignRecordedAudio.Clip(e.id);Assert.That(clip,Is.Not.Null,e.id);
                Assert.That(clip.length,Is.GreaterThan(.05f),e.id);
                if(e.path.Contains("Received20261021"))Assert.That(clip.frequency,Is.EqualTo(44100),e.id);
                else Assert.That(clip.frequency,Is.GreaterThan(0),e.id);
                if(e.seconds>0)Assert.That(clip.length,Is.EqualTo(e.seconds).Within(.06f),e.id);
                Assert.That(e.gain,Is.InRange(.1f,1));Assert.That(CampaignSoundscape.Clip(e.id),Is.SameAs(clip));
            }
        }
        [TestCase("RenanPulse","wayofx-glitch_hit_variation1")]
        [TestCase("OuzanaGlass","magiaz-vidro-quebrando")]
        [TestCase("ChildExplosion","soundreality-explosion-fx")]
        [TestCase("FabioShield","dragon-studio-angelical-synth-pad")]
        [TestCase("ChurchBell","alex_jauk-old-town-church-bells")]
        [TestCase("MenuMusic","musicamenuinicialslowed.mp3")]
        [TestCase("ChurchAmbience","igrejamedo.mp3")]
        public void AuthoredAssignmentsKeepTheirSourceIdentity(string id,string file)
            =>Assert.That(CampaignRecordedAudio.Definition(id).source,Does.Contain(file));
        [Test] public void RecordedBossAliasesDoNotReuseTheChildExplosion()
        {
            Assert.That(CampaignRecordedAudio.Clip("BossClaw"),Is.SameAs(CampaignRecordedAudio.Clip("EnemyAttack")));
            Assert.That(CampaignRecordedAudio.Clip("ManifestationRise"),Is.SameAs(CampaignRecordedAudio.Clip("BossRoar")));
            Assert.That(CampaignRecordedAudio.Definition("AlienBurst"),Is.Null);
            Assert.That(CampaignSoundscape.Clip("AlienBurst"),Is.Not.SameAs(CampaignRecordedAudio.Clip("ChildExplosion")));
            Assert.That(CampaignRecordedAudio.Clip("RadioInterference"),Is.Not.SameAs(CampaignRecordedAudio.Clip("RenanPulse")));
        }
        [Test] public void BedsStreamAndShortEffectsPreloadWithoutLossyCompression()
        {
            foreach(var e in CampaignRecordedAudio.Definitions)
            {
                var importer=(AudioImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(CampaignRecordedAudio.Clip(e.id)));
                var s=importer.defaultSampleSettings;
                Assert.That(s.loadType,Is.EqualTo(e.loop?AudioClipLoadType.Streaming:AudioClipLoadType.DecompressOnLoad),e.id);
                Assert.That(s.preloadAudioData,Is.EqualTo(!e.loop),e.id);
                Assert.That(s.compressionFormat,Is.EqualTo(e.loop?AudioCompressionFormat.Vorbis:AudioCompressionFormat.PCM),e.id);
            }
        }
        [Test] public void NightNatureAndCrowdedSchoolDoNotLeakIntoMorningOrInvadedSchool()
        {
            Assert.That(CampaignRecordedAudio.Ambient("House",0),Is.Null);
            Assert.That(CampaignRecordedAudio.Ambient("Road",0),Is.Null);
            Assert.That(CampaignRecordedAudio.Ambient("School",2),Is.Null);
            Assert.That(CampaignRecordedAudio.Ambient("School",0),Is.SameAs(CampaignRecordedAudio.Clip("SchoolAmbience")));
            Assert.That(CampaignRecordedAudio.Ambient("Forest",1),Is.SameAs(CampaignRecordedAudio.Clip("ForestAmbience")));
        }
        [Test] public void LaboratoryUsesTheReceivedLoopingBed()
        {
            var clip=CampaignRecordedAudio.Clip("LabAmbience");
            Assert.That(CampaignRecordedAudio.Ambient("Lab",0),Is.SameAs(clip));
            Assert.That(CampaignRecordedAudio.Definition("LabAmbience").loop,Is.True);
            Assert.That(clip,Is.Not.Null);
        }
        [TestCase(0)][TestCase(1)][TestCase(2)]
        public void ChurchKeepsReceivedDarkBedAcrossTensionChanges(int band)
        {
            var clip=CampaignRecordedAudio.Clip("ChurchAmbience");
            Assert.That(clip,Is.Not.Null);
            Assert.That(CampaignAtmosphereAudio.Bed("Church",band),Is.SameAs(clip));
            Assert.That(clip,Is.Not.SameAs(CampaignRecordedAudio.Clip("ChurchBell")));
            Assert.That(CampaignRecordedAudio.Definition("ChurchAmbience").loop,Is.True);
        }
        [Test] public void FootstepsVaryWithoutConsumingGameplayRandomness()
        {
            var saved=Random.state;try
            {
                Random.InitState(555);float expected=Random.value;Random.InitState(555);
                var variants=Enumerable.Range(0,3).Select(_=>CampaignRecordedAudio.NextFoot("FootWood")).ToArray();
                Assert.That(variants.Distinct().Count(),Is.EqualTo(3));Assert.That(Random.value,Is.EqualTo(expected));
                foreach(var v in variants)Assert.That(CampaignRecordedAudio.Clip(v),Is.Not.Null);
            }finally{Random.state=saved;}
        }
    }
}
