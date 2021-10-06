using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VEE
{
    internal class WorldComp_Purple : WorldComponent
    {
        internal int tickLastPurpleEvent = 0;

        public WorldComp_Purple(World world) : base(world) { }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.tickLastPurpleEvent, "tickLastPurpleEvent", 0);
        }
    }
}
