using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.AI;

namespace VEE.Jobs
{
    class JobDriver_HuntAndLeave : JobDriver
    {
        public Pawn Victim
        {
            get
            {
                Corpse corpse = this.Corpse;
                return corpse != null ? corpse.InnerPawn : (Pawn)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        private Corpse Corpse => this.job.GetTarget(TargetIndex.A).Thing as Corpse;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.jobStartTick, "jobStartTick", 0, false);
        }

        public override string GetReport() => this.Victim != null ? JobUtility.GetResolvedJobReport(this.job.def.reportString, this.Victim) : base.GetReport();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo target = this.Victim;
            Job job = this.job;
            return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(delegate ()
            {
                if (!this.job.ignoreDesignations)
                {
                    Pawn victim = this.Victim;
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
                    this.jobStartTick = Find.TickManager.TicksGame;
                }
            };
            yield return Toils_Combat.TrySetJobToUseAttackVerb(TargetIndex.A);
            Toil startCollectCorpseLabel = Toils_General.Label();
            Toil slaughterLabel = Toils_General.Label();
            Toil gotoCastPos = Toils_Combat.GotoCastPosition(TargetIndex.A, TargetIndex.None, true, 0.95f).JumpIfDespawnedOrNull(TargetIndex.A, startCollectCorpseLabel).FailOn(() => Find.TickManager.TicksGame > this.jobStartTick + MaxHuntTicks);
            yield return gotoCastPos;
            Toil slaughterIfPossible = Toils_Jump.JumpIf(slaughterLabel, delegate
            {
                Pawn victim = this.Victim;
                return (victim.RaceProps.DeathActionWorker == null || !victim.RaceProps.DeathActionWorker.DangerousInMelee) && victim.Downed;
            });
            yield return slaughterIfPossible;
            yield return Toils_Jump.JumpIfTargetNotHittable(TargetIndex.A, gotoCastPos);
            yield return Toils_Combat.CastVerb(TargetIndex.A, false).JumpIfDespawnedOrNull(TargetIndex.A, startCollectCorpseLabel).FailOn(() => Find.TickManager.TicksGame > this.jobStartTick + MaxHuntTicks);
            yield return Toils_Jump.JumpIfTargetDespawnedOrNull(TargetIndex.A, startCollectCorpseLabel);
            yield return Toils_Jump.Jump(slaughterIfPossible);
            yield return slaughterLabel;
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnMobile(TargetIndex.A);
            yield return Toils_General.WaitWith(TargetIndex.A, 180, true, false).FailOnMobile(TargetIndex.A);
            yield return Toils_General.Do(delegate
            {
                if (this.Victim.Dead)
                {
                    return;
                }
                ExecutionUtility.DoExecutionByCut(this.pawn, this.Victim);
                this.pawn.records.Increment(RecordDefOf.AnimalsSlaughtered);
                if (this.pawn.InMentalState)
                {
                    this.pawn.MentalState.Notify_SlaughteredAnimal();
                }
            });
            yield return Toils_Jump.Jump(startCollectCorpseLabel);
            yield return startCollectCorpseLabel;
            yield return this.StartCollectCorpseToil();
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false, false);
            Toil gotoCell = Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
            gotoCell.AddPreTickAction(delegate
            {
                if (base.Map.exitMapGrid.IsExitCell(this.pawn.Position))
                {
                    this.pawn.ExitMap(true, CellRect.WholeMap(base.Map).GetClosestEdge(this.pawn.Position));
                }
            });
            yield return gotoCell;
            yield return new Toil
            {
                initAction = delegate ()
                {
                    if (this.pawn.Position.OnEdge(this.pawn.Map) || this.pawn.Map.exitMapGrid.IsExitCell(this.pawn.Position))
                    {
                        this.pawn.ExitMap(true, CellRect.WholeMap(base.Map).GetClosestEdge(this.pawn.Position));
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
                if (this.Victim == null)
                {
                    toil.actor.jobs.EndCurrentJob(JobCondition.Incompletable, true);
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.Hunted, new object[]
                {
                    this.pawn,
                    this.Victim
                });
                Corpse corpse = this.Victim.Corpse;
                if (corpse == null || !this.pawn.CanReserveAndReach(corpse, PathEndMode.ClosestTouch, Danger.Deadly, 1, -1, null, false))
                {
                    this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true);
                    return;
                }
                corpse.SetForbidden(false, true);
                if (RCellFinder.TryFindBestExitSpot(this.pawn, out IntVec3 c))
                {
                    this.pawn.Reserve(corpse, this.job, 1, -1, null, true);
                    this.pawn.Reserve(c, this.job, 1, -1, null, true);
                    this.job.SetTarget(TargetIndex.B, c);
                    this.job.SetTarget(TargetIndex.A, corpse);
                    this.job.count = 1;
                    this.job.haulMode = HaulMode.ToCellNonStorage;
                    return;
                }
                this.pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true);
            };
            return toil;
        }

        private int jobStartTick = -1;
        private const int MaxHuntTicks = 60000;
    }
}
