using RimWorld;
using Verse;

namespace VEE.RegularEvents
{
    public class Drought : GameCondition
    {
        public override void Init()
        {
            SingleMap.GetComponent<MapComp_Drought>().droughtGoingOn = true;
        }

        public override float PlantDensityFactor(Map map)
        {
            return 0f;
        }

        public override WeatherDef ForcedWeather()
        {
            return WeatherDefOf.Clear;
        }
    }
}