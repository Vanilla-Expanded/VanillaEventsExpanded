using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
namespace VEE
{
    public class ThingSetMaker_CrashlandedColonists : ThingSetMaker
    {
        private const float RelationWithColonistWeight = 20f;

        protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings)
        {
            PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, parms.makingFaction, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 20f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: true);
            int num = 0;
            Pawn pawn = null;
            while (num < 10 && (pawn == null || !pawn.Downed))
            {
                num++;
                if (pawn != null)
                {
                    Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
                }
                pawn = PawnGenerator.GeneratePawn(request);
               
            }
            outThings.Add(pawn);
        }

        protected override IEnumerable<ThingDef> AllGeneratableThingsDebugSub(ThingSetMakerParams parms)
        {
            yield return PawnKindDefOf.SpaceRefugee.race;
        }
    }
}