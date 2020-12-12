using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VEE.PurpleEvents
{
    public class PsychicRain : GameCondition
    {
        private SkyColorSet PsychicRainColors;
        private List<SkyOverlay> overlays;

        public PsychicRain()
        {
            ColorInt colorInt = new ColorInt(147, 112, 219);
            Color toColor = colorInt.ToColor;
            ColorInt colorInt2 = new ColorInt(234, 200, 255);
            this.PsychicRainColors = new SkyColorSet(toColor, colorInt2.ToColor, new Color(0.6f, 0.4f, 0.9f), 0.85f);
            this.overlays = new List<SkyOverlay>
            {
                new WeatherOverlay_Rain()
            };
        }

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
                    for (int i = 0; i < affectedMaps.Count; i++)
                    {
                        this.DoPawnsAgeFaster(affectedMaps[i]);
                    }
                }
                for (int j = 0; j < this.overlays.Count; j++)
                {
                    for (int k = 0; k < affectedMaps.Count; k++)
                    {
                        this.overlays[j].TickOverlay(affectedMaps[k]);
                    }
                }
            }
        }

        private void DoPawnsAgeFaster(Map map)
        {
            List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn = allPawnsSpawned[i];
                if (!pawn.Position.Roofed(map) && pawn.def.race.IsFlesh)
                {
                    pawn.ageTracker.AgeBiologicalTicks += 20706;
                }
            }
        }

        public override void GameConditionDraw(Map map)
        {
            for (int i = 0; i < this.overlays.Count; i++)
            {
                this.overlays[i].DrawOverlay(map);
            }
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, 5000f, 0.5f);
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget?(new SkyTarget(0.85f, this.PsychicRainColors, 1f, 1f));
        }

        public override List<SkyOverlay> SkyOverlays(Map map)
        {
            return this.overlays;
        }
    }
}
