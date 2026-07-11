using RimWorld;
using System;
using System.Collections.Generic;
using VEF.Genes;
using Verse;
using Verse.Noise;
using static RimWorld.PsychicRitualRoleDef;

namespace VEE
{
    public class DropPodSpawner : Building
    {

        protected override void Tick()
        {

            if (Map != null)
            {

                int numberOfPawns = new IntRange(1, 3).RandomInRange;
              
                for (int i = 0; i < numberOfPawns; i++)
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.SpaceRefugee);
                    HealthUtility.DamageUntilDowned(pawn, true);
                    pawn.guest.getRescuedThoughtOnUndownedBecauseOfPlayer = true;
                    pawn.apparel.WornApparel.RemoveAll((Apparel a) => a.MarketValue > 300);


                    if (i > 0 && !pawn.Dead && Rand.Chance(0.9f))
                    {
                        DamageInfo damageInfo = new DamageInfo(DamageDefOf.Bullet, 50);
                        pawn.Kill(damageInfo);
                    }

                    RCellFinder.TryFindRandomCellNearWith(Position, c => c.Walkable(Map), Map, out IntVec3 intVec);
                    if (pawn.Dead)
                    {
                        Corpse corpse = pawn.Corpse;
                        corpse.SetForbidden(true);
                        GenPlace.TryPlaceThing(corpse, intVec, Map, ThingPlaceMode.Near);
                    }
                    else {
                        if (Map.Biome.inVacuum && Rand.Chance(0.2f))
                        {
                            Apparel apparelArmor = (Apparel)ThingMaker.MakeThing(ThingDefOf.Apparel_Vacsuit);
                            Apparel apparelHelmet = (Apparel)ThingMaker.MakeThing(ThingDefOf.Apparel_VacsuitHelmet);
                            
                            pawn.apparel.Wear(apparelArmor, dropReplacedApparel: false);
                            pawn.apparel.Wear(apparelHelmet, dropReplacedApparel: false);
                        }
                        GenPlace.TryPlaceThing(pawn, intVec, Map, ThingPlaceMode.Near); 
                    
                    }
                }

                


                this.DeSpawn();




            }

        }

    }

}