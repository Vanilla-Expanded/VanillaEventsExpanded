using RimWorld;
using Verse;
using Verse.AI;

namespace VEE
{
    [DefOf]
    public static class VEE_DefOf
    {
        /* Job DEF */
        public static JobDef HuntAndLeave;

        /* Weather DEF */
        public static WeatherDef SnowHard;
        public static WeatherDef Rain;
        public static WeatherDef DryThunderstorm;

        /* Biome DEF */
        public static BiomeDef TemperateSwamp;
        public static BiomeDef ColdBog;
        public static BiomeDef TropicalSwamp;
        public static BiomeDef ExtremeDesert;
        public static BiomeDef AridShrubland;
        public static BiomeDef TropicalRainforest;

        /* Hediff */
        public static HediffDef Traitor;
        public static HediffDef MightJoin;

        /* PawnKindDefOf */
        public static PawnKindDef StrangerInBlack;
        public static PawnKindDef VEE_Hunter;
        public static PawnKindDef VEE_TribalHunter;

        /* ThingDefOf */

        public static ThingDef VEE_Shuttle;
        public static ThingDef ShuttleChunkIncoming;
        public static ThingDef VEE_Shuttle_Combat;
        public static ThingDef VEE_ShuttleChunkIncoming_Combat;
        public static ThingDef VEE_Shuttle_Heavy;
        public static ThingDef VEE_ShuttleChunkIncoming_Heavy;
        public static ThingDef VEE_DeepSnow;
        public static ThingDef VEE_EarthquakeEpicenter;
        public static ThingDef SlagIncoming;
        public static ThingDef Gun_Autopistol;
        public static ThingDef VEE_ShipChunkHumanIncoming;
        public static ThingDef VEE_ShipChunkHuman;
        public static ThingDef VEE_ShipChunkHumanIncoming_Volatile;
        public static ThingDef VEE_ShipChunkHuman_Volatile_Spawner;
        public static ThingDef VEE_ShipChunkHumanIncoming_Cargo;
        public static ThingDef VEE_ShipChunkHuman_Cargo;
        public static ThingDef VEE_ShipChunkHumanIncoming_DropPod;
        public static ThingDef VEE_ShipChunkHuman_DropPod_Spawner;


        /* IncidentDefOf */
        public static IncidentDef RaidEnemyPurple;
        public static IncidentDef ManhunterPackPurple;
        public static IncidentDef AnimalInsanityMassPurple;
        public static IncidentDef VEE_WhiteoutRefugees;

        public static LetterDef VEE_WhiteoutRefugeesLetter;

        /* GameConditionDefOf */
        public static GameConditionDef VEE_Whiteout;
        public static GameConditionDef GlobalWarming;
        public static GameConditionDef SpaceBattle;
        public static GameConditionDef PsychicRain;
        public static GameConditionDef VEE_Drought;

        /* Letter def */
        public static LetterDef PurpleEvent;

        public static DutyDef VEE_CarryAndLeave;

        /* Meme def */

        [MayRequireIdeology]
        public static MemeDef AnimalPersonhood;

        [MayRequireBiotech]
        public static FactionDef OutlanderRoughPig;

        public static ThingSetMakerDef VEE_MeteoriteShowerMaker;
        public static ThingSetMakerDef VEE_ShuttleCrash_Resources;
        public static ThingSetMakerDef VEE_CrashlandedColonists;

        public static EffecterDef VEE_EmergencePointSustained8X8;
    }
}
