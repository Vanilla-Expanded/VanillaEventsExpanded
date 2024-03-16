using System.Collections.Generic;

using RimWorld;
using Verse;
using Verse.AI;

namespace VEE.Jobs
{
    internal class JobDriver_HuntAndLeave : JobDriver
    {
        private Corpse Corpse => job.GetTarget(TargetIndex.A).Thing as Corpse;

        public Pawn Victim
        {
            get
            {
                Corpse corpse = Corpse;
                return corpse != null ? corpse.InnerPawn : (Pawn)job.GetTarget(TargetIndex.A).Thing;
            }
        }

        public override string GetReport() => Victim != null ? JobUtility.GetResolvedJobReport(job.def.reportString, Victim) : base.GetReport();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (Corpse != null)
                return !Map.reservationManager.IsReservedByAnyoneOf(Corpse, pawn.Faction);
            if (Victim != null)
                return !Map.reservationManager.IsReservedByAnyoneOf(Victim, pawn.Faction);

            return false;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => Victim == null && Corpse == null);
            // Start hunt
            yield return Toils_Combat.TrySetJobToUseAttackVerb(TargetIndex.A);
            // Create label
            var startCollectCorpseLabel = Toils_General.Label();
            var slaughterLabel = Toils_General.Label();
            var fleeToil = Toils_General.Label();
            // Jump if already dead
            yield return Toils_Jump.JumpIf(startCollectCorpseLabel, () => Corpse != null);
            // Go to cast position toil
            var gotoCastPos = Toils_Combat.GotoCastPosition(TargetIndex.A, TargetIndex.None, true, 0.95f)
                .JumpIfDespawnedOrNull(TargetIndex.A, startCollectCorpseLabel);
            yield return gotoCastPos;
            // Slaughter toil
            var slaughterIfPossible = Toils_Jump.JumpIf(slaughterLabel, delegate
            {
                Pawn victim = Victim;
                return (victim.RaceProps.DeathActionWorker == null || !victim.RaceProps.DeathActionWorker.DangerousInMelee) && victim.Downed;
            });
            yield return slaughterIfPossible;
            // Jump to gotoCastPos if can't not hittable
            yield return Toils_Jump.JumpIfTargetNotHittable(TargetIndex.A, gotoCastPos);
            yield return Toils_Combat.CastVerb(TargetIndex.A, false)
                .JumpIfDespawnedOrNull(TargetIndex.A, startCollectCorpseLabel);
            yield return Toils_Combat.CastVerb(TargetIndex.A, false);
            yield return Toils_Jump.JumpIfTargetDespawnedOrNull(TargetIndex.A, startCollectCorpseLabel);
            yield return Toils_Jump.Jump(slaughterIfPossible);
            // Slaughter
            yield return slaughterLabel;
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnMobile(TargetIndex.A);
            yield return Toils_General.WaitWith(TargetIndex.A, 180, true, false).FailOnMobile(TargetIndex.A);
            yield return Toils_General.Do(delegate
            {
                if (Victim.Dead)
                    return;

                ExecutionUtility.DoExecutionByCut(pawn, Victim);
                pawn.records.Increment(RecordDefOf.AnimalsSlaughtered);
            });
            yield return Toils_Jump.Jump(startCollectCorpseLabel);

            yield return startCollectCorpseLabel;
            yield return StartCollectCorpseToil();
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false, false);
            var gotoCell = Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
            gotoCell.AddPreTickAction(delegate
            {
                var map = Map;
                var pos = pawn.Position;

                if (map.exitMapGrid.IsExitCell(pos))
                    pawn.ExitMap(true, CellRect.WholeMap(map).GetClosestEdge(pos));
            });
            yield return gotoCell;
            yield return new Toil
            {
                initAction = delegate ()
                {
                    var map = Map;
                    var pos = pawn.Position;

                    if (pos.OnEdge(map) || map.exitMapGrid.IsExitCell(pos))
                        pawn.ExitMap(true, CellRect.WholeMap(map).GetClosestEdge(pos));
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
        }

        private Toil StartCollectCorpseToil()
        {
            Toil toil = new Toil();
            toil.initAction = delegate ()
            {
                if (Victim == null)
                {
                    toil.actor.jobs.EndCurrentJob(JobCondition.Incompletable, true);
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.Hunted, new object[]
                {
                    pawn,
                    Victim
                });
                Corpse corpse = Victim.Corpse;
                if (corpse == null || !pawn.CanReserveAndReach(corpse, PathEndMode.ClosestTouch, Danger.Deadly, 1, -1, null, false))
                {
                    pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true);
                    return;
                }
                corpse.SetForbidden(false, true);
                if (RCellFinder.TryFindBestExitSpot(pawn, out IntVec3 c))
                {
                    pawn.Reserve(corpse, job, 1, -1, null, true);
                    pawn.Reserve(c, job, 1, -1, null, true);
                    job.SetTarget(TargetIndex.B, c);
                    job.SetTarget(TargetIndex.A, corpse);
                    job.count = 1;
                    job.haulMode = HaulMode.ToCellNonStorage;
                    return;
                }
                pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true);
            };
            return toil;
        }
    }
}