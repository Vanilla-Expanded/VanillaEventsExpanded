using RimWorld;
using Verse;

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
        public static WeatherDef VEE_Hail;

        /* Biome DEF */
        public static BiomeDef TemperateSwamp;
        public static BiomeDef ColdBog;
        public static BiomeDef TropicalSwamp;
        public static BiomeDef ExtremeDesert;
        public static BiomeDef AridShrubland;

        /* ThingSetMakerDef DEF */
        public static ThingSetMakerDef AnimalPod;

        /* Hediff */
        public static HediffDef Traitor;
        public static HediffDef StandOff;
        public static HediffDef MightJoin;

        /* PawnKindDefOf */
        public static PawnKindDef StrangerInBlack;
        public static PawnKindDef Hunter;
        public static PawnKindDef VEE_TribalHunter;

        /* ThingDefOf */
        public static ThingDef VEE_ShipChunkHuman;
        public static ThingDef VEE_Shuttle;
        public static ThingDef ShuttleChunkIncoming;
        public static ThingDef SlagIncoming;

        /* IncidentDefOf */
        public static IncidentDef RaidEnemyPurple;
        public static IncidentDef ManhunterPackPurple;
        public static IncidentDef AnimalInsanityMassPurple;

        /* GameConditionDefOf */
        public static GameConditionDef IceAge;
        public static GameConditionDef GlobalWarming;
        public static GameConditionDef Drought;
        public static GameConditionDef HailStorm;
        public static GameConditionDef SpaceBattle;
        public static GameConditionDef PsychicRain;

        /* Letter def */
        public static LetterDef PurpleEvent;

        /* Meme def */

        [MayRequireIdeology]
        public static MemeDef AnimalPersonhood;
    }
}