using RimWorld;
using Verse;

namespace VEE.RegularEvents
{
    public class Drought : GameCondition
    {
       

        public override float PlantDensityFactor(Map map)
        {
            return 0f;
        }

        public override float AnimalDensityFactor(Map map)
        {
            return 0f;
        }

        public override float WeatherCommonalityFactor(WeatherDef weather, Map map)
        {
            if (weather == WeatherDefOf.Clear || weather == VEE_DefOf.DryThunderstorm)
            {
                return 1f;
            }
            return 0f;
        }
    }
}