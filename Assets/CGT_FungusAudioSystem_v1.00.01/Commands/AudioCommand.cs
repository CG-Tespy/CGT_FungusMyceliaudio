using System.Collections.Generic;
using UnityEngine;
using Fungus;

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

        protected Dictionary<AudioType, AudioHandler> playEvents = new Dictionary<AudioType, AudioHandler>();

        protected virtual void SetUpSetVolDict()
        {
            setVolEvents[AudioType.Music] = AudioEvents.TriggerSetMusicVolume;
            setVolEvents[AudioType.SFX] = AudioEvents.TriggerSetSFXVolume;
            setVolEvents[AudioType.Ambience] = AudioEvents.TriggerSetAmbienceVolume;
        }

        protected Dictionary<AudioType, AudioHandler> setVolEvents = new Dictionary<AudioType, AudioHandler>();
        
        protected NeoAudioManager AudioManager { get { return NeoAudioManager.Instance; } }

        protected virtual float RelevantStartingVolume
        {
            get
            {
                switch (audioType)
                {
                    case AudioType.Music:
                        return AudioManager.MusicVolume;
                    case AudioType.SFX:
                        return AudioManager.SFXVolume;
                    case AudioType.Ambience:
                        return AudioManager.AmbienceVolume;
                    default:
                        throw new System.NotImplementedException("Not accounting for enough relevant starting volumes.");
                }

            }
        }

        public override Color GetButtonColor()
        {
            return audioCommandColor;
        }

        protected static Color32 audioCommandColor = new Color32(242, 209, 176, 255);
    }
}