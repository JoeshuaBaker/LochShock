using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "MyBuff", menuName = "NemesisShock/Buff")]
public class Buff : ScriptableObject
{
    public enum StackType
    {
        Stackable,
        Copyable
    }
    public Sprite icon;
    public string buffName;
    public float duration;
    public StackType stackType;
    public int stackLimit = 1;
    public NewStatBlock newStats;

    [System.Serializable]
    public class Instance
    {
        public Buff buff;
        public NewStatBlock newStats;
        public float currentDuration;
    }

    public Instance GetInstance()
    {
        if (string.IsNullOrEmpty(buffName))
        {
            buffName = name;
            Debug.LogWarning($"Warning: Set buff name on buff {name}");
        }

        return new Instance
        {
            buff = this,
            newStats = this.newStats.DeepCopy(),
            currentDuration = duration
        };
    }
}
