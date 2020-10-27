using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VEE
{
    class DummySpaceBattle : Building
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.t, "t", 0, false);
            Scribe_Values.Look<int>(ref this.s, "s", 0, false);
            Scribe_Values.Look<int>(ref this.i, "i", 1, false);
            Scribe_Values.Look<int>(ref this.n, "n", 6, false);
        }

        public override void Tick()
        {
            base.Tick();
            System.Random rnd = new System.Random();
            if (t == 0)
            {
                this.delay = rnd.Next(90, 210);
                this.n = rnd.Next(8, 15);
            }
            if (t == this.delay)
            {
                if (i <= this.n)
                {
                    IntVec3 p;
                    CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.ShipChunkIncoming, this.Map, out p, 10, this.Position, 6, true, false, false, false, true, false, null);
                    CreateRandomExplosion(p, this.Map);
                    StartRandomFire(p, this.Map);
                    this.s = rnd.Next(25, 40);
                    this.delay = this.s;
                    this.t = 1;
                    this.i++;
                }
                else
                {
                    this.DeSpawn();
                }
            }
            this.t++;
        }

        public void CreateRandomExplosion(IntVec3 pos, Map map)
        {
            IntVec3 intVec = (from x in GenRadial.RadialCellsAround(pos, 15f, true)
                              where x.InBounds(map)
                              select x).RandomElementByWeight((IntVec3 x) => this.DistanceChanceFactor.Evaluate(x.DistanceTo(pos)));
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
                         select x).RandomElementByWeight((IntVec3 x) => this.DistanceChanceFactor.Evaluate(x.DistanceTo(pos)));
            FireUtility.TryStartFireIn(c, map, Rand.Range(0.1f, 0.925f));
        }

        public readonly SimpleCurve DistanceChanceFactor = new SimpleCurve
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

        int t;
        int i = 1;
        int s = 0;
        int n = 6;
        int delay;
    }
}
