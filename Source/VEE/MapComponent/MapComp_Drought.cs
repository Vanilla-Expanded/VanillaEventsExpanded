using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VEE
{
    public class MapComp_Drought : MapComponent
    {
        public MapComp_Drought(Map map) : base(map)
        {
        }

        public bool droughtGoingOn = false;
        public Dictionary<Plant, bool> affectedPlants = new Dictionary<Plant, bool>();

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (HarmonyInit.mapCompDrought == null)
                HarmonyInit.mapCompDrought = new Dictionary<Map, MapComp_Drought>();

            if (Find.TickManager.TicksGame % 1024 == 0)
            {
                if (!map.gameConditionManager.ConditionIsActive(VEE_DefOf.Drought))
                {
                    droughtGoingOn = false;
                }
                if (!droughtGoingOn)
                {
                    affectedPlants.Clear();
                }

                if (droughtGoingOn && map.weatherDecider.ForcedWeather.rainRate > 0)
                {
                    map.gameConditionManager.GetActiveCondition(VEE_DefOf.Drought).End();
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref droughtGoingOn, "droughtGoingOn");
            HarmonyInit.mapCompDrought = null;
        }
    }
}