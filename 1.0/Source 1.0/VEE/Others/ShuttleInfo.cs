using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.Sound;
using RimWorld;

namespace VEE
{
    class Shuttle : Building
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.t, "t", 0, false);
        }

        public override void Tick()
        {
            base.Tick();
            if(t == 250)
            {
                System.Random rnd = new System.Random();
                int nbP = rnd.Next(2, 5);

                for (int i = 0; i <= nbP; i++)
                {
                    PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, null);
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    HealthUtility.DamageUntilDowned(pawn, true);
                    pawn.guest.getRescuedThoughtOnUndownedBecauseOfPlayer = true;

                    int d = rnd.Next(1, 4);
                    if (d == 2 && !pawn.Dead)
                    {
                        DamageInfo damageInfo = new DamageInfo(DamageDefOf.Bullet, 50);
                        pawn.Kill(damageInfo);
                    }
                    pawn.apparel.WornApparel.RemoveAll((Apparel a) => a.MarketValue > 300);

                    IntVec3 intVec;
                    DropCellFinder.TryFindDropSpotNear(this.Position, this.Map, out intVec, false, true);
                    GenPlace.TryPlaceThing(pawn, intVec, this.Map, ThingPlaceMode.Near, null, null);
                }
            }
            this.t++;
        }

        int t;
    }
}
