using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Same photograph for both chapters, composed from the project's existing pixel artwork.
    public static class CampaignSchoolEvidence
    {
        public static void Draw(Rect area,bool timeMarks)
        {
            ExperimentGUI.Box(area,new Color(.72f,.66f,.48f));
            var photo=new Rect(area.x+12,area.y+12,area.width-24,area.height-65);
            var texture=Resources.Load<Texture2D>(VarginhaIndustrialSchoolFacade.ResourcePath);
            if(texture!=null)GUI.DrawTexture(photo,texture,ScaleMode.StretchToFill);
            var figure=CampaignStorySprites.Frame("Life",0,0);
            if(figure!=null)
            {
                var old=GUI.color;GUI.color=new Color(.06f,.06f,.07f,.95f);
                // The existing frontage artwork contains a central gateway. Keep the clue inside it.
                GUI.DrawTextureWithTexCoords(new Rect(photo.x+photo.width*.535f,photo.y+photo.height*.43f,photo.width*.04f,photo.height*.32f),figure.texture,
                    new Rect(figure.rect.x/figure.texture.width,figure.rect.y/figure.texture.height,figure.rect.width/figure.texture.width,figure.rect.height/figure.texture.height));
                GUI.color=old;
            }
            var color=GUI.color;GUI.color=new Color(.14f,.16f,.15f);
            ExperimentGUI.Label(new Rect(area.x+20,area.yMax-43,area.width-40,32),timeMarks?"23  :  23":"FOTOGRAFIA • 1996",small:true);
            GUI.color=color;
        }
    }
}
