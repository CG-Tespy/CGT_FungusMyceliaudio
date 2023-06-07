using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CGT.FungusExt.Audio.Internal
{
    public class AudioManager
    {
        protected IDictionary<int, FungusAudioSource> channels = new Dictionary<int, FungusAudioSource>();

        public AudioManager(GameObject gameObject)
        {
            forTweens = gameObject;
            SetUpInitialChannels();
        }

        protected GameObject forTweens;

        protected virtual void SetUpInitialChannels()
        {
            for (int i = 0; i < initChannelCount; i++)
            {
                channels[i] = new FungusAudioSource(forTweens);
            }
        }

        protected int initChannelCount = 10;

        public virtual void Play(AudioArgs args)
        {
            channels[args.Channel].Play(args);
        }

        public virtual void SetVolume(AudioArgs args)
        {
            channels[args.Channel].SetVolume(args);
        }

        public virtual void FadeVolume(AudioArgs args)
        {
            channels[args.Channel].FadeVolume(args);
        }

        public virtual void SetPitch(AudioArgs args)
        {
            channels[args.Channel].SetPitch(args);
        }

        public virtual void Stop(AudioArgs args)
        {
            channels[args.Channel].Stop(args);
        }

        public virtual float GetVolume(AudioArgs args)
        {
            return channels[args.Channel].CurrentVolume;
        }

        public virtual float GetPitch(AudioArgs args)
        {
            return channels[args.Channel].CurrentPitch;
        }
    
        public virtual string Name { get; set; }
    }
}