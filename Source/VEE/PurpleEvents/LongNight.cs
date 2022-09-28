using RimWorld;
using UnityEngine;
using Verse;

namespace VEE.PurpleEvents
{
    public class LongNight : GameCondition
    {
        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, 200f, 1f);
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget?(new SkyTarget(0f, EclipseSkyColors, 1f, 0f));
        }

        public override void Init()
        {
            base.Init();
            VEE_DefOf.RaidEnemyPurple.targetTags.Add(IncidentTargetTagDefOf.Map_PlayerHome); // More raids
            VEE_DefOf.ManhunterPackPurple.targetTags.Add(IncidentTargetTagDefOf.Map_PlayerHome);
            VEE_DefOf.AnimalInsanityMassPurple.targetTags.Add(IncidentTargetTagDefOf.Map_PlayerHome);
        }

        public override void End()
        {
            base.End();
            VEE_DefOf.RaidEnemyPurple.targetTags.Remove(IncidentTargetTagDefOf.Map_PlayerHome); // More raids
            VEE_DefOf.ManhunterPackPurple.targetTags.Remove(IncidentTargetTagDefOf.Map_PlayerHome);
            VEE_DefOf.AnimalInsanityMassPurple.targetTags.Remove(IncidentTargetTagDefOf.Map_PlayerHome);
        }

        private const int LerpTicks = 200;

        private SkyColorSet EclipseSkyColors = new SkyColorSet(new Color(0.482f, 0.603f, 0.682f), Color.white, new Color(0.6f, 0.6f, 0.6f), 1f);
    }
}