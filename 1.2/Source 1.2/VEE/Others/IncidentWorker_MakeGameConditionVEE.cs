using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace VEE
{
    public class IncidentWorker_MakeGameConditionVEE : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            GameConditionManager gameConditionManager = parms.target.GameConditionManager;
            if (gameConditionManager == null)
            {
                Log.ErrorOnce(string.Format("Couldn't find condition manager for incident target {0}", parms.target), 70849667, false);
                return false;
            }
            if (gameConditionManager.ConditionIsActive(this.def.gameCondition))
            {
                return false;
            }
            List<GameCondition> activeConditions = gameConditionManager.ActiveConditions;
            for (int i = 0; i < activeConditions.Count; i++)
            {
                if (activeConditions[i].def == VEE_DefOf.IceAge || activeConditions[i].def == VEE_DefOf.GlobalWarming)
                {
                    return false;
                }
                if (!this.def.gameCondition.CanCoexistWith(activeConditions[i].def))
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            GameConditionManager gameConditionManager = parms.target.GameConditionManager;
            int duration = Mathf.RoundToInt(this.def.durationDays.RandomInRange * 60000f);
            GameCondition gameCondition = GameConditionMaker.MakeCondition(this.def.gameCondition, duration);
            gameConditionManager.RegisterCondition(gameCondition);
            parms.letterHyperlinkThingDefs = gameCondition.def.letterHyperlinks;
            base.SendStandardLetter(this.def.letterLabel, this.def.letterText, this.def.letterDef, parms, LookTargets.Invalid, Array.Empty<NamedArgument>());
            return true;
        }
    }
}
