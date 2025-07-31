using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

#if v1_5
using PlanetTile = int;
#endif

namespace Lethe
{
    public class QuestNode_AncientBunker_GetBunkerTile : QuestNode
    {
        public const int MinDist = 10;

        public const int MaxDist = 200;
        
        [NoTranslate]
        public SlateRef<string> storeAs;

        public override bool TestRunInt(Slate slate)
        {
            if (!TryFindTile(slate, out PlanetTile tile))
            {
                return false;
            } 
            slate.Set(storeAs.GetValue(slate), tile);
            return true;
        }

        public override void RunInt()
        {
            Slate slate = QuestGen.slate;
            if (!slate.TryGet<PlanetTile>(storeAs.GetValue(slate), out PlanetTile _) && TryFindTile(QuestGen.slate, out PlanetTile tile))
            {
                QuestGen.slate.Set(storeAs.GetValue(slate), tile);
            }
        }

        public bool TryFindTile(Slate slate, out PlanetTile tile)
        {
            Map map = slate.Get<Map>("map", (Map)null, false) ?? Find.RandomPlayerHomeMap;

            static int findTile(int root)
            {
                // Tile Validator:
                // - No other existing world objects
                // - No caves
                // - Must be mountainous
                // - Must be otherwise suitable for new settlements
                static bool validator(PlanetTile x) =>
                    !Find.WorldObjects.AnyWorldObjectAt(x)
                    && !Find.World.HasCaves(x)
                    && Find.WorldGrid[x].hilliness == Hilliness.Mountainous
                    && TileFinder.IsValidTileForNewSettlement(x, null);
                if (TileFinder.TryFindPassableTileWithTraversalDistance(root, MinDist, MaxDist, out PlanetTile tile, validator))
                {
                    return tile;
                }
                return -1;
            }

            if (!TileFinder.TryFindRandomPlayerTile(out PlanetTile arg, true, (PlanetTile x) => findTile(x) != -1))
            {
                tile = -1;
                return false;
            }
            tile = findTile(arg);
            return tile != -1;
        }
    }
}
