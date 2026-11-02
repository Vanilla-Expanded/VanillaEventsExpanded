using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Noise;

namespace VEE.RegularEvents
{
    public struct RichterScaleChances
    {
        public int scale;
        public float chance;
    }

    [StaticConstructorOnStartup]
    public class EarthQuake : IncidentWorker
    {

        private static readonly RichterScaleChances[] RichterScaleChances;

        static EarthQuake()
        {
            RichterScaleChances[] array = new RichterScaleChances[10];
            RichterScaleChances richterScaleChances = new RichterScaleChances{scale = 1,chance = 0.2f};
            array[0] = richterScaleChances;
            richterScaleChances = new RichterScaleChances { scale = 2, chance = 0.18f };
            array[1] = richterScaleChances;
            richterScaleChances = new RichterScaleChances { scale = 3, chance = 0.16f };
            array[2] = richterScaleChances;
            richterScaleChances = new RichterScaleChances { scale = 4, chance = 0.14f };
            array[3] = richterScaleChances;
            richterScaleChances = new RichterScaleChances { scale = 5, chance = 0.11f };
            array[4] = richterScaleChances;
            richterScaleChances = new RichterScaleChances { scale = 6, chance = 0.08f };
            array[5] = richterScaleChances;
            richterScaleChances = new RichterScaleChances { scale = 7, chance = 0.06f };
            array[6] = richterScaleChances;
            richterScaleChances = new RichterScaleChances { scale = 8, chance = 0.04f };
            array[7] = richterScaleChances;
            richterScaleChances = new RichterScaleChances { scale = 9, chance = 0.02f };
            array[8] = richterScaleChances;
            richterScaleChances = new RichterScaleChances { scale = 10, chance = 0.01f };
            array[9] = richterScaleChances;

            RichterScaleChances = array;
        }


        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            return true;
        }

        public int GetScale()
        {
            return RichterScaleChances.RandomElementByWeight(x => x.chance).scale;

        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 epicenter = new IntVec3();
            CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.ShipChunkIncoming, map, TerrainAffordanceDefOf.Walkable, out epicenter, 30, map.Center, 999, true, false, false, false, true, false, null);
            int earthquakeScale = GetScale();
            int earthquakeRadius = 10 + earthquakeScale * 5;
            int earthquakeDuration = earthquakeScale * 60;

            EarthquakeEpicenter earthquakeItem = GenSpawn.Spawn(VEE_DefOf.VEE_EarthquakeEpicenter, epicenter, map) as EarthquakeEpicenter;
            earthquakeItem.earthquakeScale = earthquakeScale;
            earthquakeItem.earthquakeRadius = earthquakeRadius;
            earthquakeItem.earthquakeDuration = earthquakeDuration;

            string label = "VEE_EarthquakeLabel".Translate(earthquakeScale);
            string text = "VEE_EarthquakeDesc".Translate(earthquakeScale);
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.ThreatBig, new TargetInfo(epicenter, map, false), null, null);
            return true;
        }

       

        
    }
}