using System.IO;
using Game.Varginha;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Render the actual classroom sprites and layouts without modifying saved game scenes.</summary>
public static class VarginhaClassroomPreview
{
    [MenuItem("Tools/Varginha/Preview sala de informática")]
    public static void Capture()
    {
        var scene=EditorSceneManager.NewPreviewScene();
        var root=new GameObject("ClassroomPreview");
        SceneManager.MoveGameObjectToScene(root,scene);
        try
        {
            var school=VarginhaEnvironmentArt.EnsureSchool(root.transform);
            var names=VarginhaPhase2Controller.StudentNames;
            for(int i=0;i<names.Length;i++)
            {
                Actor(root.transform,names[i],VarginhaClassroomMap.StudentPositions[i],
                    VarginhaPixelArtSprites.Create("Student_"+names[i],Color.white),10+i);
                Actor(root.transform,"Jaula_"+names[i],VarginhaClassroomMap.StudentPositions[i],
                    VarginhaPixelArtSprites.Create("HostageCage_"+names[i],new Color(.2f,.78f,.34f)),22);
            }
            foreach(var position in VarginhaClassroomMap.EnemyPositions)
                Actor(root.transform,"ET",position,VarginhaPixelArtSprites.Create("ET_Subordinate_Sentinel",new Color(.68f,.36f,.18f)),21);
            Actor(root.transform,"Edelzio",new Vector3(.75f,-4.5f),VarginhaReferenceSprites.EdelzioWalkFrames()[0][0],23);
            var cameraObject=new GameObject("PreviewCamera");
            cameraObject.transform.SetParent(root.transform);
            var camera=cameraObject.AddComponent<Camera>();
            camera.scene=scene;
            camera.orthographic=true;
            camera.orthographicSize=7f;
            camera.backgroundColor=new Color(.04f,.07f,.10f);
            camera.clearFlags=CameraClearFlags.SolidColor;
            camera.transform.position=new Vector3(.5f,-.45f,-10);
            VarginhaPixelPresentation.Configure(camera);
            Save(camera,"Logs/classroom-preview.png",1280,960);
            // Render a real seated Edelzio at an existing blue classroom chair.
            var seat=school.Find("SalaV4_Cadeira_0");
            Actor(root.transform,"EdelzioSentado",seat.position+Vector3.up*.13f,VarginhaSeatedSprites.Frame(3,2),24);
            camera.transform.position=seat.position+new Vector3(0,.3f,-10);
            camera.orthographicSize=1.4f;
            Save(camera,"Logs/edelzio-seated-preview.png",640,640);
            Debug.Log("Classroom previews saved in Logs.");
        }
        finally { EditorSceneManager.ClosePreviewScene(scene); }
    }

    private static void Actor(Transform parent,string name,Vector3 position,Sprite sprite,int order)
    {
        var go=new GameObject(name);
        go.transform.SetParent(parent,false);
        go.transform.position=position;
        var renderer=go.AddComponent<SpriteRenderer>();
        renderer.sprite=sprite;
        renderer.sortingOrder=order;
    }

    private static void Save(Camera camera,string path,int width,int height)
    {
        var target=new RenderTexture(width,height,24) { filterMode=FilterMode.Point,antiAliasing=1 };
        var image=new Texture2D(width,height,TextureFormat.RGB24,false);
        var previous=RenderTexture.active;
        try
        {
            camera.targetTexture=target;
            camera.Render();
            RenderTexture.active=target;
            image.ReadPixels(new Rect(0,0,width,height),0,0);
            image.Apply();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path,image.EncodeToPNG());
        }
        finally
        {
            RenderTexture.active=previous;
            camera.targetTexture=null;
            Object.DestroyImmediate(image);
            Object.DestroyImmediate(target);
        }
    }
}
