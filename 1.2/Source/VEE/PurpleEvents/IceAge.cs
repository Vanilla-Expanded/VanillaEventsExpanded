﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using VEE.Others;
using VEE.Settings;
using Verse;


namespace VEE.PurpleEvents
{
    public class IceAge : GameCondition
    {
        public static List<float> TilesTemp = new List<float>();
        public static List<BiomeDef> TilesBiome = new List<BiomeDef>();

        public static void SaveTileTemp()
        {
            foreach (Tile tile in Find.World.grid.tiles)
            {
                TilesTemp.Add(tile.temperature);
            }
        }
        public static void SaveTileBiome()
        {
            foreach (Tile tile in Find.World.grid.tiles)
            {
                TilesBiome.Add(tile.biome);
            }
        }

        public static void ChangeBiomes()
        {
            System.Random r = new System.Random();

            foreach (Tile tile in Find.World.grid.tiles)
            {
                if (tile.biome == BiomeDefOf.TemperateForest || tile.biome == BiomeDefOf.TropicalRainforest)
                {
                    tile.biome = BiomeDefOf.BorealForest;
                }
                else if (tile.biome == VEE_DefOf.TemperateSwamp || tile.biome == VEE_DefOf.TropicalSwamp)
                {
                    tile.biome = VEE_DefOf.ColdBog;
                }
                else if (tile.biome == BiomeDefOf.Desert || tile.biome == VEE_DefOf.AridShrubland || tile.biome == VEE_DefOf.ExtremeDesert)
                {
                    tile.biome = BiomeDefOf.Tundra;
                }
                else if (tile.biome == BiomeDefOf.BorealForest || tile.biome == BiomeDefOf.Tundra || tile.biome == VEE_DefOf.ColdBog 
                    || tile.biome == BiomeDefOf.Ocean || tile.biome == BiomeDefOf.Lake || tile.biome == BiomeDefOf.IceSheet || tile.biome == BiomeDefOf.SeaIce)
                {
                }
                else  /* Modded biome */
                {
                    tile.biome = BiomeDefOf.Tundra;
                }
            }
        }

        public void ChangeTileTemp()
        {
            System.Random r = new System.Random();
            foreach (Tile tile in Find.World.grid.tiles)
            {
                    tile.temperature -= 4;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref TilesTemp, "TilesTemp", LookMode.Value);
            Scribe_Collections.Look(ref TilesBiome, "TilesBiome", LookMode.Def);
            Scribe_Values.Look<int>(ref tempChangeCounter, "tempChangeCounterIceAge", 0, false);
        }

        public override void Init()
        {
            if (VEEData.toRegenerate == null) VEEData.toRegenerate = new List<int>();

            base.Init();
            SaveTileTemp();
            SaveTileBiome();
            ChangeBiomes();

            for (int i = 0; i < Find.WorldGrid.TilesCount; i++)
            {
                VEEData.toRegenerate.Append(i);
            }

            Find.World.renderer.SetDirty<WorldLayerRegenerateBiomes>();

            VEE_DefOf.RaidEnemyPurple.targetTags.Add(IncidentTargetTagDefOf.Map_PlayerHome); // More raids
        }

        public override void End()
        {
            base.End();
            int i = 0;
            foreach (Tile tile in Find.World.grid.tiles)
            {
                tile.temperature = TilesTemp[i];
                tile.biome = TilesBiome[i];
                i++;
            }

            for (int e = 0; e < Find.WorldGrid.TilesCount; e++)
            {
                VEEData.toRegenerate.Append(e);
            }

            Find.World.renderer.SetDirty<WorldLayerRegenerateBiomes>();

            VEE_DefOf.RaidEnemyPurple.targetTags.Remove(IncidentTargetTagDefOf.Map_PlayerHome);
        }

        public int tempChangeCounter = 0;
        public override void GameConditionTick()
        {
            if (Find.TickManager.TicksGame % 60000 == 0 && tempChangeCounter < 6)
            {
                this.ChangeTileTemp();
                tempChangeCounter++;
            }
            base.GameConditionTick();
        }
    }
}
