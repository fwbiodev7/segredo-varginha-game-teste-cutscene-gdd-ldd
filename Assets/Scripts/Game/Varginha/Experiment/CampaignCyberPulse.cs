using System.Collections.Generic;
using UnityEngine;
namespace Game.Varginha.Experiment
{
 // One mesh per cast, capped at 256 quads and six targets; no emitters or gameplay RNG.
 public sealed class CampaignCyberPulse:MonoBehaviour
 {
  public const int MaxQuads=256,MaxTargets=6;
  readonly List<Vector3> vertices=new(MaxQuads*4);readonly List<Color> colours=new(MaxQuads*4);
  readonly List<int> triangles=new(MaxQuads*6);readonly List<Vector2> uv=new(MaxQuads*4);
  Mesh mesh;Material material;GameObject meshRoot;public int QuadCount=>vertices.Count/4;
  static readonly Color Cyan=new(.15f,.95f,1),Pink=new(1,.18f,.72f),Green=new(.4f,1,.55f);
  void Awake()
  {
   mesh=new Mesh{name="Pulso_cyber_limitado"};mesh.MarkDynamic();
   meshRoot=new GameObject("Pacotes_cyber");meshRoot.transform.SetParent(transform,false);meshRoot.AddComponent<MeshFilter>().sharedMesh=mesh;
   var renderer=meshRoot.AddComponent<MeshRenderer>();renderer.sortingOrder=30001;
   material=new Material(Shader.Find("Sprites/Default"));material.mainTexture=Texture2D.whiteTexture;renderer.sharedMaterial=material;
  }
  void Block(float x,float y,float w,float h,Color colour)
  {
   if(QuadCount>=MaxQuads)return;const float grid=48;x=Mathf.Round(x*grid)/grid;y=Mathf.Round(y*grid)/grid;int n=vertices.Count;
   vertices.Add(transform.InverseTransformPoint(new Vector3(x,y)));vertices.Add(transform.InverseTransformPoint(new Vector3(x+w,y)));
   vertices.Add(transform.InverseTransformPoint(new Vector3(x+w,y+h)));vertices.Add(transform.InverseTransformPoint(new Vector3(x,y+h)));
   for(int i=0;i<4;i++){colours.Add(colour);uv.Add(Vector2.one*.5f);}
   triangles.Add(n);triangles.Add(n+2);triangles.Add(n+1);triangles.Add(n);triangles.Add(n+3);triangles.Add(n+2);
  }
  public void Draw(Vector2 start,Vector2 end,float progress,float clock,bool impact,IReadOnlyList<SpriteRenderer> targets)
  {
   vertices.Clear();colours.Clear();triangles.Clear();uv.Clear();bool reduced=VarginhaGameSettings.Current.reducedMotion;
   int frame=reduced?0:Mathf.FloorToInt(clock*12);float alpha=impact?Mathf.Clamp01(1-progress):1;
   // A radial footprint matches the existing hit test, including targets behind the sender.
   for(int i=0;i<64;i++)
   {float angle=i*Mathf.PI*2/64;var p=start+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*CampaignFinalAllies.PulseRadius;var c=Cyan;c.a=alpha*.27f;Block(p.x-.06f,p.y-.04f,.12f,.08f,c);}
   if(!impact)
   {
    for(int i=0;i<22;i++)
    {float u=(i/22f+(reduced?.25f:progress*1.6f))%1;var p=Vector2.Lerp(start,end,u);p.y+=((i*7%5)-2)*.055f;var c=i%5==0?Pink:Cyan;c.a=.8f;Block(p.x,p.y,.12f+(i%3)*.045f,.045f,c);}
    for(int i=0;i<12;i++){var c=i%3==0?Green:Cyan;c.a=.7f;Block(start.x-.6f+(i%4)*.14f,start.y+.35f+(i/4)*.12f,.08f,.045f,c);}
   }
   else if(targets!=null)
   {
    for(int t=0;t<Mathf.Min(MaxTargets,targets.Count);t++)
    {
     var target=targets[t];if(target==null)continue;var b=target.bounds;
     for(int i=0;i<24;i++)
     {int seed=(i*37+frame*19+t*11)%101;float y=b.min.y+b.size.y*((i*17+frame*3)%97)/97f,x=b.min.x+b.size.x*(seed/101f)-.08f;var c=i%4==0?Pink:i%3==0?Green:Cyan;c.a=alpha*(i%2==0?.75f:.45f);Block(x,y,b.size.x*(.12f+(seed%7)*.035f),.035f+(i%3)*.018f,c);}
    }
   }
   mesh.Clear();mesh.SetVertices(vertices);mesh.SetColors(colours);mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);mesh.RecalculateBounds();
  }
  void OnDestroy(){if(meshRoot!=null)Destroy(meshRoot);if(mesh!=null)Destroy(mesh);if(material!=null)Destroy(material);}
 }
}
