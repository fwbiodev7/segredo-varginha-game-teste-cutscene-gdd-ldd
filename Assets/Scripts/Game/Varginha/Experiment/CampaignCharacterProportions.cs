using System.Collections.Generic;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Preserve the original church scale. Other maps use the same adult scale
    // as Edelzio/Renan/Ouzana, by changing PPU and foot registration only.
    public static class CampaignCharacterProportions
    {
        static readonly Dictionary<Sprite,Sprite> PriestFrames=new();
        public static bool IsChurch(int phase)=>phase==8||phase==14;
        public static Sprite Priest(Sprite source,int phase)
        {
            if(source==null||IsChurch(phase))return source;
            if(PriestFrames.TryGetValue(source,out var cached)&&cached!=null)return cached;
            float factor=CampaignTeamEdelzio.StandingHeight/CampaignTeamEdelzio.ChurchStandingHeight;
            float ppu=source.pixelsPerUnit/factor;
            var rect=source.rect;float centre=rect.center.x;
            if(VarginhaCharacterFrameGeometry.TryBounds(source.texture,rect,out var body,out var anchor)){rect=body;centre=anchor;}
            var pivot=new Vector2((centre-rect.x)/rect.width,.58f*ppu/rect.height);
            var sprite=VarginhaCharacterFrameGeometry.Create(source.texture,rect,pivot,ppu,0,SpriteMeshType.FullRect);
            sprite.name=source.name+"_AdultProportion";return PriestFrames[source]=sprite;
        }
    }
}
