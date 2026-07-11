using RimWorld;
using Verse;

namespace VEE
{
    public class CompProperties_SpawnOtherBuilding : CompProperties
    {


        public CompProperties_SpawnOtherBuilding()
        {
            this.compClass = typeof(CompSpawnOtherBuilding);
        }

        public ThingDef defOfBuildingToSpawn;
       
    }
}