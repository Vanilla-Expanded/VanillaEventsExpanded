using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;

namespace VEE
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            Harmony harmonyInstance = new Harmony("Kikohi.VanillaEventExpanded");
            harmonyInstance.PatchAll();
        }

        public static bool droughtGoingOn = false;
        public static List<Map> maps = new List<Map>();
        public static Dictionary<Plant, bool> plantDecaying = new Dictionary<Plant, bool>();
    }

    [HarmonyPatch(typeof(FertilityGrid))]
    [HarmonyPatch("CalculateFertilityAt", MethodType.Normal)]
    public class FertilityGrid_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(IntVec3 loc, ref Map ___map, ref float __result)
        {
            if (HarmonyInit.droughtGoingOn && HarmonyInit.maps.Contains(___map))
            {
                Thing t = loc.GetEdifice(___map);
                if ((t != null && !t.def.AffectsFertility) || t == null)
                {
                    __result = Mathf.Clamp(__result, 0f, 0.1f);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("TickLong", MethodType.Normal)]
    public class Plant_TickLong_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Plant __instance, ref int ___madeLeaflessTick)
        {
            if (HarmonyInit.droughtGoingOn && HarmonyInit.maps.Contains(__instance.Map))
            {
                if (HarmonyInit.plantDecaying.ContainsKey(__instance))
                {
                    if (Rand.Bool)
                    {
                        if (!__instance.def.plant.dieIfLeafless && __instance.def.plant.leaflessGraphic != null)
                        {
                            bool flag = !__instance.LeaflessNow;
                            ___madeLeaflessTick = Find.TickManager.TicksGame;
                            if (flag)
                            {
                                __instance.Map.mapDrawer.MapMeshDirty(__instance.Position, MapMeshFlag.Things);
                            }
                        }
                        //__instance.TakeDamage(new DamageInfo(DamageDefOf.Rotting, 1f));
                    }
                }
                else// if (!__instance.def.plant.IsTree)
                {
                    HarmonyInit.plantDecaying.Add(__instance, __instance.Map.fertilityGrid.FertilityAt(__instance.Position) <= 0.1f && __instance.def.plant.fertilityMin > 0.1f);
                }
            }
        }
    }

    /*[HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("GetInspectString", MethodType.Normal)]
    public class Plant_GetInspectString_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Plant __instance, ref string __result)
        {
            if (HarmonyInit.droughtGoingOn && HarmonyInit.plantDecaying.TryGetValue(__instance, false))
            {
                StringBuilder stringBuilder = new StringBuilder(__result);
                stringBuilder.AppendInNewLine("VEE_Decay".Translate());
                __result = stringBuilder.ToString().TrimEndNewlines();
            }
        }
    }*/

    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("GrowthRate", MethodType.Getter)]
    public class PlantUtility_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Plant __instance, ref float __result)
        {
            if (HarmonyInit.droughtGoingOn && HarmonyInit.plantDecaying.TryGetValue(__instance, false))
            {
                __result = 0f;
            }
        }
    }

    [HarmonyPatch(typeof(MouseoverReadout))]
    [HarmonyPatch("MouseoverReadoutOnGUI", MethodType.Normal)]
    public class MouseoverReadout_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref TerrainDef ___cachedTerrain, ref string ___cachedTerrainString)
        {
            if (HarmonyInit.droughtGoingOn && HarmonyInit.maps.Contains(Find.CurrentMap))
            {
                if (___cachedTerrain != null)
                {
                    string t = ((double)___cachedTerrain.fertility > 0.0001) ? (" " + "FertShort".TranslateSimple() + " " + Find.CurrentMap.fertilityGrid.FertilityAt(UI.MouseCell()).ToStringPercent()) : "";
                    ___cachedTerrainString = ___cachedTerrain.LabelCap + ((___cachedTerrain.passability != Traversability.Impassable) ? (" (" + "WalkSpeed".Translate((13f / ((float)___cachedTerrain.pathCost + 13f)).ToStringPercent()) + t + ")") : null);
                }
            }
        }
    }
}