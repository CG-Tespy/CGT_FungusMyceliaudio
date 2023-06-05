using UnityEngine;

namespace CGT.FungusExt.Audio
{
    /// <summary>
    /// Alternative to Fungus's built-in MusicManager that works with commands that alter
    /// music, ambiance and sfx volume separately.
    /// </summary>
    public class NeoAudioManager : MonoBehaviour
    {
        public static void EnsureExists()
        {
            bool alreadyExists = Instance != null;
            if (alreadyExists)
                return;

            GameObject managerGO = new GameObject("CGTMusicManager");
            managerGO.AddComponent<NeoAudioManager>();
        }

        public static NeoAudioManager Instance { get; protected set; }

        protected virtual void Awake()
        {
            bool managerAlreadySetUp = Instance != null;
            if (managerAlreadySetUp)
                return;

            Instance = this;
            SetUpAudioSources();
            DontDestroyOnLoad(this.gameObject);
        }

        protected virtual void SetUpAudioSources()
        {
            forMusic = new FungusAudioSource(this.gameObject);
            forSFX = new FungusAudioSource(this.gameObject);
            forAmbiance = new FungusAudioSource(this.gameObject);
        }

        protected FungusAudioSource forMusic, forSFX, forAmbiance;

        protected virtual void OnEnable()
        {
            ListenForAudioEvents();
        }

        protected virtual void ListenForAudioEvents()
        {
            AudioEvents.PlayMusic += forMusic.Play;
            AudioEvents.PlaySFX += forSFX.Play;
            AudioEvents.PlayAmbiance += forAmbiance.Play;

            AudioEvents.SetMusicVol += forMusic.SetVolume;
            AudioEvents.SetSFXVol += forSFX.SetVolume;
            AudioEvents.SetAmbienceVol += forAmbiance.SetVolume;

            AudioEvents.SetMusicPitch += forMusic.SetPitch;
            AudioEvents.SetSFXPitch += forSFX.SetPitch;
            AudioEvents.SetAmbiancePitch += forAmbiance.SetPitch;

            AudioEvents.StopMusic += forMusic.Stop;
            AudioEvents.StopAmbiance += forAmbiance.Stop;
        }

        protected virtual void OnDisable()
        {
            UNListenForAudioEvents();
        }

        protected virtual void UNListenForAudioEvents()
        {
            AudioEvents.PlayMusic -= forMusic.Play;
            AudioEvents.PlaySFX -= forSFX.Play;
            AudioEvents.PlayAmbiance -= forAmbiance.Play;

            AudioEvents.SetMusicVol -= forMusic.SetVolume;
            AudioEvents.SetSFXVol -= forSFX.SetVolume;
            AudioEvents.SetAmbienceVol -= forAmbiance.SetVolume;

            AudioEvents.SetMusicPitch -= forMusic.SetPitch;
            AudioEvents.SetSFXPitch -= forSFX.SetPitch;
            AudioEvents.SetAmbiancePitch -= forAmbiance.SetPitch;

            AudioEvents.StopMusic -= forMusic.Stop;
            AudioEvents.StopAmbiance -= forAmbiance.Stop;
        }

        public virtual float MusicVolume { get { return forMusic.CurrentVol; } }
        public virtual float SFXVolume { get { return forSFX.CurrentVol; } }
        public virtual float AmbienceVolume { get { return forAmbiance.CurrentVol; } }

        public virtual float MusicPitch { get { return forMusic.CurrentPitch; } }
        public virtual float SFXPitch { get { return forSFX.CurrentPitch; } }
        public virtual float AmbiencePitch { get { return forAmbiance.CurrentPitch; } }
    }
}