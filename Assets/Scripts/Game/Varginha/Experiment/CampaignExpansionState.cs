using System;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    [Serializable]
    public sealed class CampaignExpansionState
    {
        public int visited, truthClues, forestSigns, anchorClues, labClues, sprayed, reagentCharges = 6, trust;
        public int anchorYear = 1898, anchorSymbol, anchorRecord = 1;
        public bool mapSolved, fabioMet, anchorFound, evidencePresented, reagentUnlocked, stabilized, testDriven;
        public bool workshopParked,workshopDeparted,workshopHasCarPosition;
        public float workshopCarX,workshopCarY;
        public int workshopHeading;
        public bool workshopHasYaw;
        public float workshopYaw;
        public bool forestHasCheckpoint;
        public float forestCheckpointX,forestCheckpointY;
        public int forestCheckpointSigns;
        public int[] mapOrder = { 2, 0, 1 }, sampleOrder = { 2, 1, 0 }, sealOrder = { 2, 0, 1 };
        public int[] mapDirections = { 0, 2, 1 };
        public int[] trailChoices = { -1, -1, -1 };
        public int trailHints,trailFeedback;
        public int[] bookOrder={2,0,1},bookLinks={-1,-1,-1};
        public int bookHints,bookFeedback;
        public bool SolveMap()
        {
            if(mapSolved&&visited==7)return true;
            if(!CampaignTrailPuzzle.Validate(this))return false;
            return mapSolved = true;
        }
        public bool SolveAnchor(int year, int symbol, int record)
        {
            if(!CampaignBookPuzzle.Ready(this))return false;
            if (year != 1996 || symbol != 1 || record != 23) return false;
            anchorYear = year; anchorSymbol = symbol; anchorRecord = record;
            anchorClues = 7;
            return anchorFound = true;
        }
        public bool SolveSamples()
        {
            if (!anchorFound || !evidencePresented) return false;
            reagentUnlocked = true; reagentCharges = Mathf.Max(6, reagentCharges); return true;
        }
        public bool Spray(int region)
        {
            if (!reagentUnlocked || region < 0 || region > 2 || reagentCharges <= 0 || (sprayed & (1 << region)) != 0) return false;
            sprayed = 7; reagentCharges--; stabilized = true; return true;
        }
        public bool Stabilize()
        {
            if (!reagentUnlocked || sprayed != 7) return false;
            return stabilized = true;
        }
        public bool Complete(int phase) => phase == 6 ? mapSolved : phase == 7 ? fabioMet : phase == 8 ? anchorFound
            : phase == 9 ? reagentUnlocked : phase == 10 && stabilized;
        public void Repair()
        {
            visited &= 7; truthClues &= 7; forestSigns = Mathf.Clamp(forestSigns, 0, 3); anchorClues &= 7; labClues &= 7; sprayed &= 7;
            forestCheckpointSigns=Mathf.Clamp(forestCheckpointSigns,0,3);
            if(float.IsNaN(forestCheckpointX)||float.IsInfinity(forestCheckpointX)||float.IsNaN(forestCheckpointY)||float.IsInfinity(forestCheckpointY))forestHasCheckpoint=false;
            reagentCharges = Mathf.Clamp(reagentCharges, 0, 6); trust = Mathf.Clamp(trust, -1, 1);
            anchorYear = anchorYear == 1996 ? 1996 : 1898; anchorSymbol = Mathf.Clamp(anchorSymbol,0,1); anchorRecord = anchorRecord == 23 ? 23 : 1;
            RepairOrder(ref mapOrder); RepairOrder(ref sampleOrder); RepairOrder(ref sealOrder);
            if(mapDirections==null||mapDirections.Length!=3)mapDirections=new[]{0,2,1};
            for(int i=0;i<3;i++)mapDirections[i]=Mathf.Clamp(mapDirections[i],0,3);
            if(mapSolved)mapDirections=new[]{1,1,0};
            if (visited != 7) mapSolved = false;
            if(trailChoices==null||trailChoices.Length!=3)trailChoices=new[]{-1,-1,-1};
            for(int i=0;i<3;i++)
            {if(trailChoices[i]<0||trailChoices[i]>1)trailChoices[i]=-1;}
            if(mapSolved)for(int i=0;i<3;i++)trailChoices[i]=CampaignTrailPuzzle.CorrectBranch(i);
            trailHints=Mathf.Clamp(trailHints,0,3);trailFeedback=Mathf.Clamp(trailFeedback,0,5);
            if (!mapSolved || forestSigns < 3) fabioMet = false;
            RepairOrder(ref bookOrder);
            if(bookLinks==null||bookLinks.Length!=3)bookLinks=new[]{-1,-1,-1};
            for(int i=0;i<3;i++)if(bookLinks[i]<0||bookLinks[i]>2||(anchorClues&(1<<bookLinks[i]))==0)bookLinks[i]=-1;
            bookHints=Mathf.Clamp(bookHints,0,3);bookFeedback=Mathf.Clamp(bookFeedback,0,3);
            // A solved book remains solved across reloads, including older saves
            // whose optional inspection/history flags were not registered.
            if (anchorFound && anchorClues != 7 && (anchorYear != 1996 || anchorSymbol != 1 || anchorRecord != 23)) anchorFound = false;
            if (anchorFound) { anchorClues = 7; anchorYear = 1996; anchorSymbol = 1; anchorRecord = 23; bookOrder=new[]{0,1,2};bookLinks=new[]{0,1,2}; }
            if (!anchorFound) evidencePresented = false;
            if (!anchorFound || !evidencePresented) reagentUnlocked = false;
            if (!reagentUnlocked) { sprayed = 0; stabilized = false; }
            if (sprayed != 7) stabilized = false;
            if (!stabilized) testDriven = false;
            // Existing investigations resume at the bench rather than repeating the arrival.
            if(sprayed!=0||stabilized||testDriven)workshopParked=true;
            if(!stabilized)workshopDeparted=false;
            workshopHeading=Mathf.Clamp(workshopHeading,0,3);
            if(float.IsNaN(workshopYaw)||float.IsInfinity(workshopYaw)){workshopHasYaw=false;workshopYaw=0;}
            workshopYaw=Mathf.Repeat(workshopYaw+180,360)-180;
            if(float.IsNaN(workshopCarX)||float.IsInfinity(workshopCarX)||float.IsNaN(workshopCarY)||float.IsInfinity(workshopCarY))
            {workshopHasCarPosition=false;workshopCarX=workshopCarY=0;}
            workshopCarX=Mathf.Clamp(workshopCarX,-10,10);workshopCarY=Mathf.Clamp(workshopCarY,-5.9f,5.9f);
        }
        private static void RepairOrder(ref int[] order)
        {
            if (order == null || order.Length != 3 || order[0] < 0 || order[0] > 2 || order[1] < 0 || order[1] > 2
                || order[2] < 0 || order[2] > 2 || order[0] == order[1] || order[0] == order[2] || order[1] == order[2]) order = new[] { 2, 0, 1 };
        }
    }
}
