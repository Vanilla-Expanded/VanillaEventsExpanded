using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.AI;

namespace VEE
{
    public class HuntingParty : IncidentWorker
    {
        private IntVec3 entryCell;
        private Faction faction;
        private List<Pawn> huntTargets = new List<Pawn>();

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;

            return base.CanFireNowSub(parms) && 
                    !map.GameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout) && 
                    map.mapTemperature.SeasonAcceptableFor(ThingDefOf.Human) && 
                    this.TryFindEntryCell(map, out this.entryCell) &&
                    this.TryFindFaction(out this.faction) &&
                    this.FindHuntPrey(map, out this.huntTargets);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            this.TryFindEntryCell(map, out this.entryCell);
            this.TryFindFaction(out this.faction);
            this.FindHuntPrey(map, out this.huntTargets);
            // Spawn hunters
            int pawnNumber = Math.Min(Rand.RangeInclusive(3, 5), this.huntTargets.Count);
            PawnKindDef kind = this.faction.def.techLevel >= TechLevel.Industrial ? VEE_DefOf.Hunter : VEE_DefOf.VEE_TribalHunter;
            for (int i = 0; i < pawnNumber; i++)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(kind, this.faction);
                GenSpawn.Spawn(pawn, this.entryCell, map, WipeMode.VanishOrMoveAside);
                pawn.jobs.TryTakeOrderedJob(new Job(VEE_DefOf.HuntAndLeave, new LocalTargetInfo(this.huntTargets[0])));
                this.huntTargets.RemoveAt(0);
            }
            // Send letter
            Find.LetterStack.ReceiveLetter("HPLabel".Translate(), "HP".Translate(this.faction), LetterDefOf.NeutralEvent, new LookTargets(this.entryCell, map), this.faction);
            return true;
        }

        private bool FindHuntPrey(Map map, out List<Pawn> huntTargets)
        {
            huntTargets = map.mapPawns.AllPawns.ToList();
            huntTargets.RemoveAll(p => p.Faction != null || p.RaceProps.manhunterOnDamageChance > 0.1f || p.IsWildMan() || p.IsPrisoner);
            huntTargets = huntTargets.OrderByDescending(p => p.RaceProps.baseBodySize).ToList();

            if (huntTargets.Count > 0) return true;
            return false;
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c), map, CellFinder.EdgeRoadChance_Ignore, out cell);
        }

        private bool TryFindFaction(out Faction faction)
        {
            List<Faction> factions = Find.FactionManager.AllFactions.ToList().FindAll(f => f != Faction.OfPlayer && !f.HostileTo(Faction.OfPlayer) && !f.Hidden && !f.defeated && f.def.techLevel >= TechLevel.Neolithic);
            if (ModLister.IdeologyInstalled && ModsConfig.IdeologyActive) factions.RemoveAll(f => f.ideos.HasAnyIdeoWithMeme(VEE_DefOf.AnimalPersonhood));
            faction = factions.RandomElement();

            if (faction != null) return true;
            return false;
        }
    }
}
