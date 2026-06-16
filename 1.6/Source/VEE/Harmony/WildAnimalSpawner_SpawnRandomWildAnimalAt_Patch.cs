using HarmonyLib;
using RimWorld;
using Verse;

namespace VEE
{
    [HarmonyPatch(typeof(WildAnimalSpawner), "SpawnRandomWildAnimalAt")]
    public static class WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch
    {
        public static bool Prefix()
        {
            var cond = Find.World.gameConditionManager.GetActiveCondition(VEE_DefOf.VEE_Whiteout) as GameCondition_MultiStage;
            if (cond is null) return true;
            if (cond.CurrentStage.disableWildAnimalSpawns)
            {
                return false;
            }
            return true;
        }
    }
}
