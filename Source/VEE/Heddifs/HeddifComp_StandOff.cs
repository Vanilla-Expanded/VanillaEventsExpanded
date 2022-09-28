using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace VEE
{
    internal class HeddifComp_StandOff : HediffComp
    {
        public HediffCompProperties Props
        {
            get
            {
                return (HeddifCompPropreties_Standoff)props;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
        }

        public int t = 0;
        public List<Pawn> pawnl = new List<Pawn>();
        public bool flag = true;

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (t == 0)
            {
                pawnl.Add(Pawn);
                Pawn.SetFaction(Find.FactionManager.RandomEnemyFaction());

                LordJob_VisitColony lordJob1 = new LordJob_VisitColony();
                LordMaker.MakeNewLord(Pawn.Faction, lordJob1, Pawn.Map);
            }
            if (t % 100 == 0)
            {
                if (GenDate.DayTick(GenTicks.TicksAbs, Find.WorldGrid.LongLatOf(Pawn.Map.Tile).x) > 29500 && GenDate.DayTick(GenTicks.TicksAbs, Find.WorldGrid.LongLatOf(Pawn.Map.Tile).x) < 30500 && flag)
                {
                    Job job = new Job(JobDefOf.Wait_Wander);
                    Pawn.jobs.TryTakeOrderedJob(job);
                    Pawn.jobs.StopAll();
                    LordJob_DefendPoint lordJob = new LordJob_DefendPoint(Pawn.Position);
                    LordMaker.MakeNewLord(Pawn.Faction, lordJob, Pawn.Map);

                    flag = false;
                }
            }
            t++;
        }
    }
}