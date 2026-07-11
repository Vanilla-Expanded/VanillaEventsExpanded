
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace VEE
{
    public class HediffGiver_SunSickness : HediffGiver
    { 

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if(pawn.Map == null) return;
            if (!pawn.Map.gameConditionManager.ConditionIsActive(VEE_DefOf.VEE_Scorch))
            {           
                return;
            }

            if (pawn.Map.roofGrid.Roofed(pawn.Position))
            {
               
                return;
            }

            if(GenCelestial.CurCelestialSunGlow(pawn.Map) <= 0.4f)
            {
                return;
            }

            if (pawn.RaceProps.Humanlike && pawn.apparel!=null)
            {
                foreach (Apparel item in pawn.apparel.WornApparel)
                {
                    if (item.def.apparel.layers.Contains(ApparelLayerDefOf.Overhead) || item.def == VEE_DefOf.Apparel_Burka)
                    {                    
                        return;
                    }
                }
            }
            
            HediffDef hediffForSunSickness = VEE_DefOf.VEE_SunSickness;

            float a = 0.000375f * pawn.GetStatValue(VEE_DefOf.VEF_SunSicknessBuildupMultiplier); 

            HealthUtility.AdjustSeverity(pawn, hediffForSunSickness, a);
            
           

        }
    }
}