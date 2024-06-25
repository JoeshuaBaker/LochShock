using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NewStatBlock
{
    [SerializeReference, SerializeReferenceMenu] public Stat[] stats;
    public float testFloat;
}
