using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace VEE.RegularEvents
{
    class HuntingParty : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            IntVec3 intVec;
            return !map.GameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout) && map.mapTemperature.SeasonAcceptableFor(ThingDefOf.Human) && this.TryFindEntryCell(map, out intVec) && FindHuntPrey(map) != null;
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 loc;
            if (!this.TryFindEntryCell(map, out loc))
            {
                return false;
            }
            Faction faction = Find.FactionManager.RandomNonHostileFaction(false, false, false, TechLevel.Undefined);
            List<Pawn> pawnL = new List<Pawn>();
            System.Random r = new System.Random();
            int rand = r.Next(2, 6);
            for (int i = 0; i <= rand; i++)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(VEE_DefOf.Hunter, faction);
                pawnL.Add(pawn);
            }
            foreach (Pawn pawn in pawnL)
            {
                GenSpawn.Spawn(pawn, loc, map, WipeMode.Vanish);
                pawn.jobs.TryTakeOrderedJob(new Verse.AI.Job(VEE_DefOf.HuntAndLeave, new LocalTargetInfo(FindHuntPrey(map))));
            }
            Find.LetterStack.ReceiveLetter("HPLabel".Translate(), "HP".Translate(faction), LetterDefOf.NeutralEvent, new LookTargets(loc, map), null, null);
            return true;
        }
        private Thing FindHuntPrey(Map map)
        {
            List<Pawn> all = map.mapPawns.AllPawns.ToList();
            all.RemoveAll((Pawn t) => t.Faction != null || t.RaceProps.manhunterOnDamageChance > 0.5 || !t.IsWildMan());

            return all.RandomElement();
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c), map, CellFinder.EdgeRoadChance_Ignore, out cell);
        }
    }
}
