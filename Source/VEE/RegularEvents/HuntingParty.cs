﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace VEE
{
    public class LordJob_HuntingParty : LordJob
    {
        private static readonly int MaxHuntTicks = 60000;
        private Faction faction;
        public List<Thing> targets;

        public LordJob_HuntingParty() { }

        public LordJob_HuntingParty(Faction faction, List<Thing> targets)
        {
            this.faction = faction;
            this.targets = targets;
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref faction, "faction");
            Scribe_Collections.Look(ref targets, "targets", LookMode.Reference);
        }

        public override StateGraph CreateGraph()
        {
            var stateGraph = new StateGraph();

            var toilHunt = new LordToil_Hunt();
            stateGraph.AddToil(toilHunt);

            var toilHuntEnemies = new LordToil_HuntEnemies(targets[0].Position)
            {
                useAvoidGrid = true
            };
            stateGraph.AddToil(toilHuntEnemies);

            var transToHuntEnemies = new Transition(toilHunt, toilHuntEnemies);
            transToHuntEnemies.AddTrigger(new Trigger_Custom((signal) =>
            {
                // Is there any manhunter?
                if (signal.type == TriggerSignalType.Tick)
                {
                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (targets[i] is Pawn pawn && pawn.MentalStateDef == MentalStateDefOf.Manhunter)
                            return true;
                    }
                    return false;
                }
                return false;
            }));
            transToHuntEnemies.AddPostAction(new TransitionAction_EndAllJobs());
            stateGraph.AddTransition(transToHuntEnemies);

            var transToHuntEnemies2 = new Transition(toilHunt, toilHuntEnemies);
            transToHuntEnemies2.AddTrigger(new Trigger_PawnHarmed());
            transToHuntEnemies2.AddPostAction(new TransitionAction_EndAllJobs());
            stateGraph.AddTransition(transToHuntEnemies2);

            var transToHunt = new Transition(toilHuntEnemies, toilHunt);
            transToHunt.AddTrigger(new Trigger_Custom((signal) =>
            {
                // Is there no manhunter?
                if (signal.type == TriggerSignalType.Tick)
                {
                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (targets[i] is Pawn pawn && pawn.MentalStateDef == MentalStateDefOf.Manhunter)
                            return false;
                    }
                    return true;
                }
                return false;
            }));
            transToHunt.AddPostAction(new TransitionAction_EndAllJobs());
            stateGraph.AddTransition(transToHunt);

            var toilExit = new LordToil_ExitMap(LocomotionUrgency.Jog, interruptCurrentJob: true)
            {
                useAvoidGrid = true
            };
            stateGraph.AddToil(toilExit);

            var transToExit = new Transition(toilHunt, toilExit);
            transToExit.AddTrigger(new Trigger_TicksPassed(MaxHuntTicks));
            transToExit.AddPostAction(new TransitionAction_EndAllJobs());
            transToExit.AddSource(toilHuntEnemies);
            stateGraph.AddTransition(transToExit);

            var transToExit2 = new Transition(toilHunt, toilExit);
            transToExit2.AddTrigger(new Trigger_Custom((signal) =>
            {
                // Is there no target left?
                if (signal.type == TriggerSignalType.Tick)
                {
                    return targets.Count == 0;
                }
                return false;
            }));
            transToExit2.AddPostAction(new TransitionAction_EndAllJobs());
            transToExit2.AddSource(toilHuntEnemies);
            stateGraph.AddTransition(transToExit2);

            return stateGraph;
        }
    }

    public class LordToil_Hunt : LordToil
    {
        public LordToil_Hunt() { }

        public override void UpdateAllDuties()
        {
            var lordJob = (LordJob_HuntingParty)lord.LordJob;
            var targets = lordJob.targets;
            var usable = new List<Pawn>();

            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];
                if (target is Pawn pawn && (pawn.Spawned || pawn.Dead))
                {
                    usable.Add(pawn);
                }
            }
            usable.OrderByDescending(p => p.Dead || p.Downed);


            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                var target = targets[i];
                var pawn = lord.ownedPawns[i];
                var duty = pawn.mindState.duty;
                var focus = (Pawn)duty?.focus;

                if (duty == null || focus == null || focus.IsWorldPawn() || duty.def != VEE_DefOf.VEE_CarryAndLeave)
                {
                    pawn.mindState.duty = new PawnDuty(VEE_DefOf.VEE_CarryAndLeave, target);
                }
            }
        }
    }

    public class JobGiver_AICarryDutyFocusAndExit : ThinkNode_JobGiver
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            return (JobGiver_AICarryDutyFocusAndExit)base.DeepCopy(resolve);
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.mindState.duty == null || pawn.mindState.duty.focus == null || !pawn.mindState.duty.focus.Pawn.Spawned)
                return null;

            var job = new Job(VEE_DefOf.HuntAndLeave, pawn.mindState.duty.focus);
            return job;
        }
    }

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
                   && FindHuntPrey(map, faction, out huntTargets);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = (Map)parms.target;
            // Spawn hunters
            var lordPawns = new List<Pawn>();
            var pawnNumber = Math.Min(Rand.RangeInclusive(4, 7), huntTargets.Count);
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

        private bool FindHuntPrey(Map map, Faction faction, out List<Pawn> huntTargets)
        {
            huntTargets = new List<Pawn>();
            var allPawns = map.mapPawns.AllPawns.ToList();

            for (int i = 0; i < allPawns.Count; i++)
            {
                var p = allPawns[i];
                if (p.Faction == null && !p.RaceProps.DeathActionWorker.DangerousInMelee && !p.IsWildMan() && !p.IsPrisoner)
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