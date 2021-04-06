using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Data;
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

        private static float AddDroughtLine(IntVec3 cell, float num)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.8f);
            Map map = Find.CurrentMap;
            MapComp_Drought mapComp_Drought = map.GetComponent<MapComp_Drought>();
            if (mapComp_Drought != null && map.GetComponent<MapComp_Drought>().droughtGoingOn)
            {
                if (cell.GetTerrain(map).fertility > 0)
                { 
                    Widgets.Label(new Rect(15f, (float)UI.screenHeight - 65f - num, 999f, 999f), "VEE_DroughtGui".Translate(map.fertilityGrid.FertilityAt(cell).ToStringPercent())); 
                }
                num += 19f;
            }
            GUI.color = Color.white;
            return num;
        }
    }

    [HarmonyPatch(typeof(FertilityGrid))]
    [HarmonyPatch("CalculateFertilityAt", MethodType.Normal)]
    public class FertilityGrid_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(IntVec3 loc, ref Map ___map, ref float __result)
        {
            MapComp_Drought mapComp_Drought = ___map.GetComponent<MapComp_Drought>();
            if (mapComp_Drought != null && mapComp_Drought.droughtGoingOn)
            {
                Thing t = loc.GetEdifice(___map);
                if ((t != null && !t.def.AffectsFertility) || t == null)
                {
                    float fa = 0f;
                    if (__result > 1f)
                    {
                        fa = 0.6f;
                    }
                    __result = Mathf.Clamp(__result, 0f, 0.1f + fa);
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
            if (__instance.Map != null)
            {
                MapComp_Drought mapComp_Drought = __instance.Map.GetComponent<MapComp_Drought>();
                if (mapComp_Drought != null && mapComp_Drought.droughtGoingOn && !mapComp_Drought.affectedPlants.ContainsKey(__instance))
                {
                    mapComp_Drought.affectedPlants.Add(__instance, __instance.Map.fertilityGrid.FertilityAt(__instance.Position) < __instance.def.plant.fertilityMin);
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
            if (HarmonyInit.droughtGoingOn && HarmonyInit.affectedPlants.TryGetValue(__instance, false))
            {
                StringBuilder stringBuilder = new StringBuilder(__result);
                stringBuilder.AppendInNewLine("VEE_Decay".Translate());
                __result = stringBuilder.ToString().TrimEndNewlines();
            }
        }
    }*/

    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("GrowthRate", MethodType.Getter)]
    public class Plant_GrowthRate_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Plant __instance, ref float __result)
        {
            MapComp_Drought mapComp_Drought = __instance.Map.GetComponent<MapComp_Drought>();
            if (mapComp_Drought != null && mapComp_Drought.droughtGoingOn && __instance.Map.fertilityGrid.FertilityAt(__instance.Position) < __instance.def.plant.fertilityMin && mapComp_Drought.affectedPlants.ContainsKey(__instance))
            {
                __result = 0f;
            }
        }
    }
    
    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("LeaflessNow", MethodType.Getter)]
    public class Plant_LeaflessNow_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Plant __instance, ref bool __result)
        {
            MapComp_Drought mapComp_Drought = __instance.Map.GetComponent<MapComp_Drought>();
            if (mapComp_Drought != null && mapComp_Drought.droughtGoingOn && __instance.Map.fertilityGrid.FertilityAt(__instance.Position) < __instance.def.plant.fertilityMin && mapComp_Drought.affectedPlants.ContainsKey(__instance))
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("PostMapInit", MethodType.Normal)]
    public class Plant_PostMapInit_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Plant __instance, ref int ___madeLeaflessTick)
        {
            MapComp_Drought mapComp_Drought = __instance.Map.GetComponent<MapComp_Drought>();
            if (mapComp_Drought != null && mapComp_Drought.droughtGoingOn && __instance.Map.fertilityGrid.FertilityAt(__instance.Position) < __instance.def.plant.fertilityMin)
            {
                ___madeLeaflessTick = Find.TickManager.TicksGame;
            }
        }
    }

    [HarmonyPatch(typeof(MouseoverReadout))]
    [HarmonyPatch("MouseoverReadoutOnGUI", MethodType.Normal)]
    public class MouseoverReadout_Patch
    {
        /*[HarmonyPostfix]
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
        }*/
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ret && i == codes.Count - 1)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_0, null);
                    yield return new CodeInstruction(OpCodes.Ldloc_1, null);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HarmonyInit), "AddDroughtLine", null, null));
                    yield return new CodeInstruction(OpCodes.Stloc_1, null);
                    yield return codes[i];
                }
                else
                {
                    yield return codes[i];
                }
            }
            yield break;
        }
    }
}