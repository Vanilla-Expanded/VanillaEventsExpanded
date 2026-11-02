using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace VEE.RegularEvents
{
    public class WeaponPod : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;          

            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);

            List<Thing> list = new List<Thing>();      
            
            list.Add(DecideWeapon(map));
            
            DropPodUtility.DropThingsNear(intVec, map, list, 110, false, true, true);

            Find.LetterStack.ReceiveLetter("VEE_WeaponCargoPodLabel".Translate(), "VEE_WeaponCargoPodDesc".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(intVec, map, false), null, null);
            return true;
        }

        public Thing DecideWeapon(Map map)
        {
                              
            List<Pawn> allColonists = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists.ToList();
            int totalQuality = 0;
            int count = 0;
            int rangedCount = 0;
            int meleeCount = 0;
            int totalTech = 0;
            int techCount = 0;

            foreach (Pawn pawn in allColonists)
            {
                ThingWithComps equipment = pawn.equipment?.Primary;
                if (equipment != null)
                {
                    // Quality average
                    CompQuality compQuality = equipment.TryGetComp<CompQuality>();
                    if (compQuality != null)
                    {
                        totalQuality += (int)compQuality.Quality;
                        count++;
                    }
                    // Melee vs Ranged
                    if (equipment.def.IsRangedWeapon)
                        rangedCount++;
                    else
                        meleeCount++;
                    // Tech level
                    totalTech += (int)equipment.def.techLevel;
                    techCount++;
                }
            }

            // Quality average
            QualityCategory averageQuality = QualityCategory.Normal;
            if (count > 0)
            {
                float avg = (float)totalQuality / count;
                int rounded = Mathf.RoundToInt(avg);
                averageQuality = (QualityCategory)rounded;
            }
            // Melee vs Ranged
            bool mostlyRanged = rangedCount > meleeCount;
            // Tech level
            TechLevel averageTechLevel = TechLevel.Industrial;
            if (techCount > 0)
            {
                float avgTech = (float)totalTech / techCount;
                int roundedTech = Mathf.RoundToInt(avgTech);
                roundedTech = Mathf.Clamp(roundedTech, 0, (int)TechLevel.Archotech);
                averageTechLevel = (TechLevel)roundedTech;
            }


            IEnumerable<ThingStuffPair> weaponList = ThingStuffPair.AllWith((ThingDef td) => td.equipmentType == EquipmentType.Primary && 
            td.recipeMaker != null && ((mostlyRanged && td.IsRangedWeapon)||(!mostlyRanged && !td.IsRangedWeapon)) && td.techLevel== averageTechLevel);

            ThingStuffPair weapon;
            if (weaponList.EnumerableNullOrEmpty()) {
                weapon = ThingStuffPair.AllWith((ThingDef td) => td == VEE_DefOf.Gun_Autopistol).RandomElement();
            }
            else
            {
                weapon = weaponList.RandomElement();
            }
      
            Thing item = ThingMaker.MakeThing(weapon.thing, weapon.stuff);
            float itemHP = item.GetStatValue(StatDefOf.MaxHitPoints);
            float randomDamage = new FloatRange(0, 0.2f).RandomInRange;
            DamageInfo damage = new DamageInfo(DamageDefOf.Crush, itemHP * randomDamage);
            item.TakeDamage(damage);
            CompQuality compQualityWeapon = item.TryGetComp<CompQuality>();
            if (compQualityWeapon != null)
            {
                compQualityWeapon.SetQuality(averageQuality,null);
            }
            return item;

        }
    }
}