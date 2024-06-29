using RimWorld;
using UnityEngine;
using Verse;

namespace Lethe
{
    public class CompBuildingExtraGraphics : ThingComp
    {
        public CompProperties_BuildingExtraGraphics Props
        {
            get
            {
                return (CompProperties_BuildingExtraGraphics)props;
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();

            GraphicData extraGraphicData = Props.extraGraphicData;

            if (extraGraphicData != null)
            {
                Mesh extraMesh = extraGraphicData.Graphic.MeshAt(parent.Rotation);
                Graphics.DrawMesh(extraMesh, parent.DrawPos + extraGraphicData.drawOffset, Quaternion.identity, extraGraphicData.Graphic.GetColoredVersion(extraGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(parent.Rotation), 0);
            }

            GraphicData glowGraphicData = Props.glowGraphicData;

            if (glowGraphicData != null && parent.GetComp<CompPowerTrader>().PowerOn)
            {
                Mesh glowMesh = glowGraphicData.Graphic.MeshAt(parent.Rotation);
                Graphics.DrawMesh(glowMesh, parent.DrawPos + glowGraphicData.drawOffset, Quaternion.identity, FadedMaterialPool.FadedVersionOf(glowGraphicData.Graphic.MatAt(parent.Rotation, null), 1), 0);
            }
        }
    }
}
