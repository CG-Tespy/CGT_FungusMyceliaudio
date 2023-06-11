﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace CGT.FungusExt.Audio
{
    [CommandInfo("Audio/CGT",
        "CrossfadeVo",
        "Lets you fade one channel's ~Volume~ out and another channel's in at the same time")]
    [AddComponentMenu("")]
    public class CrossfadeVo : AudioCommand
    {
        [Header("For Fading Out")]
        [SerializeField] protected IntegerData fadeOutCh = new IntegerData(0);
        [SerializeField] protected FloatData fadeOutTargVol = new FloatData(0);
        [SerializeField] protected FloatData fadeOutDuration = new FloatData(1f);

        [Header("For Fading In")]
        [SerializeField] protected IntegerData fadeInCh = new IntegerData(1);
        [SerializeField] protected FloatData fadeInTargVol = new FloatData(0.25f);
        [SerializeField] protected FloatData fadeInDuration = new FloatData(1f);

        [Tooltip("Whether or not this should wait for the (longer) fading to finish before moving to the next Command")]
        [SerializeField] protected BooleanData waitForFade = new BooleanData(false);

        public override void OnEnter()
        {
            base.OnEnter();

            if (BothSameChannels)
            {
                AlertSameChannelIssue();
            }
            else
            {
                AudioArgs fadeOutArgs = GetAudioArgs(fadeOutTargVol, fadeOutDuration, fadeOutCh),
                    fadeInArgs = GetAudioArgs(fadeInTargVol, fadeInDuration, fadeInCh);

                EnsureContinueGetsCalledProperly(fadeOutArgs, fadeInArgs);
                var applyFade = setVolEvents[audioType];
                applyFade(fadeOutArgs);
                applyFade(fadeInArgs);
            }

            if (!waitForFade)
                Continue();
        }
        
        protected virtual bool BothSameChannels { get { return fadeInCh.Value == fadeOutCh.Value; } }

        protected virtual void AlertSameChannelIssue()
        {
            string flowchartName = gameObject.name;
            string blockName = ParentBlock.BlockName;
            int index = CommandIndex;
            string errorMessage = $"Crossfade: Same channels in Flowchart {flowchartName}, Block {blockName} Index {index}";
            Debug.LogError(errorMessage);
        }

        protected virtual AudioArgs GetAudioArgs(float targetVolume, float fadeDuration, int channel)
        {
            AudioArgs args = GetBaseAudioArgs();
            args.TargetVolume = Mathf.Clamp(targetVolume, AudioStatics.MinVolume, AudioStatics.MaxVolume);
            args.FadeDuration = Mathf.Max(0, fadeDuration);
            args.Channel = channel;

            bool triggerContinueAfterFade = args.WantsFade && waitForFade && IsLongerFade(fadeDuration);
            if (triggerContinueAfterFade)
                args.OnComplete = CallContinueForOnComplete;

            return args;
        }

        protected virtual AudioArgs GetBaseAudioArgs()
        {
            AudioArgs args = new AudioArgs();
            args.WantsVolumeSet = true;
            args.WantsPitchSet = false;
            return args;
        }

        protected virtual bool IsLongerFade(float fade)
        {
            // So we don't have BOTH AudioArgs triggering Continue
            return fade == Mathf.Max(fadeOutDuration, fadeInDuration);
        }

        protected virtual void EnsureContinueGetsCalledProperly(AudioArgs first, AudioArgs second)
        {
            bool bothSetToCallIt = first.OnComplete == CallContinueForOnComplete &&
                second.OnComplete == CallContinueForOnComplete;
            // ^Perhaps for when their fade values are the same non-zero value
            if (bothSetToCallIt)
            {
                second.OnComplete = (AudioArgs args) => { };
            }

            bool neitherSetToCallIt = !first.WantsFade && !second.WantsFade;
            // ^For when neither are going to fade
            if (neitherSetToCallIt)
                first.OnComplete = CallContinueForOnComplete;
        }

        public override string GetSummary()
        {
            bool bothSameChannels = fadeInCh.Value == fadeOutCh.Value;
            if (bothSameChannels)
                return "ERROR: both channels are the same";

            return $"{audioType} Out: Ch {fadeOutCh.Value} | In: Ch {fadeInCh.Value}";
        }

    }
}