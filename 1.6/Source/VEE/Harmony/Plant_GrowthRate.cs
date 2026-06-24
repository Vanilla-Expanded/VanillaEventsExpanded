using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

using RimWorld;
using Verse.Noise;

namespace VEE
{
    [HarmonyPatch]
    public static class VEE_Plant_GrowthRate_Patches
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            var targetMethod = AccessTools.DeclaredPropertyGetter(typeof(Plant), "GrowthRate");
            yield return targetMethod;
            foreach (var subclass in typeof(Plant).AllSubclasses())
            {
                var method = AccessTools.DeclaredPropertyGetter(subclass, "GrowthRate");
                if (method != null)
                {
                    yield return method;
                }
            }
        }

        public static void Postfix(ref float __result, Plant __instance)
        {
            if (__instance.Map != null)
            {         
                if (StaticCollections.cachedPlantGrowthMultiplier != 1 && !__instance.Map.roofGrid.Roofed(__instance.Position))
                {
                    List<Thing> thingList = __instance.Map.thingGrid.ThingsListAtFast(__instance.Position);
                    bool tagFound = false;
                    foreach (Thing thing in thingList)
                    {
                        if (thing.def.building?.sowTag == "Hydroponic")
                        {
                            tagFound = true;
                        }
                    }
                    if (!tagFound)
                    {
                        __result *= StaticCollections.cachedPlantGrowthMultiplier;
                    }
                }
            }
        }
    }
}
