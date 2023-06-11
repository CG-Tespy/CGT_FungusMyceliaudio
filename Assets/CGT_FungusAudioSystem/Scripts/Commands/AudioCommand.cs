using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace CGT.FungusExt.Audio
{
    public abstract class AudioCommand : Command
    {
        [SerializeField] protected AudioType audioType;

        protected virtual void Awake()
        {
            AudioSys.EnsureExists();
            // We want to set up a foundation so it's easy for subclasses to access
            // basic parts of the system so they don't have to set up (as much of)
            // that access themselves
            SetUpEventDicts();
        }

        protected virtual void SetUpEventDicts()
        {
            SetUpPlayEventDict();
            SetUpStopEventDict();
            SetUpSetVolEventDict();
            SetUpPitchEventDict();
        }

        protected virtual void SetUpPlayEventDict()
        {
            playEvents[AudioType.Music] = AudioEvents.TriggerPlayMusic;
            playEvents[AudioType.SFX] = AudioEvents.TriggerPlaySFX;
        }

        protected IDictionary<AudioType, AudioHandler> playEvents = new Dictionary<AudioType, AudioHandler>();

        protected virtual void SetUpStopEventDict()
        {
            stopEvents[AudioType.Music] = AudioEvents.TriggerStopMusic;
            stopEvents[AudioType.SFX] = AudioEvents.TriggerStopSFX;
        }

        protected IDictionary<AudioType, AudioHandler> stopEvents = new Dictionary<AudioType, AudioHandler>();

        protected virtual void SetUpSetVolEventDict()
        {
            setVolEvents[AudioType.Music] = AudioEvents.TriggerSetMusicVolume;
            setVolEvents[AudioType.SFX] = AudioEvents.TriggerSetSFXVolume;
        }

        protected IDictionary<AudioType, AudioHandler> setVolEvents = new Dictionary<AudioType, AudioHandler>();

        protected virtual void SetUpPitchEventDict()
        {
            setPitEvents[AudioType.Music] = AudioEvents.TriggerSetMusicPitch;
            setPitEvents[AudioType.SFX] = AudioEvents.TriggerSetSFXPitch;
        }

        protected IDictionary<AudioType, AudioHandler> setPitEvents = new Dictionary<AudioType, AudioHandler>();

        protected AudioSys AudioSys { get { return AudioSys.Instance; } }

        protected static float minVolume = 0f, maxVolume = 1f, minPitch = -3f, maxPitch = 3f;

        protected virtual void CallContinueForOnComplete(AudioArgs args)
        {
            Continue();
        }

        public override Color GetButtonColor()
        {
            return audioCommandColor;
        }

        protected static Color32 audioCommandColor = new Color32(242, 209, 176, 255);
    }

}