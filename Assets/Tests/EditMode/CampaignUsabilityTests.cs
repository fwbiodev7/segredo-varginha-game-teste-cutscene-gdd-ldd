using Game.Varginha;
using Game.Varginha.Experiment;
using Game.Player;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    [Category("HudPuzzles")] public class CampaignUsabilityTests
    {
        [Test] public void DifficultyOptionsAffectCampaignBoss()
        {
            var original=VarginhaDifficulty.Selected;
            var root=new GameObject("Difficulty_test");
            try
            {
                var plan=CampaignMapPlan.Create(20);var actor=CampaignMapConstruction.CreatePlayer(root.transform,plan);
                VarginhaDifficulty.Select(InvestigationDifficulty.Easy);
                var easy=CampaignManifestationCombat.Spawn(root.transform,actor,plan,Vector2.zero);
                float easyHealth=easy.GetComponent<HealthSystem>().MaxHealth;
                VarginhaDifficulty.Select(InvestigationDifficulty.Medium);
                var medium=CampaignManifestationCombat.Spawn(root.transform,actor,plan,Vector2.zero);
                float mediumHealth=medium.GetComponent<HealthSystem>().MaxHealth;
                VarginhaDifficulty.Select(InvestigationDifficulty.Hard);
                var hard=CampaignManifestationCombat.Spawn(root.transform,actor,plan,Vector2.zero);
                medium.transform.position=plan.spawn+Vector2.up*.58f;
                Assert.That(medium.SpawnMinion(),Is.True);
                var minor=System.Array.Find(root.GetComponentsInChildren<CampaignManifestationCombat>(),item=>item.IsMinor);
                Assert.That(minor.GetComponent<HealthSystem>().MaxHealth,Is.EqualTo(65),"New minions inherit the current attempt, even if the setting changes.");
                medium.ClearMinions();Assert.That(medium.SpawnMinion(),Is.True);
                Assert.That(minor.GetComponent<HealthSystem>().MaxHealth,Is.EqualTo(65),"Pool reuse must not multiply health again.");
                Assert.That(easyHealth,Is.LessThan(mediumHealth));
                Assert.That(mediumHealth,Is.EqualTo(650),"Preserve the existing medium balance.");
                Assert.That(hard.GetComponent<HealthSystem>().MaxHealth,Is.GreaterThan(mediumHealth));
                Assert.That(CampaignManifestationCombat.MaximumMinions,Is.EqualTo(5));
            }
            finally{Object.DestroyImmediate(root);VarginhaDifficulty.Select(original);}
        }
    }
}
