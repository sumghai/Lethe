
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;

namespace Lethe;

public class WorldObjectCompProperties_AncientBunker : WorldObjectCompProperties
{
    public WorldObjectCompProperties_AncientBunker()
    {
        compClass = typeof(AncientBunkerComp);
    }

    public override IEnumerable<string> ConfigErrors(WorldObjectDef parentDef)
    {
        foreach (string item in base.ConfigErrors(parentDef))
        {
            yield return item;
        }
        if (!typeof(MapParent).IsAssignableFrom(parentDef.worldObjectClass))
        {
            yield return parentDef.defName + " has WorldObjectCompProperties_AncientBunker but it's not MapParent.";
        }
    }
}
