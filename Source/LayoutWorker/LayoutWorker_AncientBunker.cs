using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Lethe
{
    // TODO: Don't inherit from LayoutWorker_Labyrinth, but instead use a custom layout worker for
    // the ancient bunker, as 1) it's DLC locked for Anomaly and 2) it probably has a different
    // layout than we actually want.
    [StaticConstructorOnStartup]
    public class LayoutWorker_AncientBunker : LayoutWorker_Labyrinth
    {
        public static readonly ThingDef WallDef;
        public static readonly ThingDef DoorDef;
        public static readonly TerrainDef FloorTerrain;

        public static readonly ThingDef WallStuff;
        public static readonly ThingDef DoorStuff;

        public LayoutWorker_AncientBunker(LayoutDef def) : base(def)
        {
            /*WallDef = ThingDefOf.GrayWall;
            DoorDef = ThingDefOf.GrayDoor;
            FloorTerrain = TerrainDefOf.GraySurface;

            WallStuff = ThingDefOf.LabyrinthMatter;
            DoorStuff = ThingDefOf.LabyrinthMatter;*/
        }

        public override LayoutSketch GenerateSketch(StructureGenParams parms)
        {
            LayoutSketch layoutSketch = new()
            {
                wall = WallDef,
                door = DoorDef,
                floor = FloorTerrain,
                defaultAffordanceTerrain = FloorTerrain,
                wallStuff = WallStuff,
                doorStuff = DoorStuff
            };
            using (new ProfilerBlock("Generate Ancient Bunker"))
            {
                layoutSketch.structureLayout = Generate(parms);
                return layoutSketch;
            }
        }

        private StructureLayout Generate(StructureGenParams parms)
        {
            CellRect cellRect = new(0, 0, parms.size.x, parms.size.z);
            StructureLayout structureLayout = new(parms.sketch, cellRect);

            using (new ProfilerBlock("Scatter L Rooms"))
            {
                ScatterLRooms(cellRect, structureLayout);
            }
            using (new ProfilerBlock("Scatter Square Rooms"))
            {
                ScatterSquareRooms(cellRect, structureLayout);
            }
            using (new ProfilerBlock("Generate Graphs"))
            {
                GenerateGraphs(structureLayout);
            }
            structureLayout.FinalizeRooms(avoidDoubleWalls: false);
            using (new ProfilerBlock("Create Doors"))
            {
                CreateDoors(structureLayout);
            }
            using (new ProfilerBlock("Create Corridors"))
            {
                CreateCorridorsAStar(structureLayout);
            }
            using (new ProfilerBlock("Fill Empty Spaces"))
            {
                FillEmptySpaces(structureLayout);
            }

            return structureLayout;
        }

        public override void Spawn(LayoutStructureSketch layoutStructureSketch, Map map, IntVec3 pos, float? threatPoints = null, List<Thing> allSpawnedThings = null, bool roofs = true, bool canReuseSketch = false, Faction faction = null)
        {
            // Destroy any impassable edifices in the layout sketch, so that the bunker is cleared of obstacles.
            Thing.allowDestroyNonDestroyable = true;
            LayoutSketch layoutSketch = layoutStructureSketch.layoutSketch;
            List<SketchTerrain> cachedTerrain = layoutSketch.cachedTerrain;
            IntVec3 offset = layoutSketch.GetOffset(pos, Sketch.SpawnPosType.Unchanged);
            for (int i = 0; i < cachedTerrain.Count; i++)
            {
                Building edifice = (cachedTerrain[i].pos + offset).GetEdifice(map);
                if (edifice != null && edifice.def.passability == Traversability.Impassable)
                {
                    edifice.Destroy();
                }
            }
            Thing.allowDestroyNonDestroyable = false;

            base.Spawn(layoutStructureSketch, map, pos, threatPoints, allSpawnedThings, roofs, canReuseSketch, faction);
        }
    }
}