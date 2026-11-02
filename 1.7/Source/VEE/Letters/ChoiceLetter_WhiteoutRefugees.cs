using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VEE
{
    public class ChoiceLetter_WhiteoutRefugees : ChoiceLetter
    {
        public List<Pawn> pawns;
        public Map map;
        public IntVec3 spawnSpot;
        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                var accept = new DiaOption("AcceptButton".Translate())
                {
                    action = () =>
                    {
                        foreach (var p in pawns)
                        {
                            Find.WorldPawns.RemovePawn(p);
                            GenSpawn.Spawn(p, spawnSpot, map);
                            p.SetFaction(Faction.OfPlayer);
                        }
                        Messages.Message("VEE_WhiteoutRefugees_Accepted".Translate(), pawns, MessageTypeDefOf.PositiveEvent);
                        Find.LetterStack.RemoveLetter(this);
                    },
                    resolveTree = true
                };
                var reject = new DiaOption("RejectLetter".Translate())
                {
                    action = () =>
                    {
                        var parms = new FactionGeneratorParms(FactionDefOf.Pirate, default, true);
                        var hostile = FactionGenerator.NewGeneratedFactionWithRelations(parms, new List<FactionRelation>());
                        hostile.temporary = true;
                        Find.FactionManager.Add(hostile);
                        foreach (var p in pawns)
                        {
                            Find.WorldPawns.RemovePawn(p);
                            GenSpawn.Spawn(p, spawnSpot, map);
                            p.SetFaction(hostile);
                        }
                        LordMaker.MakeNewLord(hostile, new LordJob_AssaultColony(hostile), map, pawns);
                        Messages.Message("VEE_WhiteoutRefugees_Rejected".Translate(), pawns, MessageTypeDefOf.NegativeEvent);
                        Find.LetterStack.RemoveLetter(this);
                        if (ModsConfig.IdeologyActive)
                        {
                            Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.CharityRefused_Beggars));

                        }
                    },
                    resolveTree = true
                };
                yield return accept;
                yield return reject;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref pawns, "pawns", LookMode.Reference);
            Scribe_References.Look(ref map, "map");
            Scribe_Values.Look(ref spawnSpot, "spawnSpot");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                pawns ??= new List<Pawn>();
                pawns.RemoveAll(x => x == null);
            }
        }
    }
}
