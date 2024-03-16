using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace VEE
{
    public class HuntingParty : IncidentWorker
    {
        private IntVec3 entryCell;
        private Faction faction;
        private List<Pawn> huntTargets;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;

            return base.CanFireNowSub(parms)
                   && !map.GameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout)
                   && TryFindEntryCell(map, out entryCell)
                   && TryFindFaction(out faction)
                   && FindHuntPrey(map, faction, entryCell, out huntTargets);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = (Map)parms.target;
            // Spawn hunters
            var lordPawns = new List<Pawn>();
            var pawnNumber = Math.Min(Rand.RangeInclusive(4, 7), huntTargets.Count);
            if (pawnNumber == 0)
            {
                // Prevent an NRE if the event is triggered by force when CanFireNowSub returns false.
                return false;
            }

            var pawnKind = faction.def.techLevel >= TechLevel.Industrial ? VEE_DefOf.VEE_Hunter : VEE_DefOf.VEE_TribalHunter;

            for (int i = 0; i < pawnNumber; i++)
            {
                var pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKind, faction));
                GenSpawn.Spawn(pawn, entryCell, map, WipeMode.VanishOrMoveAside);

                lordPawns.Add(pawn);
            }

            LordMaker.MakeNewLord(faction, new LordJob_HuntingParty(faction, huntTargets.Cast<Thing>().ToList()), map, lordPawns);

            // Send letter
            Find.LetterStack.ReceiveLetter("HPLabel".Translate(), "HP".Translate(faction), LetterDefOf.NeutralEvent, new LookTargets(entryCell, map), faction);

            return true;
        }

        private bool FindHuntPrey(Map map, Faction faction, IntVec3 entryCell, out List<Pawn> huntTargets)
        {
            huntTargets = new List<Pawn>();
            var allPawns = map.mapPawns.AllPawns.ToList();

            for (int i = 0; i < allPawns.Count; i++)
            {
                var p = allPawns[i];
                if (p.Faction == null && !p.RaceProps.DeathActionWorker.DangerousInMelee && !p.IsWildMan() && !p.IsPrisoner
                    && map.reachability.CanReach(entryCell, (LocalTargetInfo) p, PathEndMode.OnCell, TraverseMode.NoPassClosedDoors))
                {
                    if (faction.def.techLevel >= TechLevel.Industrial && p.RaceProps.manhunterOnDamageChance <= 0.5f)
                        huntTargets.Add(p);
                    else if (faction.def.techLevel < TechLevel.Industrial && p.RaceProps.manhunterOnDamageChance <= 0.1f)
                        huntTargets.Add(p);
                }
            }
            huntTargets = huntTargets.OrderByDescending(p => p.RaceProps.baseBodySize).ToList();

            return huntTargets.Count > 0;
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