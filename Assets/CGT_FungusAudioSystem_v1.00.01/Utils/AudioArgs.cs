using UnityEngine;

namespace CGT.FungusExt.Audio
{
    public class AudioArgs
    {
        public virtual bool WantsClipPlayed { get { return Clip != null; } }
        public virtual AudioClip Clip { get; set; }

        public virtual bool WantsVolumeSet { get; set; }

        /// <summary>
        /// To avoid weirdness, best keep this between 0 and 1
        /// </summary>
        public virtual float Volume { get; set; }

        public virtual bool WantsFade
        {
            get { return !Mathf.Approximately(FadeDuration, 0f); }
        }

        public virtual float FadeDuration { get; set; }

        public virtual bool WantsPitchSet { get; set; }

        public virtual float Pitch { get; set; }

        public virtual bool Loop { get; set; }

        public virtual float AtTime { get; set; }

        /// <summary>
        /// By default a func that does nothing.
        /// </summary>
        public virtual System.Action OnComplete { get; set; } = () => { };

        public virtual AudioSource TargetAudioSource { get; set; }

    }
}