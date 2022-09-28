using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VEE.RegularEvents
{
    public class battleAnimal : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            IntVec3 intVec;
            PawnKindDef pawnKindDef;
            return RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, CellFinder.EdgeRoadChance_Animal, false, null) && this.TryFindRandomPawnKind(map, out pawnKindDef);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 intVec;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, CellFinder.EdgeRoadChance_Animal, false, null))
            {
                return false;
            }
            PawnKindDef pawnKindDef;
            if (!this.TryFindRandomPawnKind(map, out pawnKindDef))
            {
                return false;
            }
            //Log.Message(pawnKindDef.ToString());
            int num = Mathf.Clamp(GenMath.RoundRandom(2.5f / pawnKindDef.RaceProps.baseBodySize), 2, 10);
            for (int i = 0; i < num; i++)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 12, null);
                Pawn pawn = PawnGenerator.GeneratePawn(pawnKindDef, null);
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random, WipeMode.Vanish, false);
                pawn.SetFaction(Faction.OfPlayer, null);
            }
            Find.LetterStack.ReceiveLetter("BAWILabel".Translate(pawnKindDef.GetLabelPlural(-1)).CapitalizeFirst(), "BAWI".Translate(pawnKindDef.GetLabelPlural(-1)), LetterDefOf.PositiveEvent, new TargetInfo(intVec, map, false), null, null);
            return true;
        }

        private bool TryFindRandomPawnKind(Map map, out PawnKindDef kind)
        {
            return (from x in DefDatabase<PawnKindDef>.AllDefs
                    where x.RaceProps.Animal && map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(x.race) && x.race.tradeTags != null && x.race.tradeTags.Contains("AnimalFighter")
                    select x).TryRandomElementByWeight((PawnKindDef k) => k.RaceProps.wildness, out kind);
        }

        private const float MaxWildness = 0.35f;
        private const float TotalBodySizeToSpawn = 2.5f;
    }
}
