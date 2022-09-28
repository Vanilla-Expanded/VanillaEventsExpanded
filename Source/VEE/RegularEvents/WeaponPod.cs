using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VEE.RegularEvents
{
    public class WeaponPod : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Thing> things = ThingSetMakerDefOf.ResourcePod.root.Generate();
            IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
            IEnumerable<ThingStuffPair> baseW = ThingStuffPair.AllWith((ThingDef td) => td.equipmentType == EquipmentType.Primary && td.recipeMaker != null);
            IEnumerable<ThingStuffPair> source = from w in baseW
                                                 where w.Price != 0 && w.Price <= map.wealthWatcher.WealthTotal * 0.01
                                                 select w;

            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);

            List<Thing> list = new List<Thing>();
            System.Random r = new System.Random();
            int n = r.Next(1, 3);
            for (int i = 1; i <= n; i++)
            {
                ThingStuffPair weapon = source.RandomElement();
                Thing item = ThingMaker.MakeThing(weapon.thing, weapon.stuff);
                list.Add(item);
            }
            DropPodUtility.DropThingsNear(intVec, map, list, 110, false, true, true);

            Find.LetterStack.ReceiveLetter("CPWLabel".Translate(), "CPW".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(intVec, map, false), null, null);
            return true;
        }
    }
}