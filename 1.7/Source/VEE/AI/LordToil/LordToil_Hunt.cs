using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace VEE
{
    public class LordToil_Hunt : LordToil
    {
        public LordToil_Hunt()
        { }

        public override void UpdateAllDuties()
        {
            var lordJob = (LordJob_HuntingParty)lord.LordJob;
            var targets = lordJob.targets;
            var usable = new List<Pawn>();

            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];
                if (target is Pawn pawn && (pawn.Spawned || pawn.Dead))
                {
                    usable.Add(pawn);
                }
            }
            usable.OrderByDescending(p => p.Dead || p.Downed);

            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                var target = targets[i];
                var pawn = lord.ownedPawns[i];
                var duty = pawn.mindState.duty;
                var focus = (Pawn)duty?.focus;

                if (duty == null || focus == null || focus.IsWorldPawn() || duty.def != VEE_DefOf.VEE_CarryAndLeave)
                {
                    pawn.mindState.duty = new PawnDuty(VEE_DefOf.VEE_CarryAndLeave, target);
                }
            }
        }
    }
}