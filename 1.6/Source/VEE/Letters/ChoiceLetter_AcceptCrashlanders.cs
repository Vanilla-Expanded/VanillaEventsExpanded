using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;
namespace VEE
{
    public class ChoiceLetter_AcceptCrashlanders : ChoiceLetter
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
                           
                            p.SetFaction(Faction.OfPlayer);
                        }
                      
                        Find.LetterStack.RemoveLetter(this);
                    },
                    resolveTree = true
                };
                var reject = new DiaOption("RejectLetter".Translate())
                {
                    action = () =>
                    {
                       
                        Find.LetterStack.RemoveLetter(this);
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