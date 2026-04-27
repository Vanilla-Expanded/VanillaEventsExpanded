using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VEE
{
    public class ThingSetMaker_MeteoriteShower : ThingSetMaker
    {
        

        public static readonly IntRange MineablesCountRange = new IntRange(8, 20);

        private const float PreciousMineableMarketValue = 5f;

    

        protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings)
        {
            int randomInRange = (parms.countRange ?? MineablesCountRange).RandomInRange;
            ThingDef def = FindRandomMineableDef();
            for (int i = 0; i < randomInRange; i++)
            {
                Building building = (Building)ThingMaker.MakeThing(def);
                building.canChangeTerrainOnDestroyed = false;
                outThings.Add(building);
            }
        }

        private ThingDef FindRandomMineableDef()
        {
           
            float value = Rand.Value;
            if (value < 0.8f)
            {
                return StaticCollections.nonSmoothedMineables.Where((ThingDef x) => !x.building.isResourceRock).RandomElement();
            }
            if (value < 0.95f)
            {
                return StaticCollections.nonSmoothedMineables.Where((ThingDef x) => x.building.isResourceRock && x.building.mineableThing.BaseMarketValue < PreciousMineableMarketValue).RandomElement();
            }
            return StaticCollections.nonSmoothedMineables.Where((ThingDef x) => x.building.isResourceRock && x.building.mineableThing.BaseMarketValue >= PreciousMineableMarketValue).RandomElement();
        }

        protected override IEnumerable<ThingDef> AllGeneratableThingsDebugSub(ThingSetMakerParams parms)
        {
            return StaticCollections.nonSmoothedMineables;
        }
    }
}