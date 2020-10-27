using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VEE.RegularEvents
{
    public class HailStorm : GameCondition
    {
        private List<SkyOverlay> overlays;
        public HailStorm()
        {
            this.overlays = new List<SkyOverlay>
            {
                new WeatherOverlay_Rain()
            };
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
                if(map.weatherManager.curWeather != VEE_DefOf.Rain)
                {
                    map.weatherManager.TransitionTo(VEE_DefOf.Rain);
                }
                
                if (Find.TickManager.TicksGame % 3451 == 0)
                {
                    this.DoPawnsBluntamage(map);
                }
                for (int j = 0; j < this.overlays.Count; j++)
                {
                    this.overlays[j].TickOverlay(map);
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

        private void DoPawnsBluntamage(Map map)
        {
            List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn = allPawnsSpawned[i];
                if (!pawn.Position.Roofed(map) && pawn.def.race.IsFlesh)
                {
                    DamageInfo dinfo = new DamageInfo(DamageDefOf.Blunt, 2, 1);
                    pawn.TakeDamage(dinfo);
                }
            }
        }
    }
}
