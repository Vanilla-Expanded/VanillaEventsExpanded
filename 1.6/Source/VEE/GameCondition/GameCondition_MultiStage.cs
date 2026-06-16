using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using VEF;
using Verse;

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
            if (CurrentStage.spawnDeepSnow && TicksPassed % 2000 == 0)
            {
                SpawnDeepSnow();
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

        public void EnterStage(int index)
        {
            var currentOffset = TemperatureOffsetWorld();
            currentStageIndex = index;
            currentStageStartTick = Find.TickManager.TicksGame;
            prevTargetTempOffset = currentOffset;
            var stage = CurrentStage;
            currentStageDurationTicks = Mathf.RoundToInt(stage.durationDays.RandomInRange * 60000f);
            Find.LetterStack.ReceiveLetter(stage.letterLabel.CapitalizeFirst(), stage.letterText, stage.letterDef);
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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref currentStageIndex, "currentStageIndex", 0);
            Scribe_Values.Look(ref currentStageStartTick, "currentStageStartTick", 0);
            Scribe_Values.Look(ref currentStageDurationTicks, "currentStageDurationTicks", 0);
            Scribe_Values.Look(ref prevTargetTempOffset, "prevTargetTempOffset", 0f);
        }
    }
}
