using System.Collections.Generic;
using RimWorld;
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

        public LordJob_HuntingParty()
        { }

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
}