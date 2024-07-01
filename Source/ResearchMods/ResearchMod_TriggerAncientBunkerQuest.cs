using RimWorld;
using Verse;

namespace Lethe
{
    public class ResearchMod_TriggerAncientBunkerQuest : ResearchMod
    {
        public override void Apply()
        {
            if (!GameComponent_Lethe.Instance.triggeredAncientBunkerQuest)
            {
                IncidentDef incidentDef = LetheDefOf.Lethe_GiveQuest_AncientBunker;
                IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.World);
                incidentDef.Worker.TryExecute(incidentParms);

                GameComponent_Lethe.Instance.triggeredAncientBunkerQuest = true;
            }
        }
    }
}
