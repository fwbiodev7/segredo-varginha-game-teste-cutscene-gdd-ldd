using System;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    [Serializable] public sealed class CampaignContinuationState
    {
        public int[] clues = new int[11];
        public bool[] solved = new bool[11];
        public int area, chambersPrepared, finalStep;
        public bool serviceRevealed, serviceKey, manifestationDispelled, finished;
        public bool AgreementComplete => solved[7] && solved[8];
        public bool CanReturn => manifestationDispelled && finalCalibrated && chambersPrepared == 7 && (finalStep > 0 || FinalStable);
        public bool CalibrateReturn()
        {
            if (!manifestationDispelled || chambersPrepared != 7 || !FinalStable) return false;
            solved[9]=true; return finalCalibrated = true;
        }
        public int[] chamberValues = { 0, 0, 0 };
        public int[] battleStudents={-1,-1,-1};
        public int battleSupport=-1;
        public int[] finalRegulators={0,0,0};
        public bool finalSealActive=true,finalCalibrated;
        public int FinalReading(int index)=>(2*finalRegulators[index]+finalRegulators[(index+2)%3])%4;
        public bool FinalStable=>finalSealActive&&FinalReading(0)==3&&FinalReading(1)==1&&FinalReading(2)==2;
        public void Repair()
        {
            if (clues == null || clues.Length != 11) Array.Resize(ref clues,11);
            if (solved == null || solved.Length != 11) Array.Resize(ref solved,11);
            for (int i=0;i<11;i++) clues[i] &= 255;
            if (chamberValues == null || chamberValues.Length != 3) chamberValues = new int[3];
            for (int i=0;i<3;i++) chamberValues[i]=Mathf.Clamp(chamberValues[i],0,2);
            area=Mathf.Clamp(area,0,2);chambersPrepared &= 7;finalStep=Mathf.Clamp(finalStep,0,4);
            if(battleStudents==null||battleStudents.Length!=3)battleStudents=new[]{-1,-1,-1};
            for(int i=0;i<3;i++){battleStudents[i]=Mathf.Clamp(battleStudents[i],-1,8);for(int j=0;j<i;j++)if(battleStudents[j]==battleStudents[i])battleStudents[i]=-1;}
            battleSupport=Mathf.Clamp(battleSupport,-1,2);
            if(finalRegulators==null||finalRegulators.Length!=3)finalRegulators=new int[3];
            for(int i=0;i<3;i++)finalRegulators[i]=Mathf.Clamp(finalRegulators[i],0,3);
        }
        public bool HasAll(int phase,int count) => (clues[phase-11]&((1<<count)-1))==((1<<count)-1);
        public void Read(int phase,int index) => clues[phase-11] |= 1<<index;
        public void TurnFinalRegulator(int index)
        {
            finalRegulators[index]=(finalRegulators[index]+1)%4;
            finalCalibrated=solved[9]=false;
        }
        public void SetFinalSeal(bool active)
        {
            finalSealActive=active;finalCalibrated=solved[9]=false;
        }
    }
}
