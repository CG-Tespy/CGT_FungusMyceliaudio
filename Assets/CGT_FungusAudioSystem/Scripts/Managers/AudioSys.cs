using UnityEngine;
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
            if (musicManager == null)
                musicManager = CreateAudioManager(musicManagerName);

            string sfxManagerName = "CGT_FungusSFXManager";
            if (sfxManager == null)
                sfxManager = CreateAudioManager(sfxManagerName);
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

        //protected AudioManager musicManager, sfxManager;

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