using Verse;

namespace VEE
{
    public class HediffCompPropreties_Traitor : HediffCompProperties
    {
        public HediffCompPropreties_Traitor()
        {
            compClass = typeof(HediffComp_Traitor);
        }

        public IntRange disappearsAfterTicks = default;
    }
}