using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace VEE.Others
{
    class MakeGameConditionPurpleEvents : IncidentWorker
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
                if (!this.def.gameCondition.CanCoexistWith(activeConditions[i].def))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
