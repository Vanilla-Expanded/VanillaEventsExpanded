
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VEE.RegularEvents
{
  

    public class PsychicOverdrive : IncidentWorker
    {

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.PsychicDrone) || map.gameConditionManager.ConditionIsActive(GameConditionDefOf.PsychicSoothe) ||
                map.gameConditionManager.ConditionIsActive(VEE_DefOf.VEE_PsychicStimulation) ||
                map.gameConditionManager.ConditionIsActive(VEE_DefOf.VEE_PsychicHum))
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
            DoConditionAndLetter(parms, map, Mathf.RoundToInt(def.durationDays.RandomInRange * 60000f), map.mapPawns.FreeColonists.RandomElement().gender, parms.points);
            VEE_DefOf.VEE_PsychicOverdriveSound.PlayOneShotOnCamera((Map)parms.target);
            return true;
        }

        public void DoConditionAndLetter(IncidentParms parms, Map map, int duration, Gender gender, float points)
        {
            if (points < 0f)
            {
                points = StorytellerUtility.DefaultThreatPointsNow(map);
            }
            GameCondition_PsychicOverdrive gameCondition_PsychicOverdrive = (GameCondition_PsychicOverdrive)GameConditionMaker.MakeCondition(VEE_DefOf.VEE_PsychicOverdrive, duration);
            gameCondition_PsychicOverdrive.gender = gender;
            map.gameConditionManager.RegisterCondition(gameCondition_PsychicOverdrive);
            SendStandardLetter(gameCondition_PsychicOverdrive.LabelCap, gameCondition_PsychicOverdrive.LetterText, gameCondition_PsychicOverdrive.def.letterDef, parms, LookTargets.Invalid);
        }

       
    }
}