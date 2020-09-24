using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VEE.RegularEvents
{
    class AnimalTransportPodCrash : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Thing> things = VEE_DefOf.AnimalPod.root.Generate();
            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
            Pawn pawn = this.RandomPawnFromThingList(things, parms);
            pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full);
            pawn.health.AddHediff(VEE_DefOf.MightJoin);

            string label = "LetterLabelAnimalPodCrash".Translate();
            string text = "AnimalPodCrash".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");

            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, new TargetInfo(intVec, map, false), null, null);
            ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
            activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(things, true, false);
            activeDropPodInfo.openDelay = 180;
            activeDropPodInfo.leaveSlag = true;
            DropPodUtility.MakeDropPodAt(intVec, map, activeDropPodInfo);
            return true;
        }

        private Pawn RandomPawnFromThingList(List<Thing> things, IncidentParms parms)
        {
            Map map = (Map)parms.target;
            bool done = false;
            float percent = 0.001f;
            while (!done)
            {
                Pawn randPawn = things.RandomElement() as Pawn;
                if (randPawn.MarketValue < map.wealthWatcher.WealthTotal * percent)
                {
                    return randPawn;
                }
                else
                {
                    percent += 0.001f;
                }
            }
            return null;
        }
    }
}
