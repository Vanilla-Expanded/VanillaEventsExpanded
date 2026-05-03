using System;
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
            float targetPrice = (float)Math.Min(map.wealthWatcher.WealthTotal * 0.01, 1000);
            IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, map);
            IEnumerable<ThingStuffPair> baseW = ThingStuffPair.AllWith((ThingDef td) => td.equipmentType == EquipmentType.Primary && td.recipeMaker != null);
            IEnumerable<ThingStuffPair> source = from w in baseW
                                                 where w.Price != 0 && w.Price <= targetPrice
                                                 select w;

            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);

            List<Thing> list = new List<Thing>();
          
            ThingStuffPair weapon = source.RandomElement();
            Thing item = ThingMaker.MakeThing(weapon.thing, weapon.stuff);
            float itemHP = item.GetStatValue(StatDefOf.MaxHitPoints);
            float randomDamage = new FloatRange(0, 0.2f).RandomInRange;
            DamageInfo damage = new DamageInfo(DamageDefOf.Crush, itemHP * randomDamage);
            item.TakeDamage(damage);
            list.Add(item);
            
            DropPodUtility.DropThingsNear(intVec, map, list, 110, false, true, true);

            Find.LetterStack.ReceiveLetter("CPWLabel".Translate(), "CPW".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(intVec, map, false), null, null);
            return true;
        }
    }
}