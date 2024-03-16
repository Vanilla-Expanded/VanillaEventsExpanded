using RimWorld.Planet;
using Verse;

namespace VEE
{
    public class WorldComp_Purple : WorldComponent
    {
        internal int tickLast = 0;

        public WorldComp_Purple(World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref tickLast, "tickLast");
        }
    }
}