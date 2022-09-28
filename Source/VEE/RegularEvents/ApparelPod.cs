using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VEE.RegularEvents
{
    public class ApparelPod : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Thing> things = ThingSetMakerDefOf.ResourcePod.root.Generate();
            IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
            IEnumerable<ThingStuffPair> baseA = ThingStuffPair.AllWith((ThingDef apparel) => apparel.apparel != null && apparel.apparel.defaultOutfitTags != null
                                                        && apparel.apparel.defaultOutfitTags.Contains("Soldier") == true
                                                        && apparel.apparel.defaultOutfitTags.Contains("Worker") == true && apparel.apparel.tags != null);
            IEnumerable<ThingStuffPair> source = from w in baseA
                                                 where w.Price != 0 && w.Price <= map.wealthWatcher.WealthTotal * 0.01
                                                 select w;

            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);

            List<Thing> list = new List<Thing>();
            System.Random r = new System.Random();
            int n = r.Next(1, 3);
            for (int i = 1; i <= n; i++)
            {
                ThingStuffPair apparel = source.RandomElement();
                Thing item = ThingMaker.MakeThing(apparel.thing, apparel.stuff);
                list.Add(item);
            }
            DropPodUtility.DropThingsNear(intVec, map, list, 110, false, true, true);

            Find.LetterStack.ReceiveLetter("CPALabel".Translate(), "CPA".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(intVec, map, false), null, null);
            return true;
        }
    }
}