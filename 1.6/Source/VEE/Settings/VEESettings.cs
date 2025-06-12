using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace VEE.Settings
{
    internal class VEESettings : ModSettings
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public Dictionary<string, bool> incidentsStatus = new Dictionary<string, bool>();
        public Dictionary<string, float> incidentsOccurence = new Dictionary<string, float>();

        public Dictionary<string, float> incidentsOccurenceForReset = new Dictionary<string, float>();

        internal int numberOfMods = 0;
        internal int incidentsLoaded = 0;
        internal int daysBetweenPurpleEvent = 300;
        internal string daysBetweenPurpleEventBuffer;

        private readonly float startPos = 48f;
        private readonly float offset = 10f;
        private readonly float lineHeight = 32f;
        private readonly float sLineHeight = 24f;
        private readonly float borderOffsest = 30f;

        public void DoSettingsWindowContents(Rect inRect)
        {
            float y = startPos;
            // Reset everything
            Rect resetRect = new Rect(inRect.x, y, inRect.width, sLineHeight);
            if (Widgets.ButtonText(resetRect, "BReset".Translate()))
            {
                for (int num = 0; num < incidentsStatus.Count; num++)
                {
                    string key = incidentsStatus.ElementAt(num).Key;
                    incidentsStatus[key] = true;
                }
                for (int num = 0; num < incidentsOccurence.Count; num++)
                {
                    string key = incidentsOccurence.ElementAt(num).Key;
                    incidentsOccurence[key] = incidentsOccurenceForReset[key];
                }
                daysBetweenPurpleEvent = 300;
                daysBetweenPurpleEventBuffer = "300";
            }
            y += offset + sLineHeight;
            // Stop conditions
            Rect fullRect = new Rect(inRect.x, y, inRect.width, sLineHeight);
            if (Current.Game != null)
            {
                ResetWorldCondButton(fullRect.LeftHalf().Rounded());
                ResetMapCondButton(fullRect.RightHalf().Rounded());
                y += offset + sLineHeight;
            }
            else
            {
                CenteredLabel(fullRect, "NeedToBeIngame".Translate());
                y += offset + sLineHeight;
            }
            // Days between purple events
            Rect entryLabelRect = new Rect(inRect.x, y, inRect.width, sLineHeight);
            Widgets.Label(entryLabelRect, "VEE_DaysBetweenPurpleEvents".Translate());
            y += sLineHeight;
            Rect entryRect = new Rect(inRect.x, y, inRect.width, sLineHeight);
            Widgets.IntEntry(entryRect, ref daysBetweenPurpleEvent, ref daysBetweenPurpleEventBuffer);
            y += offset + sLineHeight;
            // Incident settings
            Rect outRect = new Rect(inRect.x, y, inRect.width, inRect.height - (offset + sLineHeight) * 4);

            Rect viewRect = new Rect(inRect.x, y, inRect.width - borderOffsest, (incidentsLoaded + numberOfMods) * lineHeight);
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect, true);

            Listing_Standard lst = new Listing_Standard();
            lst.Begin(viewRect);

            string tempModName = "";
            for (int num = 0; num < incidentsStatus.Count; num++)
            {
                IncidentDef i = VEEData.tempDefs.Find(inc => inc.defName == incidentsStatus.ElementAt(num).Key);
                if (i != null)
                {
                    if (tempModName != i.modContentPack.Name)
                    {
                        tempModName = i.modContentPack.Name;
                        CenteredLabel(lst, tempModName);
                    }
                    bool status = incidentsStatus[i.defName];

                    Rect r = lst.GetRect(lineHeight);
                    if (status)
                    {
                        Rect rFHalf = r.LeftHalf();
                        Widgets.CheckboxLabeled(rFHalf, i.label.CapitalizeFirst(), ref status);
                        Rect rSHalf = r.RightHalf();
                        incidentsOccurence[i.defName] = Widgets.HorizontalSlider(rSHalf, incidentsOccurence[i.defName], 0.01f, 20f, true, $"Base chance: {incidentsOccurence[i.defName]}", roundTo: 0.01f);
                    }
                    else
                    {
                        Widgets.CheckboxLabeled(r, i != null ? i.label.CapitalizeFirst() : incidentsStatus.ElementAt(num).Key, ref status);
                    }
                    incidentsStatus[i.defName] = status;
                }
            }

            lst.End();
            Widgets.EndScrollView();
            base.Write();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref daysBetweenPurpleEvent, "daysBetweenPurpleEvent");
            Scribe_Collections.Look(ref incidentsStatus, "incidentsStatus", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref incidentsOccurence, "incidentsOccurence", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref incidentsOccurenceForReset, "incidentsOccurenceForReset", LookMode.Value, LookMode.Value);
        }

        private void ResetWorldCondButton(Rect leftRect)
        {
            if (Widgets.ButtonText(leftRect, "GCWResetOBO".Translate()) && Find.World != null)
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                if (Find.World.GameConditionManager != null && !Find.World.GameConditionManager.ActiveConditions.NullOrEmpty())
                {
                    var activeConditions = Find.World.GameConditionManager.ActiveConditions;
                    if (activeConditions.Count > 0)
                    {
                        foreach (GameCondition item in activeConditions)
                        {
                            if (!item.Permanent)
                                floatMenuOptions.Add(new FloatMenuOption(item.Label, () => item.End()));
                        }
                    }
                }

                if (floatMenuOptions.Count == 0) floatMenuOptions.Add(new FloatMenuOption("Nothing to end", null));
                Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
            }
        }

        private void ResetMapCondButton(Rect rightRect)
        {
            if (Widgets.ButtonText(rightRect, "GCMResetOBO".Translate()) && Find.CurrentMap != null)
            {
                var floatMenuOptions = new List<FloatMenuOption>();
                if (Find.CurrentMap.GameConditionManager != null && !Find.CurrentMap.GameConditionManager.ActiveConditions.NullOrEmpty())
                {
                    var activeConditions = Find.CurrentMap.GameConditionManager.ActiveConditions;
                    if (activeConditions.Count > 0)
                    {
                        foreach (GameCondition item in activeConditions)
                        {
                            if (!item.Permanent)
                                floatMenuOptions.Add(new FloatMenuOption(item.Label, () => item.End()));
                        }
                    }
                }

                if (floatMenuOptions.Count == 0) floatMenuOptions.Add(new FloatMenuOption("Nothing to end", null));
                Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
            }
        }

        private void CenteredLabel(Rect rect, string stringKey)
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, stringKey);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void CenteredLabel(Listing_Standard lst, string stringKey)
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            lst.Label(stringKey);
            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}