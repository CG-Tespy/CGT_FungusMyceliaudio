using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace CGT.FungusExt.Audio
{
    [CommandInfo("Audio/CGT",
        "StopAudio",
        "Stops the audio of the given type in the given channel")]
    public class StopAudio : AudioCommand
    {
        [SerializeField] protected IntegerData channel = new IntegerData(0);

        public override void OnEnter()
        {
            base.OnEnter();

            AudioArgs args = GetAudioArgs();
            var stopper = stopEvents[audioType];
            stopper(args);
        }

        protected virtual AudioArgs GetAudioArgs()
        {
            AudioArgs args = new AudioArgs();
            args.Channel = channel;
            args.WantsPitchSet = args.WantsVolumeSet = false;
            args.OnComplete = (AudioArgs maybeOtherArgs) => { Continue(); };
            // ^We won't need to call Continue in OnEnter

            return args;
        }

    }
}