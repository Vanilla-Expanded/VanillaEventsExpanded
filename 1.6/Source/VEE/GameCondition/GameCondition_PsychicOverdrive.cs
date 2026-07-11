using System.Collections.Generic;
using RimWorld;
using VEE.RegularEvents;
using Verse;

namespace VEE
{
    public class GameCondition_PsychicOverdrive : GameCondition
    {
        public Gender gender;

        public override string Label
        {
            get
            {
                return def.label + ": " + ("VEE_" + gender).Translate();
            }
        }
        public override string LetterText
        {
            get
            {
                return def.letterText.Formatted(("VEE_" + gender).Translate().ToLower());
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref gender, "gender", Gender.Male);

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
                        if (allPawnsSpawned[j].gender==gender)
                           
                        {
                            HealthUtility.AdjustSeverity(allPawnsSpawned[j], VEE_DefOf.VEE_PsychicOverdriveHediff, 1);
                        }
                    }
                }
            }
        }
    }
}