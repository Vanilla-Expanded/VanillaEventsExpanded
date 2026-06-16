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
}
