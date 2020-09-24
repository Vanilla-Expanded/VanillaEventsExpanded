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
    class Cropsprout : IncidentWorker
    {
        private static readonly IntRange CountRange = new IntRange(10, 20);
        private const int MinRoomCells = 64;
        private const int SpawnRadius = 6;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            IntVec3 intVec;
            IEnumerable<ThingStuffPair> allP = ThingStuffPair.AllWith((ThingDef p) => p.plant != null && p.plant.harvestTag == "Standard" && p.plant.harvestYield != 0 && p.plant.harvestedThingDef != null);
            ThingStuffPair plantChoosen = allP.InRandomOrder().RandomElement();
            return map.weatherManager.growthSeasonMemory.GrowthSeasonOutdoorsNow && this.TryFindRootCell(map, out intVec, plantChoosen.thing);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 root;

            IEnumerable<ThingStuffPair> allP = ThingStuffPair.AllWith((ThingDef p) => p.plant != null && p.plant.harvestTag == "Standard" && p.plant.harvestYield != 0 && p.plant.harvestedThingDef != null);
            ThingStuffPair plantChoosen = allP.InRandomOrder().RandomElement();

            if (!this.TryFindRootCell(map, out root, plantChoosen.thing))
            {
                return false;
            }
            Thing thing = null;
            int randomInRange = CountRange.RandomInRange;

            for (int i = 0; i < randomInRange; i++)
            {
                IntVec3 intVec;
                if (!CellFinder.TryRandomClosewalkCellNear(root, map, 6, out intVec, (IntVec3 x) => this.CanSpawnAt(x, map, plantChoosen.thing)))
                {
                    break;
                }
                Plant plant = intVec.GetPlant(map);
                if (plant != null)
                {
                    plant.Destroy(DestroyMode.Vanish);
                }
                Thing thing2 = GenSpawn.Spawn(plantChoosen.thing, intVec, map, WipeMode.Vanish);
                Plant plant1 = thing2 as Plant;
                plant1.Growth = 0.5f;
                if (thing == null)
                {
                    thing = thing2;
                }
            }
            if (thing == null)
            {
                return false;
            }
            string t = "CS1".Translate() + " " + thing.Label + " " +"CS2".Translate();
            Find.LetterStack.ReceiveLetter("CSLabel".Translate(), t, LetterDefOf.PositiveEvent, new TargetInfo(thing.Position, map, false), null, null);
            return true;
        }

        private bool TryFindRootCell(Map map, out IntVec3 cell, ThingDef plant)
        {
            return CellFinderLoose.TryFindRandomNotEdgeCellWith(10, (IntVec3 x) => this.CanSpawnAt(x, map, plant) && x.GetRoom(map, RegionType.Set_Passable).CellCount >= 64, map, out cell);
        }

        private bool CanSpawnAt(IntVec3 c, Map map, ThingDef plantC)
        {
            if (!c.Standable(map) || c.Fogged(map) || map.fertilityGrid.FertilityAt(c) < plantC.plant.fertilityMin || !c.GetRoom(map, RegionType.Set_Passable).PsychologicallyOutdoors || c.GetEdifice(map) != null || !PlantUtility.GrowthSeasonNow(c, map, false))
            {
                return false;
            }
            Plant plant = c.GetPlant(map);
            if (plant != null && plant.def.plant.growDays > 10f)
            {
                return false;
            }
            List<Thing> thingList = c.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i].def == plantC)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
