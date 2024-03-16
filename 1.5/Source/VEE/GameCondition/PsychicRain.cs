using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace VEE.PurpleEvents
{
    public class PsychicRain : GameCondition
    {
        private SkyColorSet PsychicRainColors;
        private readonly List<SkyOverlay> overlays;

        public PsychicRain()
        {
            ColorInt colorInt = new ColorInt(147, 112, 219);
            Color toColor = colorInt.ToColor;
            ColorInt colorInt2 = new ColorInt(234, 200, 255);
            PsychicRainColors = new SkyColorSet(toColor, colorInt2.ToColor, new Color(0.6f, 0.4f, 0.9f), 0.85f);
            overlays = new List<SkyOverlay>
            {
                new WeatherOverlay_Rain()
            };
        }

        public override bool AllowEnjoyableOutsideNow(Map map) => false;

        public override WeatherDef ForcedWeather()
        {
            if (Find.CurrentMap?.mapTemperature?.OutdoorTemp <= 0)
            {
                return VEE_DefOf.SnowHard;
            }
            else
            {
                return VEE_DefOf.Rain;
            }
        }

        public override void Init()
        {
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.ForbiddingDoors, OpportunityType.Critical);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.AllowedAreas, OpportunityType.Critical);
        }

        public override void GameConditionTick()
        {
            List<Map> affectedMaps = base.AffectedMaps;
            foreach (Map map in affectedMaps)
            {
                if (Find.TickManager.TicksGame % 3451 == 0)
                {
                    DoPawnsAgeFaster(map);
                }
                for (int j = 0; j < overlays.Count; j++)
                {
                    overlays[j].TickOverlay(map);
                }
            }
        }

        private void DoPawnsAgeFaster(Map map)
        {
            IReadOnlyList<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn = allPawnsSpawned[i];
                if (!pawn.Position.Roofed(map) && pawn.def.race.IsFlesh)
                {
                    pawn.ageTracker.AgeTickMothballed((int)(20706f * pawn.GetStatValue(StatDefOf.PsychicSensitivity)));
                }
            }
        }

        public override void GameConditionDraw(Map map)
        {
            for (int i = 0; i < overlays.Count; i++)
            {
                overlays[i].DrawOverlay(map);
            }
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, 5000f, 0.5f);
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget?(new SkyTarget(0.85f, PsychicRainColors, 1f, 1f));
        }

        public override List<SkyOverlay> SkyOverlays(Map map)
        {
            return overlays;
        }
    }
}