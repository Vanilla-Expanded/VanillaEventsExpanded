using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Noise;

namespace VEE
{
    public class WorldComp_Purple : WorldComponent
    {
        internal int tickLast = 0;

        private int traitorLetterTickCounter = -1;

        public WorldComp_Purple(World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref tickLast, "tickLast");
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
