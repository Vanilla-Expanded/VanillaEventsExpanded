using Verse;

namespace VEE
{
    internal class HeddifCompPropreties_Traitor : HediffCompProperties
    {
        public HeddifCompPropreties_Traitor()
        {
            this.compClass = typeof(VEE.HeddifComp_Traitor);
        }

        public IntRange disappearsAfterTicks = default(IntRange);
    }
}