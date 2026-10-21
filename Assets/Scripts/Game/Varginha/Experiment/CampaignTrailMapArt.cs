using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Artwork only: trail rules, hit targets and saved choices remain in CampaignTrailPuzzle.
    public static class CampaignTrailMapArt
    {
        public const string TerrainPath="Varginha/TrailPuzzle/Terrain";
        public const string LandmarksPath="Varginha/TrailPuzzle/Landmarks";
        public static readonly Rect MapRect=new(447,262,665,214);
        public static void DrawTerrain()
        {
            var terrain=CampaignCinemaTextures.ResolveResource(TerrainPath);
            if(terrain!=null)GUI.DrawTexture(MapRect,terrain);
        }
        public static bool InRiver(float x)=>x>=749&&x<=804;
        public static Rect LandmarkUv(int index)=>new(index%3/3f,1-(index/3+1)/2f,1/3f,1/2f);
        private static void Landmark(int index,Rect target)
        {
            var atlas=CampaignCinemaTextures.ResolveResource(LandmarksPath);
            if(atlas!=null)GUI.DrawTextureWithTexCoords(target,atlas,LandmarkUv(index));
        }
        public static void DrawLandmarks()
        {
            Landmark(0,new Rect(440,326,51,56)); // the tree's roots meet the start
            Landmark(1,new Rect(542,303,51,40)); // log covers upper first branch
            Landmark(2,new Rect(735,295,80,53)); // sole bridge spans both river banks
            Landmark(3,new Rect(946,297,61,48)); // continuous wall blocks upper final branch
            Landmark(5,new Rect(946,382,61,48)); // lower path crosses the open gate
            Landmark(4,new Rect(1047,317,68,76)); // door meets the final junction
        }
    }
}
