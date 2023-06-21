using UnityEngine;
using Fungus;
using StringBuilder = System.Text.StringBuilder;

namespace CGT.FungusExt.Audio
{
    [CommandInfo("Audio/CGT",
        "ChVolume",
        "Lets you get or set the volume of the given channel")]
    [AddComponentMenu("")]
    public class ChannelVolume : SoundShifter
    {
        protected override AudioArgs DecideAudioArgs()
        {
            var result = base.DecideAudioArgs();
            result.WantsVolumeSet = true;
            result.WantsPitchSet = false;
            result.TargetVolume = CorrectedTargetValue;
            return result;
        }

        protected override float MinTargetValue { get { return AudioStatics.MinVolume; } }
        protected override float MaxTargetValue { get { return AudioStatics.MaxVolume; } }

        protected override void SetValuesToSystem(AudioArgs args)
        {
            AudioEvents.TriggerSetVolume(args);
        }

        protected override void GetValueIntoOutput(AudioArgs args)
        {
            output.Value = AudioSys.GetVolume(args);
        }
    }
}