using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using UnityEngine;
using RimWorld;

namespace VEE.Settings
{
    class VEE_Mod : Mod
    {
        public static VEE_ModSettings settings;
        private Vector2 scrollPosition = Vector2.zero;

        public VEE_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<VEE_ModSettings>();
        }

        public override string SettingsCategory() => "SCat".Translate();

        public void ResetSettings()
        {
            VEE_ModSettings.IceAgeXbaseChance = 0.04f;
            VEE_ModSettings.GlobalWarmingXbaseChance = 0.04f;
            VEE_ModSettings.PsychicRainXbaseChance = 0.04f;
            VEE_ModSettings.LongNightXbaseChance = 0.04f;
            VEE_ModSettings.PsychicBloomXbaseChance = 0.04f;

            VEE_ModSettings.HailStormXbaseChance = 0.15f;
            VEE_ModSettings.MeteoriteShowerXbaseChance = 0.4f;
            VEE_ModSettings.AnimalPodCrashXbaseChance = 2f;
            VEE_ModSettings.WandererJoinTraitorXbaseChance = 0.4f;
            VEE_ModSettings.SpaceBattleXbaseChance = 0.3f;
            VEE_ModSettings.ShuttleCrashXbaseChance = 0.5f;
            VEE_ModSettings.EarthquakeXbaseChance = 0.3f;
            VEE_ModSettings.CargopodsweaponsXbaseChance = 2f;
            VEE_ModSettings.CargopodsapparelXbaseChance = 2f;
            VEE_ModSettings.CaravanAnimalsWanderInXbaseChance = 0.8f;
            VEE_ModSettings.CropSproutXbaseChance = 0.35f;
            VEE_ModSettings.BattleAnimalsWanderInXbaseChance = 0.25f;
            VEE_ModSettings.DroughtXbaseChance = 0.15f;
            VEE_ModSettings.WildMenWanderInXbaseChance = 0.50f;
            VEE_ModSettings.HuntingPartyXbaseChance = 0.45f;

            VEE_ModSettings.minRefireDaysPurple = 300;
            VEE_ModSettings.earliestDayPurple = 180;

            settings.Write();
            settings.ChangeDef();
        }

        public void ResetWorldGameCondtion()
        {
            GameConditionManager gameConditionManagerWorld = Find.World.GameConditionManager;
            List<GameCondition> gameConditionsList = new List<GameCondition>();

            foreach (GameCondition item in gameConditionManagerWorld.ActiveConditions)
            {
                gameConditionsList.Add(item);
            }

            for (int y = 0; y < gameConditionsList.Count; y++)
            {
                if (!gameConditionsList[y].Permanent)
                {
                    gameConditionsList[y].End();
                }
            }
        }

        public void ResetMapGameCondtion()
        {
            GameConditionManager gameConditionManager = Find.CurrentMap.GameConditionManager;
            List<GameCondition> gameConditionsList = new List<GameCondition>();

            foreach (GameCondition item in gameConditionManager.ActiveConditions)
            {
                gameConditionsList.Add(item);
            }

            for (int y = 0; y < gameConditionsList.Count; y++)
            {
                if (!gameConditionsList[y].Permanent)
                {
                    gameConditionsList[y].End();
                }
            }
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            settings.ChangeDef();
            Rect outRect = new Rect(rect.x, rect.y, rect.width - 30f, rect.height * 1.5f);
            Listing_Standard list = new Listing_Standard();
            Widgets.BeginScrollView(rect, ref scrollPosition, outRect, true);
            list.Begin(outRect);
            /* ========== Reset ========= */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                if(Widgets.ButtonText(fullRect, "BReset".Translate()))
                {
                    this.ResetSettings();
                }

            }
            /* ========== Reset GameCondition ========= */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                if (Find.World != null)
                {
                    if (Widgets.ButtonText(leftRect, "GCWReset".Translate()))
                    {
                        this.ResetWorldGameCondtion();
                    }
                    if (Widgets.ButtonText(rightRect, "GCMReset".Translate()))
                    {
                        this.ResetMapGameCondtion();
                    }
                }
                else
                {
                    Widgets.Label(fullRect, "NeedToBeIngame".Translate());
                }

            }

            list.Gap(20);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Widgets.Label(fullRect, "CEBC".Translate());

            }
            /* ========== Ice Age ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "IABaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.IceAgeXbaseChance.ToString());

                VEE_ModSettings.IceAgeXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.IceAgeXbaseChance, 0, 10, true);
            }
            /* ========== Global Warming ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "GWBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.GlobalWarmingXbaseChance.ToString());

                VEE_ModSettings.GlobalWarmingXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.GlobalWarmingXbaseChance, 0, 10, true);
            }
            /* ========== Psychic Rain ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "PRBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.PsychicRainXbaseChance.ToString());

                VEE_ModSettings.PsychicRainXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.PsychicRainXbaseChance, 0, 10, true);
            }
            /* ========== Long Night ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "LNBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.LongNightXbaseChance.ToString());

                VEE_ModSettings.LongNightXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.LongNightXbaseChance, 0, 10, true);
            }
            /* ========== Psychic Bloom ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "PBBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.PsychicBloomXbaseChance.ToString());

                VEE_ModSettings.PsychicBloomXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.PsychicBloomXbaseChance, 0, 10, true);
            }


            /* ========== HailStorm ========== */
            list.Gap(30);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "HSBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.HailStormXbaseChance.ToString());

                VEE_ModSettings.HailStormXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.HailStormXbaseChance, 0, 10, true);
            }
            /* ========== MeteoriteShower ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "MSBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.MeteoriteShowerXbaseChance.ToString());

                VEE_ModSettings.MeteoriteShowerXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.MeteoriteShowerXbaseChance, 0, 10, true);
            }
            /* ========== AnimalPodCrash ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "APCBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.AnimalPodCrashXbaseChance.ToString());

                VEE_ModSettings.AnimalPodCrashXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.AnimalPodCrashXbaseChance, 0, 10, true);
            }
            /* ========== WandererJoinTraitor ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "WJTBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.WandererJoinTraitorXbaseChance.ToString());

                VEE_ModSettings.WandererJoinTraitorXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.WandererJoinTraitorXbaseChance, 0, 10, true);
            }
            /* ========== SpaceBattle ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "SPBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.SpaceBattleXbaseChance.ToString());

                VEE_ModSettings.SpaceBattleXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.SpaceBattleXbaseChance, 0, 10, true);
            }
            /* ========== ShuttleCrash ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "SCBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.ShuttleCrashXbaseChance.ToString());

                VEE_ModSettings.ShuttleCrashXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.ShuttleCrashXbaseChance, 0, 10, true);
            }
            /* ========== Earthquake ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "EBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.EarthquakeXbaseChance.ToString());

                VEE_ModSettings.EarthquakeXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.EarthquakeXbaseChance, 0, 10, true);
            }
            /* ========== Cargopodsweapons ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "CPWBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.CargopodsweaponsXbaseChance.ToString());

                VEE_ModSettings.CargopodsweaponsXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.CargopodsweaponsXbaseChance, 0, 10, true);
            }
            /* ========== Cargopodsapparel ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "CPABaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.CargopodsapparelXbaseChance.ToString());

                VEE_ModSettings.CargopodsapparelXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.CargopodsapparelXbaseChance, 0, 10, true);
            }
            /* ========== CaravanAnimalsWanderIn ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "CAWIBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.CaravanAnimalsWanderInXbaseChance.ToString());

                VEE_ModSettings.CaravanAnimalsWanderInXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.CaravanAnimalsWanderInXbaseChance, 0, 10, true);
            }
            /* ========== CropSprout ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "CSBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.CropSproutXbaseChance.ToString());

                VEE_ModSettings.CropSproutXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.CropSproutXbaseChance, 0, 10, true);
            }
            /* ========== BattleAnimalsWanderIn ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "BAWIBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.BattleAnimalsWanderInXbaseChance.ToString());

                VEE_ModSettings.BattleAnimalsWanderInXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.BattleAnimalsWanderInXbaseChance, 0, 10, true);
            }
            /* ========== Drought ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "DBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.DroughtXbaseChance.ToString());

                VEE_ModSettings.DroughtXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.DroughtXbaseChance, 0, 10, true);
            }
            /* ========== Wild men wander in ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "WMWIBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.WildMenWanderInXbaseChance.ToString());

                VEE_ModSettings.WildMenWanderInXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.WildMenWanderInXbaseChance, 0, 10, true);
            }
            /* ========== Hunting party ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "HPBaseC".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.HuntingPartyXbaseChance.ToString());

                VEE_ModSettings.HuntingPartyXbaseChance = Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.HuntingPartyXbaseChance, 0, 10, true, null, null, null, -2);
            }

            list.Gap(20);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Widgets.Label(fullRect, "PES".Translate());

            }
            /* ========== Min refire days ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "MinTPE".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.minRefireDaysPurple.ToString());

                VEE_ModSettings.minRefireDaysPurple = (int)Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.minRefireDaysPurple, 120, 400, true);
            }
            /* ========== earliest days ========== */
            list.Gap(10);
            {
                Rect fullRect = list.GetRect(Text.LineHeight);
                Rect leftRect = fullRect.LeftHalf().Rounded();
                Rect rightRect = fullRect.RightHalf().Rounded();

                Rect leftRect1 = leftRect.LeftHalf().Rounded();
                Rect leftRect2 = leftRect.RightHalf().Rounded();
                leftRect1.Overlaps(leftRect2);
                Rect leftRect3 = leftRect2.RightHalf().Rounded();

                Widgets.Label(leftRect1, "ETPE".Translate());
                Widgets.Label(leftRect3, VEE_ModSettings.earliestDayPurple.ToString());

                VEE_ModSettings.earliestDayPurple = (int)Widgets.HorizontalSlider(new Rect(rightRect.xMin + rightRect.height + 10f, rightRect.y, rightRect.width - ((rightRect.height * 2) + 20f), rightRect.height),
                  VEE_ModSettings.earliestDayPurple, 5, 280, true);
            }
            list.End();
            Widgets.EndScrollView();
            settings.Write();
            settings.ChangeDef();
        }
    }
}
