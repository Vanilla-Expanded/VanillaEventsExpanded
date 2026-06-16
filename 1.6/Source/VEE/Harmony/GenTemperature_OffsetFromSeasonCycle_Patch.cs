using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VEE
{
    [HarmonyPatch(typeof(GenTemperature), "OffsetFromSeasonCycle")]
    public static class GenTemperature_OffsetFromSeasonCycle_Patch
    {
        public static void Postfix(ref float __result, PlanetTile tile)
        {
            if (tile.LayerDef == PlanetLayerDefOf.Surface)
            {
                foreach (var cond in Find.World.gameConditionManager.ActiveConditions.OfType<GameCondition_MultiStage>())
                {
                    __result += cond.TemperatureOffsetWorld();
                }
            }
        }
    }
}
