using System;
using Verse;

namespace VEE
{
    class HeddifCompPropreties_Traitor : HediffCompProperties
    {
        public HeddifCompPropreties_Traitor()
        {
            this.compClass = typeof(VEE.HeddifComp_Traitor);
        }

        public IntRange disappearsAfterTicks = default(IntRange);
    }
}
