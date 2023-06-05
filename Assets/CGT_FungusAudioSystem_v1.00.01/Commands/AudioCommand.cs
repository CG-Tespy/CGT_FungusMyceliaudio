using System.Collections.Generic;
using UnityEngine;
using Fungus;
using AudioDelegate = CGT.FungusExt.Audio.AudioEvents.AudioDelegate;

namespace CGT.FungusExt.Audio
{
    public abstract class AudioCommand : Command
    {
        public enum AudioType { Music, SFX, Ambience }

        [SerializeField] protected AudioType audioType;

        protected virtual void Awake()
        {
            NeoAudioManager.EnsureExists();
            SetUpPlayEventDict();
            SetUpSetVolDict();
        }

        protected virtual void SetUpPlayEventDict()
        {
            playEvents[AudioType.Music] = AudioEvents.TriggerPlayMusic;
            playEvents[AudioType.SFX] = AudioEvents.TriggerPlaySFX;
            playEvents[AudioType.Ambience] = AudioEvents.TriggerPlayAmbiance;
        }

        protected Dictionary<AudioType, AudioDelegate> playEvents = new Dictionary<AudioType, AudioDelegate>();

        protected virtual void SetUpSetVolDict()
        {
            setVolEvents[AudioType.Music] = AudioEvents.TriggerSetMusicVolume;
            setVolEvents[AudioType.SFX] = AudioEvents.TriggerSetSFXVolume;
            setVolEvents[AudioType.Ambience] = AudioEvents.TriggerSetAmbienceVolume;
        }

        protected Dictionary<AudioType, AudioDelegate> setVolEvents = new Dictionary<AudioType, AudioDelegate>();
        
        protected NeoAudioManager AudioManager { get { return NeoAudioManager.Instance; } }
        public override Color GetButtonColor()
        {
            return audioCommandColor;
        }

        protected static Color32 audioCommandColor = new Color32(242, 209, 176, 255);
    }
}