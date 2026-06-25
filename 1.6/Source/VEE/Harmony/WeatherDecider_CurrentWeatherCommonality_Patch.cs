using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using Verse.Noise;

namespace VEE
{
    [HarmonyPatch(typeof(WeatherDecider), "CurrentWeatherCommonality")]
    public static class VEE_WeatherDecider_CurrentWeatherCommonality_Patch
    {
        public static void Postfix(ref float __result, WeatherDef weather)
        {
            var multistageCondition = Find.World.gameConditionManager.ActiveConditions.Where(x => x is GameCondition_MultiStage);
            if (multistageCondition.Any()) {
                foreach (GameCondition_MultiStage condition in multistageCondition)
                {
                    if (weather.rainRate > 0.1f && condition.CurrentStage.preventRain)
                    {
                        __result= 0f;
                    }
                }
            }

            var cond = Find.World.gameConditionManager.GetActiveCondition(VEE_DefOf.VEE_Whiteout) as GameCondition_MultiStage;
            
            if (cond?.CurrentStage.weatherWeights != null)
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
