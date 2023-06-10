using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace CGT.FungusExt.Audio
{
    /// <summary>
    /// For setting properties of audio.
    /// </summary>
    public abstract class SoundShifter<TInput, TOutput> : AudioCommand where TOutput: Fungus.Variable
    {
        [SerializeField] protected IntegerData channel = new IntegerData(0);
        [SerializeField] protected GetOrSet action = GetOrSet.Set;

        public override void OnEnter()
        {
            base.OnEnter();

        }

        protected virtual AudioArgs GetAudioArgs()
        {
            AudioArgs args = new AudioArgs();
            return args;
        }
    }
}