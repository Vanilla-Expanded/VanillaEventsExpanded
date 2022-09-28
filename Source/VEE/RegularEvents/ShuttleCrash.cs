using RimWorld;
using Verse;

namespace VEE.RegularEvents
{
    public class ShuttleCrash : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            string label = "ShuttleCrashLabel".Translate();
            string text = "ShuttleCrash".Translate();

            CellFinderLoose.TryFindSkyfallerCell(VEE_DefOf.ShuttleChunkIncoming, map, out IntVec3 pos);
            SkyfallerMaker.SpawnSkyfaller(VEE_DefOf.ShuttleChunkIncoming, VEE_DefOf.VEE_Shuttle, pos, map);
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, new TargetInfo(pos, map, false), null, null);
            return true;
        }
    }
}