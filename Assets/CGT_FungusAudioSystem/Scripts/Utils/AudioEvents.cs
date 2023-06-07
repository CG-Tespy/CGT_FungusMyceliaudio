namespace CGT.FungusExt.Audio
{
    public static class AudioEvents
    {
        public static event AudioHandler PlayMusic = delegate { },
            PlaySFX = delegate { },

            SetMusicVol = delegate { },
            SetSFXVol = delegate { },

            SetMusicPitch = delegate { },
            SetSFXPitch = delegate { },

            StopMusic = delegate { },
            StopSFX = delegate { };
            
        public static void TriggerPlayMusic(AudioArgs args)
        {
            PlayMusic.Invoke(args);
        }

        public static void TriggerPlaySFX(AudioArgs args)
        {
            PlaySFX.Invoke(args);
        }

        public static void TriggerSetMusicVolume(AudioArgs args)
        {
            SetMusicVol.Invoke(args);
        }

        public static void TriggerSetSFXVolume(AudioArgs args)
        {
            SetSFXVol.Invoke(args);
        }

    }
}