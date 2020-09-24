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
            if(this.ticksToDisappear == 20)
            {
                this.Pawn.SetFaction(Find.FactionManager.RandomEnemyFaction(false, false, false, TechLevel.Undefined));

                string label = "TraitorLabel".Translate();
                string text = "Traitor".Translate(this.Pawn.Named("PAWN")).AdjustedFor(this.Pawn, "PAWN");

                Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.ThreatBig, new TargetInfo(this.Pawn.Position, this.Pawn.Map, false), null, null);
                this.ticksToDisappear = 0;
                List<Pawn> pawnl = new List<Pawn>();
                pawnl.Add(this.Pawn);
                LordJob_DefendPoint lordJob = new LordJob_DefendPoint(this.Pawn.Position);
                LordMaker.MakeNewLord(this.Pawn.Faction, lordJob, this.Pawn.Map, pawnl);
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
