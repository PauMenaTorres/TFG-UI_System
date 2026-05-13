using UnityEngine;
using System.Collections.Generic;

namespace ModularUIRuntime
{
    public class ModularAudioManager : MonoBehaviour
    {
        private static ModularAudioManager instance;
        public static ModularAudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("ModularAudioManager");
                    instance = go.AddComponent<ModularAudioManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private List<AudioSource> audioSourcePool = new List<AudioSource>();
        private int poolSize = 5;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                CreateNewAudioSource();
            }
        }

        private AudioSource CreateNewAudioSource()
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f; 
            audioSourcePool.Add(source);
            return source;
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip == null) return;

            AudioSource source = GetAvailableSource();
            source.clip = clip;
            source.Play();
        }

        private AudioSource GetAvailableSource()
        {
            foreach (var source in audioSourcePool)
            {
                if (!source.isPlaying) return source;
            }

            return CreateNewAudioSource();
        }
    }
}
