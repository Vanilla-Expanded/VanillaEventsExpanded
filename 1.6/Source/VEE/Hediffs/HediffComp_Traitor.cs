using System;
using System.Collections.Generic;
using RimWorld;
using VEF.Factions;
using Verse;
using Verse.AI.Group;

namespace VEE
{
    public class HediffComp_Traitor : HediffComp
    {
        public HediffCompPropreties_Traitor Props => (HediffCompPropreties_Traitor)props;

        public int ticksToDisappear;

        public Faction forcedFaction = null;

        public bool isGroup = false;

        public override void CompPostMake()
        {
            ticksToDisappear = Props.disappearsAfterTicks.RandomInRange;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            var pawn = Pawn;
            if (pawn.Spawned && pawn.IsHashIntervalTick(250))
            {
                ticksToDisappear -= 250;
                if (ticksToDisappear <= 0)
                {
                    Faction faction;
                    if (forcedFaction != null)
                    {
                        faction=forcedFaction;
                    }
                    else
                    {
                        faction = Find.FactionManager.RandomEnemyFaction(allowNonHumanlike: false);
                    }
                    
                    pawn.SetFaction(faction);

                    var map = pawn.Map;
                    LordMaker.MakeNewLord(faction, new LordJob_AssaultColony(faction, true, false, false, true, true, false, true), map, new List<Pawn> { pawn });
                    if (isGroup)
                    {
                        Find.World.GetComponent<WorldComp_Purple>().Notify_TraitorGroup(faction);
                    }
                    else
                    {
                        Find.LetterStack.ReceiveLetter("VEE_TraitorLabel".Translate(pawn.Named("PAWN")).AdjustedFor(pawn), "VEE_TraitorDesc".Translate(pawn.Named("PAWN")).AdjustedFor(pawn), LetterDefOf.ThreatSmall, new TargetInfo(pawn.Position, map, false));
                    }

                    parent.pawn.health.hediffSet.hediffs.Remove(parent);
                }
            }
        }

        public override void CompPostMerged(Hediff other)
        {
            var hediffComp = other.TryGetComp<HediffComp_Traitor>();
            if (hediffComp != null && hediffComp.ticksToDisappear > ticksToDisappear)
                ticksToDisappear = hediffComp.ticksToDisappear;
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticksToDisappear, "ticksToDisappear");
            Scribe_Values.Look(ref isGroup, "isGroup");

            Scribe_References.Look(ref forcedFaction, "forcedFaction");

        }

        public override string CompDebugString()
        {
            return "ticksToDisappear: " + ticksToDisappear;
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Now",
                    action = new Action(() =>
                    {
                        ticksToDisappear = 200;
                    })
                };
            }
        }
    }
}