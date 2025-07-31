using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

#if v1_5
using PlanetTile = int;
#endif

namespace Lethe;

public class CaravanArrivalAction_AncientBunker : CaravanArrivalAction
{
    private MapParent target;

    public override string Label => "Lethe_VisitAncientBunker".Translate(target.Label);

    public override string ReportString => "CaravanVisiting".Translate(target.Label);

    public CaravanArrivalAction_AncientBunker()
    {
    }

    public CaravanArrivalAction_AncientBunker(AncientBunkerComp ancientBunker)
    {
        target = (MapParent)ancientBunker.parent;
    }

    public override FloatMenuAcceptanceReport StillValid(Caravan caravan, PlanetTile destinationTile)
    {
        FloatMenuAcceptanceReport floatMenuAcceptanceReport = base.StillValid(caravan, destinationTile);
        return !floatMenuAcceptanceReport
            ? floatMenuAcceptanceReport
            : (target != null && target.Tile != destinationTile
                ? false
                : CanVisit(caravan, target));
    }

    public override void Arrived(Caravan caravan)
    {
        if (!target.HasMap)
        {
            LongEventHandler.QueueLongEvent(delegate
            {
                DoArrivalAction(caravan);
            }, "Lethe_GeneratingMapForAncientBunker", doAsynchronously: false, null);
        }
        else
        {
            DoArrivalAction(caravan);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref target, "target");
    }

    private void DoArrivalAction(Caravan caravan)
    {
        bool hasMap = target.HasMap;
        Map map = GetOrGenerateMapUtility.GetOrGenerateMap(target.Tile, null);
        LookTargets lookTargets = new(caravan.PawnsListForReading);
        CaravanEnterMapUtility.Enter(caravan, map, CaravanEnterMode.Edge, CaravanDropInventoryMode.DoNotDrop);
        if (!hasMap)
        {
            GenStep_AncientBunker.PostCaravanEntered(map, out IntVec3 bunkerEntrance);

            Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
            Find.LetterStack.ReceiveLetter("Lethe_AncientBunkerFoundLabel".Translate(), (!Find.Storyteller.difficulty.allowBigThreats) ? "Lethe_AncientBunkerFoundPeaceful".Translate() : "Lethe_AncientBunkerFound".Translate(), LetterDefOf.PositiveEvent, new GlobalTargetInfo(bunkerEntrance, target.Map));
        }
        else
        {
            Find.LetterStack.ReceiveLetter("Lethe_LetterLabelCaravanEnteredMap".Translate(target), "Lethe_LetterCaravanEnteredMap".Translate(caravan.Label, target).CapitalizeFirst(), LetterDefOf.NeutralEvent, lookTargets);
        }
    }

    public static FloatMenuAcceptanceReport CanVisit(Caravan _, MapParent mapParent)
    {
        return mapParent != null && mapParent.Spawned && mapParent.GetComponent<AncientBunkerComp>() != null;
    }

    public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
    {
        return CaravanArrivalActionUtility.GetFloatMenuOptions(
            () => CanVisit(caravan, mapParent),
            () => new CaravanArrivalAction_AncientBunker(mapParent.GetComponent<AncientBunkerComp>()),
            "Lethe_VisitAncientBunker".Translate(mapParent.Label),
            caravan,
            mapParent.Tile,
            mapParent);
    }
}
