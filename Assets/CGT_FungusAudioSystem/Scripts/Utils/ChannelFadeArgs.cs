using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace CGT.FungusExt.Audio
{
    /// <summary>
    /// Settings for setting one specific channel
    /// </summary>
    [System.Serializable]
    public class ChannelFadeArgs
    {
        [SerializeField] protected IntegerData channel = new IntegerData(0);
        [SerializeField] protected FloatData targetVolume = new FloatData(0f);
        [SerializeField] protected FloatData duration = new FloatData(0f);
    }
}