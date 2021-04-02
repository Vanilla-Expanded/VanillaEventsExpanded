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
        public override int TransitionTicks => 1000;

        public override void Init()
        {
            HarmonyInit.droughtGoingOn = true;
            HarmonyInit.maps.Add(this.SingleMap);
            this.SingleMap.fertilityGrid.FertilityGridUpdate();
        }

        public override void End()
        {
            HarmonyInit.droughtGoingOn = false;
            if (HarmonyInit.maps.Contains(this.SingleMap)) HarmonyInit.maps.Remove(this.SingleMap);
            this.SingleMap.fertilityGrid.FertilityGridUpdate();
        }

        public override WeatherDef ForcedWeather()
        {
            return WeatherDefOf.Clear;
        }

        public override void GameConditionTick()
        {
            if (!HarmonyInit.droughtGoingOn) HarmonyInit.droughtGoingOn = true;
            if (!HarmonyInit.maps.Contains(this.SingleMap)) HarmonyInit.maps.Add(this.SingleMap);
        }
    }
}
