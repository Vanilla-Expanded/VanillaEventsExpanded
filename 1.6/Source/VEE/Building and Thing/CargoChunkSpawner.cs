using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using VEF.Genes;
using Verse;
using Verse.Noise;
using static RimWorld.PsychicRitualRoleDef;

namespace VEE
{
    public class CargoChunkSpawner : Building
    {

        public TradeShip tradeship;

        public override void Destroy(DestroyMode mode)
        {
            Map map = base.Map;
            
            ThingSetMakerParams parms = default(ThingSetMakerParams);
            parms.traderDef = tradeship.TraderKind;
            parms.makingFaction = tradeship.Faction;
            parms.tile = map.Tile;
           
            parms.minSingleItemMarketValuePct = 0.05f;

            float totalMarketValueRange;
            if (mode == DestroyMode.KillFinalize)
            {
                totalMarketValueRange = new FloatRange(30, 60).RandomInRange;
            }
            else if (mode == DestroyMode.Vanish)
            {
                totalMarketValueRange = new FloatRange(300, 600).RandomInRange;
            }

            List<Thing> wares = ThingSetMakerDefOf.TraderStock.root.Generate(parms).ToList<Thing>();
            wares.RemoveAll(t => t is Pawn || (t is MinifiedThing tm && tm != null));

            int count = new IntRange(2, 6).RandomInRange;
            while (wares.Count > count)
            {
                wares.Shuffle();
                wares.RemoveLast();
            }

            if (wares.Count > 0) { 
                foreach (Thing thing in wares)
                {       
                    GenPlace.TryPlaceThing(thing, this.Position, map, ThingPlaceMode.Near);
                }
            }
            base.Destroy(mode);

        }



    }

}