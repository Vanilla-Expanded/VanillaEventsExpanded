using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using VEF.Buildings;
using Verse;
using Verse.Noise;

namespace VEE
{
    public class WorldComp_Purple : WorldComponent
    {
        internal int tickLast = 0;

        private int traitorLetterTickCounter = -1;

        public static WorldComp_Purple Instance;

        private HashSet<DoorTeleporter> doorTeleporters = new();
        public WorldComp_Purple(World world) : base(world) => Instance = this;
        public float cachedPlantGrowthMultiplier = 1;
        public float cachedGlobalLightLevelsMultiplier = 1;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref tickLast, "tickLast");
            Scribe_Values.Look(ref cachedPlantGrowthMultiplier, "cachedPlantGrowthMultiplier");
            Scribe_Values.Look(ref cachedGlobalLightLevelsMultiplier, "cachedGlobalLightLevelsMultiplier");

        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            if (traitorLetterTickCounter > 0)
            {
                traitorLetterTickCounter++;
                if (traitorLetterTickCounter > 1000)
                {
                    traitorLetterTickCounter = -1;
                }
            }
        }

        public void Notify_TraitorGroup(Faction faction)
        {
            if (traitorLetterTickCounter == -1)
            {
                Find.LetterStack.ReceiveLetter("VEE_TraitorGroupLabel".Translate(), "VEE_TraitorGroupDesc".Translate(faction.Name), LetterDefOf.ThreatBig);
                traitorLetterTickCounter = 0;
            }
        }
    }
}
