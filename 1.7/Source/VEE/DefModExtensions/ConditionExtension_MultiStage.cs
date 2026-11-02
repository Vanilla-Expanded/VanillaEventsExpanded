using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VEE
{
    public class ConditionExtension_MultiStage : DefModExtension
    {
        public List<MultiStage> stages;
    }

    public class MultiStage
    {
        public FloatRange durationDays;
        public float targetTempOffset;
        public float tempLerpDays = -1f;
        public string letterLabel;
        public string letterText;
        public LetterDef letterDef;
        public List<WeatherCommonalityRecord> weatherWeights;
        public WeatherDef forcedWeather;
        public bool mechanoidsOnly;
        public bool disableWildAnimalSpawns;
        public List<IncidentChanceRecord> incidentChances;
        public float humanIncidentsFactor = 1f;
        public bool spawnDeepSnow;
        public bool preventRain = false;
        public float plantGrowthMultiplier = 1;
        public bool changeSkyColours = false;
        public int changeSkyColoursTicks;
        public SkyColorSet skyColors;
        public float globalLightLevelsMultiplier = 1;
        public HediffDef hediffForOrganicPawns = null;
        public bool triggerPurpleAurora = false;
        public List<ThingDef> scatterPlants = new List<ThingDef>();
        public int scatterPlantsInterval = 0;
        public List<PlantSwap> swapPlants = new List<PlantSwap>();
        public float psychicLotusRadius = 0f;
        public float psychicLotusSpawnAmount = 0f;
        public bool triggerSuperbloomDrone = false;
        public bool displayHazeEffect = false;
    }

    public class IncidentChanceRecord
    {
        public IncidentDef def;
        public float chance;
    }

    public class WeatherCommonalityRecord
    {
        public WeatherDef weather;
        public float weight;
    }

    public class PlantSwap
    {
        public ThingDef plantToSwap;
        public ThingDef swappedPlant;
        public bool justDestroy = false;
        public EffecterDef effecter;
        public float effecterChance;
    }
}
