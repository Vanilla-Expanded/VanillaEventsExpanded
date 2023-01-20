using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace VEE.RegularEvents
{
    public class MeteoriteShower : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return TryFindCell(out IntVec3 intVec, map);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!TryFindCell(out IntVec3 intVecb, map))
            {
                return false;
            }

            System.Random r = new System.Random();
            int n = r.Next(3, 8);
            int radius = r.Next(10, 25);
            List<Thing> list = new List<Thing>();

            IntVec3 intVec = intVecb;
            for (int o = 0; o < n; o++)
            {
                intVec += (Rand.InsideUnitCircleVec3 * radius).ToIntVec3();

                while (!intVec.InBounds(map) || intVec.Fogged(map) || !intVec.Standable(map) || (intVec.Roofed(map) && intVec.GetRoof(map).isThickRoof))
                {
                    intVec += (Rand.InsideUnitCircleVec3 * radius).ToIntVec3();
                }
                list = ThingSetMakerDefOf.Meteorite.root.Generate();
                SkyfallerMaker.SpawnSkyfaller(ThingDefOf.MeteoriteIncoming, list, intVec, map);
                intVec = intVecb;
            }

            string text = string.Format(def.letterText, list[0].def.label).CapitalizeFirst();
            Find.LetterStack.ReceiveLetter(def.letterLabel, text, LetterDefOf.NeutralEvent, new TargetInfo(intVecb, map, false), null, null);
            return true;
        }

        private bool TryFindCell(out IntVec3 cell, Map map)
        {
            int maxMineables = ThingSetMaker_Meteorite.MineablesCountRange.max;
            return CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.MeteoriteIncoming, map, out cell, 10, default(IntVec3), -1, true, false, false, false, true, true, delegate (IntVec3 x)
            {
                int num = Mathf.CeilToInt(Mathf.Sqrt(maxMineables)) + 2;
                CellRect cellRect = CellRect.CenteredOn(x, num, num);
                int num2 = 0;
                foreach (IntVec3 c in cellRect)
                {
                    if (c.InBounds(map) && c.Standable(map))
                    {
                        num2++;
                    }
                }
                return num2 >= maxMineables;
            });
        }
    }
}