using UnityEngine;

namespace CGT.FungusExt.Audio
{
    /// <summary>
    /// Helper class for NeoAudioManager that also kind of wraps Unity's built-in AudioSource component
    /// </summary>
    public class FungusAudioSource
    {
        protected AudioSource baseSource;

        public FungusAudioSource(GameObject toWorkWith)
        {
            gameObject = toWorkWith;
            SetUpAudioSource();
        }

        protected GameObject gameObject;

        protected virtual void SetUpAudioSource()
        {
            baseSource = gameObject.AddComponent<AudioSource>();
            baseSource.playOnAwake = false;
        }

        public virtual void Play(AudioArgs args)
        {
            if (!args.WantsClipPlayed)
                return;

            CorrectStartingValsAsNeeded(args);

            if (args.WantsFade) // The only fade we'll consider is for volume
            {
                // We want the fading to happen first, then make sure the delayless
                // play operations happen
                args.OnComplete = SetBeforeOnComplete(args, PlayWithoutDelay);
                args.OnComplete = SetBeforeOnComplete(args, UpdateSettings);
                // ^ They're in this order so that the settings are updated before the play
                FadeVolume(args);
            }
            else
            {
                UpdateSettings(args);
                PlayWithoutDelay(args);
                args.OnComplete(args);
            }
        }

        protected virtual void CorrectStartingValsAsNeeded(AudioArgs args)
        {
            // Since we don't want the client to always worry about the starting
            // values.
            if (args.StartingVolume < 0)
                args.StartingVolume = CurrentVolume;
            if (args.StartingPitch < 0)
                args.StartingPitch = CurrentPitch;
        }

        /// <returns>A version of the args' OnComplete that has the passed toExecute executing first</returns>
        protected virtual AudioHandler SetBeforeOnComplete(AudioArgs args, AudioHandler toExecute)
        {
            AudioHandler onComplete = args.OnComplete;
            AudioHandler result = (AudioArgs maybeOtherArgs) =>
            {
                toExecute(maybeOtherArgs);
                onComplete(maybeOtherArgs);
            };

            return result;
        }
                
        protected AudioClip Clip
        {
            get { return baseSource.clip; }
            set { baseSource.clip = value; }
        }

        protected virtual void UpdateSettings(AudioArgs args)
        {
            if (args.WantsVolumeSet)
                CurrentVolume = args.TargetVolume;
            else
                CurrentVolume = args.StartingVolume;

            if (args.WantsPitchSet)
                CurrentPitch = args.Pitch;
            else
                CurrentPitch = args.StartingPitch;

            AtTime = args.AtTime;
            // ^ May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK

            if (args.Loop)
            {
                Clip = args.Clip;
                Loop = true;
            }
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

        protected virtual void FadeVolume(AudioArgs args)
        {
            float startingVolume = CurrentVolume, targetVolume = args.TargetVolume;

            System.Action onComplete = () =>
            {
                bool shouldReturnToStartingVolume = !args.WantsVolumeSet;
                if (shouldReturnToStartingVolume)
                    CurrentVolume = startingVolume;

                args.OnComplete(args);
            };

            LeanTween.value(gameObject, startingVolume, targetVolume, args.FadeDuration)
                .setOnUpdate(TweenVolume)
                .setOnComplete(onComplete);
        }

        protected virtual void TweenVolume(float newVol)
        {
            CurrentVolume = newVol;
        }

        public virtual void SetVolume(AudioArgs args)
        {
            if (!args.WantsVolumeSet)
                return;

            CorrectStartingValsAsNeeded(args);

            if (args.WantsFade)
            {
                FadeVolume(args);
            }
            else
            {
                SetVolumeWithoutDelay(args);
                args.OnComplete(args);
            }
        }

        protected virtual void SetVolumeWithoutDelay(AudioArgs args)
        {
            CurrentVolume = args.TargetVolume;
        }

        public virtual void SetPitch(AudioArgs args)
        {
            if (!args.WantsPitchSet)
                return;

            CorrectStartingValsAsNeeded(args);

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

        protected virtual void FadePitch(AudioArgs args)
        {
            float startingPitch = CurrentPitch, targetPitch = args.Pitch;
            System.Action onComplete = () =>
            {
                bool shouldReturnToStartingPitch = !args.WantsPitchSet;
                if (shouldReturnToStartingPitch)
                    CurrentPitch = startingPitch;
            };
            LeanTween.value(gameObject, startingPitch, targetPitch, args.FadeDuration)
                .setOnUpdate(TweenPitch)
                .setOnComplete(onComplete);
        }

        protected virtual void TweenPitch(float newPitch)
        {
            baseSource.pitch = newPitch;
        }

        protected virtual void SetPitchWithoutDelay(AudioArgs args)
        {
            CurrentPitch = args.Pitch;
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
        }
    }
}