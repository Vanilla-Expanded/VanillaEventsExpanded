using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VEE.RegularEvents
{
    public class AnimalTransportPodCrash : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Thing> things = VEE_DefOf.AnimalPod.root.Generate();

            Pawn pawn = this.RandomPawnFromThingList(things, map);
            pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full);
            pawn.health.AddHediff(VEE_DefOf.MightJoin);
            HealthUtility.DamageUntilDowned(pawn);

            string label = "LetterLabelAnimalPodCrash".Translate();
            string text = "AnimalPodCrash".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");

            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, new TargetInfo(intVec, map, false), null, null);
            ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
            activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(things, true, false);
            activeDropPodInfo.openDelay = 180;
            activeDropPodInfo.leaveSlag = true;
            DropPodUtility.MakeDropPodAt(intVec, map, activeDropPodInfo);
            return true;
        }

        private Pawn RandomPawnFromThingList(List<Thing> things, Map map)
        {
            if (things != null && things.Count > 0)
            {
                float wealthTotal = map.wealthWatcher.WealthTotal;
                float percent = 0.001f;
                while (true)
                {
                    if (things.RandomElement() is Pawn p && p.MarketValue < wealthTotal * percent)
                        return p;
                    else
                        percent += 0.005f;
                }
            }
            return null;
        }
    }
}