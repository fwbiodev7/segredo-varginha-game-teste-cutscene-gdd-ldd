using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using Game.Varginha.Experiment;

namespace Game.Editor.Testing
{
    public static class CampaignModernLaboratoryLayout
    {
        public const string ScenePath="Assets/Scenes/Ato6_Fase18_A_Criatura_Ferida.unity";
        private const string Folder="Assets/Art/ModernCreatureLab/";
        private const string RootName="Laboratorio_Moderno_Organize_Os_Moveis";
        private static string DetectPipeline()
        {
            var asset=GraphicsSettings.currentRenderPipeline??GraphicsSettings.defaultRenderPipeline;
            if(asset==null)return "BuiltIn";
            var name=asset.GetType().FullName;
            return name.Contains("Universal")?"URP":name.Contains("HighDefinition")?"HDRP":"Custom";
        }
        private static Sprite Import(string name,float ppu,Vector2 pivot)
        {
            string path=Folder+name+".png";
            AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
            var importer=AssetImporter.GetAtPath(path) as TextureImporter;
            if(importer==null)throw new InvalidOperationException("Missing texture: "+path);
            importer.textureType=TextureImporterType.Sprite;
            importer.spriteImportMode=SpriteImportMode.Single;
            importer.spritePixelsPerUnit=ppu;
            var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);
            settings.spriteAlignment=(int)SpriteAlignment.Custom;settings.spritePivot=pivot;
            settings.spriteMeshType=SpriteMeshType.FullRect;importer.SetTextureSettings(settings);
            importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;
            importer.isReadable=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.alphaSource=TextureImporterAlphaSource.FromInput;importer.alphaIsTransparency=true;
            importer.maxTextureSize=2048;importer.npotScale=TextureImporterNPOTScale.None;
            importer.wrapMode=TextureWrapMode.Clamp;importer.anisoLevel=0;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path)??throw new InvalidOperationException("Sprite import failed: "+path);
        }
        private static GameObject Place(Transform parent,string name,Sprite sprite,Vector2 position,int order)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.position=position;
            var sr=go.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.sortingOrder=order;
            return go;
        }
        public static void OpenForArrangement()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play mode before arranging laboratory art.");
            string pipeline=DetectPipeline();
            if(pipeline!="URP")throw new InvalidOperationException("Expected the project's existing URP renderer, found "+pipeline);
            AssetDatabase.Refresh();
            var scene=EditorSceneManager.OpenScene(ScenePath);
            GameObject root=null;
            foreach(var existing in scene.GetRootGameObjects())if(existing.name==RootName)root=existing;
            if(root==null)
            {
                root=new GameObject(RootName);root.tag="EditorOnly";
                root.AddComponent<CampaignLaboratoryArrangementDraft>();
                var floor=Import("Arquitetura",38.4f,Vector2.one*.5f);
                Place(root.transform,"01_Piso_E_Paredes",floor,Vector2.zero,-1000);
                var furniture=new GameObject("02_Moveis_Individuais_Para_Posicionar").transform;
                furniture.SetParent(root.transform,false);
                string[] names={"IncubadoraET1996","BancadaMicroscopios","ConsolePesquisa","FreezerAmostras",
                    "PiaDescontaminacao","CarrinhoAmostras","ArquivoCientifico","ScannerDiagnostico"};
                string[] labels={"Incubadora — ET de 1996 congelado","Bancada — microscópios","Console — pesquisa",
                    "Freezer — amostras","Pia — descontaminação","Carrinho — amostras","Armário — protocolos","Scanner — diagnóstico"};
                for(int i=0;i<names.Length;i++)
                {
                    var sprite=Import(names[i],64,new Vector2(.5f,0));
                    // Deliberately outside the room: the user authors every final position.
                    Place(furniture,labels[i],sprite,new Vector2(14+(i%3)*5,4-(i/3)*5),100-i);
                }
                var scientist=Import("OuzanaEstudando",186,new Vector2(.5f,0));
                Place(furniture,"Ouzana — estudando o ET",scientist,new Vector2(24,-6),110);
                Undo.RegisterCreatedObjectUndo(root,"Preparar laboratório para organização");
                EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            }
            var renderers=root.GetComponentsInChildren<SpriteRenderer>();
            foreach(var sr in renderers)
            {
                if(sr.sprite==null||sr.sprite.texture.filterMode!=FilterMode.Point)
                    throw new InvalidOperationException("Invalid sprite: "+sr.name);
                var importer=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sr.sprite));
                if(importer.mipmapEnabled||importer.alphaSource!=TextureImporterAlphaSource.FromInput)
                    throw new InvalidOperationException("Invalid alpha/pixel settings: "+sr.name);
            }
            if(root.GetComponentsInChildren<Collider2D>().Length!=0)
                throw new InvalidOperationException("Arrange first; collisions belong to the user's final layout.");
            EditorSceneManager.playModeStartScene=null;
            Selection.activeGameObject=root.transform.Find("02_Moveis_Individuais_Para_Posicionar").gameObject;
            var view=SceneView.lastActiveSceneView??EditorWindow.GetWindow<SceneView>();
            view.in2DMode=true;view.sceneLighting=false;
            view.LookAt(new Vector3(7,0,0),Quaternion.identity,13,true,true);view.Repaint();
            Directory.CreateDirectory("Preview/LaboratorioCientifico20261010");
            File.WriteAllText("Preview/LaboratorioCientifico20261010/EditorVerificado.json",
                "{\"chapter\":12,\"serializedPhase\":18,\"pipeline\":\""+pipeline+"\",\"sprites\":"+renderers.Length+
                ",\"colliders\":0,\"scene\":\""+ScenePath+"\",\"status\":\"ready_for_user_arrangement\"}");
            Debug.Log("Modern laboratory: chapter 12 open, individual furniture ready for user arrangement.");
        }
    }
}
