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
    public class Drought : GameCondition
    {
        List<Thing> allPlants;
        public override void Init()
        {
            this.SingleMap.weatherDecider.DisableRainFor(this.TicksLeft);
            this.allPlants = this.SingleMap.listerThings.AllThings.FindAll((Thing t) => t.def.plant != null && t.def.defName != "Plant_TreeAnima");
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref this.allPlants, "allPlants", LookMode.Deep);
        }

        public override void GameConditionTick()
        {
            if (this.TicksLeft % 100 == 0 && this.allPlants.Count > 0)
            {
                for (int i = 0; i < 15; i++)
                {
                    if (this.allPlants.Count > 0)
                    {
                        Thing plant = allPlants.RandomElement();
                        this.HarmPlant(this.SingleMap, plant.Position);
                        allPlants.Remove(plant);
                    }
                }
            }
        }

        private void HarmPlant(Map map, IntVec3 pos)
        {
            Plant toHarm = pos.GetPlant(map);
            if (toHarm != null)
            {
                bool flag2 = pos.GetFirstBuilding(map) != null && pos.GetFirstBuilding(map).def?.building?.sowTag != null
                            && (pos.GetFirstBuilding(map).def.building.sowTag == "Hydroponic" || pos.GetFirstBuilding(map).def.building.sowTag == "Ground");

                if (!flag2)
                {
                    System.Random r = new System.Random();
                    if(r.Next(1,3) == 1)
                    {
                        toHarm.MakeLeafless(Plant.LeaflessCause.Poison);
                    }
                    else
                    {
                        toHarm.Kill();
                    }
                }
            }
        }
    }
}
