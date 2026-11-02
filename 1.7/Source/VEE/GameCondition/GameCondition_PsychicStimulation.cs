
using System.Collections.Generic;
using RimWorld;
using UnityEngine.Analytics;
using VEE.RegularEvents;
using Verse;
using Verse.Grammar;
namespace VEE
{
    public class GameCondition_PsychicStimulation : GameCondition
    {
        public ColonistAge age;

        public override string Label
        {
            get
            {
                return def.label + ": " + ("VEE_" + age).Translate();
            }
        }
        public override string LetterText
        {
            get
            {
                return def.letterText.Formatted(("VEE_" + age).Translate().ToLower());
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref age, "age", ColonistAge.Young);

        }

        public override void GameConditionTick()
        {
            base.GameConditionTick();

            if (TicksPassed % 2500 == 0)
            {
                HediffForHumanlikes();
            }
        }

        private void HediffForHumanlikes()
        {
            List<Map> affectedMaps = base.AffectedMaps;
            for (int i = 0; i < affectedMaps.Count; i++)
            {
                IReadOnlyList<Pawn> allPawnsSpawned = affectedMaps[i].mapPawns.AllPawnsSpawned;
                for (int j = 0; j < allPawnsSpawned.Count; j++)
                {
                    if (allPawnsSpawned[j].RaceProps.Humanlike)
                    {
                        if ((age == ColonistAge.Young && allPawnsSpawned[j].ageTracker.AgeBiologicalYears < 40)
                            || (age == ColonistAge.Elder && allPawnsSpawned[j].ageTracker.AgeBiologicalYears >= 40))
                        {
                            HealthUtility.AdjustSeverity(allPawnsSpawned[j], VEE_DefOf.VEE_PsychicRelaxationHediff, 1);
                        }
                    }
                }
            }
        }
    }
}