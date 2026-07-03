
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VEE.RegularEvents
{
    public enum ColonistAge
    {
        Young,
        Elder

    }

    public class PsychicStimulation : IncidentWorker
    {

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.PsychicDrone) || map.gameConditionManager.ConditionIsActive(GameConditionDefOf.PsychicSoothe)||
                map.gameConditionManager.ConditionIsActive(VEE_DefOf.VEE_PsychicOverdrive) ||
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
            DoConditionAndLetter(parms, map, Mathf.RoundToInt(def.durationDays.RandomInRange * 60000f), GetColonistAge(map.mapPawns.FreeColonists.RandomElement()), parms.points);        
            VEE_DefOf.VEE_PsychicStimulationSound.PlayOneShotOnCamera((Map)parms.target);
            return true;
        }

        public void DoConditionAndLetter(IncidentParms parms, Map map, int duration, ColonistAge age, float points)
        {
            if (points < 0f)
            {
                points = StorytellerUtility.DefaultThreatPointsNow(map);
            }
            GameCondition_PsychicStimulation gameCondition_PsychicStimulation = (GameCondition_PsychicStimulation)GameConditionMaker.MakeCondition(VEE_DefOf.VEE_PsychicStimulation, duration);
            gameCondition_PsychicStimulation.age = age;           
            map.gameConditionManager.RegisterCondition(gameCondition_PsychicStimulation);
            SendStandardLetter(gameCondition_PsychicStimulation.LabelCap, gameCondition_PsychicStimulation.LetterText, gameCondition_PsychicStimulation.def.letterDef, parms, LookTargets.Invalid);
        }

        public ColonistAge GetColonistAge(Pawn pawn)
        {
            if (pawn.ageTracker.AgeBiologicalYears >= 40)
            {
                return ColonistAge.Elder;
            }
            return ColonistAge.Young;
        }
    }
}