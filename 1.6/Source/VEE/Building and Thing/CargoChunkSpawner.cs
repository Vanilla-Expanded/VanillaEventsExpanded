using RimWorld;
using System;
using System.Collections.Generic;
using VEF.Genes;
using Verse;
using Verse.Noise;
using static RimWorld.PsychicRitualRoleDef;

namespace VEE
{
    public class CargoChunkSpawner : Building
    {

        public bool tickOnceOnly = false;
        public TraderKindDef traderKind;

        protected override void Tick()
        {

            if (Map != null && !tickOnceOnly)
            {

                

                tickOnceOnly = true;


            }

        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref tickOnceOnly, "tickOnceOnly", false);
        }

        

    }

}