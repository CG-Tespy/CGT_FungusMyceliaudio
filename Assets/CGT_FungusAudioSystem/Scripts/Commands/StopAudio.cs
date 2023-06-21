﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace CGT.FungusExt.Audio
{
    [CommandInfo("Audio/CGT",
        "StopAudio",
        "Stops the audio of the given type in the given channel")]
    [AddComponentMenu("")]
    public class StopAudio : AudioCommand
    {
        [SerializeField] protected IntegerData channel = new IntegerData(0);

        public override void OnEnter()
        {
            base.OnEnter();

            AudioArgs args = GetAudioArgs();
            AudioEvents.TriggerStopAudio(args);
        }

        protected override AudioArgs GetAudioArgs()
        {
            AudioArgs args = base.GetAudioArgs();
            args.Channel = channel;
            args.WantsPitchSet = args.WantsVolumeSet = false;
            args.OnComplete = CallContinueForOnComplete;

            return args;
        }

        public override string GetSummary()
        {
            return $"{audioType} Ch {channel.Value}";
        }

    }
}