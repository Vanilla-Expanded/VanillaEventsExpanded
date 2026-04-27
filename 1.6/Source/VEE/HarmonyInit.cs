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



        /* [HarmonyPatch(typeof(Plant))]
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
         }*/

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