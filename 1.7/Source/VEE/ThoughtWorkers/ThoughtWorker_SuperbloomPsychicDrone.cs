using System;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VEE
{
    public class ThoughtWorker_SuperbloomPsychicDrone : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            var cond = Find.World.gameConditionManager.GetActiveCondition(VEE_DefOf.VEE_PsychicBloom) as GameCondition_MultiStage;
            if (cond == null || !cond.CurrentStage.triggerSuperbloomDrone) return false;
            return true;
        }

        public override float MoodMultiplier(Pawn p)
        {
            if (p.Map != null)
            {
                return base.MoodMultiplier(p) * p.Map.listerThings.ThingsOfDef(VEE_DefOf.VEE_Plant_PsychicLotus).Count;
            }
            return base.MoodMultiplier(p);
        }
    }
}