using System.Collections.Generic;
using System.Drawing;
using RimWorld;
using VEE.RegularEvents;
using Verse;
using Verse.Noise;

namespace VEE
{
    public class GameCondition_PsychicHum : GameCondition
    {
        public List<Pawn> affectedPawns;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref affectedPawns, "affectedPawns", LookMode.Reference);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                affectedPawns ??= new List<Pawn>();
                affectedPawns.RemoveAll(x => x == null);
            }
        }

        public override string LetterText
        {
            get
            {
                return def.letterText.Formatted(affectedPawns.ToStringSafeEnumerable());
            }
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
                for (int j = 0; j < affectedPawns.Count; j++)
                {
                    HealthUtility.AdjustSeverity(affectedPawns[j], VEE_DefOf.VEE_PsychicHumHediff, 1);
                }
            }
        }
    }
}