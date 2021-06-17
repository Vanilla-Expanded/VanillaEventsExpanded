﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VEE.RegularEvents
{
    [StaticConstructorOnStartup]
    public class SpaceBattle : GameCondition
    {
        IntVec3 aroundThis = new IntVec3();
        int delay = 0;

        public override void Init()
        {
            RCellFinder.TryFindRandomExitSpot(this.SingleMap.mapPawns.AllPawns.RandomElement(), out IntVec3 fallPos);
            RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(fallPos, this.SingleMap, 50f, out aroundThis);
            Find.LetterStack.ReceiveLetter(LetterMaker.MakeLetter("SpaceBattleLabel".Translate(), "SpaceBattle".Translate(), LetterDefOf.NegativeEvent, new LookTargets(aroundThis, SingleMap)));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref aroundThis, "aroundThis");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (!this.nextExplosionCell.IsValid)
                {
                    this.GetNextExplosionCell();
                }
                if (this.projectiles == null)
                {
                    this.projectiles = new List<Bombardment.BombardmentProjectile>();
                }
            }
        }

        private int bombIntervalTicks = 1500;
        private int ticksToNextEffect;
        private IntVec3 nextExplosionCell = new IntVec3();
        private List<Bombardment.BombardmentProjectile> projectiles = new List<Bombardment.BombardmentProjectile>();

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
                    VEETryFindSkyfallerCell(VEE_DefOf.SlagIncoming, aroundThis, map, 50, out pos);
                    Skyfaller sk1 = SkyfallerMaker.SpawnSkyfaller(VEE_DefOf.SlagIncoming, ThingDefOf.ChunkSlagSteel, pos, map);
                }
            }
            if (delay % 1200 == 0)
            {
                for (int i = 0; i < r.Next(1, 2); i++)
                {
                    IntVec3 intVec = new IntVec3();
                    VEETryFindSkyfallerCell(ThingDefOf.DropPodIncoming, aroundThis, map, 60, out intVec);
                    PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, null);
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    DamageInfo damageInfo = new DamageInfo(DamageDefOf.Bullet, 1);
                    System.Random random = new System.Random();
                    if (random.Next(0, 101) > 25)
                    {
                        pawn.Kill(damageInfo);
                    }
                    else
                    {
                        HealthUtility.DamageUntilDowned(pawn, true);
                    }
                    pawn.apparel.WornApparel.RemoveAll((Apparel a) => a.MarketValue > 400);
                    List<Thing> list = new List<Thing>();
                    list.Add(pawn);
                    ChangeDeadPawnsToTheirCorpses(list);
                    DropPodUtility.DropThingsNear(intVec, map, list, 1, false, true, true);

                }
            }
            if (delay % 2000 == 0)
            {
                IntVec3 pos = new IntVec3();
                VEETryFindSkyfallerCell(ThingDefOf.ShipChunkIncoming, aroundThis, map, 35, out pos);
                Skyfaller sk2 = SkyfallerMaker.SpawnSkyfaller(ThingDefOf.ShipChunkIncoming, VEE_DefOf.VEE_ShipChunkHuman, pos, map);
            }

            // Explosion handle

            if (!this.nextExplosionCell.IsValid)
            {
                this.ticksToNextEffect = this.bombIntervalTicks;
                this.GetNextExplosionCell();
            }
            this.ticksToNextEffect--;
            if (this.ticksToNextEffect <= 0 && base.TicksLeft >= this.bombIntervalTicks)
            {
                SoundDefOf.Bombardment_PreImpact.PlayOneShot(new TargetInfo(this.nextExplosionCell, map, false));
                this.projectiles.Add(new Bombardment.BombardmentProjectile(200, this.nextExplosionCell));
                this.ticksToNextEffect = this.bombIntervalTicks;
                this.GetNextExplosionCell();
            }
            for (int i = this.projectiles.Count - 1; i >= 0; i--)
            {
                this.projectiles[i].Tick();
                this.Draw();
                if (this.projectiles[i].LifeTime <= 0)
                {
                    IntVec3 targetCell = this.projectiles[i].targetCell;
                    DamageDef bomb = Rand.Range(1, 10) > 2 ? DamageDefOf.Bomb : DamageDefOf.Flame;
                    GenExplosion.DoExplosion(targetCell, map, Rand.Range(3f, 6f), bomb, null);
                    this.projectiles.RemoveAt(i);
                }
            }
        }

        private void Draw()
        {
            if (this.projectiles.NullOrEmpty<Bombardment.BombardmentProjectile>())
            {
                return;
            }
            for (int i = 0; i < this.projectiles.Count; i++)
            {
                this.projectiles[i].Draw(ProjectileMaterial);
            }
        }

        private void GetNextExplosionCell()
        {
            this.nextExplosionCell = (from x in GenRadial.RadialCellsAround(this.aroundThis, 30, true)
                                      where x.InBounds(this.SingleMap) && !x.Fogged(SingleMap) && !x.Roofed(SingleMap)
                                      select x).RandomElementByWeight((IntVec3 x) => Bombardment.DistanceChanceFactor.Evaluate(x.DistanceTo(this.aroundThis)));
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

        public bool VEETryFindSkyfallerCell(ThingDef skyfaller, IntVec3 nearLoc, Map map, int maxDist, out IntVec3 pos)
        {
            return CellFinderLoose.TryFindSkyfallerCell(skyfaller, map, out pos, 10, nearLoc, maxDist, false, false, false, true, true, true, null);
        }

        public void StartRandomFire(IntVec3 pos, Map map)
        {
            IntVec3 c = (from x in GenRadial.RadialCellsAround(pos, 25f, true)
                         where x.InBounds(map)
                         select x).RandomElementByWeight((IntVec3 x) => SpaceBattle.DistanceChanceFactor.Evaluate(x.DistanceTo(pos)));
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

        private static readonly Material ProjectileMaterial = MaterialPool.MatFrom("Things/Projectile/Bullet_Big", ShaderDatabase.Transparent, Color.white);
    }
}
