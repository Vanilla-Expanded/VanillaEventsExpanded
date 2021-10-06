using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace VEE.RegularEvents
{
    public class HuntingParty : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;

            return !map.GameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout) && map.mapTemperature.SeasonAcceptableFor(ThingDefOf.Human) && this.TryFindEntryCell(map, out IntVec3 intVec) && FindHuntPrey(map).Count > 0;
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            System.Random r = new System.Random();
            IntVec3 loc;
            if (!this.TryFindEntryCell(map, out loc))
            {
                return false;
            }
            Faction faction = Find.FactionManager.RandomNonHostileFaction(false, false, false, TechLevel.Undefined);
            List<Pawn> pawnL = new List<Pawn>();
            List<Pawn> huntable = FindHuntPrey(map);
            
            int rand = r.Next(3, 7);
            if(rand > huntable.Count)
            {
                rand = huntable.Count;
            }
            
            for (int i = 0; i < rand; i++)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(VEE_DefOf.Hunter, faction);
                pawn.skills.GetSkill(SkillDefOf.Shooting).levelInt = 15;
                pawnL.Add(pawn);
            }
            foreach (Pawn pawn in pawnL)
            {
                GenSpawn.Spawn(pawn, loc, map, WipeMode.Vanish);
                pawn.jobs.TryTakeOrderedJob(new Verse.AI.Job(VEE_DefOf.HuntAndLeave, new LocalTargetInfo(huntable[0])));
                huntable.RemoveAt(0);
            }
            Find.LetterStack.ReceiveLetter("HPLabel".Translate(), "HP".Translate(faction), LetterDefOf.NeutralEvent, new LookTargets(loc, map), null, null);
            return true;
        }
        private List<Pawn> FindHuntPrey(Map map)
        {
            List<Pawn> all = map.mapPawns.AllPawns.ToList();
            all.RemoveAll((Pawn t) => t.Faction != null || t.RaceProps.manhunterOnDamageChance > 0.05f || t.IsWildMan() || t.IsPrisoner);

            return all;
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c), map, CellFinder.EdgeRoadChance_Ignore, out cell);
        }
    }
}
