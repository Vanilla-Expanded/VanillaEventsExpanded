using HarmonyLib;
using RimWorld;
using Verse;

namespace VEE
{
    [HarmonyPatch(typeof(WeatherDecider), "CurrentWeatherCommonality")]
    public static class WeatherDecider_CurrentWeatherCommonality_Patch
    {
        public static void Postfix(ref float __result, WeatherDef weather)
        {
            var cond = Find.World.gameConditionManager.GetActiveCondition(VEE_DefOf.VEE_Whiteout) as GameCondition_MultiStage;
            if (cond is null) return;
            if (cond.CurrentStage.weatherWeights != null)
            {
                var w = cond.CurrentStage.weatherWeights.FirstOrDefault(x => x.weather == weather);
                if (w != null)
                {
                    __result = w.weight;
                }
                else
                {
                    __result = 0;
                }
            }
        }
    }
}
