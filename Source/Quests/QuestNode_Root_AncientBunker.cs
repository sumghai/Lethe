using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using Verse;

namespace Lethe
{
    public class QuestNode_Root_AncientBunker : QuestNode
    {
        public const int MinDistanceFromColony = 20;

        public const int MaxDistanceFromColony = 200;

        public const Hilliness minHilliness = Hilliness.Mountainous;

        public Predicate<PlanetTile> TileValidator => (PlanetTile tile) =>
            !Find.WorldObjects.AnyWorldObjectAt(tile)
            && !Find.World.HasCaves(tile)
            && Find.WorldGrid[tile].hilliness == minHilliness
            && TileFinder.IsValidTileForNewSettlement(tile, null);

        public override void RunInt()
        {
            Slate slate = QuestGen.slate;
            Quest quest = QuestGen.quest;

            if (!TryFindSiteTile(out var tile))
            {
                Log.Error("Lethe :: Could not find valid site tile for ancient bunker quest.");
                return;
            }

            string inSignal = QuestGenUtility.HardcodedSignalWithQuestID("site.MapGenerated");
            string inSignal2 = QuestGenUtility.HardcodedSignalWithQuestID("site.MapRemoved");

            Site site = QuestGen_Sites.GenerateSite(
            [
                new SitePartDefWithParams(LetheDefOf.Lethe_AncientBunker, new SitePartParams
                {
                    points = slate.Get("points", 0f),
                    threatPoints = slate.Get("points", 0f)
                })
            ], tile, null);
            slate.Set("site", site);
            quest.SpawnWorldObject(site);
            quest.End(QuestEndOutcome.Success, 0, null, inSignal);
            quest.End(QuestEndOutcome.Unknown, 0, null, inSignal2);
        }

        public override bool TestRunInt(Slate slate)
        {
            return true;
        }

        public bool TryFindSiteTile(out PlanetTile tile)
        {
            return TileFinder.TryFindNewSiteTile(out tile, MinDistanceFromColony, MaxDistanceFromColony, exitOnFirstTileFound: true, validator: TileValidator);
        }
    }
}
