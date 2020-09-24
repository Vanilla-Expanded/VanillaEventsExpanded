using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VEE.RegularEvents
{
    class WandererJoinTraitor : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            IntVec3 intVec;
            return this.TryFindEntryCell(map, out intVec);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 loc;
            if (!this.TryFindEntryCell(map, out loc))
            {
                return false;
            }
            Gender? gender = null;
            if (this.def.pawnFixedGender != Gender.None)
            {
                gender = new Gender?(this.def.pawnFixedGender);
            }
            PawnKindDef pawnKind = this.def.pawnKind;
            Faction ofPlayer = Faction.OfPlayer;
            Gender? fixedGender = gender;
            PawnGenerationRequest request = new PawnGenerationRequest(pawnKind, ofPlayer, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, true, 20f, false, true, true, false, false, false, false, null, null, null, null, null, fixedGender, null, null);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            GenSpawn.Spawn(pawn, loc, map, WipeMode.Vanish);

            pawn.health.AddHediff(VEE_DefOf.Traitor);

            string text = this.def.letterText.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
            string label = this.def.letterLabel.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, pawn, null, null);
            return true;
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c) && !c.Fogged(map), map, CellFinder.EdgeRoadChance_Neutral, out cell);
        }
    }
}
