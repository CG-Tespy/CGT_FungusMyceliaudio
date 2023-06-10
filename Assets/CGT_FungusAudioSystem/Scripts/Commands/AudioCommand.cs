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
            SetUpPlayEventDict();
            SetUpStopEventDict();
            SetUpSetVolDict();
        }

        protected virtual void SetUpPlayEventDict()
        {
            playEvents[AudioType.Music] = AudioEvents.TriggerPlayMusic;
            playEvents[AudioType.SFX] = AudioEvents.TriggerPlaySFX;
        }

        protected Dictionary<AudioType, AudioHandler> playEvents = new Dictionary<AudioType, AudioHandler>();

        protected virtual void SetUpStopEventDict()
        {
            stopEvents[AudioType.Music] = AudioEvents.TriggerStopMusic;
            stopEvents[AudioType.SFX] = AudioEvents.TriggerStopSFX;
        }

        protected Dictionary<AudioType, AudioHandler> stopEvents = new Dictionary<AudioType, AudioHandler>();

        protected virtual void SetUpSetVolDict()
        {
            setVolEvents[AudioType.Music] = AudioEvents.TriggerSetMusicVolume;
            setVolEvents[AudioType.SFX] = AudioEvents.TriggerSetSFXVolume;
        }

        protected Dictionary<AudioType, AudioHandler> setVolEvents = new Dictionary<AudioType, AudioHandler>();
        
        protected AudioSys AudioSys { get { return AudioSys.Instance; } }

        protected static float minVolume = 0f, maxVolume = 1f, minPitch = -3f, maxPitch = 3f;
        public override Color GetButtonColor()
        {
            return audioCommandColor;
        }

        protected static Color32 audioCommandColor = new Color32(242, 209, 176, 255);
    }

}