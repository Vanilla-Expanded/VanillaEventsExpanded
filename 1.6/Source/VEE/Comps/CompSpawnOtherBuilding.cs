using RimWorld;
using Verse;
using System.Collections.Generic;

namespace VEE
{
    public class CompSpawnOtherBuilding : ThingComp
    {

        public Building newBuilding;


        public CompProperties_SpawnOtherBuilding Props
        {
            get
            {
                return (CompProperties_SpawnOtherBuilding)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look<Building>(ref newBuilding, "newBuilding");

        }


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (newBuilding != null && parent.Map != null)
            {
                if (!newBuilding.Destroyed)
                {
                    newBuilding.Destroy(DestroyMode.Vanish);
                }

            }
        }

        //On despawn, destroy the building

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            base.PostDeSpawn(map, mode);
            if (newBuilding != null && map != null)
            {
                if (!newBuilding.Destroyed)
                {
                    newBuilding.Destroy(DestroyMode.Vanish);
                }

            }
        }

        //On destroy, destroy the building

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {

            if (newBuilding != null && previousMap != null)
            {
                if (!newBuilding.Destroyed)
                {
                    newBuilding.Destroy(DestroyMode.Vanish);
                }

            }
        }
      
        public override void CompTickLong()
        {
            base.CompTickLong();


            if (parent.Map != null)
            {

                //Check everything at the first building position
                bool flagToSpawnBuilding = true;
                List<Thing> list = this.parent.Map.thingGrid.ThingsListAt(this.parent.Position);
                for (int i = 0; i < list.Count; i++)
                {
                    //If there is already a defOfBuildingToSpawn here, don't do anything else

                    if ((list[i] is Building) && list[i].def == Props.defOfBuildingToSpawn)
                    {
                        flagToSpawnBuilding = false;
                    }
                }
                //If there wasn't, spawn a defOfBuildingToSpawn

                if (flagToSpawnBuilding)
                {
                    Building new_Building = (Building)ThingMaker.MakeThing(Props.defOfBuildingToSpawn);
                    GenSpawn.Spawn(new_Building, this.parent.Position, this.parent.Map);
                    //Store the new building in a variable so it can be accessed to destroy it
                    newBuilding = new_Building;
                }

            }
        }
    }
}
