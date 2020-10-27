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
    class SpaceBattle2 : IncidentWorker
    {
        private int RandomCountToDrop
        {
            get
            {
                float x2 = (float)Find.TickManager.TicksGame / 3600000f;
                float timePassedFactor = Mathf.Clamp(GenMath.LerpDouble(0f, 1.2f, 1f, 0.1f, x2), 0.1f, 1f);
                return SpaceBattle2.CountChance.RandomElementByWeight(delegate (Pair<int, float> x)
                {
                    if (x.First == 1)
                    {
                        return x.Second;
                    }
                    return x.Second * timePassedFactor;
                }).First;
            }
        }
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            IntVec3 intVec;
            return this.TryFindShipChunkDropCell(map.Center, map, 999999, out intVec);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 intVec;
            if (!this.TryFindShipChunkDropCell(map.Center, map, 999999, out intVec))
            {
                return false;
            }
            this.SpawnShipChunks(intVec, map, this.RandomCountToDrop);

            string label = "SpaceBattleLabel".Translate();
            string text = "SpaceBattle".Translate();

            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, new TargetInfo(intVec, map, false), null, null);

            this.SpawnBombardement(map.Center, map);
            return true;
        }

        public void SpawnBombardement(IntVec3 pos, Map map)
        {
            System.Random rnd = new System.Random();
            int nb = rnd.Next(1, 5);

            for (int i = 0; i <= nb; i++)
            {
                CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.ShipChunkIncoming, map, out pos, 10, pos, 99999, true, false, false, false, true, false, null);
                GenSpawn.Spawn(VEE_DefOf.VEE_Dummy, pos, map);
                /*MoteMaker.MakeBombardmentMote(pos, map);
                for (int a = 0; a < 5; a++)
                {
                    IntVec3 p;
                    CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.ShipChunkIncoming, map, out p, 10, pos, 6, true, false, false, false, true, false, null);
                    CreateRandomExplosion(p, map);
                    StartRandomFire(p, map);
                }*/
            }
        }

        public void CreateRandomExplosion(IntVec3 pos, Map map)
        {
            IntVec3 intVec = (from x in GenRadial.RadialCellsAround(pos, 15f, true)
                              where x.InBounds(map)
                              select x).RandomElementByWeight((IntVec3 x) => SpaceBattle2.DistanceChanceFactor.Evaluate(x.DistanceTo(pos)));
            float num = (float)Rand.Range(6, 8);
            IntVec3 center = intVec;
            
            float radius = num;
            DamageDef bomb = DamageDefOf.Bomb;
            GenExplosion.DoExplosion(center, map, radius, bomb, null);
        }

        public void StartRandomFire(IntVec3 pos, Map map)
        {
            IntVec3 c = (from x in GenRadial.RadialCellsAround(pos, 25f, true)
                         where x.InBounds(map)
                         select x).RandomElementByWeight((IntVec3 x) => SpaceBattle2.DistanceChanceFactor.Evaluate(x.DistanceTo(pos)));
            FireUtility.TryStartFireIn(c, map, Rand.Range(0.1f, 0.925f));
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

        public void SpawnShipChunks(IntVec3 firstChunkPos, Map map, int count)
        {
            this.SpawnChunk(firstChunkPos, map);
            for (int i = 0; i < count - 1; i++)
            {
                IntVec3 pos;
                if (this.TryFindShipChunkDropCell(firstChunkPos, map, 30, out pos))
                {
                    this.SpawnChunk(pos, map);
                }
            }
        }

        public void SpawnChunk(IntVec3 pos, Map map)
        {
            SkyfallerMaker.SpawnSkyfaller(ThingDefOf.ShipChunkIncoming, VEE_DefOf.VEE_ShipChunkHuman, pos, map);
        }

        public bool TryFindShipChunkDropCell(IntVec3 nearLoc, Map map, int maxDist, out IntVec3 pos)
        {
            ThingDef shipChunkIncoming = ThingDefOf.ShipChunkIncoming;
            return CellFinderLoose.TryFindSkyfallerCell(shipChunkIncoming, map, out pos, 10, nearLoc, maxDist, true, false, false, false, true, false, null);
        }

        private static readonly Pair<int, float>[] CountChance = new Pair<int, float>[]
        {
            new Pair<int, float>(8, 1f),
            new Pair<int, float>(10, 0.85f),
            new Pair<int, float>(12, 0.7f),
            new Pair<int, float>(14, 0.4f)
        };
    }
}
