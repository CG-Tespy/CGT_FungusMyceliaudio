using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using StringBuilder = System.Text.StringBuilder;

namespace CGT.FungusExt.Audio
{
    [CommandInfo("Audio/CGT",
        "Pitch",
        "Lets you get or set the pitch of music, sfx or voice.")]
    [AddComponentMenu("")]
    public class Pitch : SoundShifter
    {
        protected override AudioArgs DecideAudioArgs()
        {
            var result = base.DecideAudioArgs();
            result.WantsVolumeSet = false;
            result.WantsPitchSet = true;
            result.TargetPitch = targetValue;
            return result;
        }

        protected override void SetValuesToSystem(AudioArgs args)
        {
            AudioSetter.SetPitch(args);
        }

        protected override void GetValueIntoOutput(AudioArgs args)
        {
            output.Value = AudioSys.GetPitch(args);
        }

    }
}