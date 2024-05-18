using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "MyBuff", menuName = "NemesisShock/Buff")]
public class Buff : ScriptableObject
{
    public string buffName;
    public float duration;
    public int stackLimit = 1;
    public StatBlock stats;

    [System.Serializable]
    public class Instance
    {
        public Buff buff;
        public float currentDuration;
    }

    public Instance GetInstance()
    {
        return new Instance
        {
            buff = this,
            currentDuration = duration
        };
    }
}
