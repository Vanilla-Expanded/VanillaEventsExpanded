using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VEE
{
    class ThingSetMaker_AnimalPod : ThingSetMaker
    {
        protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings)
        {
            List<PawnKindDef> Allanimals = new List<PawnKindDef>();
            List<PawnKindDef> allPawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading;
            for (int i = 0; i < allPawnKindDefs.Count; i++)
            {
                PawnKindDef pawnK = allPawnKindDefs[i];
                if (pawnK.race.race.Animal && pawnK.RaceProps.IsFlesh && (pawnK.canArriveManhunter == true || pawnK.defName == "Thrumbo" || pawnK.defName == "Megascarab" || pawnK.defName == "Spelopede" || pawnK.defName == "Megaspider"))
                {
                    Allanimals.Add(pawnK);
                }
            }
            
            PawnGenerationRequest request = new PawnGenerationRequest(Allanimals.RandomElement(), null);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            outThings.Add(pawn);
            HealthUtility.DamageUntilDowned(pawn, true);
        }

        protected override IEnumerable<ThingDef> AllGeneratableThingsDebugSub(ThingSetMakerParams parms)
        {
            yield return PawnKindDefOf.SpaceRefugee.race;
            yield break;
        }
    }
}
