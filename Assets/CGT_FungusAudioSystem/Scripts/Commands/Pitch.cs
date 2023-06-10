using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace CGT.FungusExt.Audio
{
    [AddComponentMenu("")]
    public class Pitch : AudioCommand
    {
        [SerializeField] protected IntegerData channel = new IntegerData(0);


    }
}