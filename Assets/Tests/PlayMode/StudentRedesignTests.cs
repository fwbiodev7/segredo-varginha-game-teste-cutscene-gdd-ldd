using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Game.Tests.PlayMode
{
    public class StudentRedesignTests
    {
        private GameObject _root;
        private EdelzioTopDownController _player;
        private VarginhaBackpackInventory _inventory;
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        [SetUp] public void Setup()
        {
            Time.timeScale = 1;
            _root = new GameObject("StudentRedesignTests");
            _root.AddComponent<AudioListener>();
            var hero = new GameObject("Edelzio", typeof(SpriteRenderer), typeof(CircleCollider2D));
            hero.transform.SetParent(_root.transform);
            _player = hero.AddComponent<EdelzioTopDownController>();
            _player.HasBackpack = true;
            _inventory = _root.AddComponent<VarginhaBackpackInventory>();
        }

        [TearDown] public void Cleanup()
        {
            Object.DestroyImmediate(_root);
            Time.timeScale = 1;
        }

        [TestCase(InvestigationDifficulty.Easy)]
        [TestCase(InvestigationDifficulty.Medium)]
        [TestCase(InvestigationDifficulty.Hard)]
        public void TwelveStudentsUnlockOnlyInFinalAndKeepTheOriginalSaveIndices(InvestigationDifficulty difficulty)
        {
            var previous = VarginhaDifficulty.Selected;
            try
            {
                VarginhaDifficulty.Select(difficulty);
                var squad = VarginhaStudentAllySquad.BuildForFuturePhase(_root.transform, _player.transform, false);
                squad.ActivateForPhase(2);
                Assert.AreEqual(0, squad.ActiveCount);
                for (int i = 0; i < VarginhaStudentRoster.Count; i++) Assert.IsFalse(squad.SelectStudent(i));
                squad.ActivateManualAllies(5, true);
                squad.ConfigureSelectionSlots(3);
                Assert.AreEqual(12, squad.Allies.Count);
                Assert.AreEqual(12, squad.ActiveCount);
                for (int i = 0; i < 9; i++) Assert.AreEqual(VarginhaPhase2Controller.StudentNames[i], squad.Allies[i].StudentName);
                Assert.AreEqual("Giovana", squad.Allies[9].StudentName);
                Assert.AreEqual("Bianca", squad.Allies[10].StudentName);
                Assert.AreEqual("Moscardini", squad.Allies[11].StudentName);
                foreach (int i in new[] { 9, 10, 11, 0 }) Assert.IsTrue(squad.SelectStudent(i));
                CollectionAssert.AreEqual(new[] { 9, 10, 0 }, squad.SelectedStudentIndices);
                Assert.IsTrue(squad.SelectStudent(10));
                CollectionAssert.AreEqual(new[] { 9, 0 }, squad.SelectedStudentIndices);
                squad.ActivateForPhase(2);
                Assert.AreEqual(0, squad.ActiveCount);
                Assert.IsEmpty(squad.SelectedStudentIndices);
                foreach (var ally in squad.GetComponentsInChildren<VarginhaStudentAlly>(true)) Assert.IsFalse(ally.IsActive);
            }
            finally { VarginhaDifficulty.Select(previous); }
        }

        [Test] public void NewTeamIndicesSurviveSerialization()
        {
            var state = new CampaignStory();
            state.continuation.battleStudents = new[] { 9, 10, 11 };
            var restored = JsonUtility.FromJson<CampaignStory>(JsonUtility.ToJson(state));
            restored.continuation.Repair();
            CollectionAssert.AreEqual(state.continuation.battleStudents, restored.continuation.battleStudents);
        }

        [Test] public void CombatUsesTheRedesignedAtlasInsteadOfOldCostumes()
        {
            for (int i = 0; i < VarginhaStudentRoster.Count; i++)
            {
                string name = VarginhaStudentRoster.Name(i);
                Assert.AreSame(VarginhaStudentSprites.Frame(name, 1, 1), CampaignHeroArt.Pose(name, 0, true, 1));
                Assert.AreSame(VarginhaStudentSprites.Frame(name, 2, 2), CampaignHeroArt.Pose(name, 1, false, 2));
            }
        }

        [UnityTest] public IEnumerator EachNewStudentCanAttackFromItsFinalTeamSlot()
        {
            var squad = VarginhaStudentAllySquad.BuildForFuturePhase(_root.transform, _player.transform, false);
            squad.ActivateManualAllies(5, true);
            squad.ConfigureSelectionSlots(3);
            foreach (int i in new[] { 9, 10, 11 }) Assert.IsTrue(squad.SelectStudent(i));
            var enemy = new GameObject("Target", typeof(CircleCollider2D), typeof(VarginhaCombatTarget));
            enemy.transform.SetParent(_root.transform);
            enemy.transform.position = Vector2.right * 3;
            Physics2D.SyncTransforms();
            var health = enemy.GetComponent<Game.Player.HealthSystem>();
            float initialHealth = health.CurrentHealth;
            for (int slot = 0; slot < 3; slot++)
            {
                Assert.IsTrue(squad.TryInvokeSelectedAttack(slot, enemy.transform.position));
                Assert.AreEqual(VarginhaStudentRoster.Name(9 + slot), squad.LastInvokedStudentName);
                yield return new WaitForSeconds(1.5f);
                Assert.AreEqual(initialHealth - 16 * (slot + 1), health.CurrentHealth, .01f);
                Assert.Greater(squad.Allies[9 + slot].ManualCooldownRemaining, 0);
            }
        }

        [UnityTest] public IEnumerator BackpackShowsAllTwelveAndEquipsTheNewStudentsOnlyInFinal()
        {
            Assert.IsTrue(_inventory.Open(_player));
            _inventory.ShowTab(true);
            var cells = (Array)Field("_cells");
            Assert.AreEqual(12, cells.Length);
            Inspect(11);
            Assert.AreEqual("Moscardini", Text("_name"));
            Equip();
            StringAssert.Contains("somente na batalha final", Text("_availability"));
            _inventory.Close();

            var final = _root.AddComponent<CampaignFinalAllies>();
            final.Configure(_player);
            Assert.IsTrue(_inventory.Open(_player));
            _inventory.ShowTab(true);
            foreach (int i in new[] { 9, 10, 11 }) { Inspect(i); Equip(); }
            CollectionAssert.AreEqual(new[] { 9, 10, 11 }, final.Squad.SelectedStudentIndices);
            var states = (Array)Field("_states");
            foreach (int i in new[] { 9, 10, 11 }) Assert.AreEqual("EQUIPADO", states.GetValue(i).GetType().GetProperty("text").GetValue(states.GetValue(i)));
            foreach (var item in cells)
            {
                var rect = ((Component)item).GetComponent<RectTransform>();
                Assert.IsTrue(rect.gameObject.activeSelf);
                Assert.LessOrEqual(-rect.anchoredPosition.y + rect.sizeDelta.y, 474);
            }
            // A paused backpack must still allow inspecting and changing the team.
            Assert.AreEqual(0, Time.timeScale);
            yield return null;
            CaptureBackpack();
        }

        private object Field(string name) => typeof(VarginhaBackpackInventory).GetField(name, Private).GetValue(_inventory);
        private string Text(string name) { var label = Field(name); return (string)label.GetType().GetProperty("text").GetValue(label); }
        private void Equip() => typeof(VarginhaBackpackInventory).GetMethod("Equip", Private).Invoke(_inventory, null);
        private void Inspect(int index)
        {
            typeof(VarginhaBackpackInventory).GetField("_inspected", Private).SetValue(_inventory, index);
            typeof(VarginhaBackpackInventory).GetMethod("Refresh", Private).Invoke(_inventory, null);
        }

        private void CaptureBackpack()
        {
            // Render a local review artifact without touching campaign saves or opening a window.
            var canvas = ((GameObject)Field("_canvasObject")).GetComponent<Canvas>();
            var cameraObject = new GameObject("BackpackReviewCamera", typeof(Camera));
            cameraObject.transform.SetParent(_root.transform);
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.03f, .05f, .06f);
            camera.orthographic = true;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1;
            var texture = new Texture2D(1680, 975, TextureFormat.RGB24, false);
            var target = RenderTexture.GetTemporary(1680, 975, 24);
            var active = RenderTexture.active;
            try
            {
                camera.targetTexture = target;
                Canvas.ForceUpdateCanvases();
                camera.Render();
                RenderTexture.active = target;
                texture.ReadPixels(new Rect(0, 0, 1680, 975), 0, 0);
                texture.Apply();
                const string folder = "Preview/Students/Redesign20261029";
                Directory.CreateDirectory(folder);
                File.WriteAllBytes(Path.Combine(folder, "Mochila_LutaFinal.png"), texture.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = null;
                RenderTexture.active = active;
                RenderTexture.ReleaseTemporary(target);
                Object.Destroy(texture);
            }
        }
    }
}
