using UnityEngine;

namespace Game.Varginha.Experiment
{
    // New sheets are additive. Existing NPC rules, support damage and cooldowns
    // remain with their controllers; this class only chooses native art poses.
    public static class CampaignOuzanaVisual
    {
        private static Material _material;
        public static Material Material
        {
            get
            {
                if(_material==null)
                {
                    _material=new Material(Resources.Load<Shader>("Varginha/Flash1996/CrispPixelSprite")){name="Ouzana — óculos castanhos"};
                    _material.SetFloat("_Cutoff",220f/255f);
                }
                return _material;
            }
        }
        public static Sprite Idle(Vector2 face)=>CampaignStorySprites.Frame("OuzanaAttackV2",CampaignStorySprites.EightDirection(face),5);
        public static Sprite Walk(Vector2 face,float clock)=>CampaignStorySprites.Frame("OuzanaWalkV2",CampaignStorySprites.EightDirection(face),Mathf.FloorToInt(Mathf.Max(0,clock)/.11f)%6);
        public static Sprite Attack(Vector2 face,int frame)=>CampaignStorySprites.Frame("OuzanaAttackV2",CampaignStorySprites.EightDirection(face),Mathf.Clamp(frame,0,5));
    }
}
