using Verse;

namespace Lethe
{
    public class ResearchMod_TriggerAncientBunkerQuest : ResearchMod
    {
        public override void Apply()
        {
            if (!GameComponent_Lethe.Instance.triggeredAncientBunkerQuest)
            {
                // todo - trigger one-time quest to investigate ancient bunker / facility
                Log.Error("Lethe :: Should trigger ancient bunker quest now");
                GameComponent_Lethe.Instance.triggeredAncientBunkerQuest = true;
            }
        }
    }
}
