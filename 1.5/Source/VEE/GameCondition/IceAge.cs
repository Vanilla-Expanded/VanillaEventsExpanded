using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VEE.PurpleEvents
{
    public class IceAge : PlanetEvent
    {
        public override void ChangeBiomes()
        {
            foreach (Tile tile in Find.World.grid.tiles)
            {
                if (tile.biome == VEE_DefOf.ExtremeDesert) tile.biome = BiomeDefOf.Desert;
                else if (tile.biome == BiomeDefOf.Desert) tile.biome = VEE_DefOf.AridShrubland;
                else if (tile.biome == VEE_DefOf.AridShrubland) tile.biome = BiomeDefOf.Tundra;
                else if (tile.biome == VEE_DefOf.TropicalRainforest) tile.biome = BiomeDefOf.TemperateForest;
                else if (tile.biome == BiomeDefOf.TemperateForest) tile.biome = BiomeDefOf.BorealForest;
                else if (tile.biome == VEE_DefOf.TropicalSwamp) tile.biome = VEE_DefOf.TemperateSwamp;
                else if (tile.biome == VEE_DefOf.TemperateSwamp) tile.biome = VEE_DefOf.ColdBog;
                else if (tile.biome == VEE_DefOf.ColdBog) tile.biome = BiomeDefOf.IceSheet;
            }

            Find.World.renderer = new WorldRenderer();
        }

        public override void ChangeBiomesDryness()
        {
            foreach (Tile tile in Find.World.grid.tiles)
            {
                if (tile.biome == BiomeDefOf.Tundra) tile.biome = BiomeDefOf.BorealForest;
                else if (tile.biome == BiomeDefOf.BorealForest) tile.biome = VEE_DefOf.ColdBog;
                else if (tile.biome == VEE_DefOf.AridShrubland) tile.biome = BiomeDefOf.TemperateForest;
                else if (tile.biome == BiomeDefOf.TemperateForest) tile.biome = VEE_DefOf.TemperateSwamp;
                else if (tile.biome == BiomeDefOf.Desert) tile.biome = VEE_DefOf.TropicalRainforest;
                else if (tile.biome == VEE_DefOf.TropicalRainforest) tile.biome = VEE_DefOf.TropicalSwamp;
            }

            Find.World.renderer = new WorldRenderer();
        }

        public override int ChangeTileTemp() => -5;
    }
}