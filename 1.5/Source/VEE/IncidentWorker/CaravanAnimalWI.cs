using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace VEE.RegularEvents
{
    public class CaravanAnimalWI : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return !map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout) && TryFindEntryCell(map, out IntVec3 intVec) && PickPawnKindDef(map).Count > 0;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!TryFindEntryCell(map, out IntVec3 intVec))
            {
                return false;
            }

            IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10, null);
            List<Pawn> group = GenerateGroup(map);
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
            PawnKindDef pawnKind = PickPawnKindDef(map).RandomElement();
            System.Random r = new System.Random();
            int num = r.Next(1, 4);

            ThingSetMakerParams parms = default(ThingSetMakerParams);
            parms.traderDef = DefDatabase<TraderKindDef>.AllDefsListForReading.RandomElement();
            parms.makingFaction = Find.FactionManager.RandomNonHostileFaction(false, false, false);
            parms.tile = map.Tile;
            parms.totalMarketValueRange = new FloatRange(500f, 4000f);
            parms.maxTotalMass = (pawnKind.RaceProps.baseBodySize * 35f) * num;

            List<Thing> wares = ThingSetMakerDefOf.TraderStock.root.Generate(parms).ToList<Thing>();
            wares.RemoveAll(t => t is Pawn || t.MarketValue > 190 || t.TryGetComp<CompRottable>() != null || (t is MinifiedThing tm && tm != null));

            List<Pawn> pawns = new List<Pawn>();
            int i = 0;

            while (wares.Count > num * 5)
            {
                wares.Shuffle();
                wares.RemoveLast();
            }

            for (int j = 0; j < num; j++)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKind, null, PawnGenerationContext.NonPlayer));
                pawns.Add(pawn);
            }
            while (i < wares.Count)
            {
                wares[i].stackCount = Mathf.Clamp(wares[i].stackCount, 1, r.Next(2, 20));
                pawns.RandomElement().inventory.innerContainer.TryAdd(wares[i], true);
                i++;
            }

            return pawns;
            /* Old version
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
            return pawnsList; */
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
            int listValue = CalculateWealth(list);
            int listWeight = CalculateWeight(list);
            while (listValue > maxValue)
            {
                if (list.Count < 2)
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

                listValue = CalculateWealth(list);
                listWeight = CalculateWeight(list);
            }

            while (listWeight > carryCap)
            {
                if (list.Last().stackCount > 10)
                {
                    list.Last().stackCount -= 10;
                }
                else
                {
                    if (list.Count > 1)
                    {
                        list.Pop();
                    }
                    else
                    {
                        list.Last().stackCount = 1;
                        listWeight = 0;
                    }
                }
                listWeight = CalculateWeight(list);
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