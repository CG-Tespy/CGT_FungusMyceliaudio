using UnityEngine;
using System.Collections.Generic;
using Fungus;
using StringBuilder = System.Text.StringBuilder;

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

            AudioArgs args = DecideAudioArgs();

            if (action == GetOrSet.Set)
            {
                HandleSetOperation(args);
            }
            else 
            {
                HandleGetOperation(args);
            }

            bool shouldContinueRightAway = action == GetOrSet.Get || !waitForFade;
            if (shouldContinueRightAway)
                Continue();
        }

        protected virtual AudioArgs DecideAudioArgs()
        {
            AudioArgs args = new AudioArgs();
            args.WantsVolumeSet = true;
            args.WantsPitchSet = false;
            args.TargetVolume = Mathf.Clamp(volumeInput, 0f, 1);
            // ^ Sound playback might get funky if you set it to a value higher than 1, so...
            args.FadeDuration = Mathf.Max(0, fadeDuration);
            args.Channel = this.channel;

            args.OnComplete = (AudioArgs maybeOtherArgs) => { Continue(); };
            // ^OnComplete will always be called right after the volume's done adjusting, be it
            // right away or after a fade. Thus, we won't need a Continue call in OnEnter

            return args;
        }

        protected virtual void HandleSetOperation(AudioArgs args)
        {
            var setVol = setVolEvents[audioType];
            setVol(args);
        }


        protected Dictionary<AudioType, System.Action<AudioArgs>> volumeGetters = 
            new Dictionary<AudioType, System.Action<AudioArgs>>();

        protected override void Awake()
        {
            base.Awake();
            PrepareVolumeGetters();
        }

        protected virtual void PrepareVolumeGetters()
        {
            volumeGetters[AudioType.Music] = GetMusicVolume;
            volumeGetters[AudioType.SFX] = GetSFXVolume;
        }

        protected virtual void GetMusicVolume(AudioArgs args)
        {
            output.Value = AudioSys.GetMusicVolume(args);
        }

        protected virtual void GetSFXVolume(AudioArgs args)
        {
            output.Value = AudioSys.GetSFXVolume(args);
        }

        protected virtual void HandleGetOperation(AudioArgs args)
        {
            if (!OutputIsValid)
            {
                AlertForInvalidOutput();
            }
            else
            {
                SetOutputToDesiredValue(args);
            }
        }

        protected virtual bool OutputIsValid { get { return output != null; } }

        protected virtual void AlertForInvalidOutput()
        {
            string flowchartName = GetFlowchart().name;
            string blockName = ParentBlock.BlockName;
            int index = this.CommandIndex;
            string errorMessage = $"PlayAudio invalid in Flowchart in GameObject {flowchartName}, Block {blockName}, Index {index}. Reason: No valid output var assigned for Get operation";

            Debug.LogError(errorMessage);
        }

        protected virtual void SetOutputToDesiredValue(AudioArgs args)
        {
            var getter = volumeGetters[audioType];
            getter(args);
        }

        public override string GetSummary()
        {
            forSummary.Clear();
            forSummary.Append($"{action} {audioType} Ch {channel.Value} to ");

            if (action == GetOrSet.Set)
            {
                bool goWithVolumeVar = volumeInput.floatRef != null;
                if (goWithVolumeVar)
                    forSummary.Append($"{volumeInput.floatRef.Key}");
                else
                    forSummary.Append($"to {volumeInput.Value}");
            }
            else
            {
                if (!OutputIsValid)
                    return "Error: needs output var!";
                
                forSummary.Append($"into {output.Key}");
            }

            return forSummary.ToString();
        }

        protected StringBuilder forSummary = new StringBuilder();
    }
}