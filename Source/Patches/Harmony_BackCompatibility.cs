using HarmonyLib;
using System;
using Verse;

namespace Lethe
{
    [HarmonyPatch(typeof(BackCompatibility), nameof(BackCompatibility.BackCompatibleDefName))]
    public static class Harmony_BackCompatibility
    {
        public static void Postfix(Type defType, string defName, ref string __result)
        {
            if (defType == typeof(ThingDef))
            {
                if (ModCompatibility.ReplimatIsActive && defName == "IsolinearModule")
                {
                    __result = "Lethe_IsolinearModule";
                }

                if (ModCompatibility.MedPodIsActive && defName == "IsolinearProcessor")
                {
                    __result = "Lethe_IsolinearProcessor";
                }
            }
        }
    }
}
