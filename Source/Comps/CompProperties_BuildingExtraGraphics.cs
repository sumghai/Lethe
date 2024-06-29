using Verse;

namespace Lethe
{
    public class CompProperties_BuildingExtraGraphics : CompProperties
    {
        public GraphicData extraGraphicData;

        public GraphicData glowGraphicData;

        public CompProperties_BuildingExtraGraphics() 
        {
            compClass = typeof(CompBuildingExtraGraphics);
        }
    }
}
