using HarmonyLib;
using RimWorld;
using Verse;

namespace VEE
{
    [HarmonyPatch(typeof(IncidentWorker_PsychicEmanation), "CanFireNowSub")]
    public static class VEE_IncidentWorker_PsychicEmanation_CanFireNowSub_Patch
    {
        public static void Postfix(ref bool __result, IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (map.gameConditionManager.ConditionIsActive(VEE_DefOf.VEE_PsychicStimulation)||
                map.gameConditionManager.ConditionIsActive(VEE_DefOf.VEE_PsychicOverdrive))
            {
                __result= false;
            }
        }
    }
}
