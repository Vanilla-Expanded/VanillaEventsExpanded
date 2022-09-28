using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VEE.RegularEvents
{
    public class EarthQuake : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            return true;
        }

        private void DamageInRadius(List<IntVec3> list, Map map, int chanceDamage, int leftPercentDamage, int rightPercentDamage)
        {
            System.Random r = new System.Random();
            foreach (IntVec3 intVec in list)
            {
                if (r.Next(0, 101) < chanceDamage && intVec.GetFirstBuilding(map) != null)
                {
                    float rand = r.Next(leftPercentDamage, rightPercentDamage) / (float)100;
                    float damageAmount = intVec.GetFirstBuilding(map).MaxHitPoints * (float)rand;
                    // Log.Message(damageAmount.ToString());
                    DamageInfo dinfo = new DamageInfo(DamageDefOf.Mining, damageAmount);
                    intVec.GetFirstBuilding(map).TakeDamage(dinfo);
                }
            }
        }

        public void PrintList(IEnumerable<IntVec3> list)
        {
            foreach (IntVec3 item in list)
            {
                Log.Message(item.ToString());
            }
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 epicenter = new IntVec3();
            CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.ShipChunkIncoming, map, out epicenter, 30, map.Center, 999, true, false, false, false, true, false, null);
            /* ========== Near epicenter ========== */
            CellRect cellRect = CellRect.CenteredOn(epicenter, 5);
            IEnumerable<IntVec3> a0to5 = cellRect.Cells;
            DamageInRadius(a0to5.ToList(), map, 50, 25, 75);

            /* ========== 5 cell after epicenter ========== */
            CellRect cellRect2 = CellRect.CenteredOn(epicenter, 10);
            IEnumerable<IntVec3> a0to10 = cellRect2.Cells;
            List<IntVec3> a5to10 = new List<IntVec3>();
            foreach (IntVec3 item in a0to10)
            {
                if (!a0to5.Contains(item))
                {
                    a5to10.Add(item);
                }
            }
            DamageInRadius(a5to10, map, 25, 20, 40);

            /* ========== 10 cell after epicenter ========== */
            CellRect cellRect3 = CellRect.CenteredOn(epicenter, 20);
            IEnumerable<IntVec3> a0to20 = cellRect3.Cells;
            List<IntVec3> a10to20 = new List<IntVec3>();
            foreach (IntVec3 item in a0to20)
            {
                if (!a0to5.Contains(item) && !a5to10.Contains(item))
                {
                    a10to20.Add(item);
                }
            }
            DamageInRadius(a10to20, map, 10, 5, 20);

            for (int i = 0; i < 6; i++)
            {
                Find.CameraDriver.shaker.DoShake(4);
            }

            string label = "EarthquakeLabel".Translate();
            string text = "Earthquake".Translate();
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NegativeEvent, new TargetInfo(epicenter, map, false), null, null);
            return true;
        }
    }
}