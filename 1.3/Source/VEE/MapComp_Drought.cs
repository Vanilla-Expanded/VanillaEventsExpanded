using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VEE
{
    public class MapComp_Drought : MapComponent
    {
        public MapComp_Drought(Map map) : base(map) { }

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
                    this.droughtGoingOn = false;
                }
                if (!this.droughtGoingOn)
                {
                    affectedPlants.Clear();
                }

                if (this.droughtGoingOn && map.weatherDecider.ForcedWeather.rainRate > 0)
                {
                    map.gameConditionManager.GetActiveCondition(VEE_DefOf.Drought).End();
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.droughtGoingOn, "droughtGoingOn");
            HarmonyInit.mapCompDrought = null;
        }
    }
}
