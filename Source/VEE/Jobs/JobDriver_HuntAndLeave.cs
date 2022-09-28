using System.Collections.Generic;

using RimWorld;
using Verse;
using Verse.AI;

namespace VEE.Jobs
{
    internal class JobDriver_HuntAndLeave : JobDriver
    {
        public Pawn Victim
        {
            get
            {
                Corpse corpse = Corpse;
                return corpse != null ? corpse.InnerPawn : (Pawn)job.GetTarget(TargetIndex.A).Thing;
            }
        }

        private Corpse Corpse => job.GetTarget(TargetIndex.A).Thing as Corpse;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref jobStartTick, "jobStartTick", 0, false);
        }

        public override string GetReport() => Victim != null ? JobUtility.GetResolvedJobReport(job.def.reportString, Victim) : base.GetReport();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo target = Victim;
            Job job = this.job;
            return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(delegate ()
            {
                if (!job.ignoreDesignations)
                {
                    Pawn victim = Victim;
                    if (victim != null && !victim.Dead && base.Map.designationManager.DesignationOn(victim, DesignationDefOf.Hunt) == null)
                    {
                        return true;
                    }
                }
                return false;
            });
            yield return new Toil
            {
                initAction = delegate ()
                {
                    jobStartTick = Find.TickManager.TicksGame;
                }
            };
            yield return Toils_Combat.TrySetJobToUseAttackVerb(TargetIndex.A);
            Toil startCollectCorpseLabel = Toils_General.Label();
            Toil slaughterLabel = Toils_General.Label();
            Toil gotoCastPos = Toils_Combat.GotoCastPosition(TargetIndex.A, TargetIndex.None, true, 0.95f).JumpIfDespawnedOrNull(TargetIndex.A, startCollectCorpseLabel).FailOn(() => Find.TickManager.TicksGame > jobStartTick + MaxHuntTicks);
            yield return gotoCastPos;
            Toil slaughterIfPossible = Toils_Jump.JumpIf(slaughterLabel, delegate
            {
                Pawn victim = Victim;
                return (victim.RaceProps.DeathActionWorker == null || !victim.RaceProps.DeathActionWorker.DangerousInMelee) && victim.Downed;
            });
            yield return slaughterIfPossible;
            yield return Toils_Jump.JumpIfTargetNotHittable(TargetIndex.A, gotoCastPos);
            yield return Toils_Combat.CastVerb(TargetIndex.A, false).JumpIfDespawnedOrNull(TargetIndex.A, startCollectCorpseLabel).FailOn(() => Find.TickManager.TicksGame > jobStartTick + MaxHuntTicks);
            yield return Toils_Jump.JumpIfTargetDespawnedOrNull(TargetIndex.A, startCollectCorpseLabel);
            yield return Toils_Jump.Jump(slaughterIfPossible);
            yield return slaughterLabel;
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnMobile(TargetIndex.A);
            yield return Toils_General.WaitWith(TargetIndex.A, 180, true, false).FailOnMobile(TargetIndex.A);
            yield return Toils_General.Do(delegate
            {
                if (Victim.Dead)
                {
                    return;
                }
                ExecutionUtility.DoExecutionByCut(pawn, Victim);
                pawn.records.Increment(RecordDefOf.AnimalsSlaughtered);
                if (pawn.InMentalState)
                {
                    pawn.MentalState.Notify_SlaughteredAnimal();
                }
            });
            yield return Toils_Jump.Jump(startCollectCorpseLabel);
            yield return startCollectCorpseLabel;
            yield return StartCollectCorpseToil();
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false, false);
            Toil gotoCell = Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
            gotoCell.AddPreTickAction(delegate
            {
                if (base.Map.exitMapGrid.IsExitCell(pawn.Position))
                {
                    pawn.ExitMap(true, CellRect.WholeMap(base.Map).GetClosestEdge(pawn.Position));
                }
            });
            yield return gotoCell;
            yield return new Toil
            {
                initAction = delegate ()
                {
                    if (pawn.Position.OnEdge(pawn.Map) || pawn.Map.exitMapGrid.IsExitCell(pawn.Position))
                    {
                        pawn.ExitMap(true, CellRect.WholeMap(base.Map).GetClosestEdge(pawn.Position));
                    }
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

        private int jobStartTick = -1;
        private const int MaxHuntTicks = 60000;
    }
}