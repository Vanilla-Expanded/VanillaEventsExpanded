using HarmonyLib;
using Verse;
using RimWorld;

namespace VEE
{

    [HarmonyPatch(typeof(Fire))]
    [HarmonyPatch("SpreadInterval", MethodType.Getter)]
    public class VEE_Fire_SpreadInterval_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Fire __instance, ref float __result)
        {
            if (__instance.Map.gameConditionManager.ConditionIsActive(VEE_DefOf.VEE_Drought))
            {
                __result /= 2;
            }
        }
    }

}
