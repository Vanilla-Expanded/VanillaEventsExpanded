using RimWorld;
using Verse;

namespace VEE.RegularEvents
{
    public class WildMenWanderIn : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            if (!TryFindFormerFaction(out Faction faction))
            {
                return false;
            }
            Map map = (Map)parms.target;
            return !map.GameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout) && map.mapTemperature.SeasonAcceptableFor(ThingDefOf.Human) && TryFindEntryCell(map, out IntVec3 intVec);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!TryFindEntryCell(map, out IntVec3 loc))
            {
                return false;
            }
            if (!TryFindFormerFaction(out Faction faction))
            {
                return false;
            }
            for (int i = 0; i < 3; i++)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.WildMan, faction);
                pawn.SetFaction(null, null);
                GenSpawn.Spawn(pawn, loc, map, WipeMode.Vanish);
            }
            Find.LetterStack.ReceiveLetter("WMWILabel".Translate(), "WMWI".Translate(), LetterDefOf.NeutralEvent, new LookTargets(loc, map), null, null);
            return true;
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c), map, CellFinder.EdgeRoadChance_Ignore, out cell);
        }

        private bool TryFindFormerFaction(out Faction formerFaction)
        {
            return Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out formerFaction, false, true, TechLevel.Undefined);
        }
    }
}