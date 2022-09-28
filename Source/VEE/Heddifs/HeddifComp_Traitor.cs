using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VEE
{
    internal class HeddifComp_Traitor : HediffComp
    {
        public HeddifCompPropreties_Traitor Props
        {
            get
            {
                return (HeddifCompPropreties_Traitor)props;
            }
        }

        public override bool CompShouldRemove
        {
            get
            {
                return base.CompShouldRemove || ticksToDisappear <= 0;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            ticksToDisappear = Props.disappearsAfterTicks.RandomInRange;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (ticksToDisappear <= 20)
            {
                if (parent.pawn.Spawned)
                {
                    Pawn.SetFaction(Find.FactionManager.RandomEnemyFaction(false, false, false, TechLevel.Undefined));
                    Find.LetterStack.ReceiveLetter("TraitorLabel".Translate(), "Traitor".Translate(Pawn.Named("PAWN")).AdjustedFor(Pawn, "PAWN"), LetterDefOf.ThreatBig, new TargetInfo(Pawn.Position, Pawn.Map, false), null, null);

                    List<Pawn> pawnl = new List<Pawn>();
                    pawnl.Add(Pawn);
                    LordJob_AssaultColony lordJob = new LordJob_AssaultColony(Pawn.Faction, useAvoidGridSmart: true);
                    LordMaker.MakeNewLord(Pawn.Faction, lordJob, Pawn.Map, pawnl);

                    parent.pawn.health.hediffSet.hediffs.Remove(parent);
                }
                ticksToDisappear = 0;
            }
            ticksToDisappear--;
        }

        public override void CompPostMerged(Hediff other)
        {
            base.CompPostMerged(other);
            HeddifComp_Traitor HeddifComp_Traitor = other.TryGetComp<HeddifComp_Traitor>();
            if (HeddifComp_Traitor != null && HeddifComp_Traitor.ticksToDisappear > ticksToDisappear)
            {
                ticksToDisappear = HeddifComp_Traitor.ticksToDisappear;
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look<int>(ref ticksToDisappear, "ticksToDisappear", 0, false);
        }

        public override string CompDebugString()
        {
            return "ticksToDisappear: " + ticksToDisappear;
        }

        private int ticksToDisappear;
    }
}