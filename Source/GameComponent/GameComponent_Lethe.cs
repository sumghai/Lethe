using LudeonTK;
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

        public bool spawnedIsoDataChip = false;

        public bool triggeredAncientBunkerQuest = false;

        public static GameComponent_Lethe Instance;

        public GameComponent_Lethe()
        {
            Instance = this;
        }

        public GameComponent_Lethe(Game game)
        {
            Instance = this;
        }

        public void PreInit()
        {
            Instance = this;
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            PreInit();
            // Reset the initial intro research and quest flags for new games
            spawnedIsoDataChip = false; 
            triggeredAncientBunkerQuest = false;
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            PreInit();
        }

        public override void GameComponentTick()
        {
            if (spawnIsoDataChipTick > 0 && Find.TickManager.TicksGame > spawnIsoDataChipTick) 
            {
                SpawnIsoDataChipNearResearchBench();
                spawnIsoDataChipTick = -99999;
            }
        }

        public void SpawnIsoDataChipNearResearchBench()
        {
            if (!spawnedIsoDataChip)
            {
                // Spawn the Isolinear Datachip on the first research bench in the first player colony map
                Map spawnMap = Find.Maps.Where(m => m.IsPlayerHome).FirstOrDefault();
                IntVec3 spawnPos = spawnMap.listerBuildings.AllBuildingsColonistOfClass<Building_ResearchBench>().First().Position;
                Thing isoDataChip = ThingMaker.MakeThing(LetheDefOf.Lethe_Artifact_IsolinearDatachip);
                GenPlace.TryPlaceThing(isoDataChip, spawnPos, spawnMap, ThingPlaceMode.Near);

                // Notify the player with a letter
                Find.LetterStack.ReceiveLetter("Lethe_Letter_IsoDataChip".Translate(), "Lethe_Letter_IsoDataChipDesc".Translate(LetheDefOf.Lethe_JuryRiggedDatachipReader.LabelCap.Colorize(ColoredText.NameColor), LetheDefOf.Lethe_Cipher1.LabelCap.Colorize(ColoredText.NameColor)), LetterDefOf.NeutralEvent, isoDataChip);

                // Don't spawn the datachip again in the future for the current savegame
                spawnedIsoDataChip = true;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref spawnedIsoDataChip, "spawnedIsoDataChip", false, true);
            Scribe_Values.Look(ref spawnIsoDataChipTick, "spawnIsoDataChipTick", 0, true);
            Scribe_Values.Look(ref triggeredAncientBunkerQuest, "triggeredAncientBunkerQuest", false, true);
        }

        [DebugAction("Lethe", "Spawn Iso Data Chip Immediately", allowedGameStates = AllowedGameStates.Playing)]
        private static void SpawnIsoDataChipTickImmediately()
        {
            if (Instance.spawnIsoDataChipTick != -99999)
            {
                Instance.spawnIsoDataChipTick = Find.TickManager.TicksGame;
            }
        }
    }
}
