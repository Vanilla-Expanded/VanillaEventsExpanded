
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace VEE.RegularEvents
{

    public class PsychicHum : IncidentWorker
    {

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.PsychicDrone) || map.gameConditionManager.ConditionIsActive(GameConditionDefOf.PsychicSoothe) ||
                map.gameConditionManager.ConditionIsActive(VEE_DefOf.VEE_PsychicStimulation) ||
                map.gameConditionManager.ConditionIsActive(VEE_DefOf.VEE_PsychicOverdrive))
            {
                return false;
            }
            if (map.listerThings.ThingsOfDef(ThingDefOf.PsychicDronerShipPart).Count > 0)
            {
                return false;
            }
            if (map.mapPawns.FreeColonistsCount == 0)
            {
                return false;
            }
            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            DoConditionAndLetter(parms, map, Mathf.RoundToInt(def.durationDays.RandomInRange * 60000f), ChooseHalf(map), parms.points);
            VEE_DefOf.VEE_PsychicHumSound.PlayOneShotOnCamera((Map)parms.target);
            return true;
        }

        public void DoConditionAndLetter(IncidentParms parms, Map map, int duration, List<Pawn> pawns, float points)
        {
            if (points < 0f)
            {
                points = StorytellerUtility.DefaultThreatPointsNow(map);
            }
            GameCondition_PsychicHum gameCondition_PsychicHum = (GameCondition_PsychicHum)GameConditionMaker.MakeCondition(VEE_DefOf.VEE_PsychicHum, duration);
            gameCondition_PsychicHum.affectedPawns = pawns;
            map.gameConditionManager.RegisterCondition(gameCondition_PsychicHum);
            SendStandardLetter(gameCondition_PsychicHum.LabelCap, gameCondition_PsychicHum.LetterText, gameCondition_PsychicHum.def.letterDef, parms, LookTargets.Invalid);
        }

        private List<Pawn> ChooseHalf(Map map) {
            List<Pawn> selected = map.mapPawns.FreeColonists.InRandomOrder().ToList();
            return selected.GetRange(0, selected.Count / 2);
        }
    }
}