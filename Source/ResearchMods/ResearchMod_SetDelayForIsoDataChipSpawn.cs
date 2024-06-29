using Verse;

namespace Lethe
{
    internal class ResearchMod_SetDelayForIsoDataChipSpawn : ResearchMod
    {
        public override void Apply()
        {
            GameComponent_Lethe.Instance.spawnIsoDataChipTick = Find.TickManager.TicksGame + GameComponent_Lethe.spawnIsoDataChipDelayTicks;
            GameComponent_Lethe.Instance.shouldSpawnIsoDataChip = false;
        }
    }
}
