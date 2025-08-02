using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Lethe
{
    // todo - Basically a copy of RimWorld.GenStep_Labyrinth for now
    public class GenStep_AncientBunker : GenStep
    {
        private LayoutStructureSketch structureSketch;

        public override int SeedPart => 4206984;

        private static readonly List<Thing> _allSpawnedThings = new();
        public override void Generate(Map map, GenStepParams parms)
        {
            CellRect rect = map.BoundsRect();
            CellRect cellRect = rect.ContractedBy(10);
            StructureGenParams ancientBunkerGenParams = new()
            {
                size = cellRect.Size,
            };
            LayoutWorker worker = LetheDefOf.Lethe_AncientBunker.Worker;
            structureSketch = worker.GenerateStructureSketch(ancientBunkerGenParams);

            IntVec3 layoutCenter = new(0, 0, 0);
            worker.Spawn(structureSketch, map, layoutCenter, null, _allSpawnedThings);
            map.layoutStructureSketches.Add(structureSketch);

            MapGenerator.PlayerStartSpot = IntVec3.Zero;
        }

        // XXX: This is just a hack to put a door on the otherwise "unenterable" Labyrinth.
        // The real map generation should probably add the entrance doors at the map generation
        // stage, not after the caravan has entered like this.
        public static void PostCaravanEntered(Map map, out IntVec3 bunkerEntrance)
        {
            bunkerEntrance = map.Center;

            Pawn colonist = map.mapPawns.FreeColonistsAndPrisoners.First();
            List<IntVec3> tmpPositions = new();

            bool addedDoor = false;
            Thing.allowDestroyNonDestroyable = true;
            foreach (Thing item in _allSpawnedThings.InRandomOrder())
            {
                if (item.Fogged() || !item.def.IsWall) { continue; }

                tmpPositions.Clear();
                tmpPositions.AddRange(item.CellsAdjacent8WayAndInside().Where(c => c.CardinalTo(item.Position) && c.InBounds(map) && c.GetEdifice(map) is null && c.GetRoom(map).PsychologicallyOutdoors));

                IntVec3 itemPosition = item.Position;
                foreach (IntVec3 position in tmpPositions)
                {
                    // What direction will the door be in?
                    Rot4 direction = Rot4.FromIntVec3(position - itemPosition);
                    Rot4 oppositeDirection = direction.Opposite;

                    // Is there a room behind the door? If we find an open space no more than 3 cells away, we assume it's a room.
                    bool foundRoom = false;
                    DoInLineFromPositionInDirection(itemPosition, oppositeDirection, 3, (behindDoorPosition) =>
                    {
                        Building edifice = behindDoorPosition.GetEdifice(map);
                        if (edifice is null || edifice.def.passability != Traversability.Impassable)
                        {
                            foundRoom = true;
                            return false;
                        }
                        return true;
                    });
                    if (!foundRoom) { continue; }

                    // Make sure the three spots in front of the door are free, not just the one directly in front.
                    Rot4 left = direction.Rotated(RotationDirection.Counterclockwise);
                    Rot4 right = direction.Rotated(RotationDirection.Clockwise);
                    IntVec3 leftPosition = position + left.AsIntVec3;
                    IntVec3 rightPosition = position + right.AsIntVec3;
                    if (leftPosition.GetEdifice(map) is not null || !leftPosition.GetRoom(map).PsychologicallyOutdoors ||
                        rightPosition.GetEdifice(map) is not null || !rightPosition.GetRoom(map).PsychologicallyOutdoors)
                    {
                        continue;
                    }

                    // Replace the chosen wall with the door.
                    if (!map.roofGrid.Roofed(itemPosition))
                    {
                        map.roofGrid.SetRoof(itemPosition, RoofDefOf.RoofConstructed);
                    }
                    item.Destroy(DestroyMode.WillReplace);
                    Thing door = GenSpawn.Spawn(ThingMaker.MakeThing(LayoutWorker_AncientBunker.DoorDef, LayoutWorker_AncientBunker.DoorStuff), itemPosition, map, WipeMode.Vanish);
                    if (door is Building_JammedDoor jammedDoor)
                    {
                        // Always make the door jammed, as the door by default has a 30% chance to be unjammed.
                        jammedDoor.jammed = true;
                    }
                    addedDoor = true;
                    bunkerEntrance = itemPosition;

                    // Put some floor tiles in front of the door for aesthetics.
                    map.terrainGrid.SetTerrain(position, LayoutWorker_AncientBunker.FloorTerrain);
                    map.terrainGrid.SetTerrain(leftPosition, LayoutWorker_AncientBunker.FloorTerrain);
                    map.terrainGrid.SetTerrain(rightPosition, LayoutWorker_AncientBunker.FloorTerrain);

                    // If there's a wall "behind" the door, we need to clear it out, too.
                    DoInLineFromPositionInDirection(itemPosition, oppositeDirection, 3, (behindDoorPosition) =>
                    {
                        if (behindDoorPosition.GetEdifice(map) is { } edifice && edifice.def.passability == Traversability.Impassable)
                        {
                            // Destroy the wall behind the door, so that it doesn't block the entrance.
                            if (!map.roofGrid.Roofed(behindDoorPosition))
                            {
                                map.roofGrid.SetRoof(behindDoorPosition, RoofDefOf.RoofConstructed);
                            }
                            edifice.Destroy(DestroyMode.WillReplace);
                            map.terrainGrid.SetTerrain(behindDoorPosition, LayoutWorker_AncientBunker.FloorTerrain);
                            return true;
                        }
                        return false;
                    });

                    // Jump the camera to the colonists, instead of being centered on the map.
                    map.rememberedCameraPos.rootPos = colonist.Position.ToVector3Shifted();
                    break;
                }
                if (addedDoor) { break; }
            }
            Thing.allowDestroyNonDestroyable = false;

            _allSpawnedThings.Clear();

        }

        private static void DoInLineFromPositionInDirection(IntVec3 position, Rot4 direction, int times, Func<IntVec3, bool> positionCallback)
        {
            for (int i = 0; i < times; i++)
            {
                IntVec3 directionPosition = position + (direction.AsIntVec3 * (i + 1));
                if (!positionCallback(directionPosition))
                {
                    break;
                }
            }
        }
    }
}