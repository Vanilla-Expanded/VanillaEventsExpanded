using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
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

            this.plantChoosen = ThingStuffPair.AllWith(p => 
                p.plant != null && 
                p.plant.harvestTag == "Standard" && 
                p.plant.harvestYield != 0 && 
                p.plant.harvestedThingDef != null && 
                !p.defName.Contains("RB_") &&
                !this.excludedPlant.Contains(p.defName)).InRandomOrder().RandomElement().thing;
            
            return this.TryFindRootCell(map, out this.cell, this.plantChoosen);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            int pNumber = Rand.RangeInclusive(10, 20);

            for (int i = 0; i < pNumber; i++)
            {
                if (CellFinder.TryRandomClosewalkCellNear(this.cell, map, 6, out IntVec3 intVec, x => this.CanSpawnAt(x, map, this.plantChoosen)))
                {
                    Plant p = intVec.GetPlant(map);
                    if (p == null || !this.excludedPlant.Contains(p.def.defName))
                    {
                        p?.DeSpawn();
                        Plant crop = GenSpawn.Spawn(this.plantChoosen, intVec, map, WipeMode.Vanish) as Plant;
                        crop.Growth = 0.5f;
                    }
                }
            }
            
            Find.LetterStack.ReceiveLetter("CSLabel".Translate(), $"{"CS1".Translate()} {this.plantChoosen.label} {"CS2".Translate()}", LetterDefOf.PositiveEvent, new TargetInfo(this.cell, map), hyperlinkThingDefs: new List<ThingDef> { this.plantChoosen });
            return true;
        }

        private bool TryFindRootCell(Map map, out IntVec3 cell, ThingDef plant)
        {
            return CellFinderLoose.TryFindRandomNotEdgeCellWith(10, (IntVec3 x) => this.CanSpawnAt(x, map, plant), map, out cell);
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
