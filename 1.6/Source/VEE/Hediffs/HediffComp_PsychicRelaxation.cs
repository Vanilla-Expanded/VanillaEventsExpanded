using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VEE
{
    public class HediffComp_PsychicRelaxation : HediffComp
    {
        public HediffCompProperties_PsychicRelaxation Props => (HediffCompProperties_PsychicRelaxation)props;

        

        public override void CompPostTick(ref float severityAdjustment)
        {
            var pawn = Pawn;
            if (pawn.Spawned && pawn.IsHashIntervalTick(2500))
            {
                float num = 0.06f * pawn.GetStatValue(StatDefOf.JoyGainFactor) * pawn.GetStatValue(StatDefOf.PsychicSensitivity);
                pawn.needs?.joy?.GainJoy(num, VEE_DefOf.VEE_PsychicRelaxation);
            }
        }

       
    }
}