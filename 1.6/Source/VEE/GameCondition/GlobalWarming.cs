using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VEE.PurpleEvents
{
    public class GlobalWarming : PlanetEvent
    {
        public override void ChangeBiomes()
        {
            foreach (Tile tile in Find.World.grid.Tiles)
            {
                if (tile.PrimaryBiome == BiomeDefOf.Tundra) tile.PrimaryBiome = VEE_DefOf.AridShrubland;
                else if (tile.PrimaryBiome == VEE_DefOf.AridShrubland) tile.PrimaryBiome = BiomeDefOf.Desert;
                else if (tile.PrimaryBiome == BiomeDefOf.Desert) tile.PrimaryBiome = VEE_DefOf.ExtremeDesert;
                else if (tile.PrimaryBiome == BiomeDefOf.BorealForest) tile.PrimaryBiome = BiomeDefOf.TemperateForest;
                else if (tile.PrimaryBiome == BiomeDefOf.TemperateForest) tile.PrimaryBiome = VEE_DefOf.TropicalRainforest;
                else if (tile.PrimaryBiome == BiomeDefOf.IceSheet) tile.PrimaryBiome = VEE_DefOf.ColdBog;
                else if (tile.PrimaryBiome == VEE_DefOf.ColdBog) tile.PrimaryBiome = VEE_DefOf.TemperateSwamp;
                else if (tile.PrimaryBiome == VEE_DefOf.TemperateSwamp) tile.PrimaryBiome = VEE_DefOf.TropicalSwamp;
            }

            Find.World.renderer = new WorldRenderer();
        }

        public override void ChangeBiomesDryness()
        {
            foreach (Tile tile in Find.World.grid.Tiles)
            {
                if (tile.PrimaryBiome == VEE_DefOf.TropicalSwamp) tile.PrimaryBiome = VEE_DefOf.TropicalRainforest;
                else if (tile.PrimaryBiome == VEE_DefOf.TropicalRainforest) tile.PrimaryBiome = BiomeDefOf.Desert;
                else if (tile.PrimaryBiome == VEE_DefOf.TemperateSwamp) tile.PrimaryBiome = BiomeDefOf.TemperateForest;
                else if (tile.PrimaryBiome == BiomeDefOf.TemperateForest) tile.PrimaryBiome = VEE_DefOf.AridShrubland;
                else if (tile.PrimaryBiome == VEE_DefOf.ColdBog) tile.PrimaryBiome = BiomeDefOf.BorealForest;
                else if (tile.PrimaryBiome == BiomeDefOf.BorealForest) tile.PrimaryBiome = BiomeDefOf.Tundra;
            }

            Find.World.renderer = new WorldRenderer();
        }

        public override int ChangeTileTemp() => 5;
    }
}