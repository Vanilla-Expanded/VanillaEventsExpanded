using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VEE
{
    internal class ThingSetMaker_AnimalPod : ThingSetMaker
    {
        protected override IEnumerable<ThingDef> AllGeneratableThingsDebugSub(ThingSetMakerParams parms)
        {
            return DefDatabase<ThingDef>.AllDefsListForReading.FindAll(p => p.race.Animal && p.race.IsFlesh && !p.race.Insect);
        }

        protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings)
        {
            List<PawnKindDef> allAnimals = new List<PawnKindDef>();
            List<PawnKindDef> allPawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading;

            allAnimals.AddRange(DefDatabase<PawnKindDef>.AllDefsListForReading.FindAll(p => p.race.race.Animal && p.RaceProps.IsFlesh && !p.RaceProps.Insect && p.canArriveManhunter == true));

            Pawn pawn = PawnGenerator.GeneratePawn(allAnimals.RandomElement());
            outThings.Add(pawn);
            HealthUtility.DamageUntilDowned(pawn, true);
        }
    }
}