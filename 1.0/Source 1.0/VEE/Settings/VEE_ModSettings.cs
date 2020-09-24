using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using RimWorld;
using UnityEngine;

namespace VEE.Settings
{
    class VEE_ModSettings : ModSettings
    {
        internal static float IceAgeXbaseChance = 0.04f;
        internal static float GlobalWarmingXbaseChance = 0.04f;
        internal static float PsychicRainXbaseChance = 0.04f;
        internal static float LongNightXbaseChance = 0.04f;
        internal static float PsychicBloomXbaseChance = 0.04f;

        internal static float HailStormXbaseChance = 0.15f;
        internal static float MeteoriteShowerXbaseChance = 0.4f;
        internal static float AnimalPodCrashXbaseChance = 2f;
        internal static float WandererJoinTraitorXbaseChance = 0.4f;
        internal static float SpaceBattleXbaseChance = 0.3f;
        internal static float ShuttleCrashXbaseChance = 0.5f;
        internal static float EarthquakeXbaseChance = 0.3f;
        internal static float CargopodsweaponsXbaseChance = 2f;
        internal static float CargopodsapparelXbaseChance = 2f;
        internal static float CaravanAnimalsWanderInXbaseChance = 0.8f;
        internal static float CropSproutXbaseChance = 0.35f;
        internal static float BattleAnimalsWanderInXbaseChance = 0.25f;
        internal static float DroughtXbaseChance = 0.15f;
        internal static float WildMenWanderInXbaseChance = 0.50f;
        internal static float HuntingPartyXbaseChance = 0.45f;

        internal static int minRefireDaysPurple = 300;
        internal static int earliestDayPurple = 180;
        internal static bool multipleAtOnce = false;

        public void ChangeDef()
        {
            List<IncidentDef> AllincidentDefs = DefDatabase<IncidentDef>.AllDefs.ToList();
            foreach (IncidentDef item in AllincidentDefs)
            {
                if (item.defName == "VEE_IceAge") { item.baseChance = IceAgeXbaseChance; }
                if (item.defName == "VEE_GlobalWarming") { item.baseChance = GlobalWarmingXbaseChance; }
                if (item.defName == "VEE_PsychicRain") { item.baseChance = PsychicRainXbaseChance; }
                if (item.defName == "VEE_LongNight") { item.baseChance = LongNightXbaseChance; }
                if (item.defName == "VEE_PsychicBloom") { item.baseChance = PsychicBloomXbaseChance; }

                if (item.defName == "VEE_HailStorm") { item.baseChance = HailStormXbaseChance; }
                if (item.defName == "VEE_MeteoriteShower") { item.baseChance = MeteoriteShowerXbaseChance; }
                if (item.defName == "VEE_AnimalPodCrash") { item.baseChance = AnimalPodCrashXbaseChance; }
                if (item.defName == "VEE_WandererJoinTraitor") { item.baseChance = WandererJoinTraitorXbaseChance; }
                if (item.defName == "VEE_SpaceBattle") { item.baseChance = SpaceBattleXbaseChance; }
                if (item.defName == "VEE_ShuttleCrash") { item.baseChance = ShuttleCrashXbaseChance; }
                if (item.defName == "VEE_Cargopodsweapons") { item.baseChance = CargopodsweaponsXbaseChance; }
                if (item.defName == "VEE_Cargopodsapparel") { item.baseChance = CargopodsapparelXbaseChance; }
                if (item.defName == "VEE_CaravanAnimalsWanderIn") { item.baseChance = CaravanAnimalsWanderInXbaseChance;}
                if (item.defName == "VEE_CropSprout") { item.baseChance = CropSproutXbaseChance; }
                if (item.defName == "VEE_Earthquake") { item.baseChance = EarthquakeXbaseChance; }
                if (item.defName == "VEE_BattleAnimalsWanderIn") { item.baseChance = BattleAnimalsWanderInXbaseChance; }
                if (item.defName == "VEE_Drought") { item.baseChance = DroughtXbaseChance; }
                if (item.defName == "VEE_WildMenWanderIn") { item.baseChance = WildMenWanderInXbaseChance; }
                if (item.defName == "VEE_HuntingParty") { item.baseChance = HuntingPartyXbaseChance; }

                if (item.letterDef != null && item.letterDef.defName == "PurpleEvent")
                {
                    item.minRefireDays = minRefireDaysPurple;
                    item.earliestDay = earliestDayPurple;
                }
            }
        }

        public static void ChangeDefPost()
        {
            List<IncidentDef> AllincidentDefs = DefDatabase<IncidentDef>.AllDefs.ToList();
            foreach (IncidentDef item in AllincidentDefs)
            {
                if (item.defName == "VEE_IceAge") { item.baseChance = IceAgeXbaseChance; }
                if (item.defName == "VEE_GlobalWarming") { item.baseChance = GlobalWarmingXbaseChance; }
                if (item.defName == "VEE_PsychicRain") { item.baseChance = PsychicRainXbaseChance; }
                if (item.defName == "VEE_LongNight") { item.baseChance = LongNightXbaseChance; }
                if (item.defName == "VEE_PsychicBloom") { item.baseChance = PsychicBloomXbaseChance; }

                if (item.defName == "VEE_HailStorm") { item.baseChance = HailStormXbaseChance; }
                if (item.defName == "VEE_MeteoriteShower") { item.baseChance = MeteoriteShowerXbaseChance; }
                if (item.defName == "VEE_AnimalPodCrash") { item.baseChance = AnimalPodCrashXbaseChance; }
                if (item.defName == "VEE_WandererJoinTraitor") { item.baseChance = WandererJoinTraitorXbaseChance; }
                if (item.defName == "VEE_SpaceBattle") { item.baseChance = SpaceBattleXbaseChance; }
                if (item.defName == "VEE_ShuttleCrash") { item.baseChance = ShuttleCrashXbaseChance; }
                if (item.defName == "VEE_Cargopodsweapons") { item.baseChance = CargopodsweaponsXbaseChance; }
                if (item.defName == "VEE_Cargopodsapparel") { item.baseChance = CargopodsapparelXbaseChance; }
                if (item.defName == "VEE_CaravanAnimalsWanderIn") { item.baseChance = CaravanAnimalsWanderInXbaseChance; }
                if (item.defName == "VEE_CropSprout") { item.baseChance = CropSproutXbaseChance; }
                if (item.defName == "VEE_Earthquake") { item.baseChance = EarthquakeXbaseChance; }
                if (item.defName == "VEE_BattleAnimalsWanderIn") { item.baseChance = BattleAnimalsWanderInXbaseChance; }
                if (item.defName == "VEE_Drought") { item.baseChance = DroughtXbaseChance; }
                if (item.defName == "VEE_WildMenWanderIn") { item.baseChance = WildMenWanderInXbaseChance; }
                if (item.defName == "VEE_HuntingParty") { item.baseChance = HuntingPartyXbaseChance; }

                if (item.letterDef != null && item.letterDef.defName == "PurpleEvent")
                {
                    item.minRefireDays = minRefireDaysPurple;
                    item.earliestDay = earliestDayPurple;
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref IceAgeXbaseChance, "IceAgeXbaseChance", 0.04f);
            Scribe_Values.Look(ref GlobalWarmingXbaseChance, "GlobalWarmingXbaseChance", 0.04f);
            Scribe_Values.Look(ref PsychicRainXbaseChance, "PsychicRainXbaseChance", 0.04f);
            Scribe_Values.Look(ref LongNightXbaseChance, "LongNightXbaseChance", 0.04f);

            Scribe_Values.Look(ref HailStormXbaseChance, "HailStormXbaseChance", 0.15f);
            Scribe_Values.Look(ref MeteoriteShowerXbaseChance, "MeteoriteShowerXbaseChance", 0.4f);
            Scribe_Values.Look(ref AnimalPodCrashXbaseChance, "AnimalPodCrashXbaseChance", 2f);
            Scribe_Values.Look(ref WandererJoinTraitorXbaseChance, "WandererJoinTraitorXbaseChance", 0.4f);
            Scribe_Values.Look(ref SpaceBattleXbaseChance, "SpaceBattleXbaseChance", 0.3f);
            Scribe_Values.Look(ref ShuttleCrashXbaseChance, "ShuttleCrashXbaseChance", 0.5f);
            Scribe_Values.Look(ref EarthquakeXbaseChance, "EarthquakeXbaseChance", 0.3f);
            Scribe_Values.Look(ref CargopodsweaponsXbaseChance, "CargopodsweaponsXbaseChance", 2f);
            Scribe_Values.Look(ref CargopodsapparelXbaseChance, "CargopodsapparelXbaseChance", 2f);
            Scribe_Values.Look(ref CaravanAnimalsWanderInXbaseChance, "CaravanAnimalsWanderInXbaseChance", 0.8f);
            Scribe_Values.Look(ref CropSproutXbaseChance, "CropSproutXbaseChance", 0.35f);
            Scribe_Values.Look(ref BattleAnimalsWanderInXbaseChance, "BattleAnimalsWanderInXbaseChance", 0.25f);
            Scribe_Values.Look(ref DroughtXbaseChance, "DroughtXbaseChance", 0.15f);
            Scribe_Values.Look(ref WildMenWanderInXbaseChance, "WildMenWanderInXbaseChance", 0.50f);
            Scribe_Values.Look(ref HuntingPartyXbaseChance, "HuntingPartyXbaseChance", 0.45f);

            Scribe_Values.Look(ref minRefireDaysPurple, "minRefireDaysPurple", 300);
            Scribe_Values.Look(ref earliestDayPurple, "earliestDayPurple", 180);
        }
    }
}
