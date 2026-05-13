using RimWorld;
using Verse;

namespace VEE
{
    internal class HediffComp_MightJoin : HediffComp
    {
        public HediffCompProperties Props => (HediffCompPropreties_MightJoin)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            var pawn = Pawn;
            if (!pawn.health.HasHediffsNeedingTend() && !pawn.health.Downed)
            {
                if (Rand.Chance(0.6f))
                {
                    pawn.SetFaction(Faction.OfPlayer, null);
                    Find.LetterStack.ReceiveLetter("VEE_AnimalPodCrashLabel_AceptedJoining".Translate(), "VEE_AnimalPodCrashDesc_AceptedJoining".Translate(), LetterDefOf.NeutralEvent, null, null, null);
                }
                else
                {
                    Find.LetterStack.ReceiveLetter("VEE_AnimalPodCrashLabel_RefusedJoining".Translate(), "VEE_AnimalPodCrashDesc_RefusedJoining".Translate(), LetterDefOf.NeutralEvent, null, null, null);
                }
                pawn.health.RemoveHediff(parent);
            }
        }
    }
}