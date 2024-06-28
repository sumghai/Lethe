using HarmonyLib;
using Verse;

namespace Lethe
{
    public class LetheMod : Mod
    {
        public LetheMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("com.Lethe.patches");
            harmony.PatchAll();

            if (ModCompatibility.ReplimatIsActive)
            {
                Log.Message("Lethe :: Replimat detected!");
            }

            if (ModCompatibility.MedPodIsActive)
            {
                Log.Message("Lethe :: MedPod detected!");
            }
        }
    }
}
