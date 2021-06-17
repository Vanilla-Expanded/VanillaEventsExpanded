using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VEE.RegularEvents
{
    public class Drought : GameCondition
    {
        public override void Init()
        {
            this.SingleMap.GetComponent<MapComp_Drought>().droughtGoingOn = true;
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
