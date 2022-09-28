﻿using RimWorld;
using Verse;
using Random = UnityEngine.Random;

namespace VEE
{
    internal class Shuttle : Building
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.t, "t", 0, false);
        }

        public override void Tick()
        {
            base.Tick();
            if (t == 250)
            {
                int nbP = Random.Range(2, 5);

                for (int i = 0; i <= nbP; i++)
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.SpaceRefugee);
                    HealthUtility.DamageUntilDowned(pawn, true);
                    pawn.guest.getRescuedThoughtOnUndownedBecauseOfPlayer = true;
                    pawn.apparel.WornApparel.RemoveAll((Apparel a) => a.MarketValue > 300);

                    if (Random.Range(1, 4) == 2 && !pawn.Dead)
                    {
                        DamageInfo damageInfo = new DamageInfo(DamageDefOf.Bullet, 50);
                        pawn.Kill(damageInfo);
                    }

                    RCellFinder.TryFindRandomCellNearWith(this.Position, c => c.Walkable(this.Map), this.Map, out IntVec3 intVec);
                    if (pawn.Dead) GenPlace.TryPlaceThing(pawn.Corpse, intVec, this.Map, ThingPlaceMode.Near);
                    else GenPlace.TryPlaceThing(pawn, intVec, this.Map, ThingPlaceMode.Near);
                }
            }
            this.t++;
        }

        private int t;
    }
}