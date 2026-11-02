using RimWorld;
using System;
using System.Collections.Generic;
using VEF.Genes;
using Verse;
using Verse.Noise;
using static RimWorld.PsychicRitualRoleDef;

namespace VEE
{
    public class VolatileChunkSpawner : Building
    {
       
        protected override void Tick()
        {
            
            if (Map != null)
            {
               
                    int chemFuelPuddles = new IntRange(2, 6).RandomInRange;
                    int steelSlag = new IntRange(2, 3).RandomInRange;

                    for (int i = 0; i <= chemFuelPuddles; i++)
                    {
                        RCellFinder.TryFindRandomCellNearWith(Position, c => c.Walkable(Map), Map, out IntVec3 intVec);
                        FilthMaker.TryMakeFilth(intVec, Map, ThingDefOf.Filth_Fuel, 1);
                    }

                    for (int i = 0; i <= steelSlag; i++)
                    {
                        RCellFinder.TryFindRandomCellNearWith(Position, c => c.Walkable(Map), Map, out IntVec3 intVec);
                        Thing item = ThingMaker.MakeThing(ThingDefOf.ChunkSlagSteel);
                        GenPlace.TryPlaceThing(item, intVec, Map, ThingPlaceMode.Near, squareRadius: 2);
                    }


                    this.DeSpawn();



                
            }
            
        }
       
    }

}