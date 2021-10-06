using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace VEE
{
    public class IncidentWorker_MakeGameConditionPurple : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (Find.World.GetComponent<WorldComp_Purple>() is WorldComp_Purple comp)
            {
                bool enoughDaysPassed = comp.tickLastPurpleEvent == 0 || Find.TickManager.TicksGame - comp.tickLastPurpleEvent > 60000 * Settings.VEEMod.settings.daysBetweenPurpleEvent;
                comp.tickLastPurpleEvent = Find.TickManager.TicksGame;
                return base.CanFireNowSub(parms) && enoughDaysPassed;
            }
            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            GameConditionManager conditionManager = parms.target.GameConditionManager;
            GameCondition gameCondition = GameConditionMaker.MakeCondition(this.def.gameCondition, Mathf.RoundToInt(this.def.durationDays.RandomInRange * 60000f));

            conditionManager.RegisterCondition(gameCondition);
            parms.letterHyperlinkThingDefs = gameCondition.def.letterHyperlinks;
            this.SendStandardLetter(this.def.letterLabel, this.def.letterText, this.def.letterDef, parms, LookTargets.Invalid);
            return true;
        }
    }

    public class IncidentWorker_MakeGameConditionNoLetter : IncidentWorker_MakeGameCondition
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            GameConditionManager conditionManager = parms.target.GameConditionManager;
            GameCondition gameCondition = GameConditionMaker.MakeCondition(this.def.gameCondition, Mathf.RoundToInt(this.def.durationDays.RandomInRange * 60000f));

            conditionManager.RegisterCondition(gameCondition);
            return true;
        }
    }
}
