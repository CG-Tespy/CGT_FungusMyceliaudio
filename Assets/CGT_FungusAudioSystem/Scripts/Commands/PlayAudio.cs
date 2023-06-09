using UnityEngine;
using Fungus;

namespace CGT.FungusExt.Audio
{
    [CommandInfo("Audio/CGT",
        "PlayAudio",
        "Plays an audio clip in the given channel")]
    public class PlayAudio : AudioCommand
    {
        [SerializeField] protected ObjectData clip = new ObjectData(null);

        [Tooltip("Time to be playing in seconds. If the audio's compressed, this may not be accurate. If you dont want the time to change, set this to a negative number.")]
        [SerializeField] protected FloatData atTime = new FloatData(0);

        [SerializeField] protected BooleanData loop = new BooleanData(false);

        [Tooltip("How long to fade the current audio out before playing the clip above at the previous volume")]
        [SerializeField] protected FloatData fadeDuration = new FloatData(0);

        [Tooltip("Whether or not this should wait for the fading to finish before moving to the next Command")]
        [SerializeField] protected BooleanData waitForFade = new BooleanData(false);

        public override void OnEnter()
        {
            base.OnEnter();

            if (ValidClip)
            {
                AudioArgs args = GetAudioArgs();
                AudioHandler eventToPlay = playEvents[audioType];
                eventToPlay(args);
            }
            else
            {
                PointOutClipInvalidity();
            }
        }

        protected virtual bool ValidClip { get { return clip.Value is AudioClip; } }

        protected virtual AudioArgs GetAudioArgs()
        {
            AudioArgs args = new AudioArgs();
            args.WantsVolumeSet = args.WantsPitchSet = false;
            args.Clip = (AudioClip) clip.Value;
            args.AtTime = atTime;
            args.Loop = loop;
            args.FadeDuration = fadeDuration;
            args.Channel = channel;

            args.OnComplete = (AudioArgs maybeOtherArgs) => { Continue(); };
            // ^OnComplete will always be called right after the target clip starts playing, be it
            // right away or after a fade. Thus, we won't need a Continue call in OnEnter

            return args;
        }

        protected virtual void PointOutClipInvalidity()
        {
            // To make debugging easier for the user
            string flowchartName = gameObject.name;
            string blockName = ParentBlock.BlockName;
            int index = CommandIndex;

            string errorMessage = $"PlayAudio Command invalid in Flowchart in GameObject {flowchartName}, Block {blockName}, Index {index}. Reason: No valid AudioClip assigned";
            Debug.LogWarning(errorMessage);
        }

        public override string GetSummary()
        {
            if (!ValidClip)
                return "Error: No clip given";

            string result = $"{audioType} ";

            bool goWithChannelVar = channel.integerRef != null;
            if (goWithChannelVar)
                result += $"{channel.integerRef.Key} ";
            else
                result += $"Ch {channel.Value} ";

            result += $"{clip.Value.name} @ {atTime.Value}s";
            return result;
        }

    }
}