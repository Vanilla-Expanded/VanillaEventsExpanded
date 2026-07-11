using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VEE
{
    public class IncidentWorker_WhiteoutRefugees : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = (Map)parms.target;
            if (!CellFinder.TryFindRandomEdgeCellWith(c => c.Standable(map) && !c.Fogged(map), map, CellFinder.EdgeRoadChance_Neutral, out IntVec3 spawnSpot))
            {
                return false;
            }

            var colonistCount = map.mapPawns.FreeColonistsSpawnedCount;
            var count = 5;
            if (colonistCount >= 9)
                count = 2;
            else if (colonistCount >= 4)
                count = 3;
            else if (colonistCount >= 2)
                count = 4;

            var list = new List<FactionRelation>();
            foreach (var item in Find.FactionManager.AllFactionsListForReading)
            {
                if (!item.def.PermanentlyHostileTo(FactionDefOf.Ancients))
                {
                    list.Add(new FactionRelation { other = item, kind = FactionRelationKind.Neutral });
                }
            }

            var factionParms = new FactionGeneratorParms(FactionDefOf.Ancients, default, true);
            var faction = FactionGenerator.NewGeneratedFactionWithRelations(factionParms, list);
            faction.temporary = true;
            Find.FactionManager.Add(faction);

            var pawns = new List<Pawn>();
            for (int i = 0; i < count; i++)
            {
                var pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Villager, faction, PawnGenerationContext.NonPlayer, map.Tile, forceGenerateNewPawn: true));
                HealthUtility.AdjustSeverity(pawn, HediffDefOf.Hypothermia, Rand.Range(0.14f, 0.52f));
                if (pawn.needs?.food != null)
                    pawn.needs.food.CurLevel = 0.01f;

                Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
                pawns.Add(pawn);
            }

            var letter = (ChoiceLetter_WhiteoutRefugees)LetterMaker.MakeLetter("VEE_WhiteoutRefugees".Translate(), "VEE_WhiteoutRefugeesText".Translate(count), VEE_DefOf.VEE_WhiteoutRefugeesLetter, new LookTargets(spawnSpot, map));
            letter.pawns = pawns;
            letter.map = map;
            letter.spawnSpot = spawnSpot;

            Find.LetterStack.ReceiveLetter(letter);
            letter.OpenLetter();
            Find.TickManager.Pause();
            return true;
        }
    }
}
