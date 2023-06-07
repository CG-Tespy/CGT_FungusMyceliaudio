using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace CGT.FungusExt.Audio
{
    public abstract class AudioCommand : Command
    {
        public enum AudioType { Music, SFX }

        [SerializeField] protected AudioType audioType;
        [SerializeField] protected IntegerData channel;

        protected virtual void Awake()
        {
            AudioSys.EnsureExists();
            SetUpPlayEventDict();
            SetUpSetVolDict();
        }

        protected virtual void SetUpPlayEventDict()
        {
            playEvents[AudioType.Music] = AudioEvents.TriggerPlayMusic;
            playEvents[AudioType.SFX] = AudioEvents.TriggerPlaySFX;
        }

        protected Dictionary<AudioType, AudioHandler> playEvents = new Dictionary<AudioType, AudioHandler>();

        protected virtual void SetUpSetVolDict()
        {
            setVolEvents[AudioType.Music] = AudioEvents.TriggerSetMusicVolume;
            setVolEvents[AudioType.SFX] = AudioEvents.TriggerSetSFXVolume;
        }

        protected Dictionary<AudioType, AudioHandler> setVolEvents = new Dictionary<AudioType, AudioHandler>();
        
        protected AudioSys AudioSys { get { return AudioSys.Instance; } }

        public override Color GetButtonColor()
        {
            return audioCommandColor;
        }

        protected static Color32 audioCommandColor = new Color32(242, 209, 176, 255);
    }

}