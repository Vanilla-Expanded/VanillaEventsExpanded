using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

            ThingSetMakerParams parms = default;
            parms.traderDef = tradeship.TraderKind;
            parms.makingFaction = tradeship.Faction;
            parms.tile = map.Tile;
            parms.minSingleItemMarketValuePct = 0.05f;

            float targetMarketValue;

            if (mode == DestroyMode.KillFinalize)
            {
                targetMarketValue = new FloatRange(30f, 60f).RandomInRange;
            }
            else if (mode == DestroyMode.Vanish)
            {
                targetMarketValue = new FloatRange(300f, 600f).RandomInRange;
            }
            else
            {
                targetMarketValue = 100f;
            }

            List<Thing> pool = ThingSetMakerDefOf.TraderStock.root.Generate(parms).ToList();

            pool.RemoveAll(t =>
                t is Pawn ||
                t is MinifiedThing);

            pool.Shuffle();

            List<Thing> wares = new List<Thing>();

            float currentValue = 0f;
            int maxItems = 6;

            foreach (Thing t in pool)
            {
                if (wares.Count >= maxItems)
                    break;

                float value = t.MarketValue * t.stackCount;
          
                if (currentValue + value > targetMarketValue * 1.15f)
                    continue;

                wares.Add(t);
                currentValue += value;

                if (currentValue >= targetMarketValue)
                    break;
            }
    
            if (wares.Count == 0 && pool.Count > 0)
            {
                Thing fallback = pool.MinBy(t => t.MarketValue * t.stackCount);
                wares.Add(fallback);
            }

            foreach (Thing thing in wares)
            {
                GenPlace.TryPlaceThing(
                    thing,
                    Position,
                    map,
                    ThingPlaceMode.Near);
            }

            base.Destroy(mode);
        }


    }

}