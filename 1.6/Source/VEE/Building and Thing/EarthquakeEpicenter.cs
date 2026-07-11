
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace VEE
{
    public class EarthquakeEpicenter : Thing
    {
        public int earthquakeScale;
        public int earthquakeRadius;
        public int earthquakeDuration;

        public int tickCounter = 0;

        private Sustainer sustainer;
        private Effecter sustainedFx;

        protected virtual SoundDef SustainerSound => SoundDefOf.Tunnel;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref earthquakeScale, "earthquakeScale", 0);
            Scribe_Values.Look(ref earthquakeRadius, "earthquakeRadius", 0);
            Scribe_Values.Look(ref earthquakeDuration, "earthquakeDuration", 0);
            Scribe_Values.Look(ref tickCounter, "tickCounter", 0);

        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            LongEventHandler.ExecuteWhenFinished(delegate
            {
                sustainer = SustainerSound?.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));

            });
        }



        protected override void Tick()
        {
            if (this.IsHashIntervalTick(60))
            {
                sustainedFx?.Cleanup();

                for (int i = 0; i < 21; i++)
                {

                    IntVec3 cell = Position+GenRadial.RadialPatternInRadius(earthquakeRadius).RandomElement();

                    if (cell.InBounds(Map))
                    {
                        sustainedFx = VEE_DefOf.VEE_EmergencePointSustained8X8.Spawn(cell, base.Map);
                        sustainedFx?.EffectTick(this, this);

                        List<Thing> thingsHere = Map.thingGrid.ThingsListAtFast(cell).ToList();
                        foreach (Thing thing in thingsHere)
                        {
                            if (!(thing is Pawn))
                            {
                                DamageInfo damageInfo = new DamageInfo(DamageDefOf.Crush, 50);
                                thing.TakeDamage(damageInfo);
                            }

                        }
                    }

                    

                }



            }
            if (sustainer != null && !sustainer.Ended)
            {
                this.sustainer.Maintain();
            }


            if (tickCounter > earthquakeDuration)
            {

                if (sustainer != null && !sustainer.Ended)
                {
                    this.sustainer.End();
                }
                sustainedFx?.Cleanup();
                this.Destroy();

            }

            tickCounter++;
            base.Tick();

        }


    }
}