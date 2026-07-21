using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using static HarmonyLib.Code;

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
                ThingDef newPlant = StaticCollections.plantArray.RandomElementByWeight(x => x.weight).plant;
                for(int i = 0;i< GetCondition.CurrentStage.psychicLotusSpawnAmount; i++)
                {
                    if (CellFinderLoose.TryGetRandomCellWith(c =>
                    {
                        if (!c.InBounds(Map) || !c.Standable(Map) || c.GetFirstBuilding(Map) != null || !c.GetTerrain(Map).natural || c.DistanceTo(this.Position) > GetCondition.CurrentStage.psychicLotusRadius)
                            return false;
                        return true;
                    }, Map, 1000, out IntVec3 cell))
                    {
                        Plant plant = cell.GetPlant(Map);
                        if (plant == null || !StaticCollections.plantArray.Any(x => x.plant == plant.def))
                        {
                            plant?.Kill();
                            GenSpawn.Spawn(newPlant, cell, Map, WipeMode.Vanish);
                        }
                    }
                }         
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            DeathBloom();
            base.Destroy(mode);
        }
        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            DeathBloom();
            base.DeSpawn(mode);
        }

        public void DeathBloom()
        {
            int amount = new IntRange(20, 30).RandomInRange;
            for (int i = 0; i < amount; i++)
            {
                ThingDef newPlant = StaticCollections.plantArray.RandomElementByWeight(x => x.weight).plant;
                if (CellFinderLoose.TryGetRandomCellWith(c =>
                {
                    if (!c.InBounds(Map) || !c.Standable(Map) || c.GetFirstBuilding(Map) != null || !c.GetTerrain(Map).natural ||c.DistanceTo(this.Position) > GetCondition.CurrentStage.psychicLotusRadius)
                        return false;
                    return true;
                }, Map, 1000, out IntVec3 cell))
                {
                    Plant plant = cell.GetPlant(Map);
                    if (plant == null || !StaticCollections.plantArray.Any(x => x.plant == plant.def))
                    {
                        plant?.Kill();
                        GenSpawn.Spawn(newPlant, cell, Map, WipeMode.Vanish);
                    }
                }
            }
            SoundDefOf.PsychicPulseGlobal.PlayOneShot(new TargetInfo(Position, Map, false));
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(Position.ToVector3(), Map, VEE_DefOf.PsycastPsychicEffect, 3);        
            this.Map.flecks.CreateFleck(dataStatic);
           
        }
    }
}
