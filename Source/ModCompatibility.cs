using Verse;

namespace Lethe
{
    [StaticConstructorOnStartup]
    public class ModCompatibility
    {
        public static bool ReplimatIsActive => ModsConfig.IsActive("sumghai.Replimat");

        public static bool MedPodIsActive => ModsConfig.IsActive("sumghai.Medpod");
    }
}
