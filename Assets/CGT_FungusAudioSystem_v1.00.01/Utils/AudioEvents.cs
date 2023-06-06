namespace CGT.FungusExt.Audio
{
    public static class AudioEvents
    {
        public static event AudioHandler PlayMusic = delegate { },
            PlaySFX = delegate { },
            PlayAmbiance = delegate { },

            SetMusicVol = delegate { },
            SetSFXVol = delegate { },
            SetAmbienceVol = delegate { },

            SetMusicPitch = delegate { },
            SetSFXPitch = delegate { },
            SetAmbiancePitch = delegate { },

            StopMusic = delegate { },
            StopAmbiance = delegate { };
            
        public static void TriggerPlayMusic(AudioArgs args)
        {
            PlayMusic.Invoke(args);
        }

        public static void TriggerPlaySFX(AudioArgs args)
        {
            PlaySFX.Invoke(args);
        }

        public static void TriggerPlayAmbiance(AudioArgs args)
        {
            PlayAmbiance.Invoke(args);
        }

        public static void TriggerSetMusicVolume(AudioArgs args)
        {
            SetMusicVol.Invoke(args);
        }

        public static void TriggerSetSFXVolume(AudioArgs args)
        {
            SetSFXVol.Invoke(args);
        }

        public static void TriggerSetAmbienceVolume(AudioArgs args)
        {
            SetAmbienceVol.Invoke(args);
        }
    }
}