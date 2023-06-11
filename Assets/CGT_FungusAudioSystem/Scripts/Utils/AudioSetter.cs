using System.Collections.Generic;

namespace CGT.FungusExt.Audio
{
    // Makes it easier to set parts of the system without having to worry as
    // much about the audio type
    public class AudioSetter
    {
        public virtual void SetVolume(AudioArgs args)
        {
            var setter = VolumeSetters[args.AudioType];
            setter(args);
        }

        protected virtual IDictionary<AudioType, AudioHandler> VolumeSetters { get; set; } =
            new Dictionary<AudioType, AudioHandler>();

        public virtual void SetPitch(AudioArgs args)
        {
            var setter = PitchSetters[args.AudioType];
            setter(args);
        }

        protected virtual IDictionary<AudioType, AudioHandler> PitchSetters { get; set; } =
            new Dictionary<AudioType, AudioHandler>();

        public virtual void Init()
        {
            PrepareVolumeSetters();
            PreparePitchSetters();
        }

        protected virtual void PrepareVolumeSetters()
        {
            VolumeSetters[AudioType.Music] = AudioEvents.TriggerSetMusicVolume;
            VolumeSetters[AudioType.SFX] = AudioEvents.TriggerSetSFXVolume;
        }

        protected static AudioSys AudioSys { get { return AudioSys.Instance; } }

        protected virtual void PreparePitchSetters()
        {
            PitchSetters[AudioType.Music] = AudioEvents.TriggerSetMusicPitch;
            PitchSetters[AudioType.SFX] = AudioEvents.TriggerSetSFXPitch;
        }
    }
}