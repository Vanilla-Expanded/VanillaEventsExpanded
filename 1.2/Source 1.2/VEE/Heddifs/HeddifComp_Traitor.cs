using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;
using RimWorld;

namespace VEE
{
    class HeddifComp_Traitor : HediffComp
    {
        public HeddifCompPropreties_Traitor Props
        {
            get
            {
                return (HeddifCompPropreties_Traitor)this.props;
            }
        }

        public override bool CompShouldRemove
        {
            get
            {
                return base.CompShouldRemove || this.ticksToDisappear <= 0;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.ticksToDisappear = this.Props.disappearsAfterTicks.RandomInRange;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (this.ticksToDisappear <= 20)
            {
                if (this.parent.pawn.Spawned)
                {
                    this.Pawn.SetFaction(Find.FactionManager.RandomEnemyFaction(false, false, false, TechLevel.Undefined));
                    Find.LetterStack.ReceiveLetter("TraitorLabel".Translate(), "Traitor".Translate(this.Pawn.Named("PAWN")).AdjustedFor(this.Pawn, "PAWN"), LetterDefOf.ThreatBig, new TargetInfo(this.Pawn.Position, this.Pawn.Map, false), null, null);

                    List<Pawn> pawnl = new List<Pawn>();
                    pawnl.Add(this.Pawn);
                    LordJob_AssaultColony lordJob = new LordJob_AssaultColony(this.Pawn.Faction, useAvoidGridSmart: true);
                    LordMaker.MakeNewLord(this.Pawn.Faction, lordJob, this.Pawn.Map, pawnl);

                    this.parent.pawn.health.hediffSet.hediffs.Remove(this.parent);
                }
                this.ticksToDisappear = 0;
            }
            this.ticksToDisappear--;
        }

        public override void CompPostMerged(Hediff other)
        {
            base.CompPostMerged(other);
            HeddifComp_Traitor HeddifComp_Traitor = other.TryGetComp<HeddifComp_Traitor>();
            if (HeddifComp_Traitor != null && HeddifComp_Traitor.ticksToDisappear > this.ticksToDisappear)
            {
                this.ticksToDisappear = HeddifComp_Traitor.ticksToDisappear;
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look<int>(ref this.ticksToDisappear, "ticksToDisappear", 0, false);
        }

        public override string CompDebugString()
        {
            return "ticksToDisappear: " + this.ticksToDisappear;
        }

        private int ticksToDisappear;
    }
}
