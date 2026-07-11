using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VEE.World_and_Map_Components;
using Verse;
using RimWorld;

namespace VEE
{

    public class HazeMapComponent : MapComponent
    {
        private const float TemperatureThreshold = 10f;
        private const float FadeSeconds = 3f;

        private float hazeIntensity;

        public HazeMapComponent(Map map) : base(map)
        {
        }

        public override void MapComponentUpdate()
        {
            bool doDisplay = false;

            List<GameCondition> conditions = Find.World.gameConditionManager.ActiveConditions;
            for (int i = 0; i < conditions.Count; i++)
            {
                if (conditions[i] is GameCondition_MultiStage multiStage &&
                    multiStage.CurrentStage.displayHazeEffect)
                {
                    doDisplay = true;
                    break;
                }
            }

            if (!doDisplay)
                return;

            float targetIntensity = map.mapTemperature.OutdoorTemp > TemperatureThreshold ? 1f : 0f;
            hazeIntensity = Mathf.MoveTowards(hazeIntensity, targetIntensity, Time.deltaTime / FadeSeconds);
            HazeFullscreenPass.Draw(map, hazeIntensity);
        }
    }

}
