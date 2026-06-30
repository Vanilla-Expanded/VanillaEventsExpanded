using UnityEngine;
using VEE.World_and_Map_Components;
using Verse;

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
            if (map.gameConditionManager.ConditionIsActive(VEE_DefOf.VEE_Scorch))
            {
                float targetIntensity = map.mapTemperature.OutdoorTemp > TemperatureThreshold ? 1f : 0f;
                hazeIntensity = Mathf.MoveTowards(hazeIntensity, targetIntensity, Time.deltaTime / FadeSeconds);
                HazeFullscreenPass.Draw(map, hazeIntensity);
            }
            
        }
    }

}
