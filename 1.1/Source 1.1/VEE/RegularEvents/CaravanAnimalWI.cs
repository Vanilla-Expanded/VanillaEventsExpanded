using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace VEE.RegularEvents
{
    class CaravanAnimalWI : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 intVec;
            return !map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout) && this.TryFindEntryCell(map, out intVec) && this.PickPawnKindDef(map).Count > 0;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 intVec;
            if (!this.TryFindEntryCell(map, out intVec))
            {
                return false;
            }
            
            IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10, null);
            List<Pawn> group = this.GenerateGroup(map);
            Pawn pawn = new Pawn();
            foreach (Pawn p in group)
            {
                GenSpawn.Spawn(p, loc, map, Rot4.Random, WipeMode.Vanish, false);
                p.mindState.exitMapAfterTick = Find.TickManager.TicksGame + 100000;
                pawn = p;
            }

            Find.LetterStack.ReceiveLetter("CAWILabel".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN"), "CAWI".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN"), LetterDefOf.PositiveEvent, pawn, null, null);
            return true;
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return RCellFinder.TryFindRandomPawnEntryCell(out cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f, false, null);
        }

        private List<Pawn> GenerateGroup(Map map)
        {
            System.Random r = new System.Random();
            Pawn pawn = null;
            List<Pawn> pawnsList = new List<Pawn>();

            // Choose pawnkind and number of them
            PawnKindDef kindDef = this.PickPawnKindDef(map).RandomElement();
            int numberOfAnimal = r.Next(1, 3);

            // Wealth Manager
            int pawnsValue = (int)kindDef.race.BaseMarketValue * (numberOfAnimal / 2);
            int maxValueTotal = (int)(map.wealthWatcher.WealthTotal * 0.01f);
            int leftForThings = maxValueTotal - pawnsValue;

            // Generate each pawn and choose random inventory for each
            for (int i = 0; i <= numberOfAnimal; i++)
            {
                pawn = PawnGenerator.GeneratePawn(kindDef, null);
                List<Thing> toAdd = this.GenerateInventory(kindDef, (int)(leftForThings / numberOfAnimal));
                // Add items to inner inventory
                foreach (Thing thing in toAdd)
                {
                    pawn.inventory.innerContainer.TryAdd(thing);
                }
                // Add the new pawn to list
                pawnsList.Add(pawn);
            }

            // Return list of pawn to spawn
            return pawnsList;
        }

        private List<PawnKindDef> PickPawnKindDef(Map map)
        {
            List<PawnKindDef> Allanimals = new List<PawnKindDef>();
            List<PawnKindDef> allPawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading;
            for (int i = 0; i < allPawnKindDefs.Count; i++)
            {
                if (allPawnKindDefs[i].race.race.Animal && allPawnKindDefs[i].race.race.packAnimal)
                {
                    float outdoorTemp = map.mapTemperature.OutdoorTemp;
                    if (outdoorTemp > allPawnKindDefs[i].race.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin, null) && outdoorTemp < allPawnKindDefs[i].race.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax, null) && !allPawnKindDefs[i].defName.Contains("GR_"))
                    {
                        Allanimals.Add(allPawnKindDefs[i]);
                    }
                }
            }
            return Allanimals;
        }

        private List<Thing> GenerateInventory(PawnKindDef kindDef, int maxValue)
        {
            int carryCap = (int)(30 * kindDef.race.race.baseBodySize * 0.45f);
            // Traders stocks base
            ThingSetMakerParams parms = default;
            parms.traderDef = DefDatabase<TraderKindDef>.AllDefs.RandomElementByWeight((TraderKindDef t) => t.commonality);
            List<Thing> list = ThingSetMakerDefOf.TraderStock.root.Generate(parms);

            // Things removal
            list.RemoveAll((Thing thing) => thing is Pawn || thing is MinifiedThing || thing.def.defName == "Silver"); // No pawn nor minified buildings
            list.Shuffle(); // Randomise order so we don't always have the same items
            int listValue = this.CalculateWealth(list);
            int listWeight = this.CalculateWeight(list);
            while (listValue > maxValue)
            {
                if(list.Count < 2)
                {
                    return list;
                }

                if (list.Last().stackCount > 10)
                {
                    list.Last().stackCount -= 10;
                }
                else
                {
                    list.Pop();
                }
                
                listValue = this.CalculateWealth(list);
                listWeight = this.CalculateWeight(list);
            }

            while (listWeight > carryCap)
            {
                if (list.Last().stackCount > 10)
                {
                    list.Last().stackCount -= 10;
                }
                else
                {
                    if(list.Count > 1)
                    {
                        list.Pop();
                    }
                    else
                    {
                        list.Last().stackCount = 1;
                        listWeight = 0;
                    }
                }
                listWeight = this.CalculateWeight(list);
            }
            
            // Return the thing list
            return list;
        }

        private int CalculateWealth(List<Thing> things)
        {
            float wealth = 0;
            foreach (Thing thing in things)
            {
                wealth += thing.MarketValue * thing.stackCount;
            }
            return (int)wealth;
        }

        private int CalculateWeight(List<Thing> things)
        {
            float weight = 0;
            foreach (Thing thing in things)
            {
                weight += thing.def.BaseMass * thing.stackCount;
            }
            return (int)weight;
        }
    }
}
