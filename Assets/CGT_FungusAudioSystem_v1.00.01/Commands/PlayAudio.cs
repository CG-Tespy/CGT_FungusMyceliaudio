using UnityEngine;
using Fungus;
using AudioDelegate = CGT.FungusExt.Audio.AudioEvents.AudioDelegate;

namespace CGT.FungusExt.Audio
{
    [CommandInfo("Audio/CGT",
        "PlayAudio",
        "Plays an audio clip of the type of your choosing")]
    public class PlayAudio : AudioCommand
    {
        [SerializeField] protected AudioClip clip;

        [Tooltip("Time to be playing in seconds. If the audio's compressed, this may not be accurate")]
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
                AudioDelegate eventToPlay = playEvents[audioType];
                eventToPlay(args);
            }
            else
            {
                PointOutLackOfClip();
            }

        }

        protected virtual bool ValidClip { get { return clip != null; } }

        protected virtual AudioArgs GetAudioArgs()
        {
            AudioArgs args = new AudioArgs();
            args.Clip = clip;
            args.AtTime = atTime;
            args.Loop = loop;
            args.FadeDuration = fadeDuration;

            args.OnComplete = Continue;
            // ^OnComplete will always be called right after the target clip starts playing, be it
            // right away or after a fade. Thus, we won't need a Continue call in OnEnter

            return args;
        }

        protected virtual void PointOutLackOfClip()
        {
            // To make debugging easier for the user
            string flowchartName = gameObject.name;
            string blockName = ParentBlock.BlockName;
            int index = CommandIndex;

            string errorMessage = $"Flowchart in GameObject {flowchartName}, Block {blockName}, Index {index} has no AudioClip assigned to it.";
            Debug.LogWarning(errorMessage);
        }

        public override string GetSummary()
        {
            if (clip == null)
                return "Needs a clip!";

            string result = $"{audioType} {clip.name} at time {atTime.Value}s";
            return result;
        }

    }
}