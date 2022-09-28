using RimWorld;
using Verse;

namespace VEE.RegularEvents
{
    public class WandererJoinTraitor : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            return TryFindEntryCell(map, out IntVec3 intVec);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!TryFindEntryCell(map, out IntVec3 loc))
            {
                return false;
            }
            Gender? gender = null;
            if (def.pawnFixedGender != Gender.None)
            {
                gender = new Gender?(def.pawnFixedGender);
            }
            PawnKindDef pawnKind = def.pawnKind;
            Faction ofPlayer = Faction.OfPlayer;
            Gender? fixedGender = gender;
            PawnGenerationRequest request = new PawnGenerationRequest(pawnKind, ofPlayer, mustBeCapableOfViolence: true);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            GenSpawn.Spawn(pawn, loc, map, WipeMode.Vanish);

            pawn.health.AddHediff(VEE_DefOf.Traitor);

            string text = def.letterText.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
            string label = def.letterLabel.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, pawn, null, null);
            return true;
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c) && !c.Fogged(map), map, CellFinder.EdgeRoadChance_Neutral, out cell);
        }
    }
}