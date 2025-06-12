using Verse;
using Verse.AI;

namespace VEE
{
    public class JobGiver_AICarryDutyFocusAndExit : ThinkNode_JobGiver
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            return (JobGiver_AICarryDutyFocusAndExit)base.DeepCopy(resolve);
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.mindState.duty == null || pawn.mindState.duty.focus == null || !pawn.mindState.duty.focus.Pawn.Spawned)
                return null;

            var job = new Job(VEE_DefOf.HuntAndLeave, pawn.mindState.duty.focus);
            return job;
        }
    }
}