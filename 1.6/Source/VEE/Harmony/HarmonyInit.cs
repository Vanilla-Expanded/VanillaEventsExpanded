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