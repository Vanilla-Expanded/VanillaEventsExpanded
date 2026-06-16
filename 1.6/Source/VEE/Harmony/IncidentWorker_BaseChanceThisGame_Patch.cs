using HarmonyLib;
using RimWorld;
using Verse;

namespace VEE
{
    [HarmonyPatch(typeof(IncidentWorker), "BaseChanceThisGame", MethodType.Getter)]
    public static class IncidentWorker_BaseChanceThisGame_Patch
    {
        public static void Postfix(IncidentWorker __instance, ref float __result)
        {
            var cond = Find.World.gameConditionManager.GetActiveCondition(VEE_DefOf.VEE_Whiteout) as GameCondition_MultiStage;
            if (cond == null) return;
            var stage = cond.CurrentStage;
            if (stage.incidentChances != null)
            {
                var customChance = stage.incidentChances.FirstOrDefault(x => x.def == __instance.def);
                if (customChance != null)
                {
                    __result = customChance.chance;
                }
            }
        }
    }
}
