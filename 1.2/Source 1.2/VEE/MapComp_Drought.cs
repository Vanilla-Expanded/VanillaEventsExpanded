using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VEE
{
    class MapComp_Drought : MapComponent
    {
        public MapComp_Drought(Map map) : base(map) { }

        public bool droughtGoingOn = false;
        public Dictionary<Plant, bool> affectedPlants = new Dictionary<Plant, bool>();

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (Find.TickManager.TicksGame % 1024 == 0)
            {
                Log.Message($"Is drought going on: {droughtGoingOn}. Affected plant(s): {affectedPlants.Count}.");
                if (!map.gameConditionManager.ConditionIsActive(VEE_DefOf.Drought))
                {
                    this.droughtGoingOn = false;
                }
                if (!this.droughtGoingOn)
                {
                    affectedPlants.Clear();
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.droughtGoingOn, "droughtGoingOn");
        }
    }
}
