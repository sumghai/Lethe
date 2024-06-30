using Verse;

namespace Lethe
{
    public class ResearchMod_SetDelayForIsoDataChipSpawn : ResearchMod
    {
        public override void Apply()
        {
            if (!GameComponent_Lethe.Instance.spawnedIsoDataChip && GameComponent_Lethe.Instance.spawnIsoDataChipTick < 0)
            {
                GameComponent_Lethe.Instance.spawnIsoDataChipTick = Find.TickManager.TicksGame + GameComponent_Lethe.spawnIsoDataChipDelayTicks;
            }
        }
    }
}
