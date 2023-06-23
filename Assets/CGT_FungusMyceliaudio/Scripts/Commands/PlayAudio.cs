using UnityEngine;
using Fungus;
using StringBuilder = System.Text.StringBuilder;

namespace CGT.FungusExt.Myceliaudio
{
    [CommandInfo("Audio/CGT",
        "PlayAudio",
        "Plays an audio clip in the given track.")]
    [AddComponentMenu("")]
    public class PlayAudio : AudioCommand
    {
        [SerializeField] protected IntegerData track = new IntegerData(0);

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
                AudioEvents.TriggerPlayAudio(args);
            }
            else
            {
                PointOutClipInvalidity();
            }
        }

        protected virtual bool ValidClip { get { return clip.Value is AudioClip; } }

        protected override AudioArgs GetAudioArgs()
        {
            AudioArgs args = base.GetAudioArgs();
            args.WantsVolumeSet = args.WantsPitchSet = false;
            args.Clip = (AudioClip) clip.Value;
            args.AtTime = atTime;
            args.Loop = loop;
            args.FadeDuration = fadeDuration;
            args.Track = track;

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

            forSummary.Clear();
            forSummary.Append($"{audioType} Tr {track.Value} {clip.Value.name} ");

            bool wantsPlayAtTime = atTime.Value >= 0;
            if (wantsPlayAtTime)
                forSummary.Append($"@ {atTime.Value}s");

            return forSummary.ToString();
        }

        protected StringBuilder forSummary = new StringBuilder();

    }
}