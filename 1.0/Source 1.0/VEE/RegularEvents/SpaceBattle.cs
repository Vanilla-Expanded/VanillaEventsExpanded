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
    class SpaceBattle : GameCondition
    {
        IntVec3 aroundThis = new IntVec3();
        int delay = 0;

        public override void Init()
        {
            TryFindShipChunkDropCell(this.SingleMap.Center, this.SingleMap, 999, out aroundThis);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref aroundThis, "aroundThis");
        }

        public override void GameConditionTick()
        {
            Map map = this.SingleMap;
            System.Random r = new System.Random();
            delay++;

            if (delay % 500 == 0)
            {
                for (int i = 0; i < r.Next(1, 2); i++)
                {
                    IntVec3 pos = new IntVec3();
                    TryFindShipChunkDropCell(aroundThis, map, 60, out pos);
                    Skyfaller sk1 = SkyfallerMaker.SpawnSkyfaller(VEE_DefOf.SlagIncoming, ThingDefOf.ChunkSlagSteel, pos, map);
                }
            }
            if (delay % 1200 == 0)
            {
                for (int i = 0; i < r.Next(1, 2); i++)
                {
                    IntVec3 intVec = new IntVec3();
                    TryFindShipChunkDropCell(aroundThis, map, 40, out intVec);
                    PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, null);
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    DamageInfo damageInfo = new DamageInfo(DamageDefOf.Bullet, 1);
                    pawn.Kill(damageInfo);
                    pawn.apparel.WornApparel.RemoveAll((Apparel a) => a.MarketValue > 300);
                    List<Thing> list = new List<Thing>();
                    pawn.apparel.WornApparel.RemoveAll((Apparel a) => a.MarketValue > 300);
                    list.Add(pawn);
                    ChangeDeadPawnsToTheirCorpses(list);
                    DropPodUtility.DropThingsNear(intVec, map, list, 1, false, true, true);
                    
                }
            }
            if (delay % 1500 == 0)
            {
                for (int i = 0; i < r.Next(1,2); i++)
                {
                    IntVec3 pos = new IntVec3();
                    TryFindShipChunkDropCell(aroundThis, map, 40, out pos);
                    Skyfaller sk2 = SkyfallerMaker.SpawnSkyfaller(ThingDefOf.ShipChunkIncoming, VEE_DefOf.VEE_ShipChunkHuman, pos, map);
                }
            }
            if (delay % 900 == 0)
            {
                for (int i = 0; i < r.Next(1, 4); i++)
                {
                    float radius = (float)Rand.Range(5, 11);
                    DamageDef bomb = DamageDefOf.Bomb;
                    IntVec3 pos2 = new IntVec3();
                    TryFindShipChunkDropCell(aroundThis, map, 40, out pos2);
                    GenExplosion.DoExplosion(pos2, map, radius, bomb, null);
                    this.StartRandomFire(pos2, map);
                }
            }
        }

        private void ChangeDeadPawnsToTheirCorpses(List<Thing> things)
        {
            for (int i = 0; i < things.Count; i++)
            {
                if (things[i].ParentHolder is Corpse)
                {
                    things[i] = (Corpse)things[i].ParentHolder;
                }
            }
        }

        public bool TryFindShipChunkDropCell(IntVec3 nearLoc, Map map, int maxDist, out IntVec3 pos)
        {
            ThingDef shipChunkIncoming = ThingDefOf.ShipChunkIncoming;
            return CellFinderLoose.TryFindSkyfallerCell(shipChunkIncoming, map, out pos, 10, nearLoc, maxDist, true, false, false, false, true, false, null);
        }

        public void StartRandomFire(IntVec3 pos, Map map)
        {
            IntVec3 c = (from x in GenRadial.RadialCellsAround(pos, 25f, true)
                         where x.InBounds(map)
                         select x).RandomElementByWeight((IntVec3 x) => SpaceBattle.DistanceChanceFactor.Evaluate(x.DistanceTo(pos)));
            FireUtility.TryStartFireIn(c, map, Rand.Range(0.1f, 0.925f));
        }

        private Pawn FindPawn(List<Thing> things)
        {
            for (int i = 0; i < things.Count; i++)
            {
                Pawn pawn = things[i] as Pawn;
                if (pawn != null)
                {
                    return pawn;
                }
                Corpse corpse = things[i] as Corpse;
                if (corpse != null)
                {
                    return corpse.InnerPawn;
                }
            }
            return null;
        }

        public static readonly SimpleCurve DistanceChanceFactor = new SimpleCurve
        {
            {
                new CurvePoint(0f, 1f),
                true
            },
            {
                new CurvePoint(15f, 0.1f),
                true
            }
        };
    }
}
