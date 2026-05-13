using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace VEE.RegularEvents
{
    public class ShuttleCrash : IncidentWorker
    {

        public List<(ThingDef, ThingDef)> shuttleTypes = new List<(ThingDef, ThingDef)>() { (VEE_DefOf.VEE_Shuttle, VEE_DefOf.ShuttleChunkIncoming),
            (VEE_DefOf.VEE_Shuttle_Combat, VEE_DefOf.VEE_ShuttleChunkIncoming_Combat),(VEE_DefOf.VEE_Shuttle_Heavy, VEE_DefOf.VEE_ShuttleChunkIncoming_Heavy) };

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            string label = "VEE_ShuttleCrashLabel".Translate();
            string text = "VEE_ShuttleCrashDesc".Translate();

            var chosenShuttle = shuttleTypes.RandomElement();
            CellFinderLoose.TryFindSkyfallerCell(chosenShuttle.Item2, map, TerrainAffordanceDefOf.Walkable, out IntVec3 pos,extraValidator:(x => !x.GetTerrain(map).IsWater));
            if (pos.IsValid)
            {
                SkyfallerMaker.SpawnSkyfaller(chosenShuttle.Item2, chosenShuttle.Item1, pos, map);
                Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, new TargetInfo(pos, map, false), null, null);
                return true;
            }
          
            return false;
        }
    }
}