using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Two tiny, shared meshes, no realtime Unity lights or shadow render targets.
    public static class CampaignFuscaLighting
    {
        public static Sprite Frame(Texture2D texture,int index)
        {
            // Measured body rectangles in the preserved 971x1619 source; all poses share a pivot.
            float x=index%2==0?122:534,y=index<2?118:885;
            var rect=new Rect(x*texture.width/971f,texture.height-(y+604)*texture.height/1619f,315*texture.width/971f,604*texture.height/1619f);
            var sprite=Sprite.Create(texture,rect,Vector2.one/2,rect.height/2,0,SpriteMeshType.FullRect);
            sprite.name="Fusca_Aligned_"+index;return sprite;
        }
        private static Mesh _mesh;private static Material _material;
        public static void Add(Transform car)
        {
            if(_mesh==null)
            {
                _mesh=new Mesh{name="Farol pixelado compartilhado"};
                _mesh.vertices=new[]{new Vector3(-.055f,0),new Vector3(.055f,0),new Vector3(1,1),new Vector3(-1,1)};
                _mesh.uv=new[]{new Vector2(0,0),new Vector2(1,0),new Vector2(1,1),new Vector2(0,1)};
                _mesh.triangles=new[]{0,2,1,0,3,2};_mesh.RecalculateBounds();_mesh.UploadMeshData(true);
                _material=new Material(Resources.Load<Shader>("Varginha/IllustratedMaps/Headlight")){name="Faróis leves",hideFlags=HideFlags.DontSave};
            }
            foreach(float side in new[]{-.27f,.275f})
            {
                var go=new GameObject("Farol_Fusca");go.transform.SetParent(car,false);go.transform.localPosition=new Vector3(side,.84f,0);go.transform.localScale=new Vector3(1,3.5f,1);
                go.AddComponent<MeshFilter>().sharedMesh=_mesh;var r=go.AddComponent<MeshRenderer>();r.sharedMaterial=_material;r.sortingOrder=6;
                r.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;r.receiveShadows=false;
            }
        }
        public static void SetEnabled(Transform car,bool enabled)
        {foreach(Transform child in car)if(child.name=="Farol_Fusca")child.gameObject.SetActive(enabled);}
    }
}
