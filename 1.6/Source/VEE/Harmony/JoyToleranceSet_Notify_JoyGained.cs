using HarmonyLib;
using RimWorld;
using Verse;

namespace VEE
{
    [HarmonyPatch(typeof(JoyToleranceSet), "Notify_JoyGained")]
    public static class VEE_JoyToleranceSet_Notify_JoyGained_Patch
    {
        public static bool Prefix(JoyKindDef joyKind)
        {
            if (joyKind == VEE_DefOf.VEE_PsychicRelaxation)
            {
                return false;
            }
            return true;
        }
    }
}
