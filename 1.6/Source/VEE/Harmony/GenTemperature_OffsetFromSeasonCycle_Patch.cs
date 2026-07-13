using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VEE
{
    [HarmonyPatch(typeof(GenTemperature), "OffsetFromSeasonCycle")]
    public static class VEE_GenTemperature_OffsetFromSeasonCycle_Patch
    {
        private static int lastOffsetCachedTick = -1;
        private static float cachedOffset;
        public static void Postfix(ref float __result, PlanetTile tile)
        {
            if (tile.LayerDef == PlanetLayerDefOf.Surface)
            {
                var ticksGame = Find.TickManager.TicksGame;
                if (lastOffsetCachedTick != ticksGame)
                {
                    lastOffsetCachedTick = ticksGame;
                    cachedOffset = 0f;
                    foreach (var activeCondition in Find.World.gameConditionManager.ActiveConditions)
                    {
                        if (activeCondition is GameCondition_MultiStage cond)
                        {
                            cachedOffset += cond.TemperatureOffsetWorld();
                        }
                    }
                }
                __result += cachedOffset;
            }
        }
    }
}
