using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VEE
{
    [StaticConstructorOnStartup]
    public class StaticCollections
    {

        public static List<ThingDef> nonSmoothedMineables = new List<ThingDef>();
        public static List<ThingDef> cropSproutCandidates = new List<ThingDef>();
        public static float cachedPlantGrowthMultiplier = 1;

        static StaticCollections()
        {

            nonSmoothedMineables.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where( x => x.mineable && !x.building.mineablePreventMeteorite && !x.IsSmoothed));

           
            List<CropSproutCandidatesDef> allCropSproutCandidates = DefDatabase<CropSproutCandidatesDef>.AllDefsListForReading;
            foreach (CropSproutCandidatesDef individualList in allCropSproutCandidates)
            {
                cropSproutCandidates.AddRange(individualList.cropSproutCandidates);
            }


        }


    }
}
