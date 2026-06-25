using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VEE
{


    [HarmonyPatch(typeof(JobGiver_WanderInRoofedCellsInPen))]
    [HarmonyPatch("ShouldSeekRoofedCells", MethodType.Normal)]
    public class VEE_JobGiver_WanderInRoofedCellsInPen_Patch
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


}