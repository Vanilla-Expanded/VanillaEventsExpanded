using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace VEE
{
    public class WeatherWorker_PsychicRain: WeatherWorker
    {
        private WeatherDef def;

        public WeatherWorker_PsychicRain(WeatherDef def)
        : base(def)
        {
        }

        public new void WeatherTick(Map map, float lerpFactor)
        {
            for (int i = 0; i < overlays.Count; i++)
            {
                overlays[i].TickOverlay(map, lerpFactor);
            }
            for (int j = 0; j < def.eventMakers.Count; j++)
            {
                def.eventMakers[j].WeatherEventMakerTick(map, lerpFactor);
            }
            if (Find.TickManager.TicksGame % 2000 != 0)
            {
                DoPawnsAgeFaster(map);
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
                    pawn.ageTracker.AgeTickMothballed(30000);
                }
            }
        }
    }
}
