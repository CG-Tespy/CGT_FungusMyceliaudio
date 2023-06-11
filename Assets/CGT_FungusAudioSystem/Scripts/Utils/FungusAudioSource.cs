using UnityEngine;

namespace CGT.FungusExt.Audio.Internal
{
    /// <summary>
    /// Helper class for NeoAudioManager that also kind of wraps Unity's built-in AudioSource component
    /// </summary>
    public class FungusAudioSource
    {
        protected AudioSource baseSource;

        public FungusAudioSource(GameObject toWorkWith)
        {
            forTweens = toWorkWith;
            SetUpAudioSource();
        }

        protected GameObject forTweens; // So we can pull off tweens

        protected virtual void SetUpAudioSource()
        {
            baseSource = forTweens.AddComponent<AudioSource>();
            baseSource.playOnAwake = false;
            baseSource.volume = 0;
        }

        public virtual void Play(AudioArgs args)
        {
            InternalAudioArgs converted = ToInternal(args);
            Play(converted);
        }

        protected virtual InternalAudioArgs ToInternal(AudioArgs source)
        {
            InternalAudioArgs result = InternalAudioArgs.CreateCopy(source);
            result.StartingVolume = CurrentVolume;
            result.StartingPitch = CurrentPitch;

            return result;
        }

        protected virtual void Play(InternalAudioArgs args)
        {
            if (!args.WantsClipPlayed)
                return;

            if (args.WantsFade) // The only fade we'll consider is for volume
            {
                args.OnComplete = SetBeforeOnComplete(args, PlayWithoutDelay);
                args.OnComplete = SetBeforeOnComplete(args, UpdateSettings);
                // ^ They're in this order so that the settings are updated right
                // after the fade and right before the sound-playing
                FadeVolume(args);
            }
            else
            {
                UpdateSettings(args);
                PlayWithoutDelay(args);
                args.OnComplete(args);
            }
        }

        /// <returns>
        /// A version of the args' OnComplete that has the passed toExecute
        /// executing first
        /// </returns>
        protected virtual InternalAudioHandler SetBeforeOnComplete(InternalAudioArgs args,
            InternalAudioHandler toExecute)
        {
            InternalAudioHandler origOnComplete = args.OnComplete;
            InternalAudioHandler result = (InternalAudioArgs maybeOtherArgs) =>
            {
                toExecute(maybeOtherArgs);
                origOnComplete(maybeOtherArgs);
            };

            return result;
        }
        
        protected AudioClip Clip
        {
            get { return baseSource.clip; }
            set { baseSource.clip = value; }
        }

        protected virtual void UpdateSettings(InternalAudioArgs args)
        {
            if (args.WantsVolumeSet && !tweeningVolume)
                SetVolumeWithoutDelay(args);
            else if (!tweeningVolume)
                CurrentVolume = args.StartingVolume;

            if (args.WantsPitchSet && !tweeningPitch)
                SetPitchWithoutDelay(args);
            else if (!tweeningPitch)
                CurrentPitch = args.StartingPitch;

            if (args.WantsPlayAtNewTime)
                AtTime = args.AtTime;
                // ^ May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK

            bool wantsToPlayNewClipOnLoop = args.Loop && args.WantsClipPlayed && Clip != args.Clip;
            if (wantsToPlayNewClipOnLoop)
            {
                Clip = args.Clip;
                Loop = true;
            }
        }

        protected bool tweeningVolume, tweeningPitch;

        protected virtual void SetVolumeWithoutDelay(AudioArgs args)
        {
            CurrentVolume = args.TargetVolume;
        }

        protected virtual void SetPitchWithoutDelay(InternalAudioArgs args)
        {
            CurrentPitch = args.TargetPitch;
        }

        protected virtual void PlayWithoutDelay(AudioArgs args)
        {
            if (args.Loop)
                baseSource.Play();
            else
                baseSource.PlayOneShot(args.Clip);
        }

        public virtual float CurrentVolume
        {
            get { return baseSource.volume; }
            protected set { baseSource.volume = value; }
        }

        public virtual float CurrentPitch
        {
            get { return baseSource.pitch; }
            protected set { baseSource.pitch = value; }
        }

        public virtual void FadeVolume(AudioArgs args)
        {
            InternalAudioArgs converted = ToInternal(args);
            FadeVolume(converted);
        }

        protected virtual void FadeVolume(InternalAudioArgs args)
        {
            float startingVolume = args.StartingVolume, targetVolume = args.TargetVolume;
            tweeningVolume = true;

            System.Action whenDoneFading = () => 
            {
                tweeningVolume = false;
                args.OnComplete(args);
            };

            LeanTween.value(forTweens, startingVolume, targetVolume, args.FadeDuration)
                .setOnUpdate(TweenVolume)
                .setOnComplete(whenDoneFading);
        }

        protected virtual void TweenVolume(float newVol)
        {
            CurrentVolume = newVol;
        }

        public virtual void SetVolume(AudioArgs args)
        {
            InternalAudioArgs converted = ToInternal(args);
            SetVolume(converted);
        }

        protected virtual void SetVolume(InternalAudioArgs args)
        {
            if (!args.WantsVolumeSet)
                return;

            if (args.WantsFade)
            {
                args.OnComplete = SetBeforeOnComplete(args, UpdateSettings);
                FadeVolume(args);
            }
            else
            {
                SetVolumeWithoutDelay(args);
                args.OnComplete(args);
            }
        }

        public virtual void SetPitch(AudioArgs args)
        {
            InternalAudioArgs converted = ToInternal(args);
            SetPitch(converted);
        }

        protected virtual void SetPitch(InternalAudioArgs args)
        {
            if (!args.WantsPitchSet)
                return;

            if (args.WantsFade)
            {
                FadePitch(args);
            }
            else
            {
                SetPitchWithoutDelay(args);
                args.OnComplete(args);
            }
        }

        protected virtual void FadePitch(InternalAudioArgs args)
        {
            float startingPitch = CurrentPitch, targetPitch = args.TargetPitch;
            tweeningPitch = true;

            System.Action onComplete = () =>
            {
                tweeningPitch = false;
                args.OnComplete(args);
            };
            LeanTween.value(forTweens, startingPitch, targetPitch, args.FadeDuration)
                .setOnUpdate(TweenPitch)
                .setOnComplete(onComplete);
        }

        protected virtual void TweenPitch(float newPitch)
        {
            baseSource.pitch = newPitch;
        }

        protected virtual bool Loop
        {
            get { return baseSource.loop; }
            set { baseSource.loop = value; }
        }

        protected virtual float AtTime
        {
            get { return baseSource.time; }
            set { baseSource.time = value; }
        }

        public virtual void Stop(AudioArgs args)
        {
            baseSource.Stop();
            args.OnComplete(args);
        }
    
    }
}