using HarmonyLib;
using RimWorld;
using Verse;

namespace VEE
{
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "FactionCanBeGroupSource")]
    public static class VEE_IncidentWorker_RaidEnemy_FactionCanBeGroupSource_Patch
    {
        public static void Postfix(Faction f, ref bool __result)
        {
            if (!__result) return;
            var cond = Find.World.gameConditionManager.GetActiveCondition(VEE_DefOf.VEE_Whiteout) as GameCondition_MultiStage;
            if (cond is null) return;
            if (cond.CurrentStage.mechanoidsOnly)
            {
                if (f.def != FactionDefOf.Mechanoid)
                {
                    __result = false;
                }
            }
            if (f.def.humanlikeFaction)
            {
                if (cond.CurrentStage.humanIncidentsFactor != 1f)
                {
                    if (Rand.Chance(cond.CurrentStage.humanIncidentsFactor) is false)
                    {
                        __result = false;
                    }
                }
            }

        }
    }
}
