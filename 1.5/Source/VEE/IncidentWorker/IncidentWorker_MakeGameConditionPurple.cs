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
                bool enoughDaysPassed = comp.tickLast == 0 || Find.TickManager.TicksGame - comp.tickLast > 60000 * Settings.VEEMod.settings.daysBetweenPurpleEvent;
                return base.CanFireNowSub(parms) && enoughDaysPassed;
            }
            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var gameCondition = GameConditionMaker.MakeCondition(def.gameCondition, Mathf.RoundToInt(def.durationDays.RandomInRange * 60000f));

            parms.target.GameConditionManager.RegisterCondition(gameCondition);
            parms.letterHyperlinkThingDefs = gameCondition.def.letterHyperlinks;
            SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, LookTargets.Invalid);

            if (Find.World.GetComponent<WorldComp_Purple>() is WorldComp_Purple comp)
                comp.tickLast = Find.TickManager.TicksGame;

            return true;
        }
    }
}