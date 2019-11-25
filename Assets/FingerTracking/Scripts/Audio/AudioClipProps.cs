using UnityEngine;

namespace FingerTracking.Audio
{

    [System.Serializable]
    public struct AudioClipProps
    {

        [SerializeField] public AudioType type;
        [SerializeField] public AudioClip clip;

    }

}
