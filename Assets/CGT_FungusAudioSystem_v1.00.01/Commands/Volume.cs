using UnityEngine;
using System.Collections.Generic;
using Fungus;

namespace CGT.FungusExt.Audio
{
    [CommandInfo("Audio/CGT",
        "Volume",
        "Lets you get or set the volume of music, sfx or ambiance.")]
    public class Volume : AudioCommand
    {
        public enum GetOrSet { Get, Set }

        [SerializeField] protected GetOrSet action = GetOrSet.Set;

        [Header("For Setting")]
        [SerializeField] protected FloatData volumeInput = new FloatData(1f);
        [SerializeField] protected FloatData fadeDuration = new FloatData(0f);

        [Tooltip("Whether or not this should wait for the fading to finish before moving to the next Command")]
        [SerializeField] protected BooleanData waitForFade = new BooleanData(false);

        [Header("For Getting")] [SerializeField]
        [VariableProperty(typeof(FloatVariable))] protected FloatVariable output;

        public override void OnEnter()
        {
            base.OnEnter();

            if (action == GetOrSet.Set)
            {
                AudioArgs args = DecideAudioArgs();
                var setVol = setVolEvents[audioType];
                setVol(args);
            }
            else if (OutputIsValid)
            {
                GiveOutputDesiredValue();
                Continue();
            }
        }

        protected virtual AudioArgs DecideAudioArgs()
        {
            AudioArgs args = new AudioArgs();
            args.WantsVolumeSet = true;
            args.Volume = Mathf.Clamp(volumeInput, 0f, 1);
            // ^ Sound playback might get funky if you set it to a value higher than 1, so...
            args.FadeDuration = Mathf.Max(0, fadeDuration);

            args.OnComplete = Continue;
            // ^OnComplete will always be called right after the volume's done adjusting, be it
            // right away or after a fade. Thus, we won't need a Continue call in OnEnter

            return args;
        }

        protected virtual bool OutputIsValid { get { return output != null; } }

        protected virtual void GiveOutputDesiredValue()
        {
            var getter = volumeGetters[audioType];
            getter();
        }

        protected Dictionary<AudioType, System.Action> volumeGetters = new Dictionary<AudioType, System.Action>();

        protected override void Awake()
        {
            base.Awake();
            PrepareVolumeGetters();
        }

        protected virtual void PrepareVolumeGetters()
        {
            volumeGetters[AudioType.Music] = GetMusicVolume;
            volumeGetters[AudioType.SFX] = GetSFXVolume;
            volumeGetters[AudioType.Ambience] = GetAmbienceVolume;
        }

        protected virtual void GetMusicVolume()
        {
            output.Value = AudioManager.MusicVolume;
        }

        protected virtual void GetSFXVolume()
        {
            output.Value = AudioManager.SFXVolume;
        }

        protected virtual void GetAmbienceVolume()
        {
            output.Value = AudioManager.AmbienceVolume;
        }

        public override string GetSummary()
        {
            string result = $"{action} {audioType} vol ";

            if (action == GetOrSet.Set)
            {
                result += $"to ";

                bool goWithVar = volumeInput.floatRef != null;
                if (goWithVar)
                    result += $"{volumeInput.floatRef.Key}";
                else
                    result += $"{volumeInput.Value}";

            }
            else
            {
                if (!OutputIsValid)
                    return "Error: needs output var!";

                result += $"into {output.Key}";
            }
            
            return result;
        }
    }
}