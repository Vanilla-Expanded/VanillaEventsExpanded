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

        private Dictionary<Pawn, bool> pawnsArmorValue = new Dictionary<Pawn, bool>();

        public override void Init()
        {
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.ForbiddingDoors, OpportunityType.Critical);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.AllowedAreas, OpportunityType.Critical);
        }

        public override bool AllowEnjoyableOutsideNow(Map map)
        {
            return false;
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look<Pawn, bool>(ref pawnsArmorValue, "pawnarmorvalue", LookMode.Deep, LookMode.Value);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                
            }
            base.ExposeData();
        }

        public override void GameConditionTick()
        {
            List<Map> affectedMaps = base.AffectedMaps;
            foreach (Map map in affectedMaps)
            {                
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

        public override WeatherDef ForcedWeather()
        {
            return VEE_DefOf.VEE_Hail;
        }

        public override void End()
        {
            base.End();
            this.SingleMap.weatherManager.TransitionTo(WeatherDefOf.Clear);
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
                if (pawn != null && !pawn.Position.Roofed(map) && pawn.def.race != null && pawn.def.race.IsFlesh && Rand.Bool)
                {
                    if (GetOverallArmor(pawn, StatDefOf.ArmorRating_Blunt) < 0.06f)
                    {
                        DamageInfo dinfo = new DamageInfo(DamageDefOf.Blunt, 0.8f);
                        dinfo.SetBodyRegion(BodyPartHeight.Top, BodyPartDepth.Outside);
                        pawn.TakeDamage(dinfo);
                    }
                }
            }
        }

        private float GetOverallArmor(Pawn pawn, StatDef stat)
        {
            float num = 0f;
            float num2 = Mathf.Clamp01(pawn.GetStatValue(stat, true) / 2f);
            List<BodyPartRecord> allParts = pawn.RaceProps.body.AllParts;
            List<Apparel> list = (pawn.apparel != null) ? pawn.apparel.WornApparel : null;
            for (int i = 0; i < allParts.Count; i++)
            {
                float num3 = 1f - num2;
                if (list != null)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].def.apparel.CoversBodyPart(allParts[i]))
                        {
                            float num4 = Mathf.Clamp01(list[j].GetStatValue(stat, true) / 2f);
                            num3 *= 1f - num4;
                        }
                    }
                }
                num += allParts[i].coverageAbs * (1f - num3);
            }
            num = Mathf.Clamp(num * 2f, 0f, 2f);

            return num;
        }
    }
}
