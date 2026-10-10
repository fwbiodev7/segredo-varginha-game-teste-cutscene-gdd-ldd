using UnityEngine;

namespace Game.Varginha
{
    public readonly struct MenuUfoFlightFrame
    {
        public readonly Vector2 Offset;
        public readonly float Presence,Trail;
        public readonly bool Fast;
        public MenuUfoFlightFrame(Vector2 offset,float presence,float trail,bool fast=false)
        {Offset=offset;Presence=presence;Trail=trail;Fast=fast;}
    }

    public static class MenuUfoFlight
    {
        public const float HoverDuration=18,ChargeDuration=1.2f,DepartureAt=HoverDuration+ChargeDuration;
        public const float DepartureDuration=.55f,ReturnAt=38,ReturnDuration=2.5f,CycleDuration=52;
        private static Vector2 Hover(float t)=>new(Mathf.Sin(t*.52f)*.025f+Mathf.Sin(t*.93f)*.008f,Mathf.Sin(t*.74f)*.011f);
        public static MenuUfoFlightFrame Sample(float elapsed)
        {
            float absolute=Mathf.Max(0,elapsed),t=Mathf.Repeat(absolute,CycleDuration),start=absolute-t;
            if(t<HoverDuration)return new(Hover(absolute),1,0);
            if(t<DepartureAt)
            {
                float p=Mathf.SmoothStep(0,1,(t-HoverDuration)/ChargeDuration);
                return new(Hover(start+HoverDuration)+new Vector2(-.008f,-.004f)*p,1,0);
            }
            if(t<DepartureAt+DepartureDuration)
            {
                float p=(t-DepartureAt)/DepartureDuration;
                return new(Hover(start+HoverDuration)+new Vector2(-.008f,-.004f)+new Vector2(.72f,.28f)*p*p*p,
                    1-Mathf.SmoothStep(0,1,p),Mathf.Sin(p*Mathf.PI),true);
            }
            if(t<ReturnAt)return new(new Vector2(.72f,.28f),0,0);
            if(t<ReturnAt+ReturnDuration)
            {
                float p=Mathf.SmoothStep(0,1,(t-ReturnAt)/ReturnDuration);
                return new(Vector2.Lerp(new Vector2(-.95f,.10f),Hover(absolute),p),p,0);
            }
            return new(Hover(absolute),1,0);
        }
    }
}
