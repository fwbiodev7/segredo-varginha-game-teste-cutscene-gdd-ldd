using System;
using UnityEngine;

namespace Game.Varginha
{
    // A private random stream keeps menu events independent of puzzles and combat RNG.
    public sealed class MenuSuspenseClock
    {
        private readonly System.Random _random;
        private float _lookStarted=-100, _glitchUntil, _titleUntil;
        public float NextIdle { get; private set; }
        public float NextGlitch { get; private set; }
        public float NextTitle { get; private set; }
        public float Gaze(float now) => Mathf.SmoothStep(0,1,Mathf.Clamp01((now-_lookStarted)/2.4f))
            *(1-Mathf.SmoothStep(0,1,Mathf.Clamp01((now-_lookStarted-4.5f)/2.5f)));
        public bool Glitch(float now) => now<_glitchUntil;
        public bool TitleGlitch(float now) => now<_titleUntil;
        public MenuSuspenseClock(int seed,float now)
        {
            _random=new System.Random(seed); Activity(now);
            NextGlitch=now+Range(11,26); NextTitle=now+Range(24,48);
        }
        private float Range(float lo,float hi) => lo+(float)_random.NextDouble()*(hi-lo);
        public void Activity(float now) { NextIdle=now+Range(40,90); _lookStarted=-100; }
        public bool Tick(float now,bool allow)
        {
            if(!allow){_lookStarted=-100;_glitchUntil=_titleUntil=0;return false;}
            if(now>=NextGlitch){_glitchUntil=now+.1f;NextGlitch=now+Range(11,26);}
            if(now>=NextTitle){_titleUntil=now+.08f;NextTitle=now+Range(24,48);}
            if(now<NextIdle)return false;
            _lookStarted=now;NextIdle=now+Range(40,90);_glitchUntil=now+.28f;return true;
        }
    }
}
