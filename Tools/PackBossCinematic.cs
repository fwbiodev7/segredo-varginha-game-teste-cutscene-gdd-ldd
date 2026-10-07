// Run in the live Editor. Keep one scale for all poses, including emerging/dissolving frames.
var names=new[]{"Manifestation","Rift"};
foreach(string name in names)
{
    var source=new UnityEngine.Texture2D(2,2,UnityEngine.TextureFormat.RGBA32,false);
    UnityEngine.ImageConversion.LoadImage(source,System.IO.File.ReadAllBytes("ArtSources/BossCinematic20261005/"+name+"Source.png"));
    int sw=source.width/6,sh=source.height/2,cw=name=="Rift"?160:128,ch=160;
    var bounds=new UnityEngine.RectInt[12];int maxW=0,maxH=0;
    for(int i=0;i<12;i++)
    {
        int left=(i%6)*sw,bottom=(1-i/6)*sh,x0=sw,y0=sh,x1=0,y1=0;
        for(int y=0;y<sh;y++)for(int x=0;x<sw;x++)if(source.GetPixel(left+x,bottom+y).a>.2f)
        {x0=System.Math.Min(x0,x);x1=System.Math.Max(x1,x);y0=System.Math.Min(y0,y);y1=System.Math.Max(y1,y);}
        bounds[i]=new UnityEngine.RectInt(x0,y0,x1-x0+1,y1-y0+1);maxW=System.Math.Max(maxW,x1-x0+1);maxH=System.Math.Max(maxH,y1-y0+1);
    }
    float scale=UnityEngine.Mathf.Min((cw-12f)/maxW,(ch-16f)/maxH);
    var atlas=new UnityEngine.Texture2D(cw*6,ch*2,UnityEngine.TextureFormat.RGBA32,false);
    atlas.SetPixels(new UnityEngine.Color[cw*6*ch*2]);
    for(int i=0;i<12;i++)
    {
        var b=bounds[i];int w=UnityEngine.Mathf.RoundToInt(b.width*scale),h=UnityEngine.Mathf.RoundToInt(b.height*scale);
        for(int y=0;y<h;y++)for(int x=0;x<w;x++)
        {
            var color=source.GetPixel((i%6)*sw+b.x+UnityEngine.Mathf.Min(b.width-1,UnityEngine.Mathf.FloorToInt(x/scale)),(1-i/6)*sh+b.y+UnityEngine.Mathf.Min(b.height-1,UnityEngine.Mathf.FloorToInt(y/scale)));
            atlas.SetPixel((i%6)*cw+(cw-w)/2+x,(1-i/6)*ch+8+y,color);
        }
    }
    string target="Assets/Resources/Varginha/StoryEffects/Boss"+name+"CinematicV1.png";
    System.IO.File.WriteAllBytes(target,atlas.EncodeToPNG());
    UnityEngine.Object.DestroyImmediate(source);UnityEngine.Object.DestroyImmediate(atlas);
    UnityEditor.AssetDatabase.ImportAsset(target,UnityEditor.ImportAssetOptions.ForceSynchronousImport);
    var importer=(UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(target);
    importer.textureType=UnityEditor.TextureImporterType.Default;importer.alphaIsTransparency=true;importer.filterMode=UnityEngine.FilterMode.Point;
    importer.mipmapEnabled=false;importer.isReadable=false;importer.textureCompression=UnityEditor.TextureImporterCompression.Uncompressed;importer.maxTextureSize=1024;
    importer.SaveAndReimport();
}
return "Packed two six-frame entrance/exit atlases with transparent pixels, consistent scale and feet pivots.";
