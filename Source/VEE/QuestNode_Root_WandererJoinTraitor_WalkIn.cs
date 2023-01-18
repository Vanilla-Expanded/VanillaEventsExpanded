using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace VEE
{
    internal class QuestNode_Root_WandererJoinTraitor_WalkIn : QuestNode_Root_WandererJoin_WalkIn
    {
        public override Pawn GeneratePawn()
        {
            var request = new PawnGenerationRequest(PawnKindDefOf.Villager, Faction.OfPlayer, mustBeCapableOfViolence: true)
            {
                AllowedDevelopmentalStages = DevelopmentalStage.Adult
            };

            var pawn = PawnGenerator.GeneratePawn(request);
            pawn.health.AddHediff(VEE_DefOf.Traitor);

            if (!pawn.IsWorldPawn())
                Find.WorldPawns.PassToWorld(pawn);

            return pawn;
        }
    }
}