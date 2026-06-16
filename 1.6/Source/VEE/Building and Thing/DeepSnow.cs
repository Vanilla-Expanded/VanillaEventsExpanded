using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VEE
{
    public class DeepSnow : Mineable
    {

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            try
            {
                WeatherBuildupUtility.AddSnowRadial(this.Position, Find.CurrentMap, 1f, 1f);
            }
            catch (Exception) { }


            base.Destroy(mode);
        }
    }
}
