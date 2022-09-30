using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VEE
{
    internal class ThingSetMaker_AnimalPod : ThingSetMaker
    {
        protected override IEnumerable<ThingDef> AllGeneratableThingsDebugSub(ThingSetMakerParams parms)
        {
            throw new System.NotImplementedException();
        }

        protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings)
        {
            var pawn = PawnGenerator.GeneratePawn(AllGeneratable().RandomElement());
            HealthUtility.DamageUntilDowned(pawn, true);
            outThings.Add(pawn);
        }

        List<PawnKindDef> AllGeneratable()
        {
            return DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(t => t.RaceProps.Animal
                                                                            && t.RaceProps.baseBodySize > 0.45f
                                                                            && t.canArriveManhunter
                                                                            && t.RaceProps.IsFlesh
                                                                            && !t.RaceProps.Insect
                                                                            && !t.race.tradeTags.Contains("VEE_Exclude"));
        }
    }
}