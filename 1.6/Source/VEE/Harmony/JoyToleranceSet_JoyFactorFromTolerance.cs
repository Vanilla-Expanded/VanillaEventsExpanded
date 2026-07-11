using HarmonyLib;
using RimWorld;
using Verse;

namespace VEE
{
    [HarmonyPatch(typeof(JoyToleranceSet), "JoyFactorFromTolerance")]
    public static class VEE_JoyToleranceSet_JoyFactorFromTolerance_Patch
    {
        public static void Postfix(ref float __result, JoyKindDef joyKind)
        {
           
            if (joyKind== VEE_DefOf.VEE_PsychicRelaxation)
            {
                __result = 1;
            }
        }
    }
}
