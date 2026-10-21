using Game.Varginha;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class VarginhaVisualPolishTests
    {
        [Test]
        public void ProtagonistIsLargerWithoutMovingHisFootBaseline()
        {
            var actor = VarginhaReferenceSprites.EdelzioWalkFrames()[0][0];
            var student = VarginhaStudentSprites.Frame("Matias", 0, 0);
            Assert.AreEqual(1.18f, student.pixelsPerUnit / actor.pixelsPerUnit, .001f);
            Assert.That(Foot(actor),Is.EqualTo(-64f*.4f/actor.pixelsPerUnit).Within(.001f));
            foreach(var direction in VarginhaReferenceSprites.EdelzioWalkFrames())
                foreach(var frame in direction)Assert.That(Foot(frame),Is.EqualTo(Foot(actor)).Within(.001f));
        }

        [Test]
        public void EveryAttackHasDedicatedWindupContactAndRecoveryWithStableGeometry()
        {
            var walk = VarginhaReferenceSprites.EdelzioWalkFrames();
            var attacks = VarginhaReferenceSprites.EdelzioAttackFrames();
            for (int d = 0; d < 4; d++)
            {
                Assert.AreEqual(18, attacks[d].Length);
                for (int combo = 0; combo < 3; combo++)
                {
                    var windup = attacks[d][combo * 6 + 1];
                    var contact = attacks[d][combo * 6 + 3];
                    Assert.AreNotSame(walk[d][0], contact);
                    Assert.That(Pixels(windup).SequenceEqual(Pixels(contact)),Is.False,"Windup and contact must contain different frame pixels.");
                    Assert.That(Foot(contact),Is.EqualTo(Foot(walk[d][0])).Within(.001f),"A punch must not shift the feet baseline.");
                    Assert.AreEqual(walk[d][0].pixelsPerUnit, contact.pixelsPerUnit);
                    Assert.AreEqual(FilterMode.Point, contact.texture.filterMode);
                    var pixels = Pixels(contact);
                    for (int i = 0; i < 64; i++)
                    {
                        Assert.AreEqual(0, pixels[i].a, "No clipping at bottom edge");
                        Assert.AreEqual(0, pixels[63 * 64 + i].a, "No clipping at top edge");
                        Assert.AreEqual(0, pixels[i * 64].a);
                        Assert.AreEqual(0, pixels[i * 64 + 63].a);
                    }
                }
            }
        }

        private static Color32[] Pixels(Sprite sprite)
        {
            var rect=sprite.rect;var source=sprite.texture.GetPixels32();
            var pixels=new Color32[(int)rect.width*(int)rect.height];
            for(int y=0;y<(int)rect.height;y++)System.Array.Copy(source,((int)rect.y+y)*sprite.texture.width+(int)rect.x,pixels,y*(int)rect.width,(int)rect.width);
            return pixels;
        }
        private static float Foot(Sprite sprite)
        {
            Assert.That(VarginhaCharacterFrameGeometry.TryBounds(sprite.texture,sprite.rect,out var visible,out _),Is.True);
            return (visible.yMin-sprite.rect.yMin-sprite.pivot.y)/sprite.pixelsPerUnit;
        }

        [Test]
        public void ClearCombatPoseClearsAttackPose()
        {
            var go = new GameObject("PlayerTest");
            var anim = go.AddComponent<VarginhaPlayerSpriteAnimation>();
            var testSprite = Sprite.Create(new Texture2D(4, 4), new Rect(0, 0, 4, 4), Vector2.one * .5f);
            testSprite.name = "TestPose";
            
            anim.SetCombatPose(testSprite, Vector2.down);
            Assert.IsTrue(anim.HasActionPose);

            anim.ClearCombatPose();
            Assert.IsFalse(anim.HasActionPose);
            Object.DestroyImmediate(go);
        }
    }
}
