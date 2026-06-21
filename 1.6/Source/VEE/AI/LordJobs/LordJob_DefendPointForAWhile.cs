
using RimWorld;
using System.Drawing;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using static Mono.Security.X509.X520;
using Verse.Noise;
using System.Collections.Generic;

namespace VEE
{
    public class LordJob_DefendPointForAWhile:LordJob
    {
        private IntVec3 point;

        private float? wanderRadius;

        private float? defendRadius;

        private bool isCaravanSendable;

        private bool addFleeToil;

        private bool onlyDoOnce = false;

        private List<Pawn> pawns = new List<Pawn>();

        int tickCounter = 0;
        int tickCounterMax = 30000;

        public override void LordJobTick()
        {
            base.LordJobTick();
            tickCounter++;
            if (tickCounter > tickCounterMax && !onlyDoOnce)
            {
                onlyDoOnce = true;
                if (PawnsFinder.AllMapsCaravansAndTravellingTransporters_AliveSpawned_FreeColonists.Count < 3)
                {
                    var letter = (ChoiceLetter_AcceptCrashlanders)LetterMaker.MakeLetter("VEE_CrashlandedChoiceLabel".Translate(), "VEE_CrashlandedChoiceText".Translate(), VEE_DefOf.VEE_SurvivorsJoin, new LookTargets(point, Map));
                    letter.pawns = pawns;
                    letter.map = Map;
                    letter.spawnSpot = point;

                    Find.LetterStack.ReceiveLetter(letter);
                    letter.OpenLetter();
                    Find.TickManager.Pause();
                }
                else
                {
                    Messages.Message("VEE_CrashlandedLeaving".Translate(), pawns, MessageTypeDefOf.NeutralEvent);
                }             
            }
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            LordToil_DefendPoint lordToil_Defend = new LordToil_DefendPoint(point, wanderRadius: wanderRadius, defendRadius: defendRadius);
            stateGraph.AddToil(lordToil_Defend);
            stateGraph.StartingToil = lordToil_Defend;
            LordToil_ExitMapRandom lordToil_ExitMapRandom = new LordToil_ExitMapRandom();
            stateGraph.AddToil(lordToil_ExitMapRandom);
            Transition transition = new Transition(lordToil_Defend, lordToil_ExitMapRandom);
            transition.AddTrigger(new Trigger_TicksPassed(tickCounterMax));
            transition.AddPostAction(new TransitionAction_EndAllJobs());
            stateGraph.AddTransition(transition);

            return stateGraph;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref point, "point");
            Scribe_Values.Look(ref wanderRadius, "wanderRadius");
            Scribe_Values.Look(ref defendRadius, "defendRadius");
            Scribe_Values.Look(ref isCaravanSendable, "isCaravanSendable", defaultValue: false);
            Scribe_Values.Look(ref addFleeToil, "addFleeToil", defaultValue: false);
            Scribe_Values.Look(ref tickCounter, "tickCounter");
            Scribe_Values.Look(ref tickCounterMax, "tickCounterMax");
            Scribe_Values.Look(ref onlyDoOnce, "onlyDoOnce");

            Scribe_Collections.Look(ref pawns, "pawns", LookMode.Reference);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                pawns ??= new List<Pawn>();
                pawns.RemoveAll(x => x == null);
            }
        }

        public LordJob_DefendPointForAWhile(IntVec3 point, List<Pawn> pawns, float? wanderRadius = null, float? defendRadius = null, bool isCaravanSendable = false, bool addFleeToil = true)
        {
            this.point = point;
            this.pawns = pawns;
            this.wanderRadius = wanderRadius;
            this.defendRadius = defendRadius;
            this.isCaravanSendable = isCaravanSendable;
            this.addFleeToil = addFleeToil;
           
            tickCounterMax = new IntRange(3000, 6000).RandomInRange;
        }

       
    }
}