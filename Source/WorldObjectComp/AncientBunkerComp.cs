using System.Collections.Generic;
using RimWorld.Planet;
using Verse;

namespace Lethe
{
    public class AncientBunkerComp : WorldObjectComp
    {
        // This is the magic sauce that allows a caravan to enter the world object as a map -- when you
        // mark a tile as the destination, the first choice in the float menu is what happens when you
        // arrive at the tile. It's also how you can get a float menu options to appear
        // when you right-click the world object on the world map in the first place.
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            foreach (FloatMenuOption floatMenuOption in CaravanArrivalAction_AncientBunker.GetFloatMenuOptions(caravan, (MapParent)parent))
            {
                yield return floatMenuOption;
            }
        }
    }
}