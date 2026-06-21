using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VEE.RegularEvents
{
    public class Crashlanded : IncidentWorker
    {

       
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            string label = "VEE_CrashlandedLabel".Translate();
            string text = "VEE_CrashlandedDesc".Translate();

            List<Thing> things = new List<Thing>();

            List<FactionRelation> list = new List<FactionRelation>();
            foreach (Faction item in Find.FactionManager.AllFactionsListForReading)
            {
                if (!item.def.PermanentlyHostileTo(FactionDefOf.Beggars))
                {
                    list.Add(new FactionRelation
                    {
                        other = item,
                        kind = FactionRelationKind.Neutral
                    });
                }
            }

            Faction faction = FactionGenerator.NewGeneratedFactionWithRelations(FactionDefOf.Beggars, list, hidden: true);
            faction.temporary = true;
            faction.def.apparelStuffFilter = FactionDefOf.PlayerColony.apparelStuffFilter;
            Find.FactionManager.Add(faction);
            ThingSetMakerParams pawnParms = new ThingSetMakerParams();
            pawnParms.makingFaction = faction;

            for (int i = 0; i < 3; i++)
            {
                Pawn pawn = ThingUtility.FindPawn(VEE_DefOf.VEE_CrashlandedColonists.root.Generate(pawnParms));
                if (Rand.Chance(0.5f))
                {
                    pawn.health.AddHediff(HediffDefOf.CryptosleepSickness);
                }
                pawn.SetFaction(faction);
                things.Add(pawn);
            }
            PawnKindDef kindDef = PossiblePets().RandomElementByWeight((PawnKindDef td) => td.RaceProps.petness);
            Pawn animal = PawnGenerator.GeneratePawn(kindDef, faction);
            things.Add(animal);

            Thing silver = ThingMaker.MakeThing(ThingDefOf.Silver);
            silver.stackCount = 200;
            things.Add(silver);

            Thing meals = ThingMaker.MakeThing(ThingDefOf.MealSurvivalPack);
            meals.stackCount = 20;
            things.Add(meals);

            Thing components = ThingMaker.MakeThing(ThingDefOf.ComponentIndustrial);
            components.stackCount = 10;
            things.Add(components);

            Thing medicine = ThingMaker.MakeThing(ThingDefOf.MedicineIndustrial);
            medicine.stackCount = 10;
            things.Add(medicine);

            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
            DropPodUtility.DropThingsNear(intVec, map, things, 110, canInstaDropDuringInit: false, leaveSlag: true);
  
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, new TargetInfo(intVec, map, false), null, null);
            return true;

        }

        private IEnumerable<PawnKindDef> PossiblePets()
        {
            return DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef td) => td.RaceProps.Animal && td.RaceProps.petness>0);
        }
    }
}