using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VEE.Settings
{
    [StaticConstructorOnStartup]
    internal class DefsAlterer
    {
        static DefsAlterer()
        {
            Setup();
            DoAlteration();
        }

        public static void AddDef(IncidentDef def)
        {
            if (DefDatabase<IncidentDef>.GetNamedSilentFail(def.defName) == null)
                DefDatabase<IncidentDef>.Add(def);
        }

        public static void RemoveDef(IncidentDef def)
        {
            if (def != null)
            {
                try
                {
                    MethodInfo methodInfo = AccessTools.Method(typeof(DefDatabase<IncidentDef>), "Remove", null, null);
                    methodInfo.Invoke(null, new object[] { def });
                }
                catch { };
            }
        }

        public static void DoAlteration()
        {
            foreach (var k in VEEMod.settings.incidentsStatus)
            {
                if (k.Value)
                {
                    IncidentDef incidentDef = VEEData.tempDefs.Find(d => d.defName == k.Key);
                    if (incidentDef != null)
                        AddDef(incidentDef);
                }
                else
                {
                    RemoveDef(DefDatabase<IncidentDef>.GetNamedSilentFail(k.Key));
                }
            }

            foreach (var k in VEEMod.settings.incidentsOccurence)
            {
                if (DefDatabase<IncidentDef>.GetNamedSilentFail(k.Key) != null)
                    DefDatabase<IncidentDef>.GetNamed(k.Key).baseChance = k.Value;
            }
        }

        public static void Setup()
        {
            if (VEEMod.settings.incidentsStatus == null)
                VEEMod.settings.incidentsStatus = new Dictionary<string, bool>();

            if (VEEMod.settings.incidentsOccurence == null)
                VEEMod.settings.incidentsOccurence = new Dictionary<string, float>();

            if (VEEMod.settings.incidentsOccurenceForReset == null)
                VEEMod.settings.incidentsOccurenceForReset = new Dictionary<string, float>();

            VEEData.tempDefs = DefDatabase<IncidentDef>.AllDefsListForReading.FindAll(i => i.category != IncidentCategoryDefOf.GiveQuest && i.baseChance > 0 && (i.targetTags.Contains(IncidentTargetTagDefOf.Map_PlayerHome) || i.targetTags.Contains(IncidentTargetTagDefOf.World))).OrderBy(x => x.modContentPack.Name).ToList();

            string tmpMod = "";
            foreach (IncidentDef i in VEEData.tempDefs)
            {
                if (tmpMod != i.modContentPack.Name)
                {
                    tmpMod = i.modContentPack.Name;
                    VEEMod.settings.numberOfMods++;
                }

                if (!VEEMod.settings.incidentsStatus.ContainsKey(i.defName))
                {
                    VEEMod.settings.incidentsStatus.Add(i.defName, true);
                    VEEMod.settings.incidentsLoaded++;
                }
                else
                {
                    VEEMod.settings.incidentsLoaded++;
                }

                if (!VEEMod.settings.incidentsOccurence.ContainsKey(i.defName))
                    VEEMod.settings.incidentsOccurence.Add(i.defName, i.baseChance);

                if (!VEEMod.settings.incidentsOccurenceForReset.ContainsKey(i.defName))
                    VEEMod.settings.incidentsOccurenceForReset.Add(i.defName, i.baseChance);
            }
        }
    }
}