using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

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
            Faction faction;
            if (!this.TryFindFormerFaction(out faction))
            {
                return false;
            }
            Map map = (Map)parms.target;
            IntVec3 intVec;
            return !map.GameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout) && map.mapTemperature.SeasonAcceptableFor(ThingDefOf.Human) && this.TryFindEntryCell(map, out intVec);
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 loc;
            if (!this.TryFindEntryCell(map, out loc))
            {
                return false;
            }
            Faction faction;
            if (!this.TryFindFormerFaction(out faction))
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
            return Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction_NewTemp(out formerFaction, false, true, TechLevel.Undefined);
        }
    }
}
