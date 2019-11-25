using System.Collections.Generic;
using UnityEngine;

namespace FingerTracking.Audio
{

    public class AudioPlayer : MonoBehaviour
    {

        public AudioClipProps[] clipProps;
        private Dictionary<AudioType, AudioClip> clips;

        public void Init()
        {
            clips = new Dictionary<AudioType, AudioClip>();

            if (clipProps.Length == 0) return;

            foreach (var props in clipProps)
                clips.Add(props.type, props.clip);
        }

        public void PlaySound(AudioType type)
        {
            CreateAudioSource(clips[type], transform);
        }

        private static void CreateAudioSource(AudioClip clip, Transform parent)
        {
            GameObject go = new GameObject($"audio_{clip.name}");
            go.transform.parent = parent;
            Destroy(go, clip.length + 0.1f);

            AudioSource audio = go.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.spatialBlend = 0;
            audio.clip = clip;
            audio.Play();
        }

    }

}
