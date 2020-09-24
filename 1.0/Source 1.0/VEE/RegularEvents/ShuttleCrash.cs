using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using RimWorld;

namespace VEE.RegularEvents
{
    class ShuttleCrash : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            string label = "ShuttleCrashLabel".Translate();
            string text = "ShuttleCrash".Translate();

            IntVec3 pos;
            ThingDef shipChunkIncoming = VEE_DefOf.ShuttleChunkIncoming;
            Predicate<IntVec3> validator = delegate (IntVec3 x)
            {
                CellRect.CellRectIterator iterator = GenAdj.OccupiedRect(x, Rot4.North, shipChunkIncoming.size).GetIterator();
                while (!iterator.Done())
                {
                    IntVec3 c = iterator.Current;
                    if (!c.InBounds(map) || c.Fogged(map) || !c.Standable(map) || (c.Roofed(map) && c.GetRoof(map).isThickRoof))
                    {
                        return false;
                    }
                    if (c.GetFirstBuilding(map) != null)
                    {
                        return false;
                    }
                    if (c.GetFirstSkyfaller(map) != null)
                    {
                        return false;
                    }
                    if (c.GetTerrain(map).IsWater)
                    {
                        return false;
                    }
                    iterator.MoveNext();
                }
                return (!SkyfallerUtility.CanPossiblyFallOnColonist(shipChunkIncoming, x, map) && (10 <= 0 || x.DistanceToEdge(map) >= 10) && map.reachability.CanReachColony(x));
            };

            CellFinderLoose.TryFindRandomNotEdgeCellWith(5, validator, map, out pos);
            SkyfallerMaker.SpawnSkyfaller(VEE_DefOf.ShuttleChunkIncoming, VEE_DefOf.VEE_Shuttle, pos, map);
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, new TargetInfo(pos, map, false), null, null);
            return true;
        }
    }
}
