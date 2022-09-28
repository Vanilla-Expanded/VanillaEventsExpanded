using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace VEE.Settings
{
    [StaticConstructorOnStartup]
    internal class VEEData
    {
        internal static List<IncidentDef> tempDefs = new List<IncidentDef>();
    }

    internal class VEEMod : Mod
    {
        public static VEESettings settings;

        public VEEMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<VEESettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory() => "Vanilla Events Expanded";

        public override void WriteSettings()
        {
            base.WriteSettings();
            DefsAlterer.DoAlteration();
        }
    }
}