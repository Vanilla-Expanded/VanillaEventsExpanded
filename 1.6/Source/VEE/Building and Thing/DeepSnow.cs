using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

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

        public override void TickLong()
        {
            float ambientTemperature = this.AmbientTemperature;
            if (ambientTemperature > 0f && Map != null)
            {
                FloatRange randomization = new FloatRange(0.8f,1.2f);
                this.TakeDamage(new DamageInfo(DamageDefOf.Rotting, (int)(ambientTemperature*randomization.RandomInRange)));

            }
            base.TickLong();
            
            
        }
    }
}
