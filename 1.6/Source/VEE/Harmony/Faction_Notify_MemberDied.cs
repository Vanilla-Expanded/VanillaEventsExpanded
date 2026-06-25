using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VEE
{
   
    [HarmonyPatch(typeof(Faction))]
    [HarmonyPatch("Notify_MemberDied", MethodType.Normal)]
    public class VEE_Faction_Notify_MemberDied_Patch
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