using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VEE.RegularEvents
{
    public class Cropsprout : IncidentWorker
    {
        private readonly List<string> excludedPlant = new List<string> { "Plant_TreeGauranlen", "Plant_MossGauranlen", "Plant_PodGauranlen", "Plant_TreeAnima", "Plant_GrassAnima" };
        private ThingDef plantChoosen;
        private IntVec3 cell;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;

            if (!base.CanFireNowSub(parms)) return false;
            if (!map.weatherManager.growthSeasonMemory.GrowthSeasonOutdoorsNow) return false;

            plantChoosen = ThingStuffPair.AllWith(p =>
                p.plant != null &&
                p.plant.harvestTag == "Standard" &&
                p.plant.harvestYield != 0 &&
                p.plant.harvestedThingDef != null &&
                !p.plant.cavePlant &&
                !p.defName.Contains("RB_") &&
                !p.defName.Contains("AB_") &&
                !excludedPlant.Contains(p.defName)).InRandomOrder().RandomElement().thing;

            return TryFindRootCell(map, out cell, plantChoosen);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            int pNumber = Rand.RangeInclusive(10, 20);

            for (int i = 0; i < pNumber; i++)
            {
                if (CellFinder.TryRandomClosewalkCellNear(cell, map, 6, out IntVec3 intVec, x => CanSpawnAt(x, map, plantChoosen)))
                {
                    Plant p = intVec.GetPlant(map);
                    if (p == null || !excludedPlant.Contains(p.def.defName))
                    {
                        p?.DeSpawn();
                        Plant crop = GenSpawn.Spawn(plantChoosen, intVec, map, WipeMode.Vanish) as Plant;
                        crop.Growth = 0.5f;
                    }
                }
            }

            Find.LetterStack.ReceiveLetter("CSLabel".Translate(), $"{"CS1".Translate()} {plantChoosen.label} {"CS2".Translate()}", LetterDefOf.PositiveEvent, new TargetInfo(cell, map), hyperlinkThingDefs: new List<ThingDef> { plantChoosen });
            return true;
        }

        private bool TryFindRootCell(Map map, out IntVec3 cell, ThingDef plant)
        {
            return CellFinderLoose.TryFindRandomNotEdgeCellWith(10, (IntVec3 x) => CanSpawnAt(x, map, plant), map, out cell);
        }

        private bool CanSpawnAt(IntVec3 c, Map map, ThingDef plantC)
        {
            if (!c.Standable(map) || c.Fogged(map) || map.fertilityGrid.FertilityAt(c) < plantC.plant.fertilityMin || c.GetEdifice(map) != null || !PlantUtility.GrowthSeasonNow(c, map))
            {
                return false;
            }
            return true;
        }
    }
}