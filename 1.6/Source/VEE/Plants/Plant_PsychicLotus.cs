using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace VEE
{
    public class Plant_PsychicLotus: Plant
    {

        public GameCondition_MultiStage cachedCondition;

        public GameCondition_MultiStage GetCondition
        {
            get
            {
                if(cachedCondition is null)
                {
                    cachedCondition = Find.World.gameConditionManager.GetActiveCondition(VEE_DefOf.VEE_PsychicBloom) as GameCondition_MultiStage;
                }
                return cachedCondition;
            }
        }

        public override void TickLong()
        {
            base.TickLong();

            if (Map?.gameConditionManager.ConditionIsActive(VEE_DefOf.VEE_PsychicBloom)==true)
            {

                if (CellFinderLoose.TryGetRandomCellWith(c =>
                {
                    if (!c.InBounds(Map) || !c.Standable(Map) || c.GetFirstBuilding(Map) != null || c.DistanceTo(this.Position)>GetCondition.CurrentStage.psychicLotusRadius)
                        return false;
                    return true;
                }, Map, 1000, out IntVec3 cell))
                {
                    Plant plant = cell.GetPlant(Map);
                    if (plant != null && !StaticCollections.plantArray.Any(x => x.plant == plant.def))
                    {                     
                        plant.Kill();
                        SpawnNewPlant(cell);
                    }
                    if (plant is null) { 
                        SpawnNewPlant(cell); 
                    }
                    
                }

            }
        }

        public void SpawnNewPlant(IntVec3 cell)
        {
            ThingDef newPlant = StaticCollections.plantArray.RandomElementByWeight(x => x.weight).plant;
            Log.Message(newPlant);
            GenSpawn.Spawn(newPlant, cell, Map, WipeMode.Vanish);
        }


    }
}
