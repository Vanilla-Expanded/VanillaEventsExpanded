using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using RimWorld.Planet;

namespace VEE
{
    public class PlantGrowth_MapComponent : MapComponent
    {

        //This is dirty. A quick hack to reset plant growth in case the player went from a map
        //with a drought game condition to one without it
        public PlantGrowth_MapComponent(Map map) : base(map)
        {

        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (Find.TickManager.TicksGame % 6000 == 0 && WorldComp_Purple.Instance.cachedPlantGrowthMultiplier != 1)
            {
                if (map?.gameConditionManager?.ConditionIsActive(VEE_DefOf.VEE_Drought) == true)
                {
                    return;
                }

                List<GameCondition> conditions = Find.World.gameConditionManager.ActiveConditions;
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (conditions[i] is GameCondition_MultiStage multiStage &&
                        multiStage.CurrentStage.plantGrowthMultiplier!=1)
                    {
                        return;
                    }
                }
                WorldComp_Purple.Instance.cachedPlantGrowthMultiplier = 1;
            }
        }


    }


}
