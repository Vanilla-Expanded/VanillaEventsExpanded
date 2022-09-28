using RimWorld.Planet;
using Verse;

namespace VEE
{
    internal class WorldComp_Purple : WorldComponent
    {
        internal int tickLastPurpleEvent = 0;

        public WorldComp_Purple(World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.tickLastPurpleEvent, "tickLastPurpleEvent", 0);
        }
    }
}