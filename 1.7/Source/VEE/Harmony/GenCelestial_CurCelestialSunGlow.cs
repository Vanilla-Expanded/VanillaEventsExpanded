using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using System.Collections.Generic;
using RimWorld.Planet;
using System.Linq;
using System;
using RimWorld.BaseGen;

namespace VEE
{
    [HarmonyPatch(typeof(GenCelestial))]
    [HarmonyPatch("CurCelestialSunGlow")]
    [HarmonyPatch(new Type[] { typeof(Map) })]
    public static class VEE_GenCelestial_CurCelestialSunGlow_Patch
    {
       
        [HarmonyPostfix]
        public static void DimLights(ref Map map, ref float __result)
        {
           
                __result = __result * WorldComp_Purple.Instance.cachedGlobalLightLevelsMultiplier;

        }
    }

}
