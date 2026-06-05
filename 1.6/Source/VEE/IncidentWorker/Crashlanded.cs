using RimWorld;
using System;
using System.Collections.Generic;
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
                pawn.SetFaction(faction);
                things.Add(pawn);
            }

            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
            DropPodUtility.DropThingsNear(intVec, map, things, 110, canInstaDropDuringInit: false, leaveSlag: true);
  
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, new TargetInfo(intVec, map, false), null, null);
            return true;

        }
    }
}