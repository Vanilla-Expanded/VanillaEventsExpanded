using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VEE.PurpleEvents
{
    public class GlobalWarming : PlanetEvent
    {
        public override void ChangeBiomes()
        {
            foreach (Tile tile in Find.World.grid.tiles)
            {
                if (tile.biome == BiomeDefOf.Tundra) tile.biome = BiomeDefOf.AridShrubland;
                else if (tile.biome == BiomeDefOf.AridShrubland) tile.biome = BiomeDefOf.Desert;
                else if (tile.biome == BiomeDefOf.Desert) tile.biome = BiomeDefOf.ExtremeDesert;
                else if (tile.biome == BiomeDefOf.BorealForest) tile.biome = BiomeDefOf.TemperateForest;
                else if (tile.biome == BiomeDefOf.TemperateForest) tile.biome = BiomeDefOf.TropicalRainforest;
                else if (tile.biome == BiomeDefOf.IceSheet) tile.biome = VEE_DefOf.ColdBog;
                else if (tile.biome == VEE_DefOf.ColdBog) tile.biome = VEE_DefOf.TemperateSwamp;
                else if (tile.biome == VEE_DefOf.TemperateSwamp) tile.biome = VEE_DefOf.TropicalSwamp;
            }

            Find.World.renderer = new WorldRenderer();
        }

        public override void ChangeBiomesDryness()
        {
            foreach (Tile tile in Find.World.grid.tiles)
            {
                if (tile.biome == VEE_DefOf.TropicalSwamp) tile.biome = BiomeDefOf.TropicalRainforest;
                else if (tile.biome == BiomeDefOf.TropicalRainforest) tile.biome = BiomeDefOf.Desert;
                else if (tile.biome == VEE_DefOf.TemperateSwamp) tile.biome = BiomeDefOf.TemperateForest;
                else if (tile.biome == BiomeDefOf.TemperateForest) tile.biome = BiomeDefOf.AridShrubland;
                else if (tile.biome == VEE_DefOf.ColdBog) tile.biome = BiomeDefOf.BorealForest;
                else if (tile.biome == BiomeDefOf.BorealForest) tile.biome = BiomeDefOf.Tundra;
            }

            Find.World.renderer = new WorldRenderer();
        }

        public override int ChangeTileTemp() => 5;
    }
}