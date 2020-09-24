using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace VEE.Settings
{
    internal class WorldComp : WorldComponent
    {
        public WorldComp(World world) : base(world)
        {
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            Log.Message("Vanilla Expanded - Settings loaded");
            VEE_ModSettings.ChangeDefPost();
        }
    }
}
