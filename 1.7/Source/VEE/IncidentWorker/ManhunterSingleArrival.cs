using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using VEF.AnimalBehaviours;
using Verse;

namespace VEE.RegularEvents
{
    public class ManhunterSingleArrival : IncidentWorker
    {
        private const float PointsFactor = 1f;

        private const int AnimalsStayDurationMin = 60000;

        private const int AnimalsStayDurationMax = 120000;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            IntVec3 result;
            if (TryFindAggressiveAnimalKind(parms.points, map, out var _))
            {
                return RCellFinder.TryFindRandomPawnEntryCell(out result, map, CellFinder.EdgeRoadChance_Animal);
            }
            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            PawnKindDef animalKind = parms.pawnKind;
            if ((animalKind == null && !TryFindAggressiveAnimalKind(parms.points, map, out animalKind)))
            {
                return false;
            }
            IntVec3 result = parms.spawnCenter;
            if (!result.IsValid && !RCellFinder.TryFindRandomPawnEntryCell(out result, map, CellFinder.EdgeRoadChance_Animal))
            {
                return false;
            }
            List<Pawn> list = AggressiveAnimalIncidentUtility.GenerateAnimals(animalKind, map.Tile, parms.points * 1f, 1);
            Rot4 rot = Rot4.FromAngleFlat((map.Center - result).AngleFlat);
            for (int i = 0; i < list.Count; i++)
            {
                Pawn pawn = list[i];
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(result, map, 10);
                QuestUtility.AddQuestTag(GenSpawn.Spawn(pawn, loc, map, rot), parms.questTag);
                pawn.health.AddHediff(HediffDefOf.Scaria);
                pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
                pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + Rand.Range(60000, 120000);
            }
          
            SendStandardLetter("VEE_ManhunterSingleArrivalLabel".Translate(animalKind.race.label), "VEE_ManhunterSingleArrivalDesc".Translate(animalKind.race.label), LetterDefOf.ThreatSmall, parms, list[0]);
            
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.ForbiddingDoors, OpportunityType.Critical);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.AllowedAreas, OpportunityType.Important);
            return true;
        }

        public static bool TryFindAggressiveAnimalKind(float points, Map map, out PawnKindDef animalKind)
        {
            List<PawnKindDef> list = (from k in map.Biomes.SelectMany((BiomeDef b) => b.AllWildAnimals)
                                      where (k.RaceProps.Animal && Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(map.Tile, k.race) 
                                      && !k.defName.Contains("GR_") && !StaticCollectionsClass.questDisabledAnimals.Contains(k)
                        && (k.race.tradeTags == null || !k.race.tradeTags.Contains("VEE_Exclude")))
                                      select k).InRandomOrder().ToList();

            if (TryGetAnimalFromList(points, list, out animalKind))
            {
                return true;
            }
           
            return false;
        }
        private static bool TryGetAnimalFromList(float points, List<PawnKindDef> animals, out PawnKindDef animalKind)
        {
            if (animals.Any())
            {
                if (animals.TryRandomElementByWeight((PawnKindDef a) => AggressiveAnimalIncidentUtility.AnimalWeight(a, points), out animalKind))
                {
                    return true;
                }
                if (points > animals.Min((PawnKindDef a) => a.combatPower) * 2f)
                {
                    animalKind = animals.MaxBy((PawnKindDef a) => a.combatPower);
                    return true;
                }
            }
            animalKind = null;
            return false;
        }

    }
}