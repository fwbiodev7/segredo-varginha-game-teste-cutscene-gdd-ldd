using UnityEngine;

namespace Game.Varginha.Experiment
{
    // The user's movable art draft is editor-only; gameplay stays intact until layout approval.
    [ExecuteAlways]
    public sealed class CampaignLaboratoryArrangementDraft : MonoBehaviour
    {
        private void OnEnable()
        {
            if(Application.isPlaying)gameObject.SetActive(false);
        }
    }
}
