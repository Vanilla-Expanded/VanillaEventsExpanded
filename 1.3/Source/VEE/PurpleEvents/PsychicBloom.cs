using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VEE.PurpleEvents
{
    public class PsychicBloom : GameCondition
    {
        /* ========== Constant aurora ======== */
        private int curColorIndex = -1;
        private int prevColorIndex = -1;
        private float curColorTransition;
        public const float MaxSunGlow = 0.5f;
        
        public Color CurrentColor
        {
            get
            {
                return Color.Lerp(PsychicBloom.Colors[this.prevColorIndex], PsychicBloom.Colors[this.curColorIndex], this.curColorTransition);
            }
        }

        private int TransitionDurationTicks
        {
            get
            {
                return (!base.Permanent) ? 280 : 3750;
            }
        }

        public override float SkyGazeChanceFactor(Map map)
        {
            return 8f;
        }

        public override float SkyGazeJoyGainFactor(Map map)
        {
            return 5f;
        }

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, 200f, 1f);
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            Color currentColor = this.CurrentColor;
            SkyColorSet colorSet = new SkyColorSet(Color.Lerp(Color.white, currentColor, 0.075f) * this.Brightness(map), new Color(0.92f, 0.92f, 0.92f), Color.Lerp(Color.white, currentColor, 0.025f) * this.Brightness(map), 1f);
            float glow = Mathf.Max(GenCelestial.CurCelestialSunGlow(map), 0.25f);
            return new SkyTarget?(new SkyTarget(glow, colorSet, 1f, 1f));
        }

        private float Brightness(Map map)
        {
            return Mathf.Max(0.73f, GenCelestial.CurCelestialSunGlow(map));
        }

        private int GetNewColorIndex()
        {
            return (from x in Enumerable.Range(0, PsychicBloom.Colors.Length)
                    where x != this.curColorIndex
                    select x).RandomElement<int>();
        }

        private static readonly Color[] Colors = new Color[]
        {
            new Color(0f, 1f, 0f),
            new Color(0.3f, 1f, 0f),
            new Color(0f, 1f, 0.7f),
            new Color(0.3f, 1f, 0.7f),
            new Color(0f, 0.5f, 1f),
            new Color(0f, 0f, 1f),
            new Color(0.87f, 0f, 1f),
            new Color(0.75f, 0f, 1f)
        };

        float number;

        /* ===== saving ===== */
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.curColorIndex, "curColorIndex", 0, false);
            Scribe_Values.Look<int>(ref this.prevColorIndex, "prevColorIndex", 0, false);
            Scribe_Values.Look<float>(ref this.curColorTransition, "curColorTransition", 0f, false);
            Scribe_Values.Look(ref number, "number", 0f, false);
        }

        private List<ThingDef> flowersList;
        private List<string> excludedPlant = new List<string> { "Plant_TreeGauranlen", "Plant_MossGauranlen", "Plant_PodGauranlen", "Plant_TreeAnima", "Plant_GrassAnima" };

        public override void Init()
        {
            base.Init();
            this.curColorIndex = Rand.Range(0, PsychicBloom.Colors.Length);
            this.prevColorIndex = this.curColorIndex;
            this.curColorTransition = 1f;
            this.number = 0f;
            this.flowersList = DefDatabase<ThingDef>.AllDefsListForReading.Where((ThingDef x) => x.plant != null && x.plant.sowTags.Contains("Decorative")).ToList();
        }

        public override void GameConditionTick()
        {
            this.curColorTransition += 1f / (float)this.TransitionDurationTicks;
            if (this.curColorTransition >= 1f)
            {
                this.prevColorIndex = this.curColorIndex;
                this.curColorIndex = this.GetNewColorIndex();
                this.curColorTransition = 0f;
            }

            List<Map> affectedMaps = base.AffectedMaps; 
            for (int k = 0; k < affectedMaps.Count; k++)
            {
                if (this.TicksPassed % 100 == 0 && number <= 800f)
                {
                    IntVec3 flowerPos = CellFinderLoose.RandomCellWith(i => i.GetTerrain(affectedMaps[k]).fertility > 0.1f && i.GetFirstBuilding(affectedMaps[k]) == null, affectedMaps[k]);
                    if (flowerPos != null && flowerPos.InBounds(affectedMaps[k]))
                    {
                        ThingDef thingDefFlower = this.flowersList.RandomElement();
                        if (flowerPos.GetFirstThing<Plant>(affectedMaps[k]) is Plant p && p != null)
                        {
                            if (!this.excludedPlant.Contains(p.def.defName))
                            {
                                p.Destroy();
                                Plant flower = GenSpawn.Spawn(thingDefFlower, flowerPos, affectedMaps[k], WipeMode.Vanish) as Plant;
                                flower.Growth = 0.8f;
                                number += 1 / affectedMaps.Count;
                            }
                        }
                    }
                }
                else if (this.TicksPassed % 60000 == 0)
                {
                    number -= 25f;
                }
            }
        }
    }
}
