using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VEE
{

    [HarmonyPatch(typeof(WeatherWorker))]
    [HarmonyPatch("WeatherTick")]
    public class VEE_WeatherWorker_WeatherTick_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(WeatherDef ___def, Map map)
        {
            if(___def == VEE_DefOf.VEE_PsychicRain)
            {
                if (Find.TickManager.TicksGame % 2000 == 0)
                {
                    DoPawnsAgeFaster(map);
                }

            }
        }

        private static void DoPawnsAgeFaster(Map map)
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
