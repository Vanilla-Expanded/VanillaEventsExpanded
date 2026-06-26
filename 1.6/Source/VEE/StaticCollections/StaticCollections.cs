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

        public struct WeightedPlants
        {
            public ThingDef plant;
            public float weight;
        }

        public static List<ThingDef> nonSmoothedMineables = new List<ThingDef>();
        public static List<ThingDef> cropSproutCandidates = new List<ThingDef>();
        public static float cachedPlantGrowthMultiplier = 1;
        public static float cachedGlobalLightLevelsMultiplier = 1;
        public static WeightedPlants[] plantArray = new WeightedPlants[8];

        static StaticCollections()
        {

            nonSmoothedMineables.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where( x => x.mineable && !x.building.mineablePreventMeteorite && !x.IsSmoothed));
           
            List<CropSproutCandidatesDef> allCropSproutCandidates = DefDatabase<CropSproutCandidatesDef>.AllDefsListForReading;
            foreach (CropSproutCandidatesDef individualList in allCropSproutCandidates)
            {
                cropSproutCandidates.AddRange(individualList.cropSproutCandidates);
            }

            WeightedPlants plants = new WeightedPlants
            {
                plant = VEE_DefOf.VEE_Plant_PinkGrass,
                weight = 1f
            };
            plantArray[0] = plants;
            plants = new WeightedPlants
            {
                plant = VEE_DefOf.VEE_Plant_TallPinkGrass,
                weight = 1f
            };
            plantArray[1] = plants;
            plants = new WeightedPlants
            {
                plant = VEE_DefOf.VEE_Plant_CyllenCluster,
                weight = 0.75f
            };
            plantArray[2] = plants;
            plants = new WeightedPlants
            {
                plant = VEE_DefOf.VEE_Plant_VyspStrands,
                weight = 0.65f
            };
            plantArray[3] = plants;
           
            plants = new WeightedPlants
            {
                plant = VEE_DefOf.VEE_Plant_PhoraxTree,
                weight = 0.4f
            };
            plantArray[4] = plants;
            plants = new WeightedPlants
            {
                plant = VEE_DefOf.VEE_Plant_XyrilTree,
                weight = 0.35f
            };
            plantArray[5] = plants;
            plants = new WeightedPlants
            {
                plant = VEE_DefOf.VEE_Plant_MyrloxTree,
                weight = 0.35f
            };
            plantArray[6] = plants;
            plants = new WeightedPlants
            {
                plant = VEE_DefOf.VEE_Plant_PsychicLotus,
                weight = 0.01f
            };
            plantArray[7] = plants;

        }


    }
}
