using System.Linq;
using RimWorld;
using Verse;

namespace VEE.RegularEvents
{
    public class AnimalTransportPodCrash : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = (Map)parms.target;

            var pawn = RandomAnimalByWeight();
            pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full);
            pawn.health.AddHediff(VEE_DefOf.MightJoin);
            HealthUtility.DamageUntilDowned(pawn);

            var intVec = DropCellFinder.RandomDropSpot(map);
            Find.LetterStack.ReceiveLetter("LetterLabelAnimalPodCrash".Translate(), "AnimalPodCrash".Translate(pawn.Named("PAWN")).AdjustedFor(pawn), LetterDefOf.NeutralEvent, new TargetInfo(intVec, map));
            var pod = new ActiveDropPodInfo()
            {
                openDelay = 180,
                leaveSlag = true
            };
            pod.innerContainer.TryAddOrTransfer(pawn, false);
            DropPodUtility.MakeDropPodAt(intVec, map, pod);

            return true;
        }

        private Pawn RandomAnimalByWeight()
        {
            var source = DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(t => t.RaceProps != null
                                                                            && t.RaceProps.Animal
                                                                            && t.RaceProps.baseBodySize > 0.45f
                                                                            && t.canArriveManhunter
                                                                            && t.RaceProps.IsFlesh
                                                                            && !t.RaceProps.Insect
                                                                            && (t.race.tradeTags == null || !t.race.tradeTags.Contains("VEE_Exclude")));
            float max = source.Max(k => k.race.BaseMarketValue) + 1f;

            var kind = source.RandomElementByWeight((k) => max - k.race.BaseMarketValue);
            return PawnGenerator.GeneratePawn(kind);
        }
    }
}