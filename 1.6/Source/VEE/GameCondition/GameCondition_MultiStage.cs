using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using VEF;
using Verse;
using Verse.Noise;

namespace VEE
{
    [HotSwappable]
    public class GameCondition_MultiStage : GameCondition
    {
        public int currentStageIndex;
        public int currentStageStartTick;
        public int currentStageDurationTicks;
        public float prevTargetTempOffset;
        public MultiStage CurrentStage => Extension.stages[currentStageIndex];
        private ConditionExtension_MultiStage _extension;
        public ConditionExtension_MultiStage Extension => _extension ??= def.GetModExtension<ConditionExtension_MultiStage>();
        private int curColorIndex = -1;
        private int prevColorIndex = -1;
        private float curColorTransition;
        public Color CurrentColor => Color.Lerp(Colors[prevColorIndex], Colors[curColorIndex], curColorTransition);
        private int AuroraTransitionDurationTicks = 60;

        private static readonly Color[] Colors = new Color[8]
        {
            new Color(1f, 0f, 1f),
            new Color(1f, 0f, 0.6f),
            new Color(0.5f, 0f, 1f),
            new Color(0.3f, 0f, 0.7f),
            new Color(0f, 0.5f, 1f),
            new Color(0f, 0f, 1f),
            new Color(0.87f, 0f, 1f),
            new Color(0.75f, 0f, 1f)
        };

        public override string TooltipString
        {
            get
            {
                var sb = new StringBuilder(base.TooltipString);
                if (DebugSettings.ShowDevGizmos)
                {
                    var ticksRemaining = (currentStageStartTick + currentStageDurationTicks) - Find.TickManager.TicksGame;
                    sb.AppendInNewLine("DEV: Time to next stage: " + ticksRemaining.ToStringTicksToPeriod());
                }
                return sb.ToString().TrimEndNewlines();
            }

        }
        public override void Init()
        {
            base.Init();
            this.Permanent = true;
            curColorIndex = Rand.Range(0, Colors.Length);
            prevColorIndex = curColorIndex;
            curColorTransition = 1f;
            EnterStage(0);
        }

        public override void GameConditionTick()
        {
            base.GameConditionTick();
            var ticksPassed = Find.TickManager.TicksGame - currentStageStartTick;
            if (ticksPassed >= currentStageDurationTicks)
            {
                if (currentStageIndex < Extension.stages.Count - 1)
                {
                    EnterStage(currentStageIndex + 1);
                }
                else
                {
                    this.End();
                    return;
                }
            }
            if (TicksPassed % 2000 == 0)
            {
                if (CurrentStage.spawnDeepSnow)
                {
                    SpawnDeepSnow();
                }
                if (CurrentStage.plantGrowthMultiplier != 1)
                {
                    StaticCollections.cachedPlantGrowthMultiplier = CurrentStage.plantGrowthMultiplier;
                }
                if (CurrentStage.globalLightLevelsMultiplier != 1)
                {
                    StaticCollections.cachedGlobalLightLevelsMultiplier = CurrentStage.globalLightLevelsMultiplier;
                }
                if (CurrentStage.hediffForOrganicPawns != null)
                {
                    HediffForOrganicPawns();
                }
            }

            if (!CurrentStage.scatterPlants.NullOrEmpty() && TicksPassed % CurrentStage.scatterPlantsInterval == 0)
            {
                ScatterPlants();
            }

            if (CurrentStage.triggerPurpleAurora)
            {
                curColorTransition += 1f / (float)AuroraTransitionDurationTicks;
                if (curColorTransition >= 1f)
                {
                    prevColorIndex = curColorIndex;
                    curColorIndex = GetNewColorIndex();
                    curColorTransition = 0f;
                }

            }

        }
        public float TemperatureOffsetWorld()
        {
            var stage = CurrentStage;
            var lerpTicks = (stage.tempLerpDays > 0 ? stage.tempLerpDays : stage.durationDays.Average) * 60000f;
            var ticksPassed = Find.TickManager.TicksGame - currentStageStartTick;
            if (lerpTicks <= 0f || ticksPassed >= lerpTicks)
            {
                return stage.targetTempOffset;
            }
            return Mathf.Lerp(prevTargetTempOffset, stage.targetTempOffset, ticksPassed / lerpTicks);
        }

        public override WeatherDef ForcedWeather()
        {
            var stage = CurrentStage;
            if (stage.forcedWeather != null)
                return stage.forcedWeather;
            return base.ForcedWeather();
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            if (CurrentStage.changeSkyColours)
                return new SkyTarget(1, CurrentStage.skyColors, 1, 1);
            if (CurrentStage.triggerPurpleAurora)
            {
                Color currentColor = CurrentColor;
                return new SkyTarget(colorSet: new SkyColorSet(Color.Lerp(Color.white, currentColor, 0.075f) * 0.7f, new Color(0.92f, 0.92f, 0.92f), Color.Lerp(Color.white, currentColor, 0.025f) * 0.7f, 1f), glow: Mathf.Max(GenCelestial.CurCelestialSunGlow(map), 0.25f), lightsourceShineSize: 1f, lightsourceShineIntensity: 1f);
            }
            return base.SkyTarget(map);
        }
        public override float SkyTargetLerpFactor(Map map)
        {
            if (CurrentStage.changeSkyColours)
                return GameConditionUtility.LerpInOutValue(this, CurrentStage.changeSkyColoursTicks, 0.5f);
            if (CurrentStage.triggerPurpleAurora)
                return GameConditionUtility.LerpInOutValue(this, 200);
            return 0;
        }

        public void EnterStage(int index)
        {
            var currentOffset = index == 0 ? 0f : TemperatureOffsetWorld();
            currentStageIndex = index;
            currentStageStartTick = Find.TickManager.TicksGame;
            prevTargetTempOffset = currentOffset;
            var stage = CurrentStage;
            StaticCollections.cachedPlantGrowthMultiplier = CurrentStage.plantGrowthMultiplier;
            StaticCollections.cachedGlobalLightLevelsMultiplier = CurrentStage.globalLightLevelsMultiplier;
            currentStageDurationTicks = Mathf.RoundToInt(stage.durationDays.RandomInRange * 60000f);
            Find.LetterStack.ReceiveLetter(stage.letterLabel.CapitalizeFirst(), stage.letterText, stage.letterDef);
            if (!CurrentStage.swapPlants.NullOrEmpty())
            {
                SwapPlants();
            }
        }

        private void SpawnDeepSnow()
        {
            foreach (var m in AffectedMaps)
            {
                for (int i = 0; i < Rand.RangeInclusive(4, 6); i++)
                {
                    if (CellFinderLoose.TryGetRandomCellWith(c =>
                    {
                        if (!c.InBounds(m) || c.Roofed(m) || !c.Standable(m) || c.GetFirstBuilding(m) != null)
                            return false;
                        return GenAdj.CellsAdjacent8Way(c, Rot4.North, new IntVec2(1, 1))
                            .Any(adj => adj.InBounds(m) && adj.GetFirstBuilding(m)?.def.fillPercent >= 0.9f);
                    }, m, 1000, out IntVec3 cell))
                    {
                        GenSpawn.Spawn(VEE_DefOf.VEE_DeepSnow, cell, m);
                    }
                }
            }
        }

        private void ScatterPlants()
        {
            foreach (var m in AffectedMaps)
            {
                foreach(ThingDef plant in CurrentStage.scatterPlants)
                {
                    if (CellFinderLoose.TryGetRandomCellWith(c =>
                    {
                        if (!c.InBounds(m) || c.Roofed(m) || !c.Standable(m) || c.GetFirstBuilding(m) != null)
                            return false;
                        return true;
                    }, m, 1000, out IntVec3 cell))
                    {
                        GenSpawn.Spawn(plant, cell, m, WipeMode.Vanish);                  
                    }
                }               
            }
        }

        private void SwapPlants()
        {
            foreach (var m in AffectedMaps)
            {
                foreach(PlantSwap plantSwap in CurrentStage.swapPlants)
                {
                    List<Thing> plants = m.listerThings.ThingsOfDef(plantSwap.plantToSwap).ToList();
                    foreach (Thing plantThing in plants)
                    {
                        if (!plantSwap.justDestroy)
                        {
                            Plant plant = plantThing as Plant;
                            float growth = plant.Growth;
                            Plant newPlant = (Plant)GenSpawn.Spawn(plantSwap.swappedPlant, plant.Position, m, WipeMode.Vanish);
                            newPlant.Growth = growth;
                        }                       
                        plantThing.Destroy();
                    }
                }             
            }
        }

        private void HediffForOrganicPawns()
        {
            List<Map> affectedMaps = base.AffectedMaps;
            for (int i = 0; i < affectedMaps.Count; i++)
            {
                IReadOnlyList<Pawn> allPawnsSpawned = affectedMaps[i].mapPawns.AllPawnsSpawned;
                for (int j = 0; j < allPawnsSpawned.Count; j++)
                {
                    if (allPawnsSpawned[j].RaceProps.IsFlesh)
                    {
                        HealthUtility.AdjustSeverity(allPawnsSpawned[j], CurrentStage.hediffForOrganicPawns, 1);
                    }
                }
            }
        }

        public override void End()
        {
            StaticCollections.cachedPlantGrowthMultiplier = 1;
            StaticCollections.cachedGlobalLightLevelsMultiplier = 1;
            base.End();
        }

        private int GetNewColorIndex()
        {
            return (from x in Enumerable.Range(0, Colors.Length)
                    where x != curColorIndex
                    select x).RandomElement();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref currentStageIndex, "currentStageIndex", 0);
            Scribe_Values.Look(ref currentStageStartTick, "currentStageStartTick", 0);
            Scribe_Values.Look(ref currentStageDurationTicks, "currentStageDurationTicks", 0);
            Scribe_Values.Look(ref prevTargetTempOffset, "prevTargetTempOffset", 0f);
            Scribe_Values.Look(ref curColorIndex, "curColorIndex", 0);
            Scribe_Values.Look(ref prevColorIndex, "prevColorIndex", 0);
            Scribe_Values.Look(ref curColorTransition, "curColorTransition", 0f);
        }
    }
}
