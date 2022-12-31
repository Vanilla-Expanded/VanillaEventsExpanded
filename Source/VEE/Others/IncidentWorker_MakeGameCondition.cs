using RimWorld;
using UnityEngine;
using Verse;

namespace VEE
{
    public class IncidentWorker_MakeGameConditionPurple : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (Find.World.GetComponent<WorldComp_Purple>() is WorldComp_Purple comp)
            {
                bool enoughDaysPassed = comp.tickLastPurpleEvent == 0 || Find.TickManager.TicksGame - comp.tickLastPurpleEvent > 60000 * Settings.VEEMod.settings.daysBetweenPurpleEvent;
                return base.CanFireNowSub(parms) && enoughDaysPassed;
            }
            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            GameConditionManager conditionManager = parms.target.GameConditionManager;
            GameCondition gameCondition = GameConditionMaker.MakeCondition(def.gameCondition, Mathf.RoundToInt(def.durationDays.RandomInRange * 60000f));

            conditionManager.RegisterCondition(gameCondition);
            parms.letterHyperlinkThingDefs = gameCondition.def.letterHyperlinks;
            SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, LookTargets.Invalid);

            Find.World.GetComponent<WorldComp_Purple>().tickLastPurpleEvent = Find.TickManager.TicksGame;

            return true;
        }
    }

    public class IncidentWorker_MakeGameConditionNoLetter : IncidentWorker_MakeGameCondition
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            GameConditionManager conditionManager = parms.target.GameConditionManager;
            GameCondition gameCondition = GameConditionMaker.MakeCondition(def.gameCondition, Mathf.RoundToInt(def.durationDays.RandomInRange * 60000f));

            conditionManager.RegisterCondition(gameCondition);
            return true;
        }
    }
}
