// Live Editor packing: fixed four directions/six intermediate poses, one scale across all attack poses.
foreach(bool child in new[]{false,true})
{
    string name=child?"Child":"Boss",path="ArtSources/BossCinematic20261005/"+name+(child?"FlowSource.png":"FlowSourceV2.png");
    var source=new UnityEngine.Texture2D(2,2,UnityEngine.TextureFormat.RGBA32,false);UnityEngine.ImageConversion.LoadImage(source,System.IO.File.ReadAllBytes(path));
    int sw=source.width/6,sh=source.height/4,cw=child?96:160,ch=child?128:192,maxW=0,maxH=0;
    int[] yEdges=FindGutters(source,4,false,0,source.width);
    var xEdges=new int[4][];for(int row=0;row<4;row++)xEdges[row]=FindGutters(source,6,true,yEdges[row],yEdges[row+1]);
    var bounds=new UnityEngine.RectInt[24];
    for(int i=0;i<24;i++)
    {
        int row=3-i/6,left=xEdges[row][i%6],bottom=yEdges[row],cellW=xEdges[row][i%6+1]-left,cellH=yEdges[row+1]-bottom,x0=cellW,y0=cellH,x1=0,y1=0;
        for(int y=0;y<cellH;y++)for(int x=0;x<cellW;x++)if(source.GetPixel(left+x,bottom+y).a>.8f)
        {x0=System.Math.Min(x0,x);x1=System.Math.Max(x1,x);y0=System.Math.Min(y0,y);y1=System.Math.Max(y1,y);}
        if(x1<=x0||y1<=y0)throw new System.Exception("Empty frame "+i+" in "+name);
        bounds[i]=new UnityEngine.RectInt(left+x0,bottom+y0,x1-x0+1,y1-y0+1);maxW=System.Math.Max(maxW,x1-x0+1);maxH=System.Math.Max(maxH,y1-y0+1);
    }
    float scale=UnityEngine.Mathf.Min((cw-12f)/maxW,(ch-16f)/maxH,(float)Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText("Assets/Resources/Varginha/StoryCharacters/Manifestation"+name+"CombatV2.json"))["frames"][0]["height"]/bounds[0].height);
    var atlas=new UnityEngine.Texture2D(cw*6,ch*4,UnityEngine.TextureFormat.RGBA32,false);atlas.SetPixels(new UnityEngine.Color[cw*6*ch*4]);
    var sizes=new Newtonsoft.Json.Linq.JArray();
    for(int i=0;i<24;i++)
    {
        var b=bounds[i];int w=UnityEngine.Mathf.RoundToInt(b.width*scale),h=UnityEngine.Mathf.RoundToInt(b.height*scale);
        for(int y=0;y<h;y++)for(int x=0;x<w;x++)
        {
            var color=source.GetPixel(b.x+UnityEngine.Mathf.Min(b.width-1,UnityEngine.Mathf.FloorToInt(x/scale)),b.y+UnityEngine.Mathf.Min(b.height-1,UnityEngine.Mathf.FloorToInt(y/scale)));
            atlas.SetPixel(i%6*cw+(cw-w)/2+x,(3-i/6)*ch+8+y,color);
        }
        sizes.Add(new Newtonsoft.Json.Linq.JObject{["width"]=w,["height"]=h});
    }
    string target="Assets/Resources/Varginha/StoryCharacters/Manifestation"+name+"FlowV1.png";System.IO.File.WriteAllBytes(target,atlas.EncodeToPNG());
    System.IO.File.WriteAllText(target.Replace(".png",".json"),new Newtonsoft.Json.Linq.JObject{["frames"]=sizes}.ToString());
    UnityEngine.Object.DestroyImmediate(source);UnityEngine.Object.DestroyImmediate(atlas);
    UnityEditor.AssetDatabase.ImportAsset(target,UnityEditor.ImportAssetOptions.ForceSynchronousImport);
    var importer=(UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(target);
    importer.textureType=UnityEditor.TextureImporterType.Default;importer.alphaIsTransparency=true;importer.filterMode=UnityEngine.FilterMode.Point;
    importer.mipmapEnabled=false;importer.isReadable=false;importer.textureCompression=UnityEditor.TextureImporterCompression.Uncompressed;importer.maxTextureSize=2048;importer.SaveAndReimport();
    UnityEditor.AssetDatabase.ImportAsset(target.Replace(".png",".json"),UnityEditor.ImportAssetOptions.ForceSynchronousImport);
}
return "Packed boss and child combat in four directions, six intermediate poses and shared frame scale.";
int[] FindGutters(UnityEngine.Texture2D texture,int cells,bool horizontal,int from,int to)
{
    int dimension=horizontal?texture.width:texture.height;float step=dimension/(float)cells;var edges=new int[cells+1];edges[cells]=dimension;
    for(int i=1;i<cells;i++)
    {
        float best=float.MaxValue;int centre=UnityEngine.Mathf.RoundToInt(i*step),selected=centre;
        for(int p=System.Math.Max(edges[i-1]+8,centre-(int)(step*.4f));p<System.Math.Min(dimension-8,centre+(int)(step*.4f));p++)
        {
            int occupied=0;for(int q=from;q<to;q++)if(texture.GetPixel(horizontal?p:q,horizontal?q:p).a>.8f)occupied++;
            float score=occupied*1000+System.Math.Abs(p-centre);if(score<best){best=score;selected=p;}
        }
        edges[i]=selected;
    }
    return edges;
}
