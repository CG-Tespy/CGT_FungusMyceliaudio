using System.Collections.Generic;

namespace CGT.FungusExt.Audio
{
    public delegate float AudioFloatFetcher(AudioArgs args);

    public class AudioGetter
    {
        public virtual float GetVolume(AudioArgs args)
        {
            var getter = VolumeGetters[args.AudioType];
            return getter(args);
        }

        protected virtual IDictionary<AudioType, AudioFloatFetcher> VolumeGetters { get; set; } =
            new Dictionary<AudioType, AudioFloatFetcher>();

        public virtual float GetPitch(AudioArgs args)
        {
            var getter = PitchGetters[args.AudioType];
            return getter(args);
        }

        protected virtual IDictionary<AudioType, AudioFloatFetcher> PitchGetters { get; set; } =
            new Dictionary<AudioType, AudioFloatFetcher>();

        public virtual void Init()
        {
            PrepareVolumeGetters();
            PreparePitchGetters();
        }

        protected virtual void PrepareVolumeGetters()
        {
            VolumeGetters[AudioType.Music] = AudioSys.GetMusicVolume;
            VolumeGetters[AudioType.SFX] = AudioSys.GetSFXVolume;
        }

        protected static AudioSys AudioSys { get { return AudioSys.Instance; } }

        protected virtual void PreparePitchGetters()
        {
            PitchGetters[AudioType.Music] = AudioSys.GetMusicPitch;
            PitchGetters[AudioType.SFX] = AudioSys.GetSFXPitch;
        }

    }
}