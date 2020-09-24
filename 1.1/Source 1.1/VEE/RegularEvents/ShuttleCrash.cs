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
            CellFinderLoose.TryFindSkyfallerCell(VEE_DefOf.ShuttleChunkIncoming, map, out pos);
            SkyfallerMaker.SpawnSkyfaller(VEE_DefOf.ShuttleChunkIncoming, VEE_DefOf.VEE_Shuttle, pos, map);
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, new TargetInfo(pos, map, false), null, null);
            return true;
        }
    }
}
