using System.Collections;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Tests.PlayMode
{
    public class VarginhaExperimentIntegrationTests
    {
        [UnityTest]
        public IEnumerator ExistingSchoolSupportsOpeningInvestigationAndReturn()
        {
            yield return SceneManager.LoadSceneAsync(VarginhaExperimentLab.SceneName);
            yield return null;
            yield return null;
            var lab = VarginhaExperimentLab.Active;
            Assert.That(lab, Is.Not.Null);
            Assert.That(Object.FindAnyObjectByType<EdelzioTopDownController>(), Is.Not.Null);
            Assert.That(GameObject.Find("Renan_Industrial"), Is.Not.Null);
            Assert.That(Object.FindObjectsByType<VarginhaCombatEnemy>(), Is.Empty);
            lab.PlayOpening();
            Assert.That(VarginhaExperimentLab.IsModalOpen, Is.True);
            lab.FinishOpening();
            Assert.That(lab.Progress.openingSeen, Is.True);
            Assert.That(VarginhaExperimentLab.IsModalOpen, Is.False);
            lab.ResetProgress();
            Assert.That(lab.OpenPuzzle(1), Is.False);
            Assert.That(lab.OpenPuzzle(0), Is.True);
            lab.Swap(0, 1);
            var restored = ExperimentSave.Load(lab.Definition);
            Assert.That(restored.mapOrder, Is.EqualTo(lab.Progress.mapOrder));
            lab.ResetProgress();
            Assert.That(VarginhaExperimentLab.IsModalOpen, Is.False);
            yield return SceneManager.LoadSceneAsync("Menu_MisterioDeVarginha");
            yield return null;
            Assert.That(VarginhaExperimentLab.Active, Is.Null);
        }
    }
}
