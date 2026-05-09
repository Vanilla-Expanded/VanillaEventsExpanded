using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace VEE.RegularEvents
{
    [StaticConstructorOnStartup]
    public class SpaceBattle : GameCondition
    {
        private IntVec3 aroundThis = new IntVec3();
        private int delay = 0;

        public override bool AllowEnjoyableOutsideNow(Map map) => false;

        public override void Init()
        {
            TraderKindDef traderKind = DefDatabase<TraderKindDef>.AllDefs.Where((TraderKindDef x) => CanSpawn(SingleMap, x))?.RandomElementByWeight((TraderKindDef traderDef) => traderDef.CalculatedCommonality);
            if (traderKind != null)
            {
                TradeShip tradeShip = new TradeShip(traderKind, GetFaction(traderKind));

                RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 reach, SingleMap, 1f, false, c => c.Walkable(SingleMap));
                RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(reach, SingleMap, 50f, out aroundThis);
                Find.LetterStack.ReceiveLetter(LetterMaker.MakeLetter("SpaceBattleLabel".Translate(), "SpaceBattle".Translate(tradeShip.name), LetterDefOf.ThreatBig, new LookTargets(aroundThis, SingleMap)));

            }
        }

        private bool CanSpawn(Map map, TraderKindDef trader)
        {
            if (!trader.orbital)
            {
                return false;
            }
            if (trader.faction == null)
            {
                return true;
            }
            Faction faction = GetFaction(trader);
            if (faction == null)
            {
                return false;
            }
            foreach (Pawn freeColonist in map.mapPawns.FreeColonists)
            {
                if (freeColonist.CanTradeWith(faction, trader).Accepted)
                {
                    return true;
                }
            }
            return false;
        }

        private Faction GetFaction(TraderKindDef trader)
        {
            if (trader.faction == null)
            {
                return null;
            }
            if (!Find.FactionManager.AllFactions.Where((Faction f) => f.def == trader.faction).TryRandomElement(out var result))
            {
                return null;
            }
            return result;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref aroundThis, "aroundThis");

          
        }

       
        public override void GameConditionTick()
        {
            Map map = SingleMap;
            
            delay++;

            if (delay % 500 == 0)
            {
                int randomInstances = new IntRange(1, 2).RandomInRange;
                for (int i = 0; i < randomInstances; i++)
                {
                    IntVec3 pos = new IntVec3();
                    VEETryFindSkyfallerCell(VEE_DefOf.VEE_ShipChunkHumanIncoming_Volatile, aroundThis, map, 30, out pos);
                    Skyfaller sk1 = SkyfallerMaker.SpawnSkyfaller(VEE_DefOf.VEE_ShipChunkHumanIncoming_Volatile, VEE_DefOf.VEE_ShipChunkHuman_Volatile_Spawner, pos, map);
                }
            }
            /*if (delay % 1200 == 0)
            {
                int randomInstances = new IntRange(1, 2).RandomInRange;
                for (int i = 0; i < randomInstances; i++)
                {
                    IntVec3 intVec = new IntVec3();
                    VEETryFindSkyfallerCell(ThingDefOf.DropPodIncoming, aroundThis, map, 30, out intVec);
                    PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, null);
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    DamageInfo damageInfo = new DamageInfo(DamageDefOf.Bullet, 1);
                   
                    if (Rand.Chance(0.9f))
                    {
                        pawn.Kill(damageInfo);
                    }
                    else
                    {
                        HealthUtility.DamageUntilDowned(pawn, true);
                    }
                    pawn.apparel.WornApparel.RemoveAll((Apparel a) => a.MarketValue > 400);
                    List<Thing> list = new List<Thing>
                    {
                        pawn
                    };
                    ChangeDeadPawnsToTheirCorpses(list);
                    DropPodUtility.DropThingsNear(intVec, map, list, 1, false, true, true);
                }
            }*/
            if (delay % 2000 == 0)
            {
                IntVec3 pos = new IntVec3();
                VEETryFindSkyfallerCell(ThingDefOf.ShipChunkIncoming, aroundThis, map, 35, out pos);
                Skyfaller sk2 = SkyfallerMaker.SpawnSkyfaller(ThingDefOf.ShipChunkIncoming, VEE_DefOf.VEE_ShipChunkHuman, pos, map);
            }

         
        }

      

       

        private void ChangeDeadPawnsToTheirCorpses(List<Thing> things)
        {
            for (int i = 0; i < things.Count; i++)
            {
                if (things[i].ParentHolder is Corpse corpse)
                {
                    things[i] = corpse;
                }
            }
        }

        public bool VEETryFindSkyfallerCell(ThingDef skyfaller, IntVec3 nearLoc, Map map, int maxDist, out IntVec3 pos)
        {
            return CellFinderLoose.TryFindSkyfallerCell(skyfaller, map,TerrainAffordanceDefOf.Walkable, out pos, 10, nearLoc, maxDist, false, true, true, true, false, false, c => c.InBounds(map) && !c.Fogged(map) && c.Walkable(map));
        }

       
    }
}