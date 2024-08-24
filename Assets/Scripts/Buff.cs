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
    public float baseDuration = 1f;
    public StackType stackType;
    public int stackLimit = 1;
    public StatBlock newStats;

    [System.Serializable]
    public class Instance
    {
        public Buff buff;
        public StatBlock newStats;
        public Item source;
        public float currentDuration;
    }

    public Instance GetInstance(Item source = null, float overwriteDuration = 0f)
    {
        if (string.IsNullOrEmpty(buffName))
        {
            buffName = name;
            Debug.LogWarning($"Warning: Set buff name on buff {name}");
        }

        StatBlock buffStatsCopy = this.newStats.DeepCopy();
        buffStatsCopy.AddSource(source);

        return new Instance
        {
            buff = this,
            newStats = buffStatsCopy,
            source = source,
            currentDuration = overwriteDuration == 0f ? baseDuration : overwriteDuration
        };
    }
}
