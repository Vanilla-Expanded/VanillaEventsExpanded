using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using RimWorld;

namespace VEE
{
    class HeddifComp_MightJoin : HediffComp
    {
        public HediffCompProperties Props
        {
            get
            {
                return (HeddifCompPropreties_MightJoin)this.props;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            System.Random r = new System.Random();
            if (!this.Pawn.health.HasHediffsNeedingTend() && !this.Pawn.health.Downed)
            {
                if(r.Next(0,101) < 60)
                {
                    this.Pawn.SetFaction(Faction.OfPlayer, null);
                    string label = "AJLabel".Translate();
                    string text = "AJLetter".Translate();
                    Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, null, null, null);
                }
                else
                {
                    string label = "ADJLabel".Translate();
                    string text = "ADJLetter".Translate();
                    Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, null, null, null);
                }
                this.Pawn.health.RemoveHediff(this.parent);
            }
        }
    }
}
