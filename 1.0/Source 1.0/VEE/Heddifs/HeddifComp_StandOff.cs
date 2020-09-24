using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using RimWorld;

namespace VEE
{
    class HeddifComp_StandOff : HediffComp
    {
        public HediffCompProperties Props
        {
            get
            {
                return (HeddifCompPropreties_Standoff)this.props;
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
            if(t == 0)
            {
                pawnl.Add(this.Pawn);
                this.Pawn.SetFaction(Find.FactionManager.RandomEnemyFaction());

                LordJob_VisitColony lordJob1 = new LordJob_VisitColony();
                LordMaker.MakeNewLord(this.Pawn.Faction, lordJob1, this.Pawn.Map);
            }
            if (t%100 == 0)
            {
                if (GenDate.DayTick(GenTicks.TicksAbs, Find.WorldGrid.LongLatOf(this.Pawn.Map.Tile).x) > 29500 && GenDate.DayTick(GenTicks.TicksAbs, Find.WorldGrid.LongLatOf(this.Pawn.Map.Tile).x) < 30500 && flag)
                {
                    Job job = new Job(JobDefOf.Wait_Wander);
                    this.Pawn.jobs.TryTakeOrderedJob(job);
                    this.Pawn.jobs.StopAll();
                    LordJob_DefendPoint lordJob = new LordJob_DefendPoint(this.Pawn.Position);
                    LordMaker.MakeNewLord(this.Pawn.Faction, lordJob, this.Pawn.Map);

                    flag = false;
                }
            }
            t++;
        }
    }
}
