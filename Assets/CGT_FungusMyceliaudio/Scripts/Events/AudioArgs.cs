using UnityEngine;

namespace CGT.FungusExt.Myceliaudio
{
    public class AudioArgs : System.EventArgs
    {
        public virtual bool WantsClipPlayed { get { return Clip != null; } }
        public virtual AudioClip Clip { get; set; }

        public virtual bool WantsVolumeSet { get; set; }

        /// <summary>
        /// To avoid weirdness, best keep this between 0 and 1
        /// </summary>
        public virtual float TargetVolume { get; set; }

        public virtual bool WantsFade
        {
            get { return !Mathf.Approximately(FadeDuration, 0f); }
        }

        public virtual float FadeDuration { get; set; }

        public virtual bool WantsPitchSet { get; set; }

        public virtual float TargetPitch { get; set; }

        public virtual bool Loop { get; set; }

        public virtual float AtTime { get; set; }
        public virtual bool WantsPlayAtNewTime
        {
            get { return WantsClipPlayed && AtTime >= 0; }
        }

        public virtual int Track { get; set; }
        public virtual AudioType AudioType { get; set; }

        /// <summary>
        /// By default a func that does nothing.
        /// </summary>
        public virtual AudioHandler OnComplete { get; set; } = (AudioArgs args) => { };

        public AudioArgs() { }

        public static AudioArgs CreateCopy(AudioArgs source)
        {
            AudioArgs theCopy = new AudioArgs();

            theCopy.Clip = source.Clip;
            theCopy.WantsVolumeSet = source.WantsVolumeSet;
            theCopy.TargetVolume = source.TargetVolume;
            theCopy.FadeDuration = source.FadeDuration;
            theCopy.WantsPitchSet = source.WantsPitchSet;
            theCopy.TargetPitch = source.TargetPitch;
            theCopy.Loop = source.Loop;
            theCopy.AtTime = source.AtTime;
            theCopy.Track = source.Track;
            theCopy.OnComplete = source.OnComplete;
            theCopy.AudioType = source.AudioType;

            return theCopy;
            
        }
    }
}