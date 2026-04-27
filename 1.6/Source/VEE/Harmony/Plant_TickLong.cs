using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VEE
{

    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("TickLong", MethodType.Normal)]
    public class Plant_TickLong_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Plant __instance, ref int ___madeLeaflessTick)
        {
            if (__instance.Map != null && __instance.Map.GameConditionManager.ConditionIsActive(VEE_DefOf.VEE_Drought))
            {


                if (!__instance.def.plant.dieIfLeafless || __instance.def.label.Contains("grass"))
                {
                    ___madeLeaflessTick = Find.TickManager.TicksGame;
                    if (!__instance.LeaflessNow)
                    {
                        __instance.Map.mapDrawer.MapMeshDirty(__instance.Position, MapMeshFlagDefOf.Things);
                    }
                }
                else
                {
                    ___madeLeaflessTick = -99999;
                }

            }
        }
    }



}
