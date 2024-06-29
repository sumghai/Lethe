using RimWorld;
using System.Linq;
using Verse;

namespace Lethe
{
    public class GameComponent_Lethe : GameComponent
    {
        public const int spawnIsoDataChipDelayDays = 1;

        public const int spawnIsoDataChipDelayTicks = spawnIsoDataChipDelayDays * GenDate.TicksPerDay;
        
        public int spawnIsoDataChipTick = -99999;

        public bool shouldSpawnIsoDataChip = true;

        public GameComponent_Lethe(Game game)
        { 
        }

        public override void GameComponentTick()
        {
            if (shouldSpawnIsoDataChip && Find.TickManager.TicksGame % GenTicks.TickLongInterval == 0 && LetheDefOf.AdvancedFabrication.IsFinished) 
            {
                spawnIsoDataChipTick = Find.TickManager.TicksGame + spawnIsoDataChipDelayTicks;
                shouldSpawnIsoDataChip = false;
            }

            if (spawnIsoDataChipTick > 0 && Find.TickManager.TicksGame > spawnIsoDataChipTick) 
            {
                SpawnIsoDataChipNearResearchBench();
                spawnIsoDataChipTick = -99999;
            }
        }

        public void SpawnIsoDataChipNearResearchBench()
        {
            Map spawnMap = Find.Maps.Where(m => m.IsPlayerHome).FirstOrDefault();
            IntVec3 spawnPos = spawnMap.listerBuildings.AllBuildingsColonistOfClass<Building_ResearchBench>().First().Position;
            Thing isoDataChip = ThingMaker.MakeThing(LetheDefOf.Lethe_Artifact_IsolinearDatachip);
            GenPlace.TryPlaceThing(isoDataChip, spawnPos, spawnMap, ThingPlaceMode.Near);
            Find.LetterStack.ReceiveLetter("Lethe_Letter_IsoDataChip".Translate(), "Lethe_Letter_IsoDataChipDesc".Translate(LetheDefOf.Lethe_JuryRiggedDatachipReader.LabelCap.Colorize(ColoredText.NameColor), LetheDefOf.Lethe_Cipher1.LabelCap.Colorize(ColoredText.NameColor)), LetterDefOf.NeutralEvent, isoDataChip);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref shouldSpawnIsoDataChip, "shouldSpawnIsoDataChip", defaultValue: false);
            Scribe_Values.Look(ref spawnIsoDataChipTick, "spawnIsoDataChipTick", 0, true);
        }
    }
}
