using RimWorld;
using System;
using System.Collections.Generic;
using VEF.Genes;
using Verse;
using Verse.Noise;

namespace VEE
{
    public class Shuttle : Building
    {
       

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if (Map != null && !respawningAfterLoad)
            {

                int numberOfPawns = new IntRange(1, 3).RandomInRange;
                int chemFuelPuddles = new IntRange(4, 10).RandomInRange;
                int totalScatterablesValue = Math.Min(2500, (int)(Map.wealthWatcher.WealthTotal * 0.01f));

                for (int i = 0; i < numberOfPawns; i++)
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.SpaceRefugee);
                    HealthUtility.DamageUntilDowned(pawn, true);
                    pawn.guest.getRescuedThoughtOnUndownedBecauseOfPlayer = true;
                    pawn.apparel.WornApparel.RemoveAll((Apparel a) => a.MarketValue > 300);


                    if (i > 0 && !pawn.Dead && Rand.Chance(0.75f))
                    {
                        DamageInfo damageInfo = new DamageInfo(DamageDefOf.Bullet, 50);
                        pawn.Kill(damageInfo);
                    }

                    RCellFinder.TryFindRandomCellNearWith(Position, c => c.Walkable(Map), Map, out IntVec3 intVec);
                    if (pawn.Dead) GenPlace.TryPlaceThing(pawn.Corpse, intVec, Map, ThingPlaceMode.Near);
                    else GenPlace.TryPlaceThing(pawn, intVec, Map, ThingPlaceMode.Near);
                }

                for (int i = 0; i <= chemFuelPuddles; i++)
                {

                    RCellFinder.TryFindRandomCellNearWith(Position, c => c.Walkable(Map), Map, out IntVec3 intVec);
                    FilthMaker.TryMakeFilth(intVec, Map, ThingDefOf.Filth_Fuel, 1);
                }

                ThingSetMakerParams parms = default(ThingSetMakerParams);

                parms.totalMarketValueRange = new FloatRange(totalScatterablesValue, totalScatterablesValue);

                List<Thing> list2 = VEE_DefOf.VEE_ShuttleCrash_Resources.root.Generate(parms);

                if (list2 != null)
                {

                    foreach (Thing thing in list2)
                    {
                        RCellFinder.TryFindRandomCellNearWith(Position, c => c.Walkable(Map), Map, out IntVec3 intVec);
                        GenPlace.TryPlaceThing(thing, intVec, Map, ThingPlaceMode.Near, squareRadius: 6);
                    }

                }




            }

        }
    }
    
}