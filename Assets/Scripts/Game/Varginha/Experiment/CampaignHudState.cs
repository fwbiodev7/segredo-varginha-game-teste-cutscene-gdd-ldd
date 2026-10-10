using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Advances only while the player has control; IMGUI repaint events never tick it.
    public sealed class CampaignHudState
    {
        public const float IdentificationSeconds=12, FadeSeconds=2;
        public float Elapsed { get; private set; }
        public float GoalElapsed { get; private set; }
        public float RoomElapsed { get; private set; }
        public float TutorialElapsed { get; private set; }
        private string _tutorial;
        public string Goal { get; private set; }
        public string Room { get; private set; }
        public float IdentificationAlpha=>Fade(Elapsed,IdentificationSeconds,FadeSeconds);
        public float GoalAlpha=>Fade(GoalElapsed,4,1);
        public float TutorialAlpha=>Fade(TutorialElapsed,6,2);
        public void BeginTutorial(string mechanic)
        {if(mechanic==_tutorial)return;_tutorial=mechanic;TutorialElapsed=0;}
        public float RoomAlpha=>string.IsNullOrEmpty(Room)?0:Fade(RoomElapsed,3,1);
        public void Tick(float delta,bool playing,string goal,string room=null)
        {
            if(!playing||delta<0||float.IsNaN(delta)||float.IsInfinity(delta))return;
            if(goal!=Goal){Goal=goal;GoalElapsed=0;}else GoalElapsed+=delta;
            if(room!=Room){Room=room;RoomElapsed=0;}else RoomElapsed+=delta;
            Elapsed+=delta;
            TutorialElapsed+=delta;
        }
        private static float Fade(float elapsed,float hold,float duration)
            =>1-Mathf.SmoothStep(0,1,Mathf.Clamp01((elapsed-hold)/duration));
    }

    public static class CampaignHud
    {
        private static int _chapter=-1;
        private static CampaignHudState _current;
        public static CampaignHudState For(int phase)
        {
            int chapter=CampaignSequence.Chapter(phase);
            if(_current==null||_chapter!=chapter){_chapter=chapter;_current=new CampaignHudState();}
            return _current;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset(){_chapter=-1;_current=null;}
    }
}
