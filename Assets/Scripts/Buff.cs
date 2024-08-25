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
    public StatBlock levelUpStats;

    [System.Serializable]
    public class Instance
    {
        public Buff buff;
        public StatBlock newStats;
        public Item source;
        public float currentDuration;
        public int level = 1;
    }

    public Instance GetInstance(Item source = null, float overwriteDuration = 0f)
    {
        if (string.IsNullOrEmpty(buffName))
        {
            buffName = name;
            Debug.LogWarning($"Warning: Set buff name on buff {name}");
        }

        StatBlock buffStatsCopy = GetStatBlockCopy(source.level);
        buffStatsCopy.AddSource(source);

        return new Instance
        {
            buff = this,
            newStats = buffStatsCopy,
            source = source,
            currentDuration = overwriteDuration == 0f ? baseDuration : overwriteDuration,
            level = source.level
        };
    }

    public StatBlock GetStatBlockCopy(int level = 1)
    {
        StatBlock buffStatsCopy = this.newStats.DeepCopy();
        if (level > 1)
        {
            StatBlock levelUpStatsCopy = this.levelUpStats.DeepCopy();
            levelUpStatsCopy.Stacks = level - 1;
            buffStatsCopy.Add(levelUpStatsCopy);
        }
        return buffStatsCopy;
    }

    public List<StatBlock> GetStatBlocks(int level = 1)
    {
        List<StatBlock> blocks = new List<StatBlock>();
        blocks.Add(this.newStats.DeepCopy());
        if(level > 1)
        {
            var levelUpStats = this.levelUpStats.DeepCopy();
            levelUpStats.Stacks = level - 1;
            blocks.Add(levelUpStats);
        }

        return blocks;
    }

    public CombinedStatBlock GetCombinedStatBlock(int level = 1)
    {
        CombinedStatBlock csb = new CombinedStatBlock();
        csb.UpdateSources(GetStatBlocks(level));
        return csb;
    }
}
