using UnityEngine;
using System.Collections.Generic;
using CGT.FungusExt.Audio.Internal;

namespace CGT.FungusExt.Audio
{
    /// <summary>
    /// Alternative to Fungus's built-in MusicManager that works with commands that alter
    /// music or sfx properties separately.
    /// </summary>
    public class AudioSys : MonoBehaviour
    {
        public static void EnsureExists()
        {
            // We check here to avoid creating craploads of AudioSyses from lots of
            // AudioCommands being executed in short order
            bool alreadySetUp = Instance != null;
            if (alreadySetUp)
            {
                return;
            }

            GameObject sysGO = new GameObject(sysName);
            AudioSys theSys = sysGO.AddComponent<AudioSys>();
            Instance = theSys;
        }

        protected static string sysName = "CGT_FungusAudioSystem";

        public static AudioSys Instance { get; protected set; }

        protected virtual void Awake()
        {
            EnsureAudioManagersAreThere();
            DontDestroyOnLoad(this.gameObject);
        }

        protected virtual void EnsureAudioManagersAreThere()
        {
            string musicManagerName = "CGT_FungusMusicManager";
            bool noMusicManager = !audioManagers.ContainsKey(AudioType.Music) ||
                audioManagers[AudioType.Music] == null;
            if (noMusicManager)
                musicManager = audioManagers[AudioType.Music] = CreateAudioManager(musicManagerName);

            string sfxManagerName = "CGT_FungusSFXManager";
            bool noSfxManager = !audioManagers.ContainsKey(AudioType.SFX) ||
                audioManagers[AudioType.SFX] == null;
            if (noSfxManager)
                sfxManager = audioManagers[AudioType.SFX] = CreateAudioManager(sfxManagerName);

        }

        protected virtual AudioManager CreateAudioManager(string name)
        {
            // We have separate game objects for the managers so we can check the
            // channel-counts and such in the Scene view
            GameObject holdsManager = new GameObject(name);
            holdsManager.transform.SetParent(this.transform);
            AudioManager manager = new AudioManager(holdsManager);
            manager.Name = name;
            return manager;
        }

        protected IDictionary<AudioType, AudioManager> audioManagers = new Dictionary<AudioType, AudioManager>();

        protected AudioManager musicManager, sfxManager;

        protected virtual void OnEnable()
        {
            EnsureAudioManagersAreThere();
            ListenForAudioEvents();
        }

        protected virtual void ListenForAudioEvents()
        {
            AudioEvents.PlayMusic += musicManager.Play;
            AudioEvents.PlaySFX += sfxManager.Play;

            AudioEvents.SetMusicVol += musicManager.SetVolume;
            AudioEvents.SetSFXVol += sfxManager.SetVolume;

            AudioEvents.SetMusicPitch += musicManager.SetPitch;
            AudioEvents.SetSFXPitch += sfxManager.SetPitch;

            AudioEvents.StopMusic += musicManager.Stop;
            AudioEvents.StopSFX += sfxManager.Stop;
        }

        protected virtual void OnDisable()
        {
            UNListenForAudioEvents();
        }

        protected virtual void UNListenForAudioEvents()
        {
            AudioEvents.PlayMusic -= musicManager.Play;
            AudioEvents.PlaySFX -= sfxManager.Play;

            AudioEvents.SetMusicVol -= musicManager.SetVolume;
            AudioEvents.SetSFXVol -= sfxManager.SetVolume;

            AudioEvents.SetMusicPitch -= musicManager.SetPitch;
            AudioEvents.SetSFXPitch -= sfxManager.SetPitch;

            AudioEvents.StopMusic -= musicManager.Stop;
            AudioEvents.StopSFX -= sfxManager.Stop;
        }

        public float GetVolume(AudioArgs args)
        {
            var managerToUse = audioManagers[args.AudioType];
            return managerToUse.GetVolume(args);
        }

        public float GetPitch(AudioArgs args)
        {
            var managerToUse = audioManagers[args.AudioType];
            return managerToUse.GetPitch(args);
        }

        public float GetMusicVolume(AudioArgs args)
        {
            return musicManager.GetVolume(args);
        }

        public virtual float GetSFXVolume(AudioArgs args)
        {
            return sfxManager.GetVolume(args);
        }

        public virtual float GetMusicPitch(AudioArgs args)
        {
            return musicManager.GetPitch(args);
        }

        public virtual float GetSFXPitch(AudioArgs args)
        {
            return sfxManager.GetPitch(args);
        }
    }
}