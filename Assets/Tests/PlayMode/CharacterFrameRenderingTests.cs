using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Varginha;
using Game.Varginha.Experiment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Game.Tests.PlayMode
{
    [Category("CharacterFrames")]
    public sealed class CharacterFrameRenderingTests
    {
        [Serializable] private class Frame { public int[] rect; }
        [Serializable] private class Sheet { public int columns,rows,width,height; public Frame[] frames; }
        const string Folder="Preview/PersonagensCorrigidos20261009";

        [UnityTest] public IEnumerator ChildFramesExcludeNeighbourHeadsAndKeepFeetAnchored()
        {
            var texture=Resources.Load<Texture2D>("Varginha/Experiment/ChildEdelzio");
            var frames=new List<Sprite>();
            try
            {
                for(int d=0;d<4;d++) for(int f=0;f<3;f++)
                {
                    var cell=new RectInt(f*texture.width/3,(3-d)*texture.height/4,texture.width/3,texture.height/4);
                    var sprite=CampaignPresentation.AlignedFrame(texture,cell,texture.height/4/1.8f,true);frames.Add(sprite);
                }
                yield return null;
                foreach(var sprite in frames)
                {
                    Assert.That(sprite.vertices.Length,Is.GreaterThan(4));
                    float lowest=100,highest=-100;
                    foreach(var i in sprite.triangles){lowest=Mathf.Min(lowest,sprite.vertices[i].y);highest=Mathf.Max(highest,sprite.vertices[i].y);}
                    Assert.That(lowest,Is.GreaterThan(-.025f),"Neighbour head rendered below the feet.");
                    Assert.That(lowest,Is.LessThan(.025f));Assert.That(highest,Is.InRange(1.1f,1.8f));
                }
                yield return Capture(frames[6],"Crianca_Corrigida.png",.65f);
            }
            finally{foreach(var sprite in frames)Object.Destroy(sprite);}
        }

        [UnityTest,Timeout(90000)] public IEnumerator AllStoryDirectionsActionsPortraitsAndEquipmentHaveValidMeshes()
        {
            int count=0;var reports=new List<string>{"sheet,frames,native_width,native_height"};
            foreach(var path in Directory.GetFiles("Assets/Resources/Varginha/StoryCharacters","*.json"))
            {
                var name=Path.GetFileNameWithoutExtension(path);var png=Path.ChangeExtension(path,".png");if(!File.Exists(png)||name=="RenanProps"||name.EndsWith("Effects"))continue;
                var sheet=JsonUtility.FromJson<Sheet>(File.ReadAllText(path));if(sheet.frames==null)continue;
                var sprites=new List<Sprite>();
                for(int row=0;row<sheet.rows;row++)for(int col=0;col<sheet.columns;col++)
                {
                    var sprite=CampaignStorySprites.Frame(name,row,col);Assert.That(sprite,Is.Not.Null,name+" "+row+":"+col);
                    Assert.That(sprite.texture.width,Is.EqualTo(sheet.width));Assert.That(sprite.texture.height,Is.EqualTo(sheet.height));
                    Assert.That(sprite.texture.filterMode,Is.EqualTo(FilterMode.Point));Assert.That(sprite.texture.mipmapCount,Is.EqualTo(1));
                    sprites.Add(sprite);count++;
                }
                yield return null;
                foreach(var sprite in sprites) Validate(sprite);
                for(int row=0;row<sheet.rows;row++) Assert.That(CampaignStorySprites.Frame(name,row,0,true),Is.Not.Null);
                yield return null;
                reports.Add(name+","+sprites.Count+","+sheet.width+","+sheet.height);
            }
            Directory.CreateDirectory(Folder);File.WriteAllLines(Folder+"/ValidatedFrames.csv",reports);
            Assert.That(count,Is.GreaterThan(400));
            yield return Capture(CampaignStorySprites.Frame("LifeGray",1,1),"Adulto_Corrigido.png",.1f);
        }

        [UnityTest] public IEnumerator StudentAndLegacyPlayerCyclesKeepGeometryAndShareAtlases()
        {
            var sprites=new List<Sprite>();
            foreach(var student in new[]{"Yasmin","Pedro","AnaTavares","AnnaSabia","Marcos","Matias","Fabio","LuisMartins","LuisMiguelMessias"})
                for(int d=0;d<4;d++)for(int f=0;f<4;f++)
                {var s=VarginhaStudentSprites.Frame(student,d,f);Assert.That(s,Is.Not.Null,student);sprites.Add(s);}
            foreach(var name in new[]{"EdelzioWalk","EdelzioActions","EdelzioInteractions","EdelzioPunch"})
                foreach(bool bag in new[]{false,true})for(int d=0;d<4;d++)for(int f=0;f<(name=="EdelzioWalk"?4:name=="EdelzioPunch"?3:6);f++)
                {var s=CampaignTeamEdelzio.Frame(name,d,f,bag);Assert.That(s,Is.Not.Null,name);sprites.Add(s);}
            foreach(var cycle in VarginhaReferenceSprites.EdelzioWalkFrames())sprites.AddRange(cycle);
            foreach(var cycle in VarginhaReferenceSprites.EdelzioAttackFrames())sprites.AddRange(cycle);
            for(int d=0;d<4;d++)for(int f=0;f<4;f++)
            {
                sprites.Add(VarginhaReferenceSprites.PadreFabio(d,f));
                sprites.Add(VarginhaSeatedSprites.Frame(d,f));
            }
            for(int row=0;row<3;row++)for(int f=0;f<4;f++)sprites.Add(VarginhaInteractionSprites.Frame(row,f));
            for(int cell=0;cell<4;cell++)sprites.Add(VarginhaExperimentArt.Body(cell));
            foreach(bool minor in new[]{false,true})for(int d=0;d<4;d++)for(int f=0;f<8;f++)
                sprites.Add(CampaignManifestationCombat.CombatFrame(d,f,minor));
            yield return null;
            foreach(var sprite in sprites){Assert.That(sprite,Is.Not.Null);Validate(sprite);}
            File.WriteAllText(Folder+"/LegacyFramesCount.txt",sprites.Count.ToString());
        }

        static void Validate(Sprite sprite)
        {
            Assert.That(sprite.vertices.Length,Is.GreaterThanOrEqualTo(4),sprite.name);
            Assert.That(sprite.triangles.Length,Is.GreaterThan(0),sprite.name);
            var vertices=sprite.vertices;var uv=sprite.uv;
            for(int i=0;i<vertices.Length;i++)
            {
                Assert.That(float.IsNaN(vertices[i].x)||float.IsNaN(vertices[i].y),Is.False,sprite.name);
                Vector2 expected=(vertices[i]*sprite.pixelsPerUnit+sprite.pivot+sprite.rect.position)/new Vector2(sprite.texture.width,sprite.texture.height);
                Assert.That(Vector2.Distance(expected,uv[i]),Is.LessThan(.0001f),sprite.name+" UV drift");
                Assert.That(uv[i].x,Is.InRange(-.000001f,1.000001f),sprite.name);Assert.That(uv[i].y,Is.InRange(-.000001f,1.000001f),sprite.name);
            }
        }
        static IEnumerator Capture(Sprite sprite,string file,float centreY)
        {
            var owner=new GameObject("Camera_QA_Personagens");var actor=new GameObject("Personagem_QA");actor.layer=29;
            var renderer=actor.AddComponent<SpriteRenderer>();renderer.sprite=sprite;
            var camera=owner.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=1.15f;camera.aspect=1;
            camera.cullingMask=1<<29;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.24f,.30f,.34f,1);
            camera.transform.position=new Vector3(0,centreY,-10);camera.allowHDR=false;camera.allowMSAA=false;
            var data=owner.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            data.renderPostProcessing=false;data.antialiasing=UnityEngine.Rendering.Universal.AntialiasingMode.None;
            var target=new RenderTexture(256,256,24,RenderTextureFormat.ARGB32);target.Create();
            var previous=RenderTexture.active;var image=new Texture2D(256,256,TextureFormat.RGBA32,false);
            try
            {
                yield return null;
                RenderTexture.active=target;GL.Clear(true,true,camera.backgroundColor);RenderTexture.active=previous;
                VarginhaPixelPresentation.RenderInto(camera,target);RenderTexture.active=target;image.ReadPixels(new Rect(0,0,256,256),0,0);image.Apply();
                Directory.CreateDirectory(Folder);File.WriteAllBytes(Folder+"/"+file,image.EncodeToPNG());
            }
            finally{RenderTexture.active=previous;target.Release();Object.Destroy(image);Object.Destroy(target);Object.Destroy(actor);Object.Destroy(owner);}
        }
    }
}
