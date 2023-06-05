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
        }

        public virtual void Play(AudioArgs args)
        {
            if (!args.WantsClipPlayed)
                return;

            if (args.WantsFade) // The only fade we'll consider is for volume
            {
                args.OnComplete = PlayAudioBeforeOnComplete(args);
                args.OnComplete = ChangeSettingsBeforeOnComplete(args);
                // ^ So that the audio is played properly right when the
                // fading is done: settings changed first, then play
                FadeVolume(args);
            }
            else
            {
                ChangeSettingsAsAppropriate(args);
                PlayRightAway(args);
                args.OnComplete();
            }
        }

        protected AudioClip Clip
        {
            get { return baseSource.clip; }
            set { baseSource.clip = value; }
        }

        protected virtual System.Action PlayAudioBeforeOnComplete(AudioArgs args)
        {
            System.Action origOnComplete = args.OnComplete;
            System.Action result = () =>
            {
                PlayRightAway(args);
                origOnComplete();
            };

            return result;
        }

        protected virtual System.Action ChangeSettingsBeforeOnComplete(AudioArgs args)
        {
            System.Action origOnComplete = args.OnComplete;
            System.Action result = () =>
            {
                ChangeSettingsAsAppropriate(args);
                origOnComplete();
            };

            return result;
        }

        protected virtual void ChangeSettingsAsAppropriate(AudioArgs args)
        {
            if (args.WantsVolumeSet)
                CurrentVol = args.Volume;

            if (args.WantsPitchSet)
                CurrentPitch = args.Pitch;

            if (args.WantsClipPlayed && args.Loop)
                Clip = args.Clip;
        }

        protected virtual void PlayRightAway(AudioArgs args)
        {
            AtTime = args.AtTime;
            // ^ May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK

            if (args.Loop)
            {
                Clip = args.Clip;
                Loop = true;
                baseSource.Play();
            }
            else
                baseSource.PlayOneShot(args.Clip);
        }

        public virtual float CurrentVol
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
            float startingVolume = CurrentVol, targetVolume = args.Volume;
            LeanTween.value(gameObject, startingVolume, targetVolume, args.FadeDuration)
                .setOnUpdate(TweenVolume)
                .setOnComplete(args.OnComplete);
        }

        protected virtual void TweenVolume(float newVol)
        {
            CurrentVol = newVol;
        }

        public virtual void SetVolume(AudioArgs args)
        {
            float startingVolume = CurrentVol;

            if (args.WantsFade)
            {
                FadeVolume(args);
            }
            else
            {
                CurrentVol = args.Volume;
                args.OnComplete();
            }
        }

        protected virtual void SetVolumeRightAway(AudioArgs args)
        {
            CurrentVol = args.Volume;
        }

        public virtual void SetPitch(AudioArgs args)
        {
            if (args.WantsFade)
            {
                FadePitch(args);
            }
            else
            {
                baseSource.pitch = args.Pitch;
                args.OnComplete();
            }
        }

        protected virtual void FadePitch(AudioArgs args)
        {
            float startingPitch = CurrentPitch, targetPitch = args.Pitch;
            LeanTween.value(gameObject, startingPitch, targetPitch, args.FadeDuration)
                .setOnUpdate(TweenPitch)
                .setOnComplete(args.OnComplete);
        }

        protected virtual void TweenPitch(float newPitch)
        {
            baseSource.pitch = newPitch;
        }

        protected virtual void SetPitchRightAway(AudioArgs args)
        {
            CurrentPitch = args.Pitch;
        }

        protected virtual bool Loop
        {
            get { return baseSource.loop; }
            set { baseSource.loop = value; }
        }

        protected virtual bool PlayOnAwake
        {
            get { return baseSource.playOnAwake; }
            set { baseSource.playOnAwake = value; }
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