using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

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

        public static Dictionary<Map, MapComp_Drought> mapCompDrought = new Dictionary<Map, MapComp_Drought>();

        private static float AddDroughtLine(IntVec3 cell, float num)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.8f);
            Map map = Find.CurrentMap;
            MapComp_Drought mapComp_Drought = map.GetComponent<MapComp_Drought>();
            if (mapComp_Drought != null && map.GetComponent<MapComp_Drought>().droughtGoingOn)
            {
                if (cell.GetTerrain(map).fertility > 0)
                {
                    Widgets.Label(new Rect(15f, UI.screenHeight - 65f - num, 999f, 999f), "VEE_DroughtGui".Translate(map.fertilityGrid.FertilityAt(cell).ToStringPercent()));
                }
                num += 19f;
            }
            GUI.color = Color.white;
            return num;
        }
    }

    [HarmonyPatch(typeof(JobGiver_WanderInRoofedCellsInPen))]
    [HarmonyPatch("ShouldSeekRoofedCells", MethodType.Normal)]
    public class JobGiver_WanderInRoofedCellsInPen_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (pawn.Map.gameConditionManager.ConditionIsActive(VEE_DefOf.SpaceBattle) ||
                pawn.Map.gameConditionManager.ConditionIsActive(VEE_DefOf.PsychicRain))
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(FertilityGrid))]
    [HarmonyPatch("CalculateFertilityAt", MethodType.Normal)]
    public class FertilityGrid_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(IntVec3 loc, ref Map ___map, ref float __result)
        {
            if (___map != null && HarmonyInit.mapCompDrought != null)
            {
                MapComp_Drought mapComp_Drought;
                if (HarmonyInit.mapCompDrought.ContainsKey(___map) && HarmonyInit.mapCompDrought.TryGetValue(___map) != null)
                {
                    mapComp_Drought = HarmonyInit.mapCompDrought.TryGetValue(___map);
                }
                else
                {
                    mapComp_Drought = ___map.GetComponent<MapComp_Drought>();
                    HarmonyInit.mapCompDrought.Add(___map, mapComp_Drought);
                }

                if (mapComp_Drought != null && mapComp_Drought.droughtGoingOn)
                {
                    Thing t = loc.GetEdifice(___map);
                    if (((t != null && !t.def.AffectsFertility) || t == null) && !loc.Roofed(___map))
                    {
                        if (__result > 1f)
                        {
                            __result = Mathf.Clamp(__result, 0f, 0.7f);
                        }
                        else
                        {
                            __result = Mathf.Clamp(__result, 0f, 0.1f);
                        }
                    }
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
            if (__instance.Map != null && HarmonyInit.mapCompDrought != null)
            {
                MapComp_Drought mcd = HarmonyInit.mapCompDrought.TryGetValue(__instance.Map);
                if (mcd != null && mcd.droughtGoingOn)
                {
                    bool test = __instance.Map.fertilityGrid.FertilityAt(__instance.Position) < __instance.def.plant.fertilityMin;
                    mcd.affectedPlants.SetOrAdd(__instance, test);

                    if (test && (!__instance.def.plant.dieIfLeafless || __instance.def.label.Contains("grass")))
                    {
                        ___madeLeaflessTick = Find.TickManager.TicksGame;
                        if (!__instance.LeaflessNow)
                        {
                            __instance.Map.mapDrawer.MapMeshDirty(__instance.Position, MapMeshFlagDefOf.Things);
                        }
                    }
                    else
                    {
                        ___madeLeaflessTick = -99999;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("GrowthRate", MethodType.Getter)]
    public class Plant_GrowthRate_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Plant __instance, ref float __result)
        {
            if (__instance.Map != null && HarmonyInit.mapCompDrought != null)
            {
                MapComp_Drought mcd = HarmonyInit.mapCompDrought.TryGetValue(__instance.Map);
                if (mcd != null && mcd.droughtGoingOn && __instance.Map.fertilityGrid.FertilityAt(__instance.Position) < __instance.def.plant.fertilityMin && mcd.affectedPlants.ContainsKey(__instance))
                {
                    __result = 0f;
                }
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
            if (__instance?.Map.GetComponent<MapComp_Drought>() is MapComp_Drought mapComp_Drought
                && mapComp_Drought != null
                && mapComp_Drought.droughtGoingOn
                && __instance.def?.plant?.leaflessGraphic != null
                && __instance.Map?.fertilityGrid?.FertilityAt(__instance.Position) < __instance.def?.plant?.fertilityMin)
            {
                ___madeLeaflessTick = Find.TickManager.TicksGame;
            }
        }
    }

    [HarmonyPatch(typeof(MouseoverReadout))]
    [HarmonyPatch("MouseoverReadoutOnGUI", MethodType.Normal)]
    public class MouseoverReadout_Patch
    {
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

    [HarmonyPatch(typeof(Faction))]
    [HarmonyPatch("Notify_MemberDied", MethodType.Normal)]
    public class Faction_Notify_MemberDied
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn member)
        {
            if (member.kindDef == VEE_DefOf.VEE_Hunter || member.kindDef == VEE_DefOf.VEE_TribalHunter)
                return false;

            return true;
        }
    }
}