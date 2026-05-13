using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VEE.RegularEvents
{
    public class Cropsprout : IncidentWorker
    {
        private ThingDef plantChoosen;
        private IntVec3 cell;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;

            if (!base.CanFireNowSub(parms)) return false;

            plantChoosen = StaticCollections.cropSproutCandidates.RandomElement();

            if (!PlantUtility.GrowthSeasonNow(map, plantChoosen))
            {
                return false;
            }

            return TryFindRootCell(map, out cell, plantChoosen);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            int pNumber = Rand.RangeInclusive(15, 30);

            for (int i = 0; i < pNumber; i++)
            {
                if (CellFinder.TryRandomClosewalkCellNear(cell, map, 8, out IntVec3 intVec, x => CanSpawnAt(x, map, plantChoosen)))
                {
                    intVec.GetPlant(map)?.Destroy();
                    Plant crop = GenSpawn.Spawn(plantChoosen, intVec, map, WipeMode.Vanish) as Plant;
                    crop.Growth = new FloatRange(0.05f, 0.15f).RandomInRange;
                    
                }
            }

            Find.LetterStack.ReceiveLetter("VEE_CropSproutLabel".Translate(plantChoosen.label), "VEE_CropSproutDesc".Translate(plantChoosen.label), LetterDefOf.PositiveEvent, new TargetInfo(cell, map), hyperlinkThingDefs: new List<ThingDef> { plantChoosen });
            return true;
        }

        private bool TryFindRootCell(Map map, out IntVec3 cell, ThingDef plant)
        {
            return CellFinderLoose.TryFindRandomNotEdgeCellWith(10, (IntVec3 x) => CanSpawnAt(x, map, plant) && x.GetRoom(map).CellCount >= 64, map, out cell);
        }

        private bool CanSpawnAt(IntVec3 c, Map map, ThingDef plantC)
        {
            if (!c.Standable(map) || c.Fogged(map) || map.fertilityGrid.FertilityAt(c) < plantC.plant.fertilityMin || !c.GetRoom(map).PsychologicallyOutdoors || c.GetEdifice(map) != null || !PlantUtility.GrowthSeasonNow(c, map, plantC))
            {
                return false;
            }
            return true;
        }
    }
}