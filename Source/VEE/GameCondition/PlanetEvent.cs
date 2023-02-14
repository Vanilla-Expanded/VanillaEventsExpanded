using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VEE.PurpleEvents
{
    public abstract class PlanetEvent : GameCondition
    {
        public List<BiomeDef> TilesBiome = new List<BiomeDef>();
        public List<float> TilesTemp = new List<float>();
        private int biomeTempChange = 0;

        private bool boolNoSave = true;

        public abstract void ChangeBiomes();

        public abstract void ChangeBiomesDryness();

        public abstract int ChangeTileTemp();

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

            Find.World.renderer = new WorldRenderer();
            DefDatabase<IncidentDef>.GetNamed(VEE_DefOf.RaidEnemyPurple.defName).targetTags.Remove(IncidentTargetTagDefOf.Map_PlayerHome);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref TilesTemp, "TilesTemp", LookMode.Value);
            Scribe_Collections.Look(ref TilesBiome, "TilesBiome", LookMode.Def);
            Scribe_Values.Look(ref biomeTempChange, "biomeTempChange", 0, false);
        }

        public override void GameConditionTick()
        {
            if (Find.TickManager.TicksGame % 240000 == 0 && biomeTempChange < 5)
            {
                Find.World.grid.tiles.ForEach(t => t.temperature += ChangeTileTemp());
                if (biomeTempChange == 1)
                {
                    ChangeBiomes();
                }
                /*else if (biomeTempChange == 2)
                {
                    this.ChangeBiomesDryness();
                }*/
                biomeTempChange++;
            }

            if (TicksLeft.TicksToDays() == 8 || TicksLeft.TicksToDays() == 4)
            {
                Find.World.grid.tiles.ForEach(t => t.temperature -= ChangeTileTemp());
            }

            if (boolNoSave)
            {
                DefDatabase<IncidentDef>.GetNamed(VEE_DefOf.RaidEnemyPurple.defName).targetTags.Add(IncidentTargetTagDefOf.Map_PlayerHome);
                boolNoSave = false;
            }

            base.GameConditionTick();
        }

        public override void Init()
        {
            base.Init();
            SaveTileTemp();
            SaveTileBiome();

            Find.World.grid.tiles.ForEach(t => t.temperature += ChangeTileTemp());
            biomeTempChange++;

            DefDatabase<IncidentDef>.GetNamed(VEE_DefOf.RaidEnemyPurple.defName).targetTags.Add(IncidentTargetTagDefOf.Map_PlayerHome);
        }

        public void SaveTileBiome()
        {
            foreach (Tile tile in Find.World.grid.tiles)
            {
                TilesBiome.Add(tile.biome);
            }
        }

        public void SaveTileTemp()
        {
            foreach (Tile tile in Find.World.grid.tiles)
            {
                TilesTemp.Add(tile.temperature);
            }
        }
    }
}