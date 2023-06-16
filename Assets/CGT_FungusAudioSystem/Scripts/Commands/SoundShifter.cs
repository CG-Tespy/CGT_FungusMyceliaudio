using UnityEngine;
using Fungus;
using StringBuilder = System.Text.StringBuilder;

namespace CGT.FungusExt.Audio
{
    /// <summary>
    /// For setting properties of audio.
    /// </summary>
    public abstract class SoundShifter: AudioCommand
    {
        [SerializeField] protected IntegerData channel = new IntegerData(0);
        [SerializeField] protected GetOrSet action = GetOrSet.Set;

        [Header("For Setting")]
        [Tooltip("In terms of percentages. 100 for 100%, 50 for 50%, and so on")]
        [SerializeField] protected FloatData targetValue = new FloatData(1f);
        [SerializeField] protected FloatData fadeDuration = new FloatData(0f);

        [Tooltip("Whether or not this should wait for the fading to finish before moving to the next Command")]
        [SerializeField] protected BooleanData waitForFade = new BooleanData(false);

        [Header("For Getting")]
        [SerializeField]
        [VariableProperty(typeof(FloatVariable))] protected FloatVariable output;

        public override void OnEnter()
        {
            base.OnEnter();

            AudioArgs args = DecideAudioArgs();

            if (action == GetOrSet.Set)
            {
                SetValuesToSystem(args);
            }
            else
            {
                if (!OutputIsValid)
                    AlertForInvalidOutput();

                GetValueIntoOutput(args);
            }

            bool shouldContinueRightAway = action == GetOrSet.Get || !waitForFade;
            if (shouldContinueRightAway)
                Continue();

        }

        // Should be overridden to fill in the missing fields
        protected virtual AudioArgs DecideAudioArgs()
        {
            AudioArgs args = new AudioArgs();
            args.AudioType = audioType;
            args.FadeDuration = Mathf.Max(0, fadeDuration);
            args.Channel = this.channel;

            args.OnComplete = (AudioArgs maybeOtherArgs) => { Continue(); };
            // ^OnComplete will always be called right after the volume's done adjusting, be it
            // right away or after a fade. Thus, we won't need a Continue call in OnEnter

            return args;
        }

        protected virtual float CorrectedTargetValue
        {
            get
            {
                // Since AudioSources prefer different target values for different things
                // like volume and pitch
                float corrected = targetValue;
                corrected /= 100;

                corrected = Mathf.Clamp(corrected, MinTargetValue, MaxTargetValue);

                return corrected;
            }
        }

        protected abstract float MinTargetValue { get; }
        protected abstract float MaxTargetValue { get; }

        protected abstract void SetValuesToSystem(AudioArgs args);

        protected abstract void GetValueIntoOutput(AudioArgs args);

        protected virtual bool OutputIsValid { get { return output != null; } }

        protected virtual void AlertForInvalidOutput()
        {
            string flowchartName = GetFlowchart().name;
            string blockName = ParentBlock.BlockName;
            int index = this.CommandIndex;
            string errorMessage = string.Format(InvalidOutputErrorMessageFormat, this.GetType().Name,
                flowchartName, blockName, index);

            Debug.LogError(errorMessage);
        }

        protected virtual AudioSetter AudioSetter { get; set; } = new AudioSetter();
        protected virtual string InvalidOutputErrorMessageFormat { get; } = "{0} invalid in Flowchart {1}, Block {2}, Index {3}. Reason: No valid output var assigned for Get operation";
                
        protected override void Awake()
        {
            base.Awake();
            AudioSetter.Init();
        }

        public override string GetSummary()
        {
            forSummary.Clear();
            if (fadeDuration > 0)
                forSummary.Append("Fade ");
            else
                forSummary.Append($"{action} ");

            forSummary.Append($"Ch {channel.Value} {audioType} ");

            if (action == GetOrSet.Set)
            {
                forSummary.Append("to ");

                bool goWithValueVar = targetValue.floatRef != null;
                if (goWithValueVar)
                    forSummary.Append($"{targetValue.floatRef.Key}");
                else
                    forSummary.Append($"{targetValue.Value}");

                forSummary.Append("%");
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