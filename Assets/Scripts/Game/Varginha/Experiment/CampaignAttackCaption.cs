using Game.UI;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignAttackCaption
    {
        public static void Draw(Vector3 world,string character,string ability,Color accent,int lane=0)
        {
            if(Camera.main==null||VarginhaGameHUD.Instance?.IsInventoryOpen==true)return;
            var point=Camera.main.WorldToScreenPoint(world);if(point.z<=0)return;
            float width=Mathf.Min(340,Screen.width-24),height=58;
            var rect=new Rect(Mathf.Round(Mathf.Clamp(point.x-width/2,12,Mathf.Max(12,Screen.width-width-12))),
                Mathf.Round(Mathf.Clamp(Screen.height-point.y+lane*64,145,Mathf.Max(145,Screen.height-height-24))),width,height);
            PixelHUDFrame.Draw(new Rect(rect.x+3,rect.y+3,width,height),Texture2D.whiteTexture,new Color(0,0,0,.55f),new Color(0,0,0,0));
            PixelHUDFrame.Draw(rect,Texture2D.whiteTexture,new Color(.025f,.045f,.065f,.96f),accent);
            var before=GUI.color;GUI.color=accent;
            GUI.DrawTexture(new Rect(rect.x+10,rect.y+11,4,4),Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x+14,rect.y+15,4,4),Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x+10,rect.y+19,4,4),Texture2D.whiteTexture);GUI.color=before;
            PixelMenuTheme.Label(new Rect(rect.x+28,rect.y+8,width-40,16),character.ToUpperInvariant(),9,accent,TextAnchor.MiddleLeft);
            PixelMenuTheme.Label(new Rect(rect.x+28,rect.y+28,width-40,22),ability.ToUpperInvariant(),12,ExperimentGUI.Paper,TextAnchor.MiddleLeft);
        }
    }
}
